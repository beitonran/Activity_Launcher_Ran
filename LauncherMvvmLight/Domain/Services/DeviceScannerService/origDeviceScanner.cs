using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using excaliburST;
using LauncherMvvmLight.Model;


namespace LauncherMvvmLight.Domain.Services.DeviceScannerService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class DeviceScanner
    {

        const string card_4000 = "Exc4000PCI";
        const string card_UNET = "ExcUNET";
        const string card_AFDX = "ExcAFDX";
        const string mod_429 = "-Mod429";
        const string mod_1553 = "-Mod1553";
        const int AFDX_BOARD_TYPE = 0xE664;

        // Device Bus types found in PC
        const int EXC_BUS_UNDEFINED = 32;
        const int EXC_BUS_PCI = 33;
        const int EXC_BUS_PCMCIA = 34;
        const int EXC_BUS_ISA = 35;

        // Board Types found in PC
        const int EXC_BOARD_UNDEFINED = 0;
        const int EXC_BOARD_PCEP = 1;
        const int EXC_BOARD_PCMCH = 2;
        const int EXC_BOARD_1553PCIEP = 3;
        const int EXC_BOARD_PCIPX = 4;
        const int EXC_BOARD_429MX = 5;
        const int EXC_BOARD_MAGIC = 6;
        const int EXC_BOARD_429PCI = 7;
        const int EXC_BOARD_429RXTX = 8;
        const int EXC_BOARD_PCHC = 9;
        const int EXC_BOARD_3900 = 10;
        const int EXC_BOARD_429PCMCIA = 11;
        const int EXC_BOARD_1553PCMCIA = 12;
        const int EXC_BOARD_1553PCMCIAEP = 13;
        const int EXC_BOARD_HOO9 = 14;
        const int EXC_BOARD_3910PCI = 15;
        const int EXC_BOARD_PCPX = 16;
        const int EXC_BOARD_4000PCI = 17;
        const int EXC_BOARD_PCIMCH = 18;
        const int EXC_BOARD_1760MMI = 19;
        const int EXC_BOARD_1553PCMCIAPX = 20;
        const int EXC_BOARD_1553PCMCIAEPPX = 21; //recognizes EP or PX
        const int EXC_BOARD_AFDX = 22;
        const int EXC_BOARD_1394 = 32;

        vecinfo mDevicesFound = new vecinfo();

        // EXC-4000PCI Board Types
        const int EXC4000_BRDTYPE_PCI = 0x4000;
        const int EXC4000_BRDTYPE_CPCI = 0x4001;
        const int EXC4000_BRDTYPE_MCH_PCI = 0x4002;
        const int EXC4000_BRDTYPE_MCH_CPCI = 0x4003;
        const int EXC4000_BRDTYPE_MCH_PMC = 0x4004;
        const int EXC4000_BRDTYPE_429_PMC = 0x4005;
        const int EXC2000_BRDTYPE_PCI = 0x4006;
        const int EXC4000_BRDTYPE_P104P = 0x4007;
        const int EXC4000_BRDTYPE_PCIHC = 0x4008;
        const int EXC4000_BRDTYPE_708_PMC = 0x4009;
        const int EXC4000_BRDTYPE_1553PX_PMC = 0x400A;
        const int EXC4000_BRDTYPE_DISCR_PCI = 0x400D;
        const int EXC4000_BRDTYPE_PCIE = 0xE400;
        const int EXC4000_BRDTYPE_PCIE64 = 0xE464;

        const int EXC2000_BRDTYPE_PCIE = 0xE406;
        const int EXCARD_BRDTYPE_1553PX = 0xE401;
        const int EXCARD_BRDTYPE_429RTX = 0xE402;
        const int MINIPCIE_BRDTYPE_1553PX = 0xE403;
        const int MINIPCIE_BRDTYPE_429RTX = 0xE404;
        const int EXC4500_BRDTYPE_PCIE_VPX = 0xE450;

        const int EXC_BRDTYPE_664PCIE = 0xE664;
        const int EXC_BRDTYPE_1394PCI = 0x1394;
        const int EXC_BRDTYPE_1394PCIE = 0xEF00;
        const int EXC_BRDTYPE_UNET = 0x5502;
        const int EXC_BRDTYPE_RNET = 0x5505;

        // EXC-4000PCI Module Types
        const int EXC4000_MODTYPE_SERIAL = 2;
        const int EXC4000_MODTYPE_1553_MCH = 3;
        const int EXC4000_MODTYPE_RTX = 4;
        const int EXC4000_MODTYPE_PX = 5;
        const int EXC4000_MODTYPE_MMSI = 6;
        const int EXC4000_MODTYPE_708 = 7;
        const int EXC4000_MODTYPE_MA = 8;
        const int EXC4000_MODTYPE_CAN = 0xc;
        const int EXC4000_MODTYPE_DIO = 0xd;
        const int EXC4000_MODTYPE_H009 = 0x9;
        const int EXC4000_MODTYPE_NONE = 0x1F;
        const int EXC4000_MODTYPE_CAN825 = 0x28;    //CAN825 only comes in 32 bit access
        const int EXC4000_MODTYPE_SERIAL_PLUS = 0x12;
        const int EXC4000_MODTYPE_AFDX_TX = 0x1C;
        const int EXC4000_MODTYPE_AFDX_RX = 0x1A;
        const int EXC4000_MODTYPE_ETHERNET = 0x1B;
        const int EXC4000_MODTYPE_ARINC_717 = 0x17;
        const int ALL_RT5_CHANNELS = 0x1F;
        const int ALL_RT10_CHANNELS = 0x03FF;

        string strCardType;      
        int mNumDevices = 34;
        
           public struct sRegisteryDevices
           {
               public string strCardType;
               public ushort cardType;
               public string ExtendedDeviceType;
               public string ExcConfigDeviceType;
               public ushort dev;
               public string IPAdd;
               public string USBSN;
               public string ConnectType;
               public string MACAdress;
               public int UDPPort;
               public string SlotID;
               public string IRQ;
               public string Mem1;
               public string Mem2;
               public int DebugTheseModules;
               public int isRemote;
               public string ProductID;
               public string VendorID;
               public sModules[] mods;
               public bool deviceStatus;
           };

           public struct sModules
           {
               public string ModuleName;
               public ushort ModuleType;
               public ushort mod;
               public bool modStatus;
               public string FirmwareVer;
               public string HardwareVer;
        }

           sRegisteryDevices[] regDevices = new sRegisteryDevices[34];

           sRegisteryDevices[] RegDevs
           {
               get { return regDevices; }
           }




        private ArrayList _devicesArray;
        private ObservableCollection<DeviceInfoModel> _devices;
        private ObservableCollection<DeviceOnlyInfoModel> _devicesOnly;
        private DeviceOnlyInfoModel _scannedDeviceOnly;
        private DeviceInfoModel _scannedDevice;
        private ModuleInfoModel _scannedModule;

        private static DeviceScanner instance = null;
        private static readonly object padlock = new object();


        //for Registery option
        public ObservableCollection<DeviceInfoModel> Devices
        {
            get
            {
                return _devices;
            }           
        }
        //for Registery option
        public ObservableCollection<DeviceOnlyInfoModel> DevicesOnly
        {
            get
            {
                return _devicesOnly;
            }
        }


        private DeviceScanner()
        {
            _devices = new ObservableCollection<DeviceInfoModel>();
            _devicesOnly = new ObservableCollection<DeviceOnlyInfoModel>();
            _devicesArray = new ArrayList();
            ScanModules();
        }

        public static DeviceScanner Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DeviceScanner();
                    }
                    return instance;
                }
            }
        }

        public bool ScanModules()
        {
            ushort ID, usCardType;
            int nNumRegistryDevices = 34;
            LinkedList<int> wIDs = new LinkedList<int>();
            LinkedList<int> wAFDX_IDs = new LinkedList<int>();


            for (int mod = 0; mod < nNumRegistryDevices; mod++)
                regDevices[mod].mods = new sModules[4];

            //find excalubur cards installed in PC - Cannot detect UNET'S
            FindPysicalDevices();

            try
            {
                for (ushort dev = 0; dev < nNumRegistryDevices; dev++)
                {
                    _scannedDevice = new DeviceInfoModel();
                    _scannedDeviceOnly = new DeviceOnlyInfoModel();

                    //load information found in the registery
                    FindRegistoryDevices(dev);

                    _scannedDevice.deviceStatus = false;
                    _scannedDeviceOnly.deviceStatus = false;

                    if (stfuncs.Get_UniqueID_P4000(dev, out ID) == 0)
                    {
                        if (stfuncs.Get_4000Board_Type(dev, out usCardType) == 0)
                        {
                            if (IsGetCardType(dev, usCardType, out strCardType))
                            {
                                _scannedDevice.strCardType = strCardType;
                                _scannedDevice.cardType = usCardType;
                                _scannedDevice.deviceStatus = true;

                                _scannedDeviceOnly.strCardType = strCardType;
                                _scannedDeviceOnly.cardType = usCardType;
                                _scannedDeviceOnly.deviceStatus = true;


                                if (mIsCardType(usCardType))  // if (usCardType == EXC4000_BRDTYPE_PCI)
                                    wIDs.AddLast(ID);
                                else if (usCardType == AFDX_BOARD_TYPE)
                                    wAFDX_IDs.AddLast(ID);
                            }

                            GetModuleInfo(dev, usCardType);
                        }
                    }


                    _devices.Add(_scannedDevice);
                    _devicesOnly.Add(_scannedDeviceOnly);
                }
            }
            catch (Exception e)
            {
                var error = "error";
                error += e.Message;
                MessageBox.Show(error);
                return false;
            }

            ComparePysicalandRegisteryDevices();

            _scannedDevice = new DeviceInfoModel();
            if (wIDs.Count() == 0) //if  no 4000PCI found in registery there may be one card installed with  device 25
            {

                if (stfuncs.Get_UniqueID_P4000(25, out ID) == 0)
                {
                    if (stfuncs.Get_4000Board_Type(25, out usCardType) == 0)
                    {
                        if (IsGetCardType(25, usCardType, out strCardType))
                        {
                            _scannedDevice.strCardType = strCardType;
                            _scannedDevice.cardType = usCardType;

                            GetModuleInfo(25, usCardType);

                            _devices.Add(_scannedDevice);
                            _devicesOnly.Add(_scannedDeviceOnly);
                        }

                    }
                }
            }

            _scannedDevice = new DeviceInfoModel();
            if (wAFDX_IDs.Count() == 0) //if  no AFDX found in registery there may be one card installed with  device 25
            {
                if (stfuncs.Get_UniqueID_P4000(34, out ID) == 0)
                {
                    if (stfuncs.Get_4000Board_Type(34, out usCardType) == 0)
                    {
                        if (usCardType == AFDX_BOARD_TYPE)
                        {
                            _scannedDevice.strCardType = strCardType;
                            _scannedDevice.cardType = usCardType;

                            strCardType = card_AFDX + "-Dev" + 34;
                            GetModuleInfo(34, usCardType);

                            _devices.Add(_scannedDevice);
                            _devicesOnly.Add(_scannedDeviceOnly);

                        }
                    }
                }
            }
            return true;
        }


        private bool GetModuleType(ushort dev, ushort mod, ushort moduleType, out string module, out string firmware, out string hardware)
        {

            module = "";
            firmware = "";
            hardware = "";
            int ret=0;

            if (moduleType == EXC4000_MODTYPE_SERIAL)
                module = "SERIAL";
            else if (moduleType == EXC4000_MODTYPE_1553_MCH)
                module = "MIL-STD-1553 MCH";
            else if (moduleType == EXC4000_MODTYPE_RTX)
            {
                /*
                 ret = stfuncs.Init_Module_RTx(dev, mod, ALL_RT10_CHANNELS, 0);

                  if (ret == 5)
                      ret = stfuncs.Init_Module_RTx(dev, mod, ALL_RT5_CHANNELS, 0);
                  else if (ret == 6)
                      ret = stfuncs.Init_Module_RTx(dev, mod, ALL_RT5_CHANNELS, 0);
                
                ret = stfuncs.Read_HwRevision_RTx(ret);
                ret = stfuncs.Read_Revision_RTx(ret);
                firmware = 
                hardware = 
                */
                      
                module = "ARINC-429";

            }
         

            else if (moduleType == EXC4000_MODTYPE_PX)
            {
                //int handle = stfuncs.Attach_Module_Px(dev, mod);
                //     int handle = stfuncs.Init_Module_Px(dev, mod);
                //     int rev = stfuncs.Get_Rev_Level_Px(handle);
                module = "MIL-STD-1553Px";
            }
           
            else if (moduleType == EXC4000_MODTYPE_MMSI)
                module = "MMSI";
            else if (moduleType == EXC4000_MODTYPE_708)
                module = "ARINC-708";
            else if (moduleType == EXC4000_MODTYPE_MA)
                module = "MA";
            else if (moduleType == EXC4000_MODTYPE_CAN)
                module = "CAN";
            else if (moduleType == EXC4000_MODTYPE_DIO)
            {
                /*
                ushort output;
                stfuncs.Get_HWRev_DISCR(ret, out output);
                hardware = 
                
                */
                module = "Discrete";
            }
                
            else if (moduleType == EXC4000_MODTYPE_H009)
                module = "H009";
            else if (moduleType == EXC4000_MODTYPE_CAN825) //0x28 this may be wrong 
                module = "CAN 825";
            else if (moduleType == EXC4000_MODTYPE_SERIAL_PLUS)
                module = "Serial Plus";
            else if (moduleType == EXC4000_MODTYPE_AFDX_TX)
                module = "AFDX_TX";
            else if (moduleType == EXC4000_MODTYPE_AFDX_RX)
                module = "AFDX_RX";
            else if (moduleType == EXC4000_MODTYPE_ETHERNET)
                module = "Ethernet";
            else if (moduleType == EXC4000_MODTYPE_ARINC_717)
                module = "ARINC-717"; 
            else if (moduleType == EXC4000_MODTYPE_NONE)
                return false;
            else
                return false;

            return true;

        }


        private bool mIsCardType(ushort usCardType)
        {

            if (usCardType == EXC4000_BRDTYPE_PCI)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_CPCI)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_MCH_PCI)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_MCH_CPCI)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_MCH_PMC)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_429_PMC)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_PCIHC)
                return true;
            else if (usCardType == EXC2000_BRDTYPE_PCI)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_P104P)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_708_PMC)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_1553PX_PMC)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_DISCR_PCI)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_PCIE)
                return true;
            else if (usCardType == EXC4000_BRDTYPE_PCIE64)
                return true;
            else if (usCardType == EXC2000_BRDTYPE_PCIE)
                return true;
            else if (usCardType == EXCARD_BRDTYPE_1553PX)
                return true;
            else if (usCardType == EXCARD_BRDTYPE_429RTX)
                return true;
            else if (usCardType == MINIPCIE_BRDTYPE_1553PX)
                return true;
            else if (usCardType == MINIPCIE_BRDTYPE_429RTX)
                return true;
            else if (usCardType == EXC4500_BRDTYPE_PCIE_VPX)
                return true;
            else if (usCardType == EXC_BRDTYPE_664PCIE)
                return true;
            else if (usCardType == EXC_BRDTYPE_1394PCI)
                return true;
            else if (usCardType == EXC_BRDTYPE_1394PCIE)
                return true;
            else if (usCardType == EXC_BRDTYPE_UNET)
                return true;
            else if (usCardType == EXC_BRDTYPE_RNET)
                return true;
            else
                return false;
        }


        private bool IsGetCardType(ushort dev, ushort cardType, out string sCardType)
        {
            if (cardType == EXC4000_BRDTYPE_PCI)
            {
                sCardType = "EXC-4000PCI";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_CPCI)
            {
                sCardType = "EXC-4000cPCI";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_MCH_PCI)
            {
                sCardType = "EXC-1553PCI/MCH";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_MCH_CPCI)
            {
                sCardType = "EXC-1553cPCI/MCH";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_MCH_PMC)
            {
                sCardType = "EXC-1553PMC/MCH";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_429_PMC)
            {
                sCardType = "DAS-429PMC/RTx";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_PCIHC)
            {
                sCardType = "EXC-4000PCI/HC";
                return true;
            }
            else if (cardType == EXC2000_BRDTYPE_PCI)
            {
                sCardType = "EXC-2000PCI";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_P104P)
            {
                sCardType = "EXC-4000P104plus";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_708_PMC)
            {
                sCardType = "EXC-708ccPMC";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_1553PX_PMC)
            {
                sCardType = "EXC-1553PMC/Px";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_DISCR_PCI)
            {
                sCardType = "EXC-DiscretePCI/Dx";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_PCIE)
            {
                sCardType = "EXC-4000PCIe";
                return true;
            }
            else if (cardType == EXC4000_BRDTYPE_PCIE64)
            {
                sCardType = "EXC-4000PCIe64";
                return true;
            }
            else if (cardType == EXC2000_BRDTYPE_PCIE)
            {
                sCardType = "EXC-2000PCIe";
                return true;
            }
            else if (cardType == EXCARD_BRDTYPE_1553PX)
            {
                sCardType = "EXC-1553ExCARD/Px";
                return true;
            }
            else if (cardType == EXCARD_BRDTYPE_429RTX)
            {
                sCardType = "DAS-429ExCARD/RTx";
                return true;
            }
            else if (cardType == MINIPCIE_BRDTYPE_1553PX)
            {
                sCardType = "EXC-1553mPCIe/Px";
                return true;
            }
            else if (cardType == MINIPCIE_BRDTYPE_429RTX)
            {
                sCardType = "DAS-429mPCIe/RTx";
                return true;
            }
            else if (cardType == EXC4500_BRDTYPE_PCIE_VPX)
            {
                sCardType = "EXC-4500ccVPX";
                return true;
            }
            else if (cardType == EXC_BRDTYPE_664PCIE)
            {
                sCardType = "EXC-664PCIe";
                return true;
            }
            else if (cardType == EXC_BRDTYPE_1394PCI)
            {
                sCardType = "EXC-1394PCI";
                return true;
            }
            else if (cardType == EXC_BRDTYPE_1394PCIE)
            {
                sCardType = "EXC-1394PCIe";
                return true;
            }
            else if (cardType == EXC_BRDTYPE_UNET)
            {
                sCardType = "EXC-1553UNET/Px";
                return true;
            }
            else if (cardType == EXC_BRDTYPE_RNET)
            {
                sCardType = "ES-1553RUNET/Px";
                return true;
            }

            else if (cardType == AFDX_BOARD_TYPE)
            {
                sCardType = card_AFDX;
                return true;
            }

            sCardType = "Undefined";
            return false;

        }

        private void GetModuleInfo(ushort dev, ushort usCardType)
        {

            int nNumMaxModules = 4;
            ushort modType;
            string moduleName, firmware, hardware;

            for (ushort mod = 0; mod < nNumMaxModules; mod++)
            {
                if (stfuncs.Get_4000Module_Type(dev, mod, out modType) == 0)
                {

                    if (GetModuleType(dev, mod, modType, out moduleName, out firmware, out hardware))
                    {
                        

                        _scannedModule = new ModuleInfoModel();
                        regDevices[dev].mods[mod].mod = mod;
                        _scannedModule.mod = mod;
                        regDevices[dev].mods[mod].ModuleType = modType;
                        _scannedModule.ModuleType = modType;
                        regDevices[dev].mods[mod].ModuleName = moduleName;
                        _scannedModule.ModuleName = moduleName;
                        regDevices[dev].mods[mod].modStatus = true;
                        _scannedModule.modStatus = true;

                        regDevices[dev].mods[mod].FirmwareVer = firmware;
                        _scannedModule.FirmwareVer = firmware;

                        regDevices[dev].mods[mod].HardwareVer = hardware;
                        _scannedModule.HardwareVer = hardware;

                        _scannedDevice.Modules.Add(_scannedModule);

                        GetModuleStatus("", dev, mod, modType);
                    }
                }
            }
        }

        private void ComparePysicalandRegisteryDevices()
        {
            if (mDevicesFound.Count > 0)
            {
                for (int cardFound = 0; cardFound < mDevicesFound.Count; cardFound++)
                {
                    
                    for (int dev = 0; dev < 33; dev++)
                    {
                        if (mDevicesFound[cardFound].dwBoardType == EXC_BOARD_4000PCI)
                        {
                          //  if ( (regDevices[dev].cardType == EXC4000_BRDTYPE_PCI) && (regDevices[dev].SlotID == mDevicesFound[cardFound].dwSocketNumber.ToString()) )
                                //regDevices[dev].

                              //  MessageBox.Show("found");
                        }
                        

                    } 
                                            
                            
                            
                    //mDevicesFound[cardFound].dwBoardType;
                   // BusType = mDevicesFound[cardFound].dwBusType;
                    //SocketNumber = mDevicesFound[cardFound].dwSocketNumber;


                }

            }
        }

        private void FindPysicalDevices()
        {
            //vecinfo info = new vecinfo();
            stfuncs.GetDevInfos(mDevicesFound);

            uint boardtype;
            uint BusType;
            uint SocketNumber;

            //test code if devices are found
            if (mDevicesFound.Count > 0)
            {
                for (int cardFound = 0; cardFound < mDevicesFound.Count; cardFound++)
                {
                    boardtype = mDevicesFound[cardFound].dwBoardType;
                    BusType = mDevicesFound[cardFound].dwBusType;
                    SocketNumber = mDevicesFound[cardFound].dwSocketNumber;


                }
             



            }
               
        }

        private void FindRegistoryDevices(ushort dev)
        {
            string subKey = "SYSTEM\\CurrentControlSet\\Services\\Excx64\\Devices\\ExcaliburDevice\\" + dev.ToString();
            string key;
            string str;

            regDevices[dev].dev = dev;
            _scannedDevice.dev = dev;
            _scannedDeviceOnly.dev = dev;


            key = "DeviceType";
            str = ReadSubKeyValue(subKey, key);
            regDevices[dev].strCardType = str;
            _scannedDevice.strCardType = str;
            _scannedDeviceOnly.strCardType = str;

            key = "CardType";
            str = ReadSubKeyValue(subKey, key);
            if (str != "")
            {
                regDevices[dev].cardType = ushort.Parse(str);
                _scannedDevice.cardType = ushort.Parse(str);
                _scannedDeviceOnly.cardType = ushort.Parse(str);
            }


            key = "ExtendedDeviceType";
            str = ReadSubKeyValue(subKey, key);
            regDevices[dev].ExtendedDeviceType = str;
            _scannedDevice.ExtendedDeviceType = str;
            _scannedDeviceOnly.ExtendedDeviceType = str;

            if ( (regDevices[dev].ExtendedDeviceType != "") && (regDevices[dev].strCardType != regDevices[dev].ExtendedDeviceType) )
            {
                regDevices[dev].strCardType = regDevices[dev].ExtendedDeviceType;
                _scannedDevice.strCardType = regDevices[dev].ExtendedDeviceType;
                _scannedDeviceOnly.strCardType = regDevices[dev].ExtendedDeviceType;

            }

            key = "ExcConfigDeviceType";
            str = ReadSubKeyValue(subKey, key);
            regDevices[dev].ExcConfigDeviceType = str;
            _scannedDevice.ExcConfigDeviceType = str;
            _scannedDeviceOnly.ExcConfigDeviceType = str;

            /* NEEDED??
            if ((str != "") && (str != null))
            {
                regDevices[dev].ExcConfigDeviceType = str;
                _scannedDevice.ExcConfigDeviceType = str;
                _scannedDeviceOnly.ExcConfigDeviceType = str;
                if (regDevices[dev].ExcConfigDeviceType != regDevices[dev].strCardType)
                {
                    regDevices[dev].strCardType = str;
                    _scannedDevice.strCardType = str;
                    _scannedDeviceOnly.strCardType = str;
                }
            }
            */
      

            key = "Serial_Number";
            str = ReadSubKeyValue(subKey, key);
            regDevices[dev].USBSN = str;
            _scannedDevice.USBSN = str;
            _scannedDeviceOnly.USBSN = str;

            key = "ID";
            str = ReadSubKeyValue(subKey, key);
            regDevices[dev].SlotID = str;
            _scannedDevice.SlotID = str;
            _scannedDeviceOnly.SlotID = str;

            key = "IRQ";
            str = ReadSubKeyValue(subKey, key);
            regDevices[dev].IRQ = str;
            _scannedDevice.IRQ = str;
            _scannedDeviceOnly.IRQ = str;

            key = "Mem";
            str = ReadSubKeyValue(subKey, key);
            regDevices[dev].Mem1 = str;
            _scannedDevice.Mem1 = str;
            _scannedDeviceOnly.Mem1 = str;

            if ((regDevices[dev].strCardType == "DAS-429UNET/RTx") || (regDevices[dev].strCardType == "EXC-9800MACC-II") || (regDevices[dev].strCardType == "EXC-1553UNET/Px")
                 || (regDevices[dev].strCardType == "ES-1553RUNET/Px") || (regDevices[dev].strCardType == "ES-1553UNET/Px"))
            {

                key = "IsRemmote";
                str = ReadSubKeyValue(subKey, key);
                if (str != "") {
                    regDevices[dev].isRemote = ushort.Parse(str);
                    _scannedDevice.isRemote = ushort.Parse(str);
                    _scannedDeviceOnly.isRemote = ushort.Parse(str);
                }
                    
                key = "ConnectType";
                str = ReadSubKeyValue(subKey, key);
                regDevices[dev].ConnectType = str;
                _scannedDevice.ConnectType = str;
                _scannedDeviceOnly.ConnectType = str;

                key = "DebugTheseModules";
                str = ReadSubKeyValue(subKey, key);
                if (str != "")
                {
                    regDevices[dev].DebugTheseModules = ushort.Parse(str);
                    _scannedDevice.DebugTheseModules = ushort.Parse(str);
                    _scannedDeviceOnly.DebugTheseModules = ushort.Parse(str);
                }
                
                if (regDevices[dev].ConnectType == "NET")
                {

                    key = "IPAddress";
                    str = ReadSubKeyValue(subKey, key);
                    regDevices[dev].IPAdd = str;
                    _scannedDevice.IPAdd = str;
                    _scannedDeviceOnly.IPAdd = str;

                    key = "MACAddress";
                    str = ReadSubKeyValue(subKey, key);
                    regDevices[dev].MACAdress = str;
                    _scannedDevice.MACAdress = str;
                    _scannedDeviceOnly.MACAdress = str;
                    
                    key = "UDPPort";
                    str = ReadSubKeyValue(subKey, key);
                    regDevices[dev].UDPPort = int.Parse(str);
                    _scannedDevice.UDPPort = int.Parse(str);
                    _scannedDeviceOnly.UDPPort = int.Parse(str);

                    regDevices[dev].USBSN = "";
                    _scannedDevice.USBSN = "";
                    _scannedDeviceOnly.USBSN = "";

//                    regDevices[dev].ProductID = "";
//                    _scannedDevice.ProductID = "";
//                    _scannedDeviceOnly.ProductID = "";

//                    regDevices[dev].VendorID = "";
//                    _scannedDevice.VendorID = "";
//                    _scannedDeviceOnly.VendorID = "";

                }
                else if (regDevices[dev].ConnectType == "USB")
                {
                    regDevices[dev].IPAdd = "";
                    _scannedDevice.IPAdd = "";
                    _scannedDeviceOnly.IPAdd = "";

                    regDevices[dev].MACAdress = "";
                    _scannedDevice.MACAdress = "";
                    _scannedDeviceOnly.MACAdress = ""; 

                    regDevices[dev].UDPPort = 0;
                    _scannedDevice.UDPPort = 0;
                    _scannedDeviceOnly.UDPPort = 0;

//                  key = "ProductID";
//                  str = ReadSubKeyValue(subKey, key);
//                  regDevices[dev].ProductID = str;
//                  _scannedDevice.ProductID = "";
//                  _scannedDeviceOnly.ProductID = "";

//                  key = "VendorID";
//                  str = ReadSubKeyValue(subKey, key);
//                  regDevices[dev].VendorID = str;
//                  regDevices[dev].VendorID = "";
//                  _scannedDevice.VendorID = "";
//                  _scannedDeviceOnly.VendorID = "";

                }

            }

        }

        private string ReadSubKeyValue(string subKey, string key)
        {
            string str = string.Empty;
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(subKey))
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
        //**********************************************************************************************************************
        //  Specific Module Handling
        //**********************************************************************************************************************       
        private int IsInitModuleOK(ushort dev, ushort mod, ushort moduleType)
        {
            int ret = -1;

            //NOTE need to expand to include all modules types
            //note if programs running and using these modules init_module will reset everything
            //not added to structue of devices
            if (moduleType == EXC4000_MODTYPE_RTX)
            {
                //  ret = stfuncs.Init_Module_RTx(dev, mod, 5, 0);
                //  if (ret >= 0)
                //      stfuncs.Release_Module_RTx(ret);
                return 0; // ret;
            }
            else if (moduleType == EXC4000_MODTYPE_PX)
            {
                ret = stfuncs.Attach_Module_Px(dev, mod);
                if (ret >= 0)
                    stfuncs.Release_Module_Px(ret);
                return ret;
            }
            else
                return ret;


        }

        private void GetModuleStatus(string content, ushort dev, ushort mod, ushort modType)
        {
           
            if (IsInitModuleOK(dev, mod, modType) < 0)
            {
               
            }
            else
            {
                
            }
        }
        //**********************************************************************************************************************
        //  API - Get Devices
        //**********************************************************************************************************************

    }
}

