using System;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LauncherMvvmLight.Domain.Messages;
using LauncherMvvmLight.Domain.Services.DeviceCollectService;
using LauncherMvvmLight.MessageInfrastructure;
using LauncherMvvmLight.Model;
using NLog;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Linq;

namespace LauncherMvvmLight.View.ConfigViews
{
    public class ConfigViewModel : ViewModelBase
    {
        #region Fields
        private IDBSrv _dbService;
        private IFrameNavigationService _navigationService;
        private bool _isLoaded = false;
        private static string ViewTitle = "Config Data";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DispatcherTimer timer = null;
        private DeviceOnlyInfoModel _SelectedDevInfo;
        public List<DeviceTableDbModel> _devicesListPtr { get; private set; }
        public List<DeviceConfigTableModel> _devicesConfigListPtr { get; private set; }
        public String DisplayDetailConfig { get; private set; }
        public RelayCommand <DeviceInfoModel> SaveConfigCommand { get; set; }
        public RelayCommand<DeviceInfoModel> CancelConfigCommand { get; set; }
        public DeviceOnlyInfoModel selectedCard;


        #endregion Fields

        #region Properties

        public string IdType { get; set; }

        
        private String _lblSelectedDevice;
        public String LblSelectedDevice
        {
            get { return _lblSelectedDevice; }
            set
            {
                _lblSelectedDevice = value;

                RaisePropertyChanged("lblSelectedDevice");
            }
        }

        private String _cmbSelectedIdTypes;
        public String CmbSelectedIdTypes
        {
            get { return _cmbSelectedIdTypes; }
            set
            {
                _cmbSelectedIdTypes = value;

                RaisePropertyChanged("cmbIdType");
            }
        }

        
        private string _radEthernet1;
        public string radEthernet1
        {
            get { return _radEthernet1; }
            set
            {
                _radEthernet1 = value;

                RaisePropertyChanged("radEthernet1");
            }
        }
        

        private bool _radEthernetIsChecked;
        public bool radEthernetIsChecked
        {
            get { return radEthernetIsChecked; }
            set
            {
                _radEthernetIsChecked = value;

                RaisePropertyChanged("radEthernet1");
                RaisePropertyChanged("txtBoxIPAddress");
            }
        }

        private bool _radEthernet1_Checked;
        public bool radEthernet1_Checked
        {
            get { return radEthernet1_Checked; }
            set
            {
                _radEthernet1_Checked = value;

                RaisePropertyChanged("radEthernet1");
                RaisePropertyChanged("txtBoxIPAddress");
            }
        }

        

        private string _radUSB1;
        public string radUSB1
        {
            get { return _radUSB1; }
            set
            {
                _radUSB1 = value;

                RaisePropertyChanged("radUSB1");
            }
        }


        private bool _radUSBIsChecked;
        public bool radUSBIsChecked
        {
            get { return radUSBIsChecked; }
            set
            {
                _radUSBIsChecked = value;

                RaisePropertyChanged("radUSB1");
                RaisePropertyChanged("txtSerialNumber");
            }
        }

        private bool _radUSB1_Checked;
        public bool radUSB1_Checked
        {
            get { return radUSB1_Checked; }
            set
            {
                _radUSB1_Checked = value;

                RaisePropertyChanged("radUSB1");
                RaisePropertyChanged("txtSerialNumber");
            }
        }


        private String _ipADDCommand;
        public String ipADDCommand
        {
            get { return _ipADDCommand; }
            set
            {
                _ipADDCommand = value;

                RaisePropertyChanged("lbIPAddress");
            }
        }

        

        private DeviceConfigTableModel _lstCardsSelectedItem;
        public DeviceConfigTableModel LstCardsSelectedItem
        {
            get { return _lstCardsSelectedItem; }
            set
            {
                if (value == _lstCardsSelectedItem)
                    return;

                _lstCardsSelectedItem = value;

                RaisePropertyChanged("LstCardsSelectedItem");

                LblSelectedDevice = "Device " + selectedCard.dev + " :: " + _lstCardsSelectedItem.DeviceTypeName;

                // selection changed - do Handler
                UpdateConfigPageDynamicDisplay(_lstCardsSelectedItem.DeviceTypeName, 
                                            _lstCardsSelectedItem.DeviceTypeDisplayConfig, 
                                            _lstCardsSelectedItem.DipConfig, 
                                            selectedCard.IPAdd,
                                            selectedCard.SlotID,
                                            selectedCard.USBSN,
                                            selectedCard.ConnectType);
                
            }
        }

       
           

        private ObservableCollection<DeviceTableDbModel> _devicesListDB;

        public ObservableCollection<DeviceTableDbModel> DevicesListDB
        {
            get
            {
                return _devicesListDB;
            }
            set
            {
                _devicesListDB = value;
                RaisePropertyChanged("DevicesListDB");
            }
        }

        private ObservableCollection<DeviceConfigTableModel> _devicesConfigListDB;
        private object groupLstCards;
        private object lstCards;

        public ObservableCollection<DeviceConfigTableModel> DevicesConfigListDB
        {
            get
            {
                return _devicesConfigListDB;
            }
            set
            {
                _devicesConfigListDB = value;
                RaisePropertyChanged("DevicesConfigListDB");
            }
        }

        public RelayCommand SelectionChangedLbx { get; private set; }
        void SelectionChangedLbxHandler()
        {
            
        }

        public DeviceOnlyInfoModel SelectedDevInfo
        {
            get { return _SelectedDevInfo; }
            set
            {
                _SelectedDevInfo = value;
                RaisePropertyChanged("SelectedDevInfo");
            }
        }


        private ObservableCollection<DeviceOnlyInfoModel> _selectedDeviceInfo;
        public ObservableCollection<DeviceOnlyInfoModel> selectedDeviceInfo
        {
            get
            {
                return _selectedDeviceInfo;
            }
            set
            {
                _selectedDeviceInfo = value;
                RaisePropertyChanged("selectedDeviceInfo");
            }
        }
        #endregion Properties

        #region Initialization
        public ConfigViewModel(IFrameNavigationService navigationService, IDBSrv dbService)
        {
           
            DisplayDetailConfig = "";
            SelectionChangedLbx = new RelayCommand(SelectionChangedLbxHandler);
       
            _navigationService = navigationService;
            _dbService = dbService;
            _devicesListPtr = _dbService.GetDeviceTableData();
            _devicesConfigListPtr = _dbService.GetConfigTypeList();
            DevicesListDB = new ObservableCollection<DeviceTableDbModel>(_devicesListPtr);
            DevicesConfigListDB = new ObservableCollection<DeviceConfigTableModel>(_devicesConfigListPtr);

            RegisterMessenger();
            SaveConfigCommand = new RelayCommand<DeviceInfoModel>(_SaveConfig);
            CancelConfigCommand = new RelayCommand<DeviceInfoModel>(_CancelConfig);

           DeviceOnlyInfoModel SelectedCard = (DeviceOnlyInfoModel)navigationService.Parameter;
           selectedCard = (DeviceOnlyInfoModel)navigationService.Parameter;

            //if(SelectedCard.strCardType == "")
            //{
            //    SelectedCard.strCardType = "Undefined";
            //}


            for (int i = 0; i < DevicesConfigListDB.Count; i++)
            {
                if ( (DevicesConfigListDB[i].DeviceTypeName == SelectedCard.strCardType) || (DevicesConfigListDB[i].DeviceTypeName == SelectedCard.ExcConfigDeviceType)||
                     ((DevicesConfigListDB[i].DeviceTypeName == "Undefined" )&&(SelectedCard.strCardType == "")))
                {
                    LstCardsSelectedItem = DevicesConfigListDB[i];
                    CmbSelectedIdTypes = SelectedCard.SlotID;
                    
                    break;
                }
            }

        }

        void _SaveConfig(DeviceInfoModel dev)
        {
            //_navigationService.NavigateTo("Shell");

            selectedCard.strCardType = LstCardsSelectedItem.DeviceTypeName;


            Messenger.Default.Send<DeviceSlectedConfigMsg>(new DeviceSlectedConfigMsg(selectedCard));
            _navigationService.GoBack();
        }

        void _CancelConfig(DeviceInfoModel dev)
        {
            //_navigationService.NavigateTo("Shell");
            _navigationService.GoBack();
        }


        private void RegisterMessenger()
        {

            //Send out a message to see if another VM has params we need
            //Messenger.Default.Send<DeviceSlectedConfigMsg>(new DeviceSlectedConfigMsg(deviceInfo =>
            //{
            //    if (deviceInfo != null)
            //    {
            //        this.SelectedDevInfo = deviceInfo;
            //    }
            //}));

            //Register any message pipes
            Messenger.Default.Register<DeviceSlectedConfigMsg>(this, msg =>
            {              
                this.SelectedDevInfo = msg.Payload;
                //LstCardsSelectedItem = this.SelectedDevInfo;
                //if (this.SelectedDevInfo.strCardType == "")
                //{
                //    this.SelectedDevInfo.strCardType = "Undefined";
                //}

                for (int i = 0; i < DevicesConfigListDB.Count; i++)
                {
                       //if ((DevicesConfigListDB[i].DeviceTypeName == this.SelectedDevInfo.strCardType) || (DevicesConfigListDB[i].DeviceTypeName == this.SelectedDevInfo.ExcConfigDeviceType))


                        if ((DevicesConfigListDB[i].DeviceTypeName == this.SelectedDevInfo.strCardType) || (DevicesConfigListDB[i].DeviceTypeName == this.SelectedDevInfo.ExcConfigDeviceType) ||
                                    ((DevicesConfigListDB[i].DeviceTypeName == "Undefined") && (this.SelectedDevInfo.strCardType == "")))


                        {
                            LstCardsSelectedItem = DevicesConfigListDB[i];
                            CmbSelectedIdTypes = this.SelectedDevInfo.SlotID;

                            break;
                        }
                }


            });
        }
        #endregion Initialization

        #region Methods        
        public void UpdateConfigPageDynamicDisplay(string deviceDisplayName, string deviceDisplayConfig, 
                                                   string dipConfig, string ipADD, string SlotID, string USBSN, string ConnectType)
        {
            
            DisplayDetailConfig = deviceDisplayName + "_" + deviceDisplayConfig + "_" + dipConfig + "_" + ipADD  + "_" + SlotID  + "_" + USBSN  + "_" +  ConnectType;


            MessengerInstance.Send<NotificationMessage>(new NotificationMessage(DisplayDetailConfig));
          
        }

        public void CallUpdateConfigPageDynamicDisplay()
        {
            // Some code
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
        #endregion Methods

            }

   
}
