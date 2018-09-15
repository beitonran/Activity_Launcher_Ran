using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using GalaSoft.MvvmLight.Messaging;
using LauncherMvvmLight.Domain.Messages;
using LauncherMvvmLight.Domain.Services.UtilServices;
using LauncherMvvmLight.MessageInfrastructure;
using LauncherMvvmLight.Model;
using Microsoft.Win32;
using System.IO;
using System.Management;
using System.Text;
using System.Runtime.InteropServices;

namespace LauncherMvvmLight.Domain.Services.DeviceCollectService
{


    public class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ModuleInformation
        {
            public IntPtr lpBaseOfDll;
            public uint SizeOfImage;
            public IntPtr EntryPoint;
        }

        internal enum ModuleFilter
        {
            ListModulesDefault = 0x0,
            ListModules32Bit = 0x01,
            ListModules64Bit = 0x02,
            ListModulesAll = 0x03,
        }

        [DllImport("psapi.dll")]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] IntPtr[] lphModule, int cb, [MarshalAs(UnmanagedType.U4)] out int lpcbNeeded, uint dwFilterFlag);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] uint nSize);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out ModuleInformation lpmodinfo, uint cb);


        [DllImport("version.dll", SetLastError = true)]
        static extern bool GetFileVersionInfo(
           /*__in*/    string lptstrFilename,
           /*__reserved*/  int dwHandleIgnored,
           /*__in      */  int dwLen,
           /*__out     */  byte[] lpData);


        [DllImport("version.dll")]
        static extern int GetFileVersionInfoSizeEx(string fileName, [Out]IntPtr dummy);


        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandleEx(
                string sModuleName);



        public Native() { }

    }

    public class Module
    {
        public Module(string moduleName, IntPtr baseAddress, uint size)
        {
            this.ModuleName = moduleName;
            this.BaseAddress = baseAddress;
            this.Size = size;
        }

        public string ModuleName { get; set; }
        public IntPtr BaseAddress { get; set; }
        public uint Size { get; set; }
    }

    /// <summary>
    /// The Interface defining methods for Create Employee and Read All Employees  
    /// </summary>
    public interface IDataCollectSrv
    {
        ObservableCollection<SystemInfoModel> GetData();
        //Collection<PSObject> ExecutePSShellGetDeviceCmdlet();
        void GetSystemFullData(String userAppName);
    }

    /// <summary>
    /// Class implementing IDataAccessService interface and implementing
    /// its methods by making call to the Entities using CompanyEntities object
    /// </summary>
    public class DataCollectSrv : IDataCollectSrv
    {


        // First create a runspace
        // You really only need to do this once. Each pipeline you create can run in this runspace.
        private String uProcessId;
        private int testedProcessId;

        private RunspaceConfiguration psConfig;
        private Runspace psRunspace;
        private InitialSessionState initial;
        private Runspace runspace;
        private RunspaceInvoke invoker;
        private Collection<PSObject> psObjectsResult = null;


        private ISystemInfoRepository _dataInfoRepository;

        //DbContextOptions<DeviceContext> options;

        private DeviceManagerDataModel _deviceMagaerDataReport;
        private ModuleDataModel _moduleDataReport;

        private SystemInfoModel _fullDataReport;

        public SystemInfoModel FullDataReport
        {
            get
            {
                return _fullDataReport;
            }
            set
            {
                _fullDataReport = value;
            }
        }

        private ObservableCollection<SystemInfoModel> _data;
        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>The notes.</value>
        public ObservableCollection<SystemInfoModel> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public DataCollectSrv()
        {
            uProcessId = "";
            //BinaryFile based==> initialize device info repository!!!! and get all listed devices !!!!!
            _dataInfoRepository = new SystemInfoRepository("ExcaliburSystemReport");
            _data = new ObservableCollection<SystemInfoModel>(_dataInfoRepository.GetAllData());

            FullDataReport = new SystemInfoModel();
            FullDataReport.DeviceDriverDataList = new List<DeviceManagerDataModel>();
            FullDataReport.ModuleDataList = new List<ModuleDataModel>();
            FullDataReport.osDataModel = new OsDataModel();


            Init();

        }

        private void Init()
        {

            //initial = InitialSessionState.CreateDefault();
            //initial.ImportPSModule(new[] { modulePathOrModuleName1, ... });
            //runspace = RunspaceFactory.CreateRunspace(initial);
            //runspace.Open();

            //invoker = new RunspaceInvoke(runspace);
            //Collection<PSObject> results = invoker.Invoke("...");

            //PSConsoleLoadException x = null; ;
            //psConfig = RunspaceConfiguration.Create(@"C:\Excalibur\ExcListDlls.ps1", out x);
            psConfig = RunspaceConfiguration.Create();
            psRunspace = RunspaceFactory.CreateRunspace(psConfig);
            psRunspace.Open();

            //RunspaceInvoke runSpaceInvoker = new RunspaceInvoke(psRunspace);
            //runSpaceInvoker.Invoke("Set-ExecutionPolicy Unrestricted");

            using (Pipeline psPipeline = psRunspace.CreatePipeline())
            {
                //import get devices manager
                Command command = new Command("Import-Module");
                command.Parameters.Add("Name", @"C:\Excalibur\DeviceManagement\DeviceManagement.dll");
                psPipeline.Commands.Add(command);
                //import get modules
                //Command commandGetModules = new Command("Import-Module");
                //commandGetModules.Parameters.Add("Name", @"C:\Excalibur\ExcListDlls.ps1");
                //psPipeline.Commands.Add(commandGetModules);


                try
                {
                    Collection<PSObject> psObjects = psPipeline.Invoke();
                }
                catch (CmdletInvocationException exception)
                {
                    // Process the exception here
                }
            }

        }


        public ObservableCollection<SystemInfoModel> GetData()
        {
            //File-based-->get from info repository
            _data = new ObservableCollection<SystemInfoModel>(_dataInfoRepository.GetAllData());

            //DB-based-->get from info repository           


            return Data;
        }

        //**********************************************************************************************************
        //  System.Environment - Get Data
        //**********************************************************************************************************
        private Boolean ExecuteEnvironmentGetData()
        {

            OperatingSystem os = Environment.OSVersion;

            FullDataReport.osDataModel.osVersion = os.Version.ToString();
            FullDataReport.osDataModel.osVersion = os.Platform.ToString();
            FullDataReport.osDataModel.osVersion = os.ServicePack.ToString();
            FullDataReport.osDataModel.osVersion = os.VersionString.ToString();



            Version ver = os.Version;

            FullDataReport.osDataModel.verMajor = ver.Major.ToString();
            FullDataReport.osDataModel.osVersion = ver.MajorRevision.ToString();
            FullDataReport.osDataModel.osVersion = ver.Minor.ToString();
            FullDataReport.osDataModel.osVersion = ver.MinorRevision.ToString();

            FullDataReport.osDataModel.osVersion = ver.Build.ToString();

            FullDataReport.osDataModel.isOS64 = Environment.Is64BitOperatingSystem; ;
            FullDataReport.osDataModel.isArch64 = Environment.Is64BitProcess;
            FullDataReport.osDataModel.processorNum = Environment.ProcessorCount;



            string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string CSDVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");

            string osVersionFriendlyName = (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                        (CSDVersion != "" ? " " + CSDVersion : "");

            FullDataReport.osDataModel.osVersionFriendlyName = osVersionFriendlyName;

            return true;
        }
        //**********************************************************************************************************
        //  Device Drivers
        //**********************************************************************************************************
        private Boolean ExecutePSShellGetDeviceCmdlet()
        {
            //Get - Device | Sort - Object - Property Name | ft Name, DriverVersion, DriverProvider, IsPresent, HasProblem - AutoSize
            Collection<PSObject> psObjects = null;
            using (Pipeline psPipeline = psRunspace.CreatePipeline())
            {
                // Define the command to be executed in this pipeline
                Command command1 = new Command("Get-Device");
                // Add this command to the pipeline
                psPipeline.Commands.Add(command1);
                Command command2 = new Command("Sort-Object");
                command2.Parameters.Add("Property", "Name");
                psPipeline.Commands.Add(command2);
                Command command3 = new Command("Where-Object");
                ScriptBlock scriptBlock = ScriptBlock.Create("$_.DriverProvider -like '*Excalibur*'");
                command3.Parameters.Add("FilterScript", scriptBlock);
                psPipeline.Commands.Add(command3);
                //command.Parameters.Add("Sort-Object -Property Name | ft Name, DriverVersion, DriverProvider, IsPresent, HasProblem -AutoSize");
                try
                {

                    // Process the results
                    psObjects = psPipeline.Invoke();
                    foreach (var psObject in psObjects)
                    {
                        ;//WriteObject(psObject);
                        _deviceMagaerDataReport = new DeviceManagerDataModel();
                        _deviceMagaerDataReport.Name = psObject.Properties["Name"].Value.ToString();
                        _deviceMagaerDataReport.Manufacturer = psObject.Properties["Manufacturer"].Value.ToString();
                        _deviceMagaerDataReport.HardwareIds = psObject.Properties["HardwareIds"].Value.ToString();
                        _deviceMagaerDataReport.DriverVersion = psObject.Properties["DriverVersion"].Value.ToString();
                        _deviceMagaerDataReport.DriverProvider = psObject.Properties["DriverProvider"].Value.ToString();
                        _deviceMagaerDataReport.LocationInfo = psObject.Properties["LocationInfo"].Value.ToString();
                        _deviceMagaerDataReport.IsPresent = psObject.Properties["IsPresent"].Value.ToString();
                        _deviceMagaerDataReport.IsEnabled = psObject.Properties["IsEnabled"].Value.ToString();
                        _deviceMagaerDataReport.HasProblem = psObject.Properties["HasProblem"].Value.ToString();

                        FullDataReport.DeviceDriverDataList.Add(_deviceMagaerDataReport);
                    }


                }
                catch (CmdletInvocationException exception)
                {
                    // Process the exception here
                }
            }

            return true;
        }
        //**********************************************************************************************************
        //  Dlls
        //**********************************************************************************************************


        //**********************************************************************************************************
        //   Data access tech!!!!!
        //**********************************************************************************************************
        /*
             //Fille report & save
            //device manager data 
            //ExecutePSShellGetDeviceCmdlet();
            //os data

            //dll list
            //ExecutePSShellGetModulesCmdlet();
            //CollectModules(null);
            //
            //RunExeTool(@"C:\Excalibur\Utilities\InfoTools\ListDlls\ListDlls.exe", "");
            //RunPsScript(@"C:\Excalibur\Utilities\PsScripts\ExcListDlls.ps1");
            //RunPsScriptDirect(@"Get-Process | Where-Object { ($_.Modules).ModuleName -Contains 'galpcep.dll'}");
            //RunPsScriptDirect(@"Get-Process", @"Where-Object { ($_.Modules).ModuleName -Contains 'galpcep.dll'}");
            //ExecuteGetUserProcessInfo(userAppNmae);
         */
        public void GetSystemFullData(String userAppName)
        {
            // Get OS Environment Data
            /*
                  OS Version;
                  OS Platform;
                  OS ServicePack;
                  OS Version String;
                  OS version Major;
                  OS version Major Revision;
                  OS version Minor;
                  OS version Minor Revision;
                  OSversion Build;

                  OS 32/64 Architecture;
                  system processor 32/64 Architecture;
                  Number of processors; 
             */
            ExecuteEnvironmentGetData();

            // Get Device Driver Data
            /*
                Name
                Manufacturer
                HardwareIds
                DriverVersion
                DriverProvider
                LocationInfo
                IsPresent
                IsEnabled
                HasProblem             
             */
            ExecutePSShellGetDeviceCmdlet();

            // Get User Process Data - based on supplied process name
            if (userAppName != "")
            {
                //ExecutePSShellGetUserProcessId(userAppName);
                //ExecutePSShellGetUserProcessModules();
                ExecuteGetUserProcessInfo(userAppName);
                //ExecuteGetUserProcessData(userAppName);
            }
            //else
            //{
            //    ExecutePSShellGetModulesCmdlet();
            //}



            //save updated Report
            _dataInfoRepository.Save(FullDataReport);

            //Update view Model
            Messenger.Default.Send<UpdateSystemReportMessage>(new UpdateSystemReportMessage()
            {
                systemInfoModelReport = FullDataReport
            });

        }
        //**********************************************************************************************************
        //  PowerShell - Execute Cmdlet
        //**********************************************************************************************************
        private void ExecutePSShellCmdletGetData(string dataType)
        {
            if (dataType == "Get-Process")
            {

            }
            else if (dataType == "Get-Modules")
            {

            }
            else if (dataType == "Get-Modules")
            {

            }
        }

        private Boolean ExecutePSShellGetUserProcessModules()
        {
            Collection<PSObject> psObjects = null;

            using (Pipeline psPipeline = psRunspace.CreatePipeline())
            {
                // Define the command to be executed in this pipeline
                Command command1 = new Command("Get-Process");
                // Add this command to the pipeline
                psPipeline.Commands.Add(command1);
                Command command2 = new Command("Where");
                ScriptBlock scriptBlock = ScriptBlock.Create("$_.Id -eq " + uProcessId);
                command2.Parameters.Add("FilterScript", scriptBlock);
                psPipeline.Commands.Add(command2);

                Command command3 = new Command("Select-Object");
                command3.Parameters.Add("Property", "ProcessName");
                command3.Parameters.Add("ExpandProperty", "modules");
                psPipeline.Commands.Add(command3);

                try
                {

                    // Process the results
                    psObjects = psPipeline.Invoke();
                    StringBuilder results = new StringBuilder();
                    String temp;
                    foreach (PSObject obj in psObjects)
                    {
                    }


                }
                catch (CmdletInvocationException exception)
                {
                    // Process the exception here
                }
            }

            return true;
        }


        private Boolean ExecutePSShellGetUserProcessId(String modulePath)
        {
            Collection<PSObject> psObjects = null;


            /*
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if(process.ProcessName == "demo_bc_general_one_message")
                    foreach (ProcessModule module in process.Modules)
                    {
                        Console.WriteLine(module.FileName);
                    }
            }


            */

            using (Pipeline psPipeline = psRunspace.CreatePipeline())
            {
                Command command1 = new Command("Get-ChildItem");
                command1.Parameters.Add("Path ", modulePath);
                psPipeline.Commands.Add(command1);
                try
                {

                    // Process the results
                    psObjects = psPipeline.Invoke();
                    foreach (var psObject in psObjects)
                    {
                        uProcessId = psObject.Properties["Id"].Value.ToString();
                    }


                }
                catch (CmdletInvocationException exception)
                {
                    // Process the exception here
                    return false;
                }
            }



            return true;
        }

        private Boolean ExecutePSShellGetModulesCmdlet()
        {

            //demo_bc_general_one_message.exe

            Collection<PSObject> psObjects = null;
            using (Pipeline psPipeline = psRunspace.CreatePipeline())
            {
                // Define the command to be executed in this pipeline
                Command command1 = new Command("Get-Process");
                // Add this command to the pipeline
                psPipeline.Commands.Add(command1);
                Command command2 = new Command("Sort-Object");
                command2.Parameters.Add("Property", "Name");
                psPipeline.Commands.Add(command2);
                Command command3 = new Command("Select");
                command3.Parameters.Add("ExpandProperty", "modules");
                psPipeline.Commands.Add(command3);
                Command command4 = new Command("Where-Object");
                ScriptBlock scriptBlock = ScriptBlock.Create("$_.ModuleName -eq 'galpcep.dll'");
                command4.Parameters.Add("FilterScript", scriptBlock);
                psPipeline.Commands.Add(command4);
                Command command5 = new Command("group");
                command5.Parameters.Add("Property", "FileName");
                psPipeline.Commands.Add(command5);
                Command command6 = new Command("Select");
                command6.Parameters.Add("Property", "name");
                psPipeline.Commands.Add(command6);
                //command.Parameters.Add("Sort-Object -Property Name | ft Name, DriverVersion, DriverProvider, IsPresent, HasProblem -AutoSize");
                try
                {

                    // Process the results
                    psObjects = psPipeline.Invoke();
                    foreach (var psObject in psObjects)
                    {
                        ;//WriteObject(psObject);

                        //_deviceMagaerDataReport.HasProblem = psObject.Properties["HasProblem"].Value.ToString();

                        FullDataReport.DeviceDriverDataList.Add(_deviceMagaerDataReport);
                    }


                }
                catch (CmdletInvocationException exception)
                {
                    // Process the exception here
                }
            }

            return true;
        }

        private Boolean ExecutePSShellGetModulesScript()
        {
            Collection<PSObject> psObjects = null;

            //File.GetAttributes("ExcListDlls.ps1");
            //string strCmdText = Path.Combine(Directory.GetCurrentDirectory(), "Start.ps1");

            using (Pipeline psPipeline = psRunspace.CreatePipeline())
            {

                psPipeline.Commands.AddScript(LoadScript(@"C:\Excalibur\ExcListDlls.ps1"));

                Command commandGetDll = new Command("Get-Dll");
                commandGetDll.Parameters.Add("ModuleName", "frontdesk.dll");
                psPipeline.Commands.Add(commandGetDll);

                try
                {
                    // Process the results
                    psObjects = psPipeline.Invoke();
                    foreach (var psObject in psObjects)
                    {
                        ;//WriteObject(psObject);
                        _deviceMagaerDataReport = new DeviceManagerDataModel();
                        _deviceMagaerDataReport.Name = psObject.Properties["Name"].Value.ToString();
                        _deviceMagaerDataReport.Manufacturer = psObject.Properties["Manufacturer"].Value.ToString();
                        _deviceMagaerDataReport.HardwareIds = psObject.Properties["HardwareIds"].Value.ToString();
                        _deviceMagaerDataReport.DriverVersion = psObject.Properties["DriverVersion"].Value.ToString();
                        _deviceMagaerDataReport.DriverProvider = psObject.Properties["DriverProvider"].Value.ToString();
                        _deviceMagaerDataReport.LocationInfo = psObject.Properties["LocationInfo"].Value.ToString();
                        _deviceMagaerDataReport.IsPresent = psObject.Properties["IsPresent"].Value.ToString();
                        _deviceMagaerDataReport.IsEnabled = psObject.Properties["IsEnabled"].Value.ToString();
                        _deviceMagaerDataReport.HasProblem = psObject.Properties["HasProblem"].Value.ToString();

                        FullDataReport.DeviceDriverDataList.Add(_deviceMagaerDataReport);
                    }
                }
                catch (CmdletInvocationException exception)
                {
                    // Process the exception here
                }
            }

            return true;
        }


        private Boolean ExecutePSShellGetModulesInfoByNameXXXXX(string moduleName)
        {

            Collection<PSObject> psObjects = null;

            string script = LoadScript(@"C:\Excalibur\Utilities\PsScripts\ExcListDlls.ps1");

            using (var powershell = PowerShell.Create())
            {

                powershell.Runspace = psRunspace;

                powershell.AddScript(script, false);

                powershell.Invoke();

                powershell.Commands.Clear();

                powershell.AddCommand("Get-Dll").AddParameter("ProcessId", testedProcessId); //.AddParameter("Process", null).AddParameter("ProcessName", "").AddParameter("ProcessId", 0).AddParameter("ModuleName", "frontdesk.dll");
                Collection<PSObject> output = powershell.Invoke();

                // process each object in the output and append to stringbuilder  
                StringBuilder results = new StringBuilder();
                foreach (PSObject psObject in output)
                {
                    //results.AppendLine(obj.ToString());
                    _moduleDataReport = new ModuleDataModel();
                    //_moduleDataReport.Name = psObject.Properties["Name"].Value.ToString();
                    //_moduleDataReport.Name = psObject.Properties["Path"].Value.ToString();

                }



            }






            return true;
        }

        private Boolean ExecutePSShellGetModulesInfoByName(string moduleNamePath, string  moduleName)
        {
            Collection<PSObject> psObjects = null;


            //if (moduleNamePath != "C:\\Excalibur\\1553Px Software Tools\\Source\\demos_1553\\bin\\Exc1553PxMs.dll")
            //    return false;


            using (Pipeline psPipeline = psRunspace.CreatePipeline())
            {

                Command commandGetDll = new Command("Get-ChildItem");
                commandGetDll.Parameters.Add("Path", moduleNamePath);
                psPipeline.Commands.Add(commandGetDll);
                Command command2 = new Command("Select-Object");
                command2.Parameters.Add("ExpandProperty", "VersionInfo");
                psPipeline.Commands.Add(command2);
                Command command3 = new Command("Select-Object");
                command3.Parameters.Add("Property", "FileVersion");
                psPipeline.Commands.Add(command3);
                Command command4 = new Command("Select-Object");
                command4.Parameters.Add("ExpandProperty", "FileVersion");
                psPipeline.Commands.Add(command4);

                try
                {
                    // Process the results
                    psObjects = psPipeline.Invoke();
                    foreach (var psObject in psObjects)
                    {

                        //results.AppendLine(obj.ToString());
                        _moduleDataReport = new ModuleDataModel();
                        _moduleDataReport.Name = moduleName;
                        _moduleDataReport.FilePath = moduleNamePath;
                        _moduleDataReport.FileVersion = psObject.ToString();
                        FullDataReport.ModuleDataList.Add(_moduleDataReport);

                    }
                }
                catch (CmdletInvocationException exception)
                {
                    // Process the exception here
                }
            }




            return true;
            /*
                Collection<PSObject> psObjects = null;

                string script = LoadScript(@"C:\Excalibur\Utilities\PsScripts\ExcListDlls.ps1");

                using (var powershell = PowerShell.Create())
                {

                    powershell.Runspace = psRunspace;

                    powershell.AddScript(script, false);

                    powershell.Invoke();

                    powershell.Commands.Clear();

                    //powershell.AddCommand("Get-Dll").AddParameter("ProcessId", moduleName); //.AddParameter("Process", null).AddParameter("ProcessName", "").AddParameter("ProcessId", 0).AddParameter("ModuleName", "frontdesk.dll");
                    powershell.AddCommand("Get-Dll").AddParameter("ModuleName", moduleName); //.AddParameter("Process", null).AddParameter("ProcessName", "").AddParameter("ProcessId", 0).AddParameter("ModuleName", "frontdesk.dll");
                                                                                             //powershell.AddParameter("Unsigned", false);
                                                                                             //powershell.AddParameter("ModuleName", "frontdesk.dll");
                                                                                             //powershell.AddCommand("Get-Dll").AddParameter("ModuleName", "frontdesk.dll");
                                                                                             //powershell.AddCommand("Test-Me").AddParameter("param1", "tttttbbb");
                    Collection<PSObject> output = powershell.Invoke();

                    // process each object in the output and append to stringbuilder  
                    StringBuilder results = new StringBuilder();
                    foreach (PSObject psObject in output)
                    {
                        //results.AppendLine(obj.ToString());
                        _moduleDataReport = new ModuleDataModel();
                        _moduleDataReport.Name = psObject.Properties["Name"].Value.ToString();
                        _moduleDataReport.Name = psObject.Properties["Path"].Value.ToString();

                    }



                }


                return true;
            */

            /*
                        //File.GetAttributes("ExcListDlls.ps1");
                        //string strCmdText = Path.Combine(Directory.GetCurrentDirectory(), "Start.ps1");

                        using (Pipeline psPipeline = psRunspace.CreatePipeline())
                        {

                            psPipeline.Commands.AddScript(LoadScript(@"C:\Excalibur\Utilities\ExcListDlls.ps1"));


                            Command commandGetDll = new Command("Get-Dll");
                            commandGetDll.Parameters.Add("ModuleName", moduleName);
                            psPipeline.Commands.Add(commandGetDll);

                            try
                            {
                                // Process the results
                                psObjects = psPipeline.Invoke();
                                foreach (var psObject in psObjects)
                                {
                                    ;//WriteObject(psObject);
                                    _deviceMagaerDataReport = new DeviceManagerDataModel();
                                    _deviceMagaerDataReport.Name = psObject.Properties["Name"].Value.ToString();
                                    _deviceMagaerDataReport.Manufacturer = psObject.Properties["Manufacturer"].Value.ToString();
                                    _deviceMagaerDataReport.HardwareIds = psObject.Properties["HardwareIds"].Value.ToString();
                                    _deviceMagaerDataReport.DriverVersion = psObject.Properties["DriverVersion"].Value.ToString();
                                    _deviceMagaerDataReport.DriverProvider = psObject.Properties["DriverProvider"].Value.ToString();
                                    _deviceMagaerDataReport.LocationInfo = psObject.Properties["LocationInfo"].Value.ToString();
                                    _deviceMagaerDataReport.IsPresent = psObject.Properties["IsPresent"].Value.ToString();
                                    _deviceMagaerDataReport.IsEnabled = psObject.Properties["IsEnabled"].Value.ToString();
                                    _deviceMagaerDataReport.HasProblem = psObject.Properties["HasProblem"].Value.ToString();

                                    FullDataReport.DeviceDriverDataList.Add(_deviceMagaerDataReport);
                                }
                            }
                            catch (CmdletInvocationException exception)
                            {
                                // Process the exception here
                            }
                        }

                        return true;

            */

        }

        //**********************************************************************************************************
        //  PowerShell - Execute Script
        //**********************************************************************************************************
        private void ExecutePsScriptGetData(string scriptName)
        {

        }
        private void RunPsScriptDirect(String script1, String script2)
        {
            //script = @"Get - Process | Where - Object { ($_.Modules).ModuleName - Contains 'galpcep.dll'}";

            using (Pipeline psPipeline = psRunspace.CreatePipeline())
            {

                psPipeline.Commands.AddScript(script1);
                psPipeline.Commands.AddScript(script2);
                // Process the results
                psObjectsResult = psPipeline.Invoke();


                // process each object in the output and append to stringbuilder  
                StringBuilder results = new StringBuilder();
                foreach (PSObject obj in psObjectsResult)
                {
                    results.AppendLine(obj.ToString());
                }

                return;


            }
            /*
            using (Pipeline psPipeline = psRunspace.CreatePipeline())
            {

                psPipeline.Commands.AddScript(LoadScript(@"C:\Excalibur\ExcListDlls.ps1"));

                //using (PowerShell powershell = PowerShell.Create())
                //{
                //powershell.Runspace = psRunspace;
                //powershell.AddScript(script);
                //Collection<PSObject> output = powershell.Invoke();

                // process each object in the output and append to stringbuilder  
                StringBuilder results = new StringBuilder();
                foreach (PSObject obj in output)
                {
                    results.AppendLine(obj.ToString());
                }

                return;


            }
            */
        }

        //Get-Process | Where-Object { ($_.Modules).ModuleName -Contains "galpcep.dll }
        private void RunPsScript(string scriptName)
        {
            string script = LoadScript(@"C:\Excalibur\Utilities\PsScripts\ExcListDlls.ps1");

            using (var powershell = PowerShell.Create())
            {

                powershell.Runspace = psRunspace;

                powershell.AddScript(script, false);

                powershell.Invoke();

                powershell.Commands.Clear();

                powershell.AddCommand("Get-Dll").AddParameter("ModuleName", "galpcep.dll"); //.AddParameter("Process", null).AddParameter("ProcessName", "").AddParameter("ProcessId", 0).AddParameter("ModuleName", "frontdesk.dll");
                //powershell.AddParameter("Unsigned", false);
                //powershell.AddParameter("ModuleName", "frontdesk.dll");
                //powershell.AddCommand("Get-Dll").AddParameter("ModuleName", "frontdesk.dll");
                //powershell.AddCommand("Test-Me").AddParameter("param1", "tttttbbb");
                Collection<PSObject> output = powershell.Invoke();

                // process each object in the output and append to stringbuilder  
                StringBuilder results = new StringBuilder();
                foreach (PSObject obj in output)
                {
                    results.AppendLine(obj.ToString());
                }

                return;


            }

            /*
                using (PowerShell PowerShellInstance = PowerShell.Create())
                {
                    // this script has a sleep in it to simulate a long running script
                    PowerShellInstance.AddScript("start-sleep -s 7; get-service");

                    // begin invoke execution on the pipeline
                    IAsyncResult result = PowerShellInstance.BeginInvoke();

                    // do something else until execution has completed.
                    // this could be sleep/wait, or perhaps some other work
                    while (result.IsCompleted == false)
                    {
                        Console.WriteLine("Waiting for pipeline to finish...");
                        Thread.Sleep(1000);

                        // might want to place a timeout here...
                    }

                    Console.WriteLine("Finished!");
                }
             */
        }

        private string LoadScript(string filename)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    StringBuilder fileContents = new StringBuilder();
                    string curLine;
                    while ((curLine = sr.ReadLine()) != null)
                    {
                        fileContents.Append(curLine + "\n");
                    }
                    return fileContents.ToString();
                }
            }
            catch (Exception e)
            {
                string errorText = "The file could not be read:";
                errorText += e.Message + "\n";
                return errorText;
            }

        }

        private Collection<PSObject> RunPsScriptTBD(string psScriptPath)
        {
            string psScript = string.Empty;
            if (File.Exists(psScriptPath))
                psScript = File.ReadAllText(psScriptPath);
            else
                throw new FileNotFoundException("Wrong path for the script file");
            RunspaceConfiguration config = RunspaceConfiguration.Create();
            PSSnapInException psEx;
            //add Microsoft SharePoint PowerShell SnapIn
            PSSnapInInfo pssnap = config.AddPSSnapIn("Microsoft.SharePoint.PowerShell", out psEx);
            //create powershell runspace
            Runspace cmdlet = RunspaceFactory.CreateRunspace(config);
            cmdlet.Open();
            RunspaceInvoke scriptInvoker = new RunspaceInvoke(cmdlet);
            // set powershell execution policy to unrestricted
            scriptInvoker.Invoke("Set-ExecutionPolicy Unrestricted");
            // create a pipeline and load it with command object
            Pipeline pipeline = cmdlet.CreatePipeline();
            try
            {
                // Using Get-SPFarm powershell command 
                pipeline.Commands.AddScript(psScript);
                pipeline.Commands.AddScript("Out-String");
                // this will format the output
                Collection<PSObject> output = pipeline.Invoke();
                pipeline.Stop();
                cmdlet.Close();
                // process each object in the output and append to stringbuilder  
                StringBuilder results = new StringBuilder();
                foreach (PSObject obj in output)
                {
                    results.AppendLine(obj.ToString());
                }
                return output;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //**********************************************************************************************************
        //  ExternalTool - Execute External Tool
        //**********************************************************************************************************       
        private void ExecuteExternalToolGetData(string filename, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filename;
            //provide powershell script full path
            //startInfo.Arguments = @"& 'C:\powershell-scripts\call-from-c-sharp.ps1'";
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = startInfo;
            // execute script call start process
            process.Start();
            // get output information
            string output = process.StandardOutput.ReadToEnd();

            // catch error information
            string errors = process.StandardError.ReadToEnd();

        }
        //**********************************************************************************************************
        //  Registry - Get Data
        //**********************************************************************************************************
        private string ExecuteRegisteryGetData(string dataType)
        {
            if (dataType == "OSVersionData")
            {
                string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
                string CSDVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
                if (ProductName != "")
                {
                    string osVersionFriendlyName = (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                                (CSDVersion != "" ? " " + CSDVersion : "");

                    FullDataReport.osDataModel.osVersionFriendlyName = osVersionFriendlyName;

                    return osVersionFriendlyName;
                }
            }

            return "";
        }

        private string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }

        //**********************************************************************************************************
        //  Extra ways - TBD
        //**********************************************************************************************************
        private Boolean ExecuteGetUserProcessInfoOrig(String userAppNmae)
        {

            // Get all instances of Notepad running on the local computer.
            // This will return an empty array if notepad isn't running.
            Process[] localByName = Process.GetProcessesByName(userAppNmae);





            //Process myProcess = new Process();

            // Get the process start information of notepad.
            //ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(userAppNmae);
            //myProcessStartInfo.UseShellExecute = true;
            // Assign 'StartInfo' of userApp to 'StartInfo' of 'myProcess' object.
            //myProcess.StartInfo = myProcessStartInfo;
            // Create a notepad.
            //myProcess.Start();
            //System.Threading.Thread.Sleep(1000);
            //ProcessModule myProcessModule;
            // Get all the modules associated with 'myProcess'.
            //ProcessModuleCollection myProcessModuleCollection = localByName[0].Modules;
            //Console.WriteLine("'FileversionInfo' of the modules associated "
            //   + "with 'notepad' are:");
            // Display the 'FileVersionInfo' of each of the modules.
            //for (int i = 0; i < myProcessModuleCollection.Count; i++)
            //{
            //    myProcessModule = myProcessModuleCollection[i];
            //    Console.WriteLine(myProcessModule.ModuleName + " : "
            //       + myProcessModule.FileVersionInfo);
            //}
            // Get the main module associated with 'myProcess'.
            //myProcessModule = myProcess.MainModule;
            // Display the 'FileVersionInfo' of main module.
            //Console.WriteLine("The process's main module's FileVersionInfo is: "
            //   + myProcessModule.FileVersionInfo);
            //myProcess.CloseMainWindow();


            return true;
        }

        private string GetValidString(string strInput)
        {
            return strInput.Trim().Equals(string.Empty) ? "---" : strInput;
        }

        public List<Module> ExecuteGetUserProcessData(String userAppNmae)
        {

            // Get all instances of Notepad running on the local computer.
            // This will return an empty array if notepad isn't running.
            Process[] ObjModulesList = Process.GetProcessesByName(userAppNmae);

            Process process = ObjModulesList[0];

            testedProcessId = process.Id;

            ProcessModuleCollection ObjModules = ObjModulesList[0].Modules;


            List<Module> collectedModules = new List<Module>();


            // Iterate through the module collection.
            foreach (ProcessModule objModule in ObjModules)
            {
                //Get valid module path
                string strModulePath = GetValidString(objModule.FileName.ToString());
                //If the module exists
                if (File.Exists(objModule.FileName.ToString()))
                  {
                    //Get version
                    string strFileVersion = GetValidString(objModule.
                                  FileVersionInfo.FileVersion.ToString());
                        //Get File size
                        string strFileSize = GetValidString
                                    (objModule.ModuleMemorySize.ToString());
                        //Get Modification date
                        FileInfo objFileInfo = new
                                    FileInfo(objModule.FileName.ToString());
                        string strFileModificationDate = GetValidString
                             (objFileInfo.LastWriteTime.ToShortDateString());
                        //Get File description
                        string strFileDescription = GetValidString
                                      (objModule.FileVersionInfo.
                                      FileDescription.ToString());
                        //Get Product Name
                        string strProductName = GetValidString
                               (objModule.FileVersionInfo.ProductName.ToString());
                        //Get Product Version
                        string strProductVersion = GetValidString
                              (objModule.FileVersionInfo.ProductVersion.ToString());
                    }
                }


            return collectedModules;

        }



        public Boolean ExecuteGetUserProcessInfo(String userAppNmae)
        //public List<Module> ExecuteGetUserProcessInfo(String userAppNmae)
        //public List<Module> CollectModules(Process process)
        {


            // Get all instances of Notepad running on the local computer.
            // This will return an empty array if notepad isn't running.
            Process[] localByName = Process.GetProcessesByName(userAppNmae);

            Process process = localByName[0];

            testedProcessId = process.Id;

            List<Module> collectedModules = new List<Module>();

            IntPtr[] modulePointers = new IntPtr[0];
            int bytesNeeded = 0;
            
            // Determine number of modules
            if (!Native.EnumProcessModulesEx(process.Handle, modulePointers, 0, out bytesNeeded, (uint)Native.ModuleFilter.ListModulesAll))
            {
                //return collectedModules;
                return false;
            }

            int totalNumberofModules = bytesNeeded / IntPtr.Size;
            modulePointers = new IntPtr[totalNumberofModules];

            // Collect modules from the process
            if (Native.EnumProcessModulesEx(process.Handle, modulePointers, bytesNeeded, out bytesNeeded, (uint)Native.ModuleFilter.ListModulesAll))
            {
                for (int index = 0; index < totalNumberofModules; index++)
                {
                    StringBuilder moduleFilePath = new StringBuilder(1024);
                    Native.GetModuleFileNameEx(process.Handle, modulePointers[index], moduleFilePath, (uint)(moduleFilePath.Capacity));

                   

                    string moduleName = Path.GetFileName(moduleFilePath.ToString());

                    string moduleFilePathStr = moduleFilePath.ToString();

                    //ExecutePSShellGetModulesInfoByName(moduleName);
                    ExecutePSShellGetModulesInfoByName(moduleFilePathStr, moduleName);


                    //StringBuilder moduleFileInfoSize = new StringBuilder(1024);
                    //moduleFileInfoSize = Native.GetFileVersionInfo.(moduleName, IntPtr.Zero);
                    /*
                    if (dwSize == 0)
                    {
                        TRACE("GetFileVersionInfoSize failed with error %d\n", GetLastError());
                        return false;
                    }

                    std::vector<BYTE> data(dwSize);

                    // load the version info
                    if (!GetFileVersionInfo(szFilename, NULL, dwSize, &data[0]))
                    {
                        TRACE("GetFileVersionInfo failed with error %d\n", GetLastError());
                        return false;
                    }
*/


                    //IntPtr hScannedModule =  Native.GetModuleHandle(moduleName);
                    //= (HMODULE)hScannedModule.FindResourcesEx();
                    //Native.ModuleInformation moduleInformation = new Native.ModuleInformation();
                    //Native.GetModuleInformation(process.Handle, modulePointers[index], out moduleInformation, (uint)(IntPtr.Size * (modulePointers.Length)));

                    // Convert to a normalized module and add it to our list
                    //Module module = new Module(moduleName, moduleInformation.lpBaseOfDll, moduleInformation.SizeOfImage);
                    //collectedModules.Add(module);
                }
            }

            //return collectedModules;
            return true;
        }
    }    
}
