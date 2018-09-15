using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using LauncherMvvmLight.Domain.Messages;
using LauncherMvvmLight.Domain.Services.DeviceScannerService;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using NLog;
using System.Collections;
using Microsoft.Win32;

namespace LauncherMvvmLight.View.ShellViews.DeviceDataView
{

    public class DeviceDataGridViewModel : ViewModelBase
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
        private IFrameNavigationService _navigationService;

        private bool _isLoaded = false;
        private static string ViewTitle = "Device Data";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DispatcherTimer timer = null;
        private bool _isBusy = false;
        //private readonly IModuleInfoRepo _moduleInfoRepo;
        private IDeviceScannerService _serviceProxy;
        private DeviceOnlyInfoModel _deviceOnlyInfoModel = null;
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

        private ObservableCollection<DeviceOnlyInfoModel> _devicesOnly;
        private int NumberOfItemsSelected;

        public ObservableCollection<DeviceOnlyInfoModel> DevicesOnly
        {
            get
            {
                return _devicesOnly;
            }
            set
            {
                _devicesOnly = value;
                RaisePropertyChanged("DevicesOnly");
            }
        }
        #endregion

        #region Public ICommands

        public RelayCommand RelayCommandSample { get; private set; }
        public RelayCommand ReadAllCommand { get; set; }
        public RelayCommand<DeviceInfoModel> SaveCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand<DeviceInfoModel> SendDeviceCommand { get; set; }
        public RelayCommand<IList> DgMouseDoubleClickCommand { get; private set; }
        public RelayCommand<IList> ClickCommand { get; private set; }
        public RelayCommand<IList> DeviceHighLighedCommand { get; set; }
        

        #endregion
        #region ICommand_handelrs


        #endregion

        #region Constructor
        public DeviceDataGridViewModel(IDeviceScannerService servPxy, IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _serviceProxy = servPxy;//data service
            Devices = new ObservableCollection<DeviceInfoModel>();
            DevicesOnly = new ObservableCollection<DeviceOnlyInfoModel>();
            SelectedModules = new ModuleInformationModel();

            GetDevices();

            _message = "Testing";


            //ClickCommand = new RelayCommand<IList>(items =>
            //{
            //    this._deviceOnlyInfoModel = (DeviceOnlyInfoModel)items[0];
               
            //});

            DgMouseDoubleClickCommand = new RelayCommand<IList>(items =>
            {
               
                if (items == null)
                {
                    NumberOfItemsSelected = 0;
                    return;
                }

                if (items.Count == 1)
                {
                    
                    this._deviceOnlyInfoModel = (DeviceOnlyInfoModel)items[0];
                    _navigationService.NavigateTo("Config", _deviceOnlyInfoModel);
                    Messenger.Default.Send<DeviceSlectedConfigMsg>(new DeviceSlectedConfigMsg((DeviceOnlyInfoModel)items[0]));
                }

            });


            RelayCommandSample = new RelayCommand(RelayCommandSampleExcFunc, RelayCommandSampleCanExcFlag);

            ReadAllCommand = new RelayCommand(GetDevices);
            
            SaveCommand = new RelayCommand<DeviceInfoModel>(SaveDevice);
            SearchCommand = new RelayCommand(SearchDevice);
            SendDeviceCommand = new RelayCommand<DeviceInfoModel>(SendDeviceInfo);
            ReceiveDeviceInfo();
            DeviceHighLighedCommand = new RelayCommand<IList>(DeviceHighLighedCommandHandler);

            logger.Log(LogLevel.Info, "--- DevicedataGridViewModel:loaded ---");

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

            logger.Log(LogLevel.Debug, "All default values are set. End of SerialCommViewModel() constructor!");

            Init();


        }

        private void RegisterMessenger()
        {
            //Register any messages
            Messenger.Default.Register<MessageCommunicator>(this, msg =>
            {
                
            });
            //Register any messages
            Messenger.Default.Register<DeviceSlectedConfigMsg>(this, msg =>
            {
                if (this._deviceOnlyInfoModel != null)
                {
                    msg.Callback(this._deviceOnlyInfoModel);
                    this._deviceOnlyInfoModel = null;
                }
            });
        }

        public void Initialize()
        {
            // TODO: Add your initialization code here 
            // This method is only called when the application is running
        }

        public void OnLoaded()
        {
            if (!_isLoaded)
            {
                // TODO: Add your loaded code here 
                _isLoaded = true;
            }
        }

        public void OnUnloaded()
        {
            if (_isLoaded)
            {
                // TODO: Add your cleanup/unloaded code here 
                _isLoaded = false;
            }
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

        public void Row_DoubleClickHandler(DataGridRow row)
        {
            _navigationService.NavigateTo("Config");
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
        void DeviceHighLighedCommandHandler(IList selectedItems)
        {

            if (selectedItems.Count == 1)
            {

                this._deviceOnlyInfoModel = (DeviceOnlyInfoModel)selectedItems[0];
                int devNum = this._deviceOnlyInfoModel.dev;

                //  string ss = Devices[devNum].Modules[0].ModuleName; //test


                if (SelectedModules.Modules == null)
                {
                    SelectedModules.Modules = new List<ModuleInfoModel>();
                    for (int mod = 0; mod < 4; mod++)
                    {
                        SelectedModules.Modules.Add(new ModuleInfoModel());
                    }
                }


                for (int mod = 0; mod < 4; mod++)
                {
                    //int a = Devices[devNum].Modules.Count;
                   // int b = this._devices[devNum].Modules.Count;
                    

                    if ((Devices[devNum].Modules.Count == 0) || (Devices[devNum].Modules.Count == mod))
                    {
                        SelectedModules.Modules[mod].ModuleName = "No Module";
                        SelectedModules.Modules[mod].ModuleType = 0;
                        SelectedModules.Modules[mod].mod = 0;
                        SelectedModules.Modules[mod].modStatus = false;
                        SelectedModules.Modules[mod].FirmwareVer = "";
                        SelectedModules.Modules[mod].HardwareVer = "";
                    }
                    else
                    {
                        SelectedModules.Modules[mod].ModuleName = Devices[devNum].Modules[mod].ModuleName;
                        SelectedModules.Modules[mod].ModuleType = Devices[devNum].Modules[mod].ModuleType;
                        SelectedModules.Modules[mod].mod = Devices[devNum].Modules[mod].mod;
                        SelectedModules.Modules[mod].modStatus = Devices[devNum].Modules[mod].modStatus;
                        SelectedModules.Modules[mod].FirmwareVer = Devices[devNum].Modules[mod].FirmwareVer;
                        SelectedModules.Modules[mod].HardwareVer = Devices[devNum].Modules[mod].HardwareVer;
                    }

                }




                Messenger.Default.Send<DeviceSelectedMessage>(new DeviceSelectedMessage()
                {
                    MsgPayload = SelectedModules
                });




            }


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
            Devices.Clear();
            Devices = _serviceProxy.GetDevices();
            //RaisePropertyChanged("Devices");

            DevicesOnly.Clear();
            DevicesOnly = _serviceProxy.GetDevicesOnly();
            
            //RaisePropertyChanged("Devices");
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
           // int cc = _devices.Count;
           // _devices[0].strCardType = "EXC-4000PCIe";
           // _devicesOnly[0].strCardType = "EXC-4000PCIe";
           // Devices[0].strCardType = "EXC-4000PCIe";
           // DevicesOnly[0].strCardType = "EXC-4000PCIe";

            Devices = _serviceProxy.GetDevices();
            DevicesOnly = _serviceProxy.GetDevicesOnly();

            //DeviceScanner ds = new DeviceScanner();
            //LauncherMvvmLight.Domain.Services.DeviceScannerService.DeviceScanner.sRegisteryDevices.

            RaisePropertyChanged("DevicesOnly");
            RaisePropertyChanged("Devices");

            SaveToRegistry();



            // DevInfo.DevNo = _serviceProxy.CreateDevice(dev);
            // if (DevInfo.DevNo != 0)
            // {
            //     Devices.Add(DevInfo);
            //     RaisePropertyChanged("DevInfo");
            // }
        }

        public void SaveToRegistry()
        {

            string key;

            for (int dev = 0; dev < 16; dev++)
            {

                if (_devices[dev].strCardType == null || _devices[dev].strCardType == "Undefined")
                    continue;

                string subKey = "SYSTEM\\CurrentControlSet\\Services\\Excx64\\Devices\\ExcaliburDevice\\" + dev.ToString();

                key = "CardType";
                WriteSubKeyValue(subKey, key, _devices[dev].cardType);
                
                if ((_devices[dev].strCardType == "DAS-429UNET/RTx") || (_devices[dev].strCardType == "EXC-9800MACC-II") || (_devices[dev].strCardType == "EXC-1553UNET/Px")
                      || (_devices[dev].strCardType == "ES-1553RUNET/Px") || (_devices[dev].strCardType == "ES-1553UNET/Px"))
                {
                    if (_devices[dev].ConnectType == "USB")
                    {
                        key = "ConnectType";
                        WriteSubKeyValue(subKey, key, _devices[dev].ConnectType);

                        key = "Serial_Number";
                        WriteSubKeyValue(subKey, key, _devices[dev].USBSN);

                        //key = "ProductID";
                        //WriteSubKeyValue(subKey, key, _devices[dev].ProductID);

                        //key = "VendorID";
                        //WriteSubKeyValue(subKey, key, _devices[dev].VendorID);
                    }
                    else if (_devices[dev].ConnectType == "NET")
                    {
                        key = "ConnectType";
                        WriteSubKeyValue(subKey, key, _devices[dev].ConnectType);

                        key = "IPAddress";
                        WriteSubKeyValue(subKey, key, _devices[dev].IPAdd);

                        key = "MACAddress";
                        WriteSubKeyValue(subKey, key, _devices[dev].MACAdress);

                        key = "UDPPort";
                        WriteSubKeyValue(subKey, key, _devices[dev].UDPPort);

                    }

                    key = "DeviceType";
                    WriteSubKeyValue(subKey, key, _devices[dev].strCardType);

                    //key = "IsRemote";
                    //WriteSubKeyValue(subKey, key, _devices[dev].isRemote);

                    //key = "DebugTheseModules";
                    //WriteSubKeyValue(subKey, key, _devices[dev].DebugTheseModules);


                }
                else
                {

                    if ((_devices[dev].strCardType == "EXC-4000PCI")
                        || (_devices[dev].strCardType == "EXC-4000cPCI") || (_devices[dev].strCardType == "EXC-4000PCIe")
                        || (_devices[dev].strCardType == "EXC-4000PCIe64") || (_devices[dev].strCardType == "EXC-4000P104plus") || (_devices[dev].strCardType == "EXC-4500ccVPX"))

                    {
                        _devices[dev].ExcConfigDeviceType = "4000PCI Series";
                        _devices[dev].ExtendedDeviceType = _devices[dev].strCardType;
                        _devices[dev].strCardType = "4000PCI";
                    }

                    key = "DeviceType";
                    WriteSubKeyValue(subKey, key, _devices[dev].strCardType);

                    key = "ExtendedDeviceType";
                    WriteSubKeyValue(subKey, key, _devices[dev].ExtendedDeviceType);

                    key = "ExcConfigDeviceType";
                    WriteSubKeyValue(subKey, key, _devices[dev].ExcConfigDeviceType);

                    int num;
                    if (_devices[dev].SlotID != "")
                    {
                        num = Convert.ToInt32(_devices[dev].SlotID);
                        key = "ID";
                        WriteSubKeyValue(subKey, key, num);
                    }

                    if (_devices[dev].Mem1 != "")
                    {
                        key = "Mem";
                        num = Convert.ToInt32(_devices[dev].Mem1);
                        WriteSubKeyValue(subKey, key, num);
                    }

                    if (_devices[dev].Mem2 != "")
                    {
                        key = "MemExtra";
                        num = Convert.ToInt32(_devices[dev].Mem2);
                        WriteSubKeyValue(subKey, key, num);

                    }

                    if (_devices[dev].USBSN != "")
                    {
                        key = "Serial_Number";
                        num = Convert.ToInt32(_devices[dev].USBSN);
                        WriteSubKeyValue(subKey, key, num);
                    }

                    key = "IRQ";
                    if (_devices[dev].IRQ != "")
                    {
                        num = Convert.ToInt32(_devices[dev].IRQ);
                        WriteSubKeyValue(subKey, key, num);
                    }
                   
               }

            }

            MessageBox.Show("The Device Information has been saved to theregistery?", "Confirm", MessageBoxButton.OK);
        }
        static void WriteSubKeyValue(string subKey, string key, object Value)

        {

            string str = string.Empty;
            RegistryKey registryKey;

            if (Value == null)
                return;
       
            using (registryKey = Registry.LocalMachine.OpenSubKey(subKey, true))
            {
                if (registryKey == null)
                    registryKey = Registry.LocalMachine.CreateSubKey(subKey);


                if (registryKey != null)
                {
                    registryKey.SetValue(key, Value);
                    registryKey.Close();
                }

            }




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
