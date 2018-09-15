using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LauncherMvvmLight.Domain;
using System.Linq;
using LauncherMvvmLight.Infrastructure.Util;
using LauncherMvvmLight.MessageInfrastructure;
using LauncherMvvmLight.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LauncherMvvmLight.Domain.Messages;
using LauncherMvvmLight.Domain.Services.DeviceCollectService;
using LauncherMvvmLight.Domain.Services.DeviceScannerService;
using NLog;
using System.Windows.Data;

namespace LauncherMvvmLight.View.ShellViews.DeviceDataView
{
    public class FileExportSettingsViewModel : ViewModelBase
    {



        #region Private Fields
        private static string ViewTitle = "System Data";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DispatcherTimer timer = null;
        private bool _isBusy = false;

        //string[,] dllsInfo = new string[10, 2] { { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" } };


        private SystemInfoModel _systemInfoDetail;
        private ObservableCollection<SystemInfoModel> _systemInfo;
        private IDataCollectSrv _serviceProxy;
        //private IVerifySrv _verfiySrvProxy;
        private string _userAppName;

        public string UserAppName
        {
            get
            {
                return _userAppName;
            }
            set
            {
                if (value != _userAppName)
                {
                    _userAppName = value;
                    RaisePropertyChanged("UserAppName");
                }
            }
        }

/*

        private ObservableCollection<dllDisplayInfoModel> _dllDisplayInfo;
        public ObservableCollection<dllDisplayInfoModel> DllDisplayInfo
        {
            get
            {
                return _dllDisplayInfo;
            }
            set
            {
                _dllDisplayInfo = value;
                RaisePropertyChanged("DllDisplayInfo");
            }
        }
*/

        private string _filterString { get; set; }
        private ICollectionView _dllInfoListView { get; set; }
        private ObservableCollection<ModuleDataModel> _dllInfoList;
        public ObservableCollection<ModuleDataModel> DllInfoList
        {
            get
            {
                //if (_dllInfoList != null)
                //{
                //    //For the car filtering
                //    this._dllInfoListView = CollectionViewSource.GetDefaultView(_dllInfoList);
                //    this._dllInfoListView.Filter = dllInfoListFilter;
                //}
                return _dllInfoList;
            }
            set
            {
                _dllInfoList = value;
                RaisePropertyChanged("DllInfoList");
            }
        }

        private bool dllInfoListFilter(object item)
        {
            ModuleDataModel dllItem = item as ModuleDataModel;

            if (dllItem.Name.Contains(_filterString))
            {
                return true;
            }
            else
            {
                return false;
            }
        }







        #endregion

        #region Public Properties  
        [Provide(PropertyName = WellKnownProperties.SelectedDevice)]
        public virtual string SelectedDevice { get; set; }
        public string WindowTitle { get; private set; }
        public List<String> DevicesNames { get; private set; }
        public bool EnableDisableSettings { get; private set; }

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
       

        
        public ObservableCollection<SystemInfoModel> SystemInfo
        {
            get { return _systemInfo; }
            set
            {
                _systemInfo = value;
                //RaisePropertyChanged("SystemInfo");
            }
        }

        #endregion

        #region Public ICommands

        public RelayCommand RelayCommandSample { get; private set; }
        public RelayCommand GetHostSystemInfo { get; set; }
        public RelayCommand VerifySystemInfo { get; set; }
        public RelayCommand OpenTechSupportLink { get; set; }
        #endregion

        #region Constructor
        public FileExportSettingsViewModel(IDataCollectSrv servPxy)
        {

            _serviceProxy = servPxy;//data service
            _systemInfo = new ObservableCollection<SystemInfoModel>();
            _systemInfoDetail = new SystemInfoModel();
            _filterString = "Exc";
            //DllInfoList = new ObservableCollection<ModuleDataModel>();



            UserAppName = "";
            GetHostSystemInfo = new RelayCommand(GetHostSystemInfoHandler);
            VerifySystemInfo = new RelayCommand(VerifySystemInfoHandler);
            
            OpenTechSupportLink = new RelayCommand(OpenTechSupportLinkHandler);

            logger.Log(LogLevel.Info, "--- FileExportSettingsViewModel:loaded ---");

            // Get lists of settings objects
            try
            {
                //Devices = ModuleDataModel.Instance.getDeviceLists();
                logger.Log(LogLevel.Debug, "All lists of settings objects are loaded.");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }

            // Set default values
            EnableDisableSettings = true;


            // We expect a message with some lists with changes.
            Messenger.Default.Register<UpdateSystemReportMessage>(this, UpdateSystemReportMessageHandler);

            logger.Log(LogLevel.Debug, "All default values are set. End of SerialCommViewModel() constructor!");

            Init();


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


        

        private void UpdateSystemReportMessageHandler(UpdateSystemReportMessage report)
        {
            ;

            //DllInfoList = report.systemInfoModelReport.ModuleDataList;
            DllInfoList = new ObservableCollection<ModuleDataModel>(report.systemInfoModelReport.ModuleDataList);

            // _dllInfoListView = CollectionViewSource.GetDefaultView(DllInfoListView);
            // _dllInfoListView.Filter = DllInfoListViewFilter;

            ;
            //handle new report;

        }
        /// <summary>
        /// Close view windows.
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
     #endregion

     #region Private Methods
        /// <summary>
        /// Init - specific module
        /// </summary>
        private void Init()
        {
            try
            {
               // CreateDynamicWPFGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }

        }
        /// <summary>
        /// Timer support util
        /// </summary>
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


      

        private void MakingNewDeviceChanges(MessageCommunicator changes)
        {
           
            //handle change
        }
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }
        #endregion   
        #region data_service
        /// <summary>
        /// Method to Read All Employees
        /// </summary>
        void GetHostSystemInfoHandler()
        {
            //SystemInfo.Clear();
            //foreach (var item in _serviceProxy.ExecutePSShellGetDeviceCmdlet())
            {
                ;//SystemInfo.Add(item);
            }

            //Get Fullsystem Data Report
            _serviceProxy.GetSystemFullData(UserAppName);

        }
        void VerifySystemInfoHandler()
        {
            //SystemInfo.Clear();
            //foreach (var item in _serviceProxy.ExecutePSShellGetDeviceCmdlet())
            {
                ;//SystemInfo.Add(item);
            }

            //Get Fullsystem Data Report
            _serviceProxy.GetSystemFullData(UserAppName);

        }
        private void OpenTechSupportLinkHandler()
        {
            //_navigationService.NavigateTo("ExcContainer");

            try
            {
                string TechSupportLink = @"https://www.mil-1553.com/technical-support";
                string Arguments = ""; //@"-appArgs -application com.openspirit.rcp.dataselector.application";

                    System.Diagnostics.Process.Start(TechSupportLink, Arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void OpenLinkHandler()
        {
            //_navigationService.NavigateTo("ExcContainer");

            try
            {
                string TechSupportLink = @"https://www.mil-1553.com/technical-support";
                string Arguments = ""; //@"-appArgs -application com.openspirit.rcp.dataselector.application";

                System.Diagnostics.Process.Start(TechSupportLink, Arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        #endregion
    }
}
