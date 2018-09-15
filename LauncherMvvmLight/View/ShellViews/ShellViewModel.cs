using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LauncherMvvmLight.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using NLog;

namespace LauncherMvvmLight.View.ShellViews
{
    public class ShellViewModel : ViewModelBase
    {
        #region Private Fields
        private readonly IFrameNavigationService _navigationService;
        private static string AppTitle = "Excalibur Launcher Pad - V1";
        private static Logger logger = LogManager.GetCurrentClassLogger();        
        private DispatcherTimer timer = null;


        
       
        private List<Guid> _moduleIds;
        private DeviceInfoModel _selectedDevice;
        private DeviceInfoModel _defaultDevice;
        #endregion

        #region Public Properties
        public string WindowTitle { get; private set; }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        #endregion

        #region Public ICommands

        public RelayCommand FSUIPCBridgeCommand { get; set; }
        public RelayCommand ExcShowHexCommand { get; set; }
        public RelayCommand ExcMysticCommand { get; set; }
        public RelayCommand ExcMerlinCommand { get; set; }
        public RelayCommand ExcExaltCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ExcaliburCommand { get; set; }
        public RelayCommand ExcaliburContact { get; set; }
        public RelayCommand ExcaliburTech { get; set; }

        #endregion

        #region Constructor
        public ShellViewModel(IFrameNavigationService navigationService)
        //public ShellViewModel()
        {
            _navigationService = navigationService;

            FSUIPCBridgeCommand = new RelayCommand(FSUIPCBridgeCommandHandler);
            ExcShowHexCommand = new RelayCommand(ExcShowHex);
            ExcMysticCommand = new RelayCommand(ExcMystic);
            ExcMerlinCommand = new RelayCommand(ExcMerlin);
            ExcExaltCommand = new RelayCommand(ExcExalt);
            ExcaliburCommand = new RelayCommand(ExcExcalibur);
            ExcaliburContact = new RelayCommand(ExcExcaliburContact);
            ExcaliburTech = new RelayCommand(ExcExcaliburTech);


            logger.Log(LogLevel.Info, "--- PROGRAM STARTED ---");

            logger.Log(LogLevel.Debug, "New instance of SerialPort() is initialized.");

            // Get lists of settings objects
            try
            {
                
                logger.Log(LogLevel.Debug, "All lists of settings objects are loaded.");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }

            // Set default values
           
            ModuleIds = new List<Guid>();
            // Default category
            //_defaultModule = Modules[0]; // The default one is always the first
            //if (!_defaultModule.IsDefault) // first time?
            //    _defaultModule.IsDefault = true;
            // WindowTitle = AppTitle;

            logger.Log(LogLevel.Debug, "All default values are set. End of SerialCommViewModel() constructor!");

            //_navigationService.NavigateTo("Config");
        }
        #endregion

        #region Events

        private void TimerTick(object send, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
               
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Close port if port is open when user closes MainWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                
                    logger.Log(LogLevel.Debug, "SerialPort.Dispose() & SerialPort.Close() are executed on OnWindowClosing() method.");
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }
        }

        //[Provide(PropertyName = WellKnownProperties.DataFromShell)]
        public virtual string DataFromShell { get; set; }



        /// <summary>
        /// Gets or sets the module ids.
        /// </summary>
        /// <value>The module ids.</value>
        public List<Guid> ModuleIds
        {
            get { return _moduleIds; }
            set
            {
                _moduleIds = value;
            }
        }
        
        #endregion




        #region Private Methods
        

        private void StartTimer(int duration)
        {
            if (timer != null)
            {
                timer.Stop();
            }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(duration);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }


        /// <summary>
        /// Method to ...
        /// </summary>
        //public IntPtr MainWindowHandle { get; set; }
        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        private void ExcShowHex()
        {
            //_navigationService.NavigateTo("ExcContainer");

            try
            {
                string ExecutableFilePath = @"C:\Excalibur\Utilities\Showhex\showhex.exe";
                string Arguments = "";//@"-appArgs -application com.openspirit.rcp.dataselector.application";

                if (File.Exists(ExecutableFilePath))
                    System.Diagnostics.Process.Start(ExecutableFilePath, Arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            


            //TabItem nPage = new TabItem();
            //nPage.Header = "nouveau";
            ////TabPage.Items.Add(nPage);
            //ProcessStartInfo pInfo = new ProcessStartInfo("showhex.exe");
            //pInfo.WorkingDirectory = @"C:\Excalibur\Utilities\Showhex";
            //Process p = Process.Start(pInfo);

            /*
                        TabItem nPage = new TabItem();
                            WindowsFormsHost host = new WindowsFormsHost();
                            System.Windows.Forms.Panel p = new System.Windows.Forms.Panel();
                            host.Child = p;
                            nPage.Header = "nouveau";
                            nPage.Content = host;
                            TabPage.Items.Add(nPage);
                            Process proc = Process.Start(
                               new ProcessStartInfo()
                               {
                                   FileName = @"C:\Multi-langue\Multi-langue\bin\Debug\Multi-langue.exe",

                                   WindowStyle = ProcessWindowStyle.Normal
                               });
                            Thread.Sleep(1000);
                            SetParent(proc.MainWindowHandle, p.Handle);
            */
        }

        private void ExcMystic()
        {
            //_navigationService.NavigateTo("ExcContainer");

            try
            {
                string ExecutableFilePath = @"C:\Excalibur\Mystic\Mystic.exe";
                string Arguments = ""; //@"-appArgs -application com.openspirit.rcp.dataselector.application";

                if (File.Exists(ExecutableFilePath))
                    System.Diagnostics.Process.Start(ExecutableFilePath, Arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

       
        private void ExcMerlin()
        {
            //_navigationService.NavigateTo("ExcContainer");

            try
            {
                string ExecutableFilePath = @"C:\Excalibur\MerlinPlus\MerlinPlus.exe";
                string Arguments = ""; //@"-appArgs -application com.openspirit.rcp.dataselector.application";

                if (File.Exists(ExecutableFilePath))
                    System.Diagnostics.Process.Start(ExecutableFilePath, Arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ExcExalt()
        {
            //_navigationService.NavigateTo("ExcContainer");

            try
            {
                string ExecutableFilePath = @"C:\Excalibur\Exalt\Bin\Exalt.exe";
                string Arguments = ""; //@"-appArgs -application com.openspirit.rcp.dataselector.application";

                if (File.Exists(ExecutableFilePath))
                    System.Diagnostics.Process.Start(ExecutableFilePath, Arguments);
                else
                {
                 
                    if (MessageBox.Show("Exalt not found, do you want to download demo version from Excalibur web site?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start("https://www.mil-1553.com/applications"); 
                    }

                }

                

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ExcExcalibur()
        {
       
            try
            {

                string ExcaliburLink = @"https://www.mil-1553.com";
                string Arguments = ""; //@"-appArgs -application com.openspirit.rcp.dataselector.application";

                System.Diagnostics.Process.Start(ExcaliburLink, Arguments);
                                


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private void ExcExcaliburContact()
        {

            try
            {
                string ExcaliburLink = @"https://www.mil-1553.com/sales-support";
                string Arguments = ""; //@"-appArgs -application com.openspirit.rcp.dataselector.application";

                System.Diagnostics.Process.Start(ExcaliburLink, Arguments);



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private void ExcExcaliburTech()
        {

            try
            {
                string ExcaliburLink = @"https://www.mil-1553.com/technical-support";
                string Arguments = ""; //@"-appArgs -application com.openspirit.rcp.dataselector.application";

                System.Diagnostics.Process.Start(ExcaliburLink, Arguments);



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private string ReadSubKeyValue(string subKey, string key)
        {
            string str = string.Empty;

            using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(subKey))
            {
                if (registryKey != null)
                {
                    object ret = registryKey.GetValue(key);
                    if (ret != null)
                    {
                        str = registryKey.GetValue(key).ToString();
                        registryKey.Close();
                    }
                }
            }
            return str;
        }



        private List<string> GetFiles(string path, string pattern)
        {
            var files = new List<string>();

            try
            {
                files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                foreach (var directory in Directory.GetDirectories(path))
                    files.AddRange(GetFiles(directory, pattern));
            }
            catch (UnauthorizedAccessException) { }

            return files;
        }


        #endregion

        private void FSUIPCBridgeCommandHandler()
        {
            _navigationService.NavigateTo("FSUIPC");           
        }
    }
}
