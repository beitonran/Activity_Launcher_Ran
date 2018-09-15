using System;
using System.Collections.Generic;
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


namespace ExcaliburLauncher

    
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string card_4000 = "4000PCI Series";  // Exc4000PCI";
        const string card_UNET = "ExcUNET";
        const string card_AFDX = "ExcAFDX";
        const string mod_429 = "-Mod429";
        const string mod_1553 = "-Mod1553";
         public const int AFDX_BOARD_TYPE = 0xE664;
               
        // EXC-4000PCI Board Types
         public const int EXC4000_BRDTYPE_PCI = 0x4000;
         public const int EXC4000_BRDTYPE_CPCI = 0x4001;
         public const int EXC4000_BRDTYPE_MCH_PCI = 0x4002;
         public const int EXC4000_BRDTYPE_MCH_CPCI = 0x4003;
         public const int EXC4000_BRDTYPE_MCH_PMC = 0x4004;
         public const int EXC4000_BRDTYPE_429_PMC = 0x4005;
         public const int EXC2000_BRDTYPE_PCI = 0x4006;
         public const int EXC4000_BRDTYPE_P104P = 0x4007;
         public const int EXC4000_BRDTYPE_PCIHC	= 0x4008;
         public const int EXC4000_BRDTYPE_708_PMC = 0x4009;
         public const int EXC4000_BRDTYPE_1553PX_PMC = 0x400A;
         public const int EXC4000_BRDTYPE_DISCR_PCI = 0x400D;
         public const int EXC4000_BRDTYPE_PCIE = 0xE400;
         public const int EXC4000_BRDTYPE_PCIE64 = 0xE464;

         public const int EXC2000_BRDTYPE_PCIE = 0xE406;
         public const int EXCARD_BRDTYPE_1553PX = 0xE401;
         public const int EXCARD_BRDTYPE_429RTX	= 0xE402;
         public const int MINIPCIE_BRDTYPE_1553PX = 0xE403;
         public const int MINIPCIE_BRDTYPE_429RTX = 0xE404;
         public const int EXC4500_BRDTYPE_PCIE_VPX = 0xE450;
        
         public const int EXC_BRDTYPE_664PCIE = 0xE664;
         public const int EXC_BRDTYPE_1394PCI = 0x1394;
         public const int EXC_BRDTYPE_1394PCIE = 0xEF00;
         public const int EXC_BRDTYPE_UNET = 0x5502;
         public const int EXC_BRDTYPE_RNET = 0x5505;

        // EXC-4000PCI Module Types
        const int EXC4000_MODTYPE_SERIAL = 2;
        const int EXC4000_MODTYPE_MCH = 3;
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
        
        Grid mExcDevicecGrid = new Grid();
        TextBlock[,] mTxtBlockA = new TextBlock[17, 11];
        static int mNumDevices = 50;
        
        int[] iRowA = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        int[] iColA = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

        public  enum DspP { eDEVICE, eCARDTYPE, eIPADDRESS, eUSBSN, eSLOTID, eUSEIRQ, eIRQ, eMEM1, eMEM2, eIO };
       
        
        public struct sRegisteryDevices
        {
            public string strDeviceType;
            public ushort DeviceType;
            public string ExcConfigDeviceType;
            public string ConnectType;
            public int DebugTheseModules;
            public int isRemote;
            public string ProductID;
            public string VendorID;
            public string MACAdress;
            public int UDPPort;
            public ushort dev;
            public string IPAdd;
            public string USBSN;
            public int SlotID;
            public int IRQ;
            public int Mem;
            public int MemExtra;
            public string IO;
            public sModules[] mods;
        };

        public struct sModules           
        {
            public string ModuleName;
            public ushort ModuleType;
            public ushort mod;
        };

   
       public sRegisteryDevices[] regDevices = new sRegisteryDevices[mNumDevices];

       public sRegisteryDevices[] RegDevs
        {
            get{ return regDevices; }
        }

        public MainWindow()
        {
            InitializeComponent();
            

            ushort ID, usCardType;
            int nNumRegistryDevices = 16;
            ComboBoxItem item = new ComboBoxItem();
            System.Collections.Generic.LinkedList<int> wIDs = new LinkedList<int>();
            System.Collections.Generic.LinkedList<int> wAFDX_IDs = new LinkedList<int>();

            CreateTextBlockArray();
            CreateDynamicWPFGrid();
            //DisplayRegistoryValues();

            string strDeviceType;
            for (int mod=0;mod< mNumDevices; mod++ )
                regDevices[mod].mods = new sModules[4];
            
            try
            {
                for (ushort dev = 0; dev < nNumRegistryDevices; dev++)
                {
                    GetRegistoryValues(dev);
                    if (stfuncs.Get_UniqueID_P4000(dev, out ID) == 0)
                    {
                       

                        if (stfuncs.Get_4000Board_Type(dev, out usCardType) == 0)
                        {
                            if (IsGetCardType(dev, usCardType, out strDeviceType))
                            {
                                if ( mIsCardType(usCardType) )  // if (usCardType == EXC4000_BRDTYPE_PCI)
                                    wIDs.AddLast(ID);
                                else if (usCardType == AFDX_BOARD_TYPE)
                                    wAFDX_IDs.AddLast(ID);
                            }

                            GetModuleInfo(dev, usCardType);
                        }
                    } 
                }
            }
            catch (Exception e)
            {
                var error = "error";
                error += e.Message;
                MessageBox.Show(error);
                return;
            }
            
            if (wIDs.Count() == 0) //if  no 4000PCI found in registery there may be one card installed with  device 25
            {
                if (stfuncs.Get_UniqueID_P4000(25, out ID) == 0)
                {
                    if (stfuncs.Get_4000Board_Type(25, out usCardType) == 0)
                    {
                        if (IsGetCardType(25, usCardType, out strDeviceType))
                        {
                           GetModuleInfo(25, usCardType);
                        }
                    }
                }
            }

            if (wAFDX_IDs.Count() == 0) //if  no AFDX found in registery there may be one card installed with  device 25
            {
                if (stfuncs.Get_UniqueID_P4000(34, out ID) == 0)
                {
                    if (stfuncs.Get_4000Board_Type(34, out usCardType) == 0)
                    {
                        if (usCardType == AFDX_BOARD_TYPE)
                        {
                            strDeviceType = card_AFDX + "-Dev" + 34;
                            GetModuleInfo(34, usCardType);
                        }
                    }
                }
            }

         //   DispRawDevices();


        }

        private bool IsGetCardType(ushort dev, ushort DeviceType, out string sCardType)
        {
            if (DeviceType == EXC4000_BRDTYPE_PCI)
            {
                sCardType = "EXC-4000PCI";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_CPCI)
            {
                sCardType = "EXC-4000cPCI";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_MCH_PCI)
            {
                sCardType = "EXC-1553PCI/MCH";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_MCH_CPCI)
            {
                sCardType = "EXC-1553cPCI/MCH";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_MCH_PMC)
            {
                sCardType = "EXC-1553PMC/MCH";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_429_PMC)
            {
                sCardType = "DAS-429PMC/RTx";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_PCIHC)
            {
                sCardType = "EXC-4000PCI/HC";
                return true;
            }
            else if (DeviceType == EXC2000_BRDTYPE_PCI)
            {
                sCardType = "EXC-2000PCI";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_P104P)
            {
                sCardType = "EXC-4000P104plus";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_708_PMC)
            {
                sCardType = "EXC-708ccPMC";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_1553PX_PMC)
            {
                sCardType = "EXC-1553PMC/Px";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_DISCR_PCI)
            {
                sCardType = "EXC-DiscretePCI/Dx";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_PCIE)
            {
                sCardType = "EXC-4000PCIe";
                return true;
            }
            else if (DeviceType == EXC4000_BRDTYPE_PCIE64)
            {
                sCardType = "EXC-4000PCIe64";
                return true;
            }
            else if (DeviceType == EXC2000_BRDTYPE_PCIE)
            {
                sCardType = "EXC-2000PCIe";
                return true;
            }
            else if (DeviceType == EXCARD_BRDTYPE_1553PX)
            {
                sCardType = "EXC-1553ExCARD/Px";
                return true;
            }
            else if (DeviceType == EXCARD_BRDTYPE_429RTX)
            {
                sCardType = "DAS-429ExCARD/RTx";
                return true;
            }
            else if (DeviceType == MINIPCIE_BRDTYPE_1553PX)
            {
                sCardType = "EXC-1553mPCIe/Px";
                return true;
            }
            else if (DeviceType == MINIPCIE_BRDTYPE_429RTX)
            {
                sCardType = "DAS-429mPCIe/RTx";
                return true;
            }
            else if (DeviceType == EXC4500_BRDTYPE_PCIE_VPX)
            {
                sCardType = "EXC-4500ccVPX";
                return true;
            }
            else if (DeviceType == EXC_BRDTYPE_664PCIE)
            {
                sCardType = "EXC-664PCIe";
                return true;
            }
            else if (DeviceType == EXC_BRDTYPE_1394PCI)
            {
                sCardType = "EXC-1394PCI";
                return true;
            }
            else if (DeviceType == EXC_BRDTYPE_1394PCIE)
            {
                sCardType = "EXC-1394PCIe";
                return true;
            }
            else if (DeviceType == EXC_BRDTYPE_UNET)
            {
                sCardType = "EXC-UNET";
                return true;
            }
            else if (DeviceType == EXC_BRDTYPE_RNET)
            {
                sCardType = "ES-1553RUNET/Px";
                return true;
            }

            else if (DeviceType == AFDX_BOARD_TYPE)
            {
                sCardType = card_AFDX;
                return true;
            }

            sCardType = "Undefined";
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
        private bool GetModuleType(ushort mod, ushort moduleType, out string module)
        {

            module = "";

            if (moduleType == EXC4000_MODTYPE_SERIAL)
                module = "Serial";
            else if (moduleType == EXC4000_MODTYPE_MCH)
                module = "429MCH";
            else if (moduleType == EXC4000_MODTYPE_RTX)
                module = "ARINC-429";
            else if (moduleType == EXC4000_MODTYPE_PX)
                module = "MIL-STD-1553";
            else if (moduleType == EXC4000_MODTYPE_MMSI)
                module = "MMSI";
            else if (moduleType == EXC4000_MODTYPE_708)
                module = "708";
            else if (moduleType == EXC4000_MODTYPE_MA)
                module = "MA";
            else if (moduleType == EXC4000_MODTYPE_CAN)
                module = "CAN";
            else if (moduleType == EXC4000_MODTYPE_DIO)
                 module = "Discrete";
            else if (moduleType == EXC4000_MODTYPE_H009)
                module = "H009";
            else if (moduleType == EXC4000_MODTYPE_CAN825) //0x28 this may be wrong 
                module = "CAN825";
            else if (moduleType == EXC4000_MODTYPE_SERIAL_PLUS)
                module = "Serial Plus";
            else if (moduleType == EXC4000_MODTYPE_AFDX_TX)
                module = "AFDX_TX";
            else if (moduleType == EXC4000_MODTYPE_AFDX_RX)
                module = "AFDX_RX";
            else if (moduleType == EXC4000_MODTYPE_ETHERNET)
                module = "Ethernet";
            else if (moduleType == EXC4000_MODTYPE_NONE)
                return false;
            else
                return false;
                
            return true;

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

          private void devices_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void devices_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            /*      sDevices selectedDevice = new sDevices();

                  string s = CboBoxDevices.SelectedItem.ToString();
                  int itemNo = CboBoxDevices.SelectedIndex;
                  selectedDevice = llDevices.ElementAt(itemNo);
                  string strDevice = "card type:" + selectedDevice.DeviceType.ToString() + "  device:" + selectedDevice.dev.ToString();
                  int ctype = selectedDevice.DeviceType;

                 MessageBox.Show(s + "  " + strDevice); 
                 */
            
        }

        private int IsInitModuleOK(ushort dev, ushort mod, ushort moduleType)
        {
            int ret = -1;

            //NOTE need to expand to include all modules types
            //note if programs running and using these modules init_module will reset everything
            //not added to structue of devices
            if (moduleType == EXC4000_MODTYPE_RTX)
            {
                ret = stfuncs.Init_Module_RTx(dev, mod, 5, 0);
                if (ret >= 0)
                    stfuncs.Release_Module_RTx(ret);
                return ret;
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

        private void SetItemParams(string content, ushort dev, ushort mod, ushort modType, out ComboBoxItem item)
        {
            item = new ComboBoxItem();
            item.Content = content;
            if (IsInitModuleOK(dev, mod, modType) < 0)
            {
                item.Foreground = Brushes.White;
                item.Background = Brushes.DarkRed;
            }
            else
            {
                item.Foreground = Brushes.White;
                item.Background = Brushes.DarkBlue;
            }
        }


        private void GetModuleInfo(ushort dev, ushort usCardType)
        {

            int nNumMaxModules = 4;
            ushort modType;
            string moduleName;
            ComboBoxItem item = new ComboBoxItem();

            for (ushort mod = 0; mod < nNumMaxModules; mod++)
            {
                if (stfuncs.Get_4000Module_Type(dev, mod, out modType) == 0)
                {

                    if (GetModuleType(mod, modType, out moduleName))
                    {
                        
                        SetItemParams(regDevices[dev] + moduleName, dev, mod, modType, out item); //set contenet and color of current item
                        CboBoxDevices.Items.Add(item);                       //add item to combo
                        listBox.Items.Add(regDevices[dev] + moduleName);

                        //Add params to array list
                       // regDevices[dev].strDeviceType = strDeviceType;
                        regDevices[dev].DeviceType = usCardType;
                        regDevices[dev].dev = dev;
                        regDevices[dev].mods[mod].mod = mod;
                        regDevices[dev].mods[mod].ModuleType = modType;
                        regDevices[dev].mods[mod].ModuleName = moduleName;

                        if (dev < 16) // NOTE: device numbers 25 and 34 are not part of execonfig, here they are saved to the regDevices[] array, but 
                                      // not displayed in the GUI.
                                      // At a later stage we may want to display these card in the GUI

                        {
                            mTxtBlockA[dev + 1, 1].Text = regDevices[dev].strDeviceType;
                            mTxtBlockA[dev + 1, 1].Foreground = new SolidColorBrush(Colors.Blue);
                        }
              
                    }
                }
             
            }
        }

        private void CboBoxDevices_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
  
        }

        private void textBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CboBoxDevices_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Hi world");

        }

        private void DispRawDevices()
        {
            vecinfo info =  new vecinfo();

            stfuncs.GetDevInfos(info);


        }
        
        private void CreateDynamicWPFGrid()
        {
            // Create the Grid  
            mExcDevicecGrid.Width = 800;
            mExcDevicecGrid.HorizontalAlignment = HorizontalAlignment.Left;
            mExcDevicecGrid.VerticalAlignment = VerticalAlignment.Top;
            mExcDevicecGrid.ShowGridLines = true;
            mExcDevicecGrid.Background = new SolidColorBrush(Colors.LightSteelBlue);
            mExcDevicecGrid.Height = 1000;

            // Create Columns  
            for (int i = 0; i < 11; i++)
            {
                ColumnDefinition GridCol = new ColumnDefinition();
                if (i >0 && i< 3)
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
            scroll1.Content = mExcDevicecGrid;
            scroll1.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            
            mExcDevicecGrid.MouseLeftButtonDown += new MouseButtonEventHandler(deviceGrid_Leftclick);

        }

       
        private void deviceGrid_Leftclick(object sender, MouseButtonEventArgs e)
        {
            //Used instead of   void txtBlock_Leftclick(object sender, RoutedEventArgs e)
            //void txtBlock_Leftclick(object sender, RoutedEventArgs e) left as example of left click on textbox
            //IInputElement elem = FocusManager.GetFocusedElement(this);

            int selectedColumnIndex = -1, selectedRowIndex = -1;
            var grid = sender as Grid;
            if (grid != null)
            {
                var pos = e.GetPosition(grid);
                var temp = pos.X;
                for (var i = 0; i < grid.ColumnDefinitions.Count; i++)
                {
                    var colDef = grid.ColumnDefinitions[i];
                    temp -= colDef.ActualWidth;
                    if (temp <= -1)
                    {
                        selectedColumnIndex = i;
                        break;
                    }
                }

                temp = pos.Y;
                for (var i = 0; i < grid.RowDefinitions.Count; i++)
                {
                    var rowDef = grid.RowDefinitions[i];
                    temp -= rowDef.ActualHeight;
                    if (temp <= -1)
                    {
                        selectedRowIndex = i;
                        break;
                    }
                }
            }

            if ((selectedColumnIndex == 0) || (selectedRowIndex == 0))
                return;
      
            string cardtype = mTxtBlockA[selectedRowIndex, 1].Text;

            Window2 wDeviceConfig = new Window2();
            wDeviceConfig.SetDeviceParams(selectedRowIndex, selectedColumnIndex, cardtype, regDevices[selectedRowIndex - 1].SlotID, regDevices);
            wDeviceConfig.Title = "Define Devices";
            wDeviceConfig.Owner = this;
            wDeviceConfig.ShowDialog();
       

        }

        private void CreateTextBlockArray()
        {

            for (int row = 0; row < 17; row++)
            {
                for (int col = 0; col<11; col++)
                {
                    mTxtBlockA[row, col] = new TextBlock();
                    if ( (row > 0) && (col > 0) )
                        mTxtBlockA[row, col].MouseLeftButtonDown += new MouseButtonEventHandler(txtBlock_Leftclick);
                }

            }
        }

        void txtBlock_Leftclick(object sender, RoutedEventArgs e)
        {

            /* We now use   private void deviceGrid_Leftclick(object sender, MouseButtonEventArgs e)
            TextBlock txtBlock = (TextBlock)sender;
            int row = Grid.GetRow(txtBlock);
            int col = Grid.GetColumn(txtBlock);
            //string text = txtBlock.Text;

            string cardtype = mTxtBlockA[row, 1].Text;

            Window2 wDeviceConfig = new Window2();
            wDeviceConfig.SetDeviceParams(row, col, cardtype, regDevices[row-1].SlotID, regDevices);
            wDeviceConfig.Title = "Define Devices";
            wDeviceConfig.Owner = this;
            wDeviceConfig.ShowDialog();
            
            */

        }

        private void AddGridHeaders()
        {

            mExcDevicecGrid.ShowGridLines = true;

            // Add column headers  
            mTxtBlockA[0, (int)DspP.eDEVICE].Text = "Device";
            mTxtBlockA[0, (int)DspP.eCARDTYPE].Text = "Card Type";
            mTxtBlockA[0, (int)DspP.eIPADDRESS].Text = "IP Address";
            mTxtBlockA[0, (int)DspP.eUSBSN].Text = "USB s/n";
            mTxtBlockA[0, (int)DspP.eSLOTID].Text = "Slot/ID";
            mTxtBlockA[0, (int)DspP.eUSEIRQ].Text = "Use IRQ";
            mTxtBlockA[0, (int)DspP.eIRQ].Text = "IRQ";
            mTxtBlockA[0, (int)DspP.eMEM1].Text = "Mem 1";
            mTxtBlockA[0, (int)DspP.eMEM2].Text = "Mem 2";
            mTxtBlockA[0, (int)DspP.eIO].Text = "IO";

            for (int col=0; col<10; col++)
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
                mTxtBlockA[row+1, 0].Text = row.ToString();
                mTxtBlockA[row+1, 0].FontSize = 14;
                mTxtBlockA[row+1, 0].FontWeight = FontWeights.Bold;
                mTxtBlockA[row+1, 0].Foreground = new SolidColorBrush(Colors.Green);
                mTxtBlockA[row+1, 0].VerticalAlignment = VerticalAlignment.Top;
                mTxtBlockA[row+1, 0].HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(mTxtBlockA[row+1, 0], row+1);
                Grid.SetColumn(mTxtBlockA[row+1, 0], 0);
                mExcDevicecGrid.Children.Add(mTxtBlockA[row+1, 0]);
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

      
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Window2 wDeviceConfig = new Window2();
            wDeviceConfig.Show();
            

        }

        private void DataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {


         

        }

        private void btnChangeText_Click(object sender, RoutedEventArgs e)
        {
            mTxtBlockA[0, 2].Text = "Device";
            mTxtBlockA[1, 2].Text = "Device";
            mTxtBlockA[2, 2].Text = "Device";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            Window1 wConfig = new Window1();
            wConfig.Show();
                       
         }

        private void GetRegistoryValues(ushort dev)
        {

            string subKey = "SYSTEM\\CurrentControlSet\\Services\\Excx64\\Devices\\ExcaliburDevice\\" + dev.ToString();
            string key;
            string str;

            regDevices[dev].dev = dev;

            key = "DeviceType";
            str = ReadSubKeyValue(subKey, key);
            if ( (str == "") || (str == "Undefned") ) return;
            mTxtBlockA[dev + 1, (int)DspP.eCARDTYPE].Text = str;
            mTxtBlockA[dev + 1, (int)DspP.eCARDTYPE].Foreground = new SolidColorBrush(Colors.Red);
            regDevices[dev].strDeviceType = str;

            key = "ExcConfigDeviceType";
            str = ReadSubKeyValue(subKey, key);
             if ( (str != "") && (str != null) )
            {
                regDevices[dev].ExcConfigDeviceType = str;
                if (regDevices[dev].ExcConfigDeviceType != regDevices[dev].strDeviceType) {
                    regDevices[dev].strDeviceType = regDevices[dev].ExcConfigDeviceType;
                    mTxtBlockA[dev + 1, (int)DspP.eCARDTYPE].Text = regDevices[dev].strDeviceType;
                }
            }
           
            key = "Serial_Number";
            str = ReadSubKeyValue(subKey, key);
            if (str != "")
                mTxtBlockA[dev + 1, (int)DspP.eUSBSN].Text = str.Remove(0, 9); 
            mTxtBlockA[dev + 1, (int)DspP.eUSBSN].Foreground = new SolidColorBrush(Colors.Red);
            regDevices[dev].USBSN = str;

            key = "ID";
            str = ReadSubKeyValue(subKey, key);
            if ((str != "") && (int.Parse(str) == 0xFFFF))
                mTxtBlockA[dev + 1, (int)DspP.eSLOTID].Text = "Auto";
            else if (str == "")
                mTxtBlockA[dev + 1, (int)DspP.eSLOTID].Text = "N/A";
            else if  ((str != "") && (int.Parse(str) != 0xFFFF))
                mTxtBlockA[dev + 1, (int)DspP.eSLOTID].Text = str;

            if (str != "")
                regDevices[dev].SlotID = int.Parse(str);
            mTxtBlockA[dev + 1, (int)DspP.eSLOTID].Foreground = new SolidColorBrush(Colors.Red);

            key = "IRQ";
            str = ReadSubKeyValue(subKey, key);
            if (str != "")
            {
                if (int.Parse(str) == 0xFFFF)
                    mTxtBlockA[dev + 1, (int)DspP.eIRQ].Text = "Auto";
                else
                    mTxtBlockA[dev + 1, (int)DspP.eIRQ].Text = str;

                mTxtBlockA[dev + 1, (int)DspP.eUSEIRQ].Text = "Yes";

                if (str != "") {

                    if (int.Parse(str) == 0)
                    {
                        mTxtBlockA[dev + 1, (int)DspP.eUSEIRQ].Text = "N/A";
                        mTxtBlockA[dev + 1, (int)DspP.eIRQ].Text = "No";
                    }

                }
            

            }
            else
            {
                mTxtBlockA[dev + 1, (int)DspP.eUSEIRQ].Text = "No";
                mTxtBlockA[dev + 1, (int)DspP.eIRQ].Text = "N/A";
            }
               
            mTxtBlockA[dev + 1, (int)DspP.eIRQ].Foreground = new SolidColorBrush(Colors.Red);

            // check again: if the irq value is zero, cancel it
            if (str != "") regDevices[dev].IRQ = int.Parse(str);

            key = "Mem";
            str = ReadSubKeyValue(subKey, key);
            if (str != "") {
                    if (int.Parse(str) == 0xFFFF)
                        mTxtBlockA[dev + 1, (int)DspP.eMEM1].Text = "Auto";
                    else
                        mTxtBlockA[dev + 1, (int)DspP.eMEM1].Text = str;
                }
            else
                mTxtBlockA[dev + 1, (int)DspP.eMEM1].Text = "N/A";

            mTxtBlockA[dev + 1, (int)DspP.eMEM1].Foreground = new SolidColorBrush(Colors.Red);
            if (str != "") regDevices[dev].Mem = int.Parse(str);

            key = "MemExtra";
            str = ReadSubKeyValue(subKey, key);
            if (str != "")
            {
                if (int.Parse(str) == 0xFFFF)
                    mTxtBlockA[dev + 1, (int)DspP.eMEM2].Text = "Auto";
                else
                    mTxtBlockA[dev + 1, (int)DspP.eMEM2].Text = str;
            }
            else
                mTxtBlockA[dev + 1, (int)DspP.eMEM2].Text = "N/A";

            mTxtBlockA[dev + 1, (int)DspP.eMEM2].Foreground = new SolidColorBrush(Colors.Red);
            if (str != "")  regDevices[dev].MemExtra = int.Parse(str);

            key = "IO";
            str = ReadSubKeyValue(subKey, key);
            if (str != "")
            {
                if (int.Parse(str) == 0xFFFF)
                    mTxtBlockA[dev + 1, (int)DspP.eIO].Text = "Auto";
                else
                    mTxtBlockA[dev + 1, (int)DspP.eIO].Text = str;
            }
            else
                mTxtBlockA[dev + 1, (int)DspP.eIO].Text = "N/A";

            mTxtBlockA[dev + 1, (int)DspP.eIO].Foreground = new SolidColorBrush(Colors.Red);
            regDevices[dev].IO = str;

            if ((regDevices[dev].strDeviceType == "DAS-429UNET/RTx") || (regDevices[dev].strDeviceType == "EXC-9800MACC-II") || (regDevices[dev].strDeviceType == "EXC-1553UNET/Px")
                || (regDevices[dev].strDeviceType == "ES-1553RUNET/Px") || (regDevices[dev].strDeviceType == "ES-1553UNET/Px"))
            {

                key = "IsRemmote";
                str = ReadSubKeyValue(subKey, key);
                if (str != "")
                    regDevices[dev].isRemote = ushort.Parse(str);

                key = "ConnectType";
                str = ReadSubKeyValue(subKey, key);
                regDevices[dev].ConnectType = str;

                key = "DebugTheseModules";
                str = ReadSubKeyValue(subKey, key);
                if (str != "")
                    regDevices[dev].DebugTheseModules = ushort.Parse(str);

                if (regDevices[dev].ConnectType == "NET")
                {
                    key = "IPAddress";
                    str = ReadSubKeyValue(subKey, key);
                    mTxtBlockA[dev + 1, (int)DspP.eIPADDRESS].Text = str;
                    mTxtBlockA[dev + 1, (int)DspP.eIPADDRESS].Foreground = new SolidColorBrush(Colors.Red);
                    regDevices[dev].IPAdd = str;

                    key = "MACAddress";
                    str = ReadSubKeyValue(subKey, key);
                    regDevices[dev].MACAdress = str;

                    key = "UDPPort";
                    str = ReadSubKeyValue(subKey, key);
                    regDevices[dev].UDPPort = int.Parse(str);

                    mTxtBlockA[dev + 1, (int)DspP.eUSBSN].Text = "";
                    regDevices[dev].USBSN = "";

                    regDevices[dev].ProductID = "";
                    regDevices[dev].VendorID = "";
                }
                else if (regDevices[dev].ConnectType == "USB")
                {
                    mTxtBlockA[dev + 1, (int)DspP.eIPADDRESS].Text = "";
                    regDevices[dev].IPAdd = "";
                    regDevices[dev].MACAdress = "";
                    regDevices[dev].UDPPort = 0;

                    key = "ProductID";
                    str = ReadSubKeyValue(subKey, key);
                    regDevices[dev].ProductID = str;

                    key = "VendorID";
                    str = ReadSubKeyValue(subKey, key);
                    regDevices[dev].VendorID = str;
                }

            }

        }

        static string ReadSubKeyValue(string subKey, string key)

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

        static void WriteSubKeyValue(string subKey, string key, object Value)

        {

            string str = string.Empty;
            RegistryKey registryKey;

            if (Value == null)
                return;

            registryKey = Registry.LocalMachine.OpenSubKey(subKey, true);
            if (registryKey == null)
                registryKey = Registry.LocalMachine.CreateSubKey(subKey);


            if (registryKey != null)
                {
                    registryKey.SetValue(key, Value);
                    registryKey.Close();
                }


        }


        public void UpdateRegDevices(int dev)
        {
            
            mTxtBlockA[dev + 1, (int)DspP.eCARDTYPE].Text = regDevices[dev].strDeviceType;

            if ((regDevices[dev].strDeviceType == "DAS-429UNET/RTx") || (regDevices[dev].strDeviceType == "EXC-9800MACC-II") || (regDevices[dev].strDeviceType == "EXC-1553UNET/Px")
                  || (regDevices[dev].strDeviceType == "ES-1553RUNET/Px") || (regDevices[dev].strDeviceType == "ES-1553UNET/Px"))
            {

                mTxtBlockA[dev + 1, (int)DspP.eSLOTID].Text = "N/A";
                if (regDevices[dev].ConnectType == "USB")
                {
                    mTxtBlockA[dev + 1, (int)DspP.eIPADDRESS].Text = RegDevs[dev].IPAdd;
                    mTxtBlockA[dev + 1, (int)DspP.eUSBSN].Text = regDevices[dev].USBSN.Remove(0, 9); 

                }
                else if (regDevices[dev].ConnectType == "NET")
                {
                    mTxtBlockA[dev + 1, (int)DspP.eIPADDRESS].Text = RegDevs[dev].IPAdd;
                }

            }
            else
            {
               ;
                if (RegDevs[dev].ExcConfigDeviceType == "2000PCI Series") 
                {
                    mTxtBlockA[dev + 1, (int)DspP.eCARDTYPE].Text = "2000PCI Series";
          
                }
                if (RegDevs[dev].SlotID == 0xffff)
                    mTxtBlockA[dev + 1, (int)DspP.eSLOTID].Text = "Auto";
                else
                     mTxtBlockA[dev + 1, (int)DspP.eSLOTID].Text = RegDevs[dev].SlotID.ToString();

                mTxtBlockA[dev + 1, (int)DspP.eIPADDRESS].Text = RegDevs[dev].IPAdd;

                if (regDevices[dev].IRQ == 0xffff)
                    mTxtBlockA[dev + 1, (int)DspP.eIRQ].Text = "Auto";
                else
                   mTxtBlockA[dev + 1, (int)DspP.eIRQ].Text = regDevices[dev].IRQ.ToString();

                mTxtBlockA[dev + 1, (int)DspP.eUSEIRQ].Text = "Yes";
                if (regDevices[dev].IRQ == 0)
                {
                    mTxtBlockA[dev + 1, (int)DspP.eUSEIRQ].Text = "N/A";
                    mTxtBlockA[dev + 1, (int)DspP.eIRQ].Text = "No";
                }

            }
        


        }

        private void mExcDevicecGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void scroll1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnSaveToRegistry_Click(object sender, RoutedEventArgs e)
        {
          
            string key;

            for (int dev = 0; dev < 16; dev++) {

                if ( regDevices[dev].strDeviceType == null || regDevices[dev].strDeviceType == "Undefined")
                        continue;
               
                string subKey = "SYSTEM\\CurrentControlSet\\Services\\Excx64\\Devices\\ExcaliburDevice\\" + dev.ToString();
 
                if ((regDevices[dev].strDeviceType == "DAS-429UNET/RTx") || (regDevices[dev].strDeviceType == "EXC-9800MACC-II") || (regDevices[dev].strDeviceType == "EXC-1553UNET/Px")
                      || (regDevices[dev].strDeviceType == "ES-1553RUNET/Px") || (regDevices[dev].strDeviceType == "ES-1553UNET/Px"))
                {
                    if (regDevices[dev].ConnectType == "USB")
                    {
                        key = "ConnectType";
                        WriteSubKeyValue(subKey, key, regDevices[dev].ConnectType);

                        key = "Serial_Number";
                        WriteSubKeyValue(subKey, key, regDevices[dev].USBSN);

                        key = "ProductID";
                        WriteSubKeyValue(subKey, key, regDevices[dev].ProductID);

                        key = "VendorID";
                        WriteSubKeyValue(subKey, key, regDevices[dev].VendorID);
                    }
                    else if (regDevices[dev].ConnectType == "NET")
                    {
                        key = "ConnectType";
                        WriteSubKeyValue(subKey, key, regDevices[dev].ConnectType);

                        key = "IPAddress";
                        WriteSubKeyValue(subKey, key, regDevices[dev].IPAdd);

                        key = "MACAddress";
                        WriteSubKeyValue(subKey, key, regDevices[dev].MACAdress);

                        key = "UDPPort";
                        WriteSubKeyValue(subKey, key, regDevices[dev].UDPPort);

                    }

                    key = "DeviceType";
                    WriteSubKeyValue(subKey, key, regDevices[dev].strDeviceType);

                    key = "IsRemote";
                    WriteSubKeyValue(subKey, key, regDevices[dev].isRemote);  

                    key = "DebugTheseModules";
                    WriteSubKeyValue(subKey, key, regDevices[dev].DebugTheseModules);
                  
                  
                }
                else
                {

                    if ( (regDevices[dev].strDeviceType == "EXC-4000PCI") 
                        || (regDevices[dev].strDeviceType == "EXC-4000cPCI") || (regDevices[dev].strDeviceType == "EXC-4000PCIe")
                        || (regDevices[dev].strDeviceType == "EXC-4000PCIe64") || (regDevices[dev].strDeviceType == "EXC-4000P104plus") || (regDevices[dev].strDeviceType == "EXC-4500ccVPX"))

                    {
                        RegDevs[dev].ExcConfigDeviceType = "4000PCI Series";
                        RegDevs[dev].strDeviceType = "4000PCI";
                    }
                
                    key = "DeviceType";
                    WriteSubKeyValue(subKey, key, regDevices[dev].strDeviceType);

                    key = "ExcConfigDeviceType";
                    WriteSubKeyValue(subKey, key, regDevices[dev].ExcConfigDeviceType);

                    key = "ID";
                    WriteSubKeyValue(subKey, key, regDevices[dev].SlotID);

                    key = "Mem";
                    WriteSubKeyValue(subKey, key, regDevices[dev].Mem);

                    key = "MemExtra";
                    WriteSubKeyValue(subKey, key, regDevices[dev].MemExtra);

                    key = "Serial_Number";
                    WriteSubKeyValue(subKey, key, regDevices[dev].USBSN);

                    key = "IRQ";
                    WriteSubKeyValue(subKey, key, regDevices[dev].IRQ);

                }
         
            }

            MessageBox.Show("The Device Information has been saved to theregistery?", "Confirm", MessageBoxButton.OK);
        }
    }
}

