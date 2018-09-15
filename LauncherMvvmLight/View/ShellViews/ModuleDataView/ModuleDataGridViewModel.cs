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
using System.Linq;
using LauncherMvvmLight.Infrastructure.Util;
using LauncherMvvmLight.MessageInfrastructure;
using LauncherMvvmLight.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LauncherMvvmLight.Domain.Services.DeviceScannerService;

using NLog;


namespace LauncherMvvmLight.View.ShellViews.ModuleDataView
{

    public class ModuleDataGridViewModel : ViewModelBase
    {


        /*
        private ExcaliburDataService excaliburDataService = new ExcaliburDataService();

        public serviceAction()
        {
            this.excaliburDataService.GetEventCompleted += (s, e) =>
            {
                this.event =
                e.result;
            };
            this.excaliburDataService.GetEventsAsync();

        }
        */

        #region Private Fields
        private static string ViewTitle = "Module Data";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DispatcherTimer timer = null;
        private bool _isBusy = false;
        //private readonly IModuleInfoRepo _moduleInfoRepo;
        private IDeviceScannerService _serviceProxy;
        /// <summary>
        /// The declaration of the Employee object for Save and Messanger purpose
        /// </summary>

        //dynamic UI definition 
        Grid mExcDevicecGrid = new Grid();
        TextBlock[,] mTxtBlockA = new TextBlock[17, 11];

        #endregion
        
        #region Public Properties  
        [Provide(PropertyName = WellKnownProperties.SelectedDevice)]
        public virtual string SelectedDevice { get; set; }
        public string WindowTitle { get; private set; }
        public List<String> DevicesNames { get; private set; }
        public bool EnableDisableSettings { get; private set; }
        public ModuleInformationModel SelectedModules; // = new ModuleInformationModel();
        public ModuleInfoModel singleModuleInfoModel; // = new ModuleInformationModel();



        private ObservableCollection<ModuleInfoModel> _modulesOnly;

        public ObservableCollection<ModuleInfoModel> ModulesOnly
        {
            get
            {
                return _modulesOnly;
            }
            set
            {
                _modulesOnly = value;
                RaisePropertyChanged("ModulesOnly");
            }
        }



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


        private string _message;
        public string Message
        {
            private set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
            get
            {
                return _message;
            }
        }



        private DeviceInfoModel _DevInfo;
        public DeviceInfoModel DevInfo
        {
            get { return _DevInfo; }
            set
            {
                _DevInfo = value;
                RaisePropertyChanged("DevInfo");
            }
        }

        string _DevName;

        public string DevName
        {
            get { return _DevName; }
            set
            {
                _DevName = value;
                RaisePropertyChanged("DevName");
            }
        }

        private ObservableCollection<DeviceInfoModel> _devices;

        public ObservableCollection<DeviceInfoModel> Devices
        {
            get
            {
                return _devices;
            }            
            set
            {
                _devices = value;
                RaisePropertyChanged("Devices");
            }            
        }

        private ObservableCollection<DeviceInfoModel> _devicesInfo;

        public ObservableCollection<DeviceInfoModel> DevicesInfo
        {
            get
            {
                return _devicesInfo;
            }
            set
            {
                _devicesInfo = value;
                RaisePropertyChanged("DevicesInfo");
            }
        }
        #endregion

        #region Public ICommands

        public RelayCommand RelayCommandSample { get; private set; }
        public RelayCommand ReadAllCommand { get; set; }
        public RelayCommand<DeviceInfoModel> SaveCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand<DeviceInfoModel> SendDeviceCommand { get; set; }


        #endregion

        #region Constructor
        public ModuleDataGridViewModel(IDeviceScannerService servPxy)
        {

            _serviceProxy = servPxy;//data service
            Devices = new ObservableCollection<DeviceInfoModel>();
            DevicesInfo = new ObservableCollection<DeviceInfoModel>();
            SelectedModules = new ModuleInformationModel();
            GetDevices();

            ModulesOnly = new ObservableCollection<ModuleInfoModel>();
            singleModuleInfoModel = new ModuleInfoModel();
           
            _message = "Testing";

            RelayCommandSample = new RelayCommand(RelayCommandSampleExcFunc, RelayCommandSampleCanExcFlag);

            ReadAllCommand = new RelayCommand(GetDevices);
            SaveCommand = new RelayCommand<DeviceInfoModel>(SaveDevice);
            SearchCommand = new RelayCommand(SearchDevice);
            SendDeviceCommand = new RelayCommand<DeviceInfoModel>(SendDeviceInfo);
            ReceiveDeviceInfo();

            logger.Log(LogLevel.Info, "--- ModuleDataGridViewModel:loaded ---");

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
            Messenger.Default.Register<MessageCommunicator>(this, MakingNewDeviceChanges);

            Messenger.Default.Register<DeviceSelectedMessage>(this, DeviceSelectedMessageHandler);


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


       // [LongRunningOperation(Message = "Scan takes time !!! ")]
        void RelayCommandSampleExcFunc()
        {
            //Devices = ModuleDataModel.Instance.getDeviceLists();
            //OnPropertyChanged("Devices");

        }

        private bool RelayCommandSampleCanExcFlag()
        {
            return true;//SelectedCategory != null;
        }

        private void MakingNewDeviceChanges(MessageCommunicator changes)
        {
           
            //handle change
        }

        private void DeviceSelectedMessageHandler(DeviceSelectedMessage newMsg)
        {

            SelectedModules = newMsg.MsgPayload;

            ModulesOnly.Clear();
            ModulesOnly.Add(SelectedModules.Modules[0]);
            ModulesOnly.Add(SelectedModules.Modules[1]);
            ModulesOnly.Add(SelectedModules.Modules[2]);
            ModulesOnly.Add(SelectedModules.Modules[3]);

            RaisePropertyChanged("ModulesOnly");

            return;
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

        #region DynamiUI
        private void deviceGrid_Leftclick(object sender, MouseButtonEventArgs e)
        {
            //not sure how too use this function at present using  void txtBlock_Leftclick(object sender, RoutedEventArgs e)
        }
        private void AddGridHeaders()
        {

            mExcDevicecGrid.ShowGridLines = true;

            // Add column headers  
            mTxtBlockA[0, 0].Text = "Device";
            mTxtBlockA[0, 1].Text = "Card Type";
            mTxtBlockA[0, 2].Text = "IP Address";
            mTxtBlockA[0, 3].Text = "USB s/n";
            mTxtBlockA[0, 4].Text = "Slot/ID";
            mTxtBlockA[0, 5].Text = "Use IRQ";
            mTxtBlockA[0, 6].Text = "IRQ";
            mTxtBlockA[0, 7].Text = "Mem 1";
            mTxtBlockA[0, 8].Text = "Mem 2";
            mTxtBlockA[0, 9].Text = "ID";

            for (int col = 0; col < 10; col++)
            {
                mTxtBlockA[0, col].FontSize = 14;
                mTxtBlockA[0, col].FontWeight = FontWeights.Bold;
                mTxtBlockA[0, col].Foreground = new SolidColorBrush(Colors.Green);
                mTxtBlockA[0, col].VerticalAlignment = VerticalAlignment.Top;
                mTxtBlockA[0, col].HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(mTxtBlockA[0, col], 0);
                Grid.SetColumn(mTxtBlockA[0, col], col);

                mExcDevicecGrid.Children.Add(mTxtBlockA[0, col]);

            }

            //Add device numbers
            for (int row = 0; row < 16; row++)
            {
                mTxtBlockA[row + 1, 0].Text = row.ToString();
                mTxtBlockA[row + 1, 0].FontSize = 14;
                mTxtBlockA[row + 1, 0].FontWeight = FontWeights.Bold;
                mTxtBlockA[row + 1, 0].Foreground = new SolidColorBrush(Colors.Green);
                mTxtBlockA[row + 1, 0].VerticalAlignment = VerticalAlignment.Top;
                mTxtBlockA[row + 1, 0].HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(mTxtBlockA[row + 1, 0], row + 1);
                Grid.SetColumn(mTxtBlockA[row + 1, 0], 0);
                mExcDevicecGrid.Children.Add(mTxtBlockA[row + 1, 0]);
            }

            //Assign TextBlock for all cells in grid (then we can just update the text using .Text
            for (int row = 1; row < 17; row++)
            {
                for (int col = 1; col < 10; col++)
                {
                    mTxtBlockA[row, col].Text = "";
                    mTxtBlockA[row, col].FontSize = 12;
                    mTxtBlockA[row, col].FontWeight = FontWeights.Bold;
                    //mTxtBlockA[row, col].Foreground = new SolidColorBrush(Colors.Green);
                    mTxtBlockA[row, col].VerticalAlignment = VerticalAlignment.Center;
                    mTxtBlockA[row, col].HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(mTxtBlockA[row, col], row);
                    Grid.SetColumn(mTxtBlockA[row, col], col);
                    mExcDevicecGrid.Children.Add(mTxtBlockA[row, col]);
                }
            }


        }
        private void CreateDynamicWPFGrid()
        {
            // Create the Grid  
            mExcDevicecGrid.Width = 700;
            mExcDevicecGrid.HorizontalAlignment = HorizontalAlignment.Left;
            mExcDevicecGrid.VerticalAlignment = VerticalAlignment.Top;
            mExcDevicecGrid.ShowGridLines = true;
            mExcDevicecGrid.Background = new SolidColorBrush(Colors.LightSteelBlue);
            mExcDevicecGrid.Height = 1000;

            // Create Columns  
            for (int i = 0; i < 11; i++)
            {
                ColumnDefinition GridCol = new ColumnDefinition();
                if (i > 0 && i < 3)
                    GridCol.Width = new GridLength(130);
                else
                    GridCol.Width = new GridLength(60);

                mExcDevicecGrid.ColumnDefinitions.Add(GridCol);
            }

            // Create Rows  
            for (int i = 0; i < 17; i++)
            {
                RowDefinition GridRow = new RowDefinition();
                GridRow.Height = new GridLength(25);
                mExcDevicecGrid.RowDefinitions.Add(GridRow);
            }

            AddGridHeaders();

            // Display grid into a Window  
            //MyScr .Content = mExcDevicecGrid;
            //DeviceGridScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            
            mExcDevicecGrid.MouseLeftButtonDown += new MouseButtonEventHandler(deviceGrid_Leftclick);



        }
        #endregion


        #region data_service
        /// <summary>
        /// Method to Read All Employees
        /// </summary>
        void GetDevices()
        {
            //Devices.Clear();
            //Devices = _serviceProxy.GetDevices("registery");
            //RaisePropertyChanged("Devices");

            Devices.Clear();
            Devices = _serviceProxy.GetDevices();
            RaisePropertyChanged("DevicesInfo");

            //foreach (var item in _serviceProxy.GetDevices("registery"))
            //{
            //    Devices.Add(item);
            //}
            //RaisePropertyChanged("Devices");
        }

        /// <summary>
        /// Method to Save Employees. Once the Employee is added in the database
        /// it will be added in the Employees observable collection and Property Changed will be raised
        /// </summary>
        /// <param name="emp"></param>
        void SaveDevice(DeviceInfoModel dev)
        {
           // DevInfo.DevNo = _serviceProxy.CreateDevice(dev);
           // if (DevInfo.DevNo != 0)
           // {
           //     Devices.Add(DevInfo);
           //     RaisePropertyChanged("DevInfo");
           // }
        }

        /// <summary>
        /// The method to search Employee baseed upon the EmpName
        /// </summary>
        void SearchDevice()
        {
            /*
            Devices.Clear();
            var Res = from e in _serviceProxy.GetDevices()
                      where e.DevName.StartsWith(DevName)
                      select e;
            foreach (var item in Res)
            {
                Devices.Add(item);
            }
            */
        }

        /// <summary>
        /// The method to send the selected Employee from the DataGrid on UI
        /// to the View Model
        /// </summary>
        /// <param name="emp"></param>
        void SendDeviceInfo(DeviceInfoModel dev)
        {
            if (dev != null)
            {
                Messenger.Default.Send<MessageCommunicator>(new MessageCommunicator()
                {
                    Dev = dev
                });
            }
        }

        /// <summary>
        /// The Method used to Receive the send Employee from the DataGrid UI
        /// and assigning it the the EmpInfo Notifiable property so that
        /// it will be displayed on the other view
        /// </summary>
        void ReceiveDeviceInfo()
        {
            if (DevInfo != null)
            {
                Messenger.Default.Register<MessageCommunicator>(this, (dev) => {
                    this.DevInfo = dev.Dev;
                });
            }
        }
#endregion



    }
}
