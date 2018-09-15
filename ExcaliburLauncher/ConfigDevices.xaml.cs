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
using System.Windows.Shapes;

namespace ExcaliburLauncher
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {

        string mStrDevice;
        int mRow;
        int mCol;
        int mDevice;
        int mSelectedSlotID;
        string[] arrCardNames = new string[] { "Undefined", "2000PCI Series", "EXC-2000PCIe", "EXC-4000PCI", "EXC-4000cPCI", "EXC-4000PCI/HC", "EXC-4000PCIe", "EXC-4000PCIe64",
                                               "EXC-4000P104plus", "4000PCI Series","1553PCI/MCH", "EXC-1553cPCI/MCH", "EXC-1553PMC/MCH", "EXC-1553ExCARD/Px", "EXC-1553mPCIe/Px", "EXC-1553PMC/Px",
                                               "1533PCI/MCH", "1553PCMCIA", "1553PCMCIA/EP", "1553PCMCIA/PX", "1553PCMCIA/Px or /EP", "DAS-429PMC/RTx", "DAS-429ExCARD/RTx", "DAS-429mPCIe/RTx",
                                               "429PCMCIA", "EXC-708ccPMC", "EXC-DiscretePCI/Dx", "EXC-4500ccVPX", "EXC-1394PCI", "EXC-1394PCIe", "3910PCI", "EXC_BOARD_3910PCI","1394PCI Series",
                                                "ES-1553RUNET/Px", "DAS-429UNET/RTx", "ES-1553UNET/Px", "EthernetPCIe", "EXC-1553UNET/Px", "EXC-9800MACC-II",
                                               "EXC-664PCIe", "664PCI", "miniPCIe Card", "Express Card"};
        MainWindow.sRegisteryDevices[] mRegDevices;

        public Window2()
        {

            InitializeComponent();

            for (int card = 0; card < arrCardNames.Count(); card++)
                lstCards.Items.Add(arrCardNames[card]);

            cmbUniqueID.Items.Add("Auto");
            for (int id = 0; id < 16; id++)
                cmbUniqueID.Items.Add(id.ToString());

        }

        private void lstCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string str = lstCards.SelectedItem.ToString();
            lblCard.Content = "Device " + mDevice.ToString() + " :: " + str; ;
            lblCard.FontWeight = FontWeights.Bold;
            lblCard.FontSize = 12;
            DispCardConfig(str);

            cmbUniqueID.SelectedIndex = mSelectedSlotID;

        }

        public void SetDeviceParams(int row, int col, string text, int slotID, MainWindow.sRegisteryDevices[] regDevices)
        {
            mStrDevice = text;
            mRow = row;
            mCol = col;
            mDevice = row - 1;
            if (slotID == 0xffff)
                mSelectedSlotID = 0;
            else
                mSelectedSlotID = slotID + 1;
            lblCard.Content = "Device " + mDevice.ToString() + " :: " + mStrDevice;
            mRegDevices = regDevices;
            Set_IP_USBChecks(mRegDevices);

            if (regDevices[mDevice].SlotID == 0xFFFF)
                cmbUniqueID.SelectedIndex = 0;
            else
                cmbUniqueID.SelectedIndex = regDevices[mDevice].SlotID + 1;

            for (int i = 0; i < lstCards.Items.Count; i++)
            {
                lstCards.SelectedIndex = i;
                string s = lstCards.SelectedItem.ToString();
                if (lstCards.SelectedItem.ToString() == mStrDevice)
                {
                    lstCards.SelectedIndex = i;
                    lstCards.ScrollIntoView(i);
                    lstCards.Focus();
                    break;
                }

                else
                {
                    lstCards.SelectedIndex = 0;
                    lstCards.Focus();

                }

            }


        }

        private void DispCardConfig(string cardName)
        {


            if ((cardName == "Undefined") || (cardName == "Express Card") || (cardName == ""))
            {
                tBlkUniqIDInfo.Text = "";
                lblUniqueID.Content = "";
            }
            
            else if ((cardName == "2000PCI Series") || (cardName == "EXC-2000PCI") || (cardName == "EXC-2000PCIe") || (cardName == "EXC-4000PCI") || (cardName == "EXC-4000cPCI") || (cardName == "EXC-4000PCIe")
                       || (cardName == "EXC-4000PCIe64") || (cardName == "EXC-4000P104plus") || (cardName == "EXC-1394PCI") || (cardName == "EXC-1394PCIe") || (cardName == "1394PCI Series")
                       || (cardName == "1553PCI/MCH") || (cardName == "1533PCI/MCH") || (cardName == "EXC-1553PMC/Px")
                       || (cardName == "DAS-429PMC/RTx") || (cardName == "DAS-429ExCARD/RTx") || (cardName == "DAS-429mPCIe/RTx")
                       || (cardName == "EXC-1553ExCARD/Px") || (cardName == "EXC-1553mPCIe/Px") || (cardName == "EXC-1553cPCI/MCH") || (cardName == "EXC-1553PMC/MCH")
                       || (cardName == "EXC-708ccPMC") || (cardName == "EXC-DiscretePCI/Dx") || (cardName == "EXC-4500ccVPX")
                       || (cardName == "EXC-664PCIe") || (cardName == "664PCI") || (cardName == "EthernetPCIe"))
            {
                groupBoxUI.Visibility = Visibility.Visible;
                groupBoxEthernetUSB.Visibility = Visibility.Hidden;
                tBlkUniqIDInfo.Text = "If you are using a single card, you may set this field to auto.  However, if you are using multiple cards, you must set a unique ID for each one, using dip switch SW1 on the card.";
                lblUniqueID.Content = "Unique ID: ";
                lblSerialNumber.Visibility = Visibility.Hidden;
                txtSerialNumber.Visibility = Visibility.Hidden;
                if (cmbUniqueID.Items.Count < 17)
                {
                    cmbUniqueID.Items.Clear();
                    cmbUniqueID.Items.Add("Auto");
                    for (int id = 0; id < 16; id++)
                        cmbUniqueID.Items.Add(id.ToString());
                }
  
            }
            else if ((cardName == "1553PCMCIA") || (cardName == "1553PCMCIA/EP") || (cardName == "1553PCMCIA/PX") || (cardName == "1553PCMCIA/Px or /EP") || (cardName == "429PCMCIA"))
            {
                groupBoxUI.Visibility = Visibility.Visible;
                groupBoxEthernetUSB.Visibility = Visibility.Hidden;
                tBlkUniqIDInfo.Text = "If you are only using a single Excalibur PCMCIA card, leave this field as AUTO.  However, if you are using multiple Excalibur PCMCIA Cards, use this field to specify socket numbers for each of the cards.";
                lblUniqueID.Content = "Socket Number: ";
                lblSerialNumber.Visibility = Visibility.Hidden;
                txtSerialNumber.Visibility = Visibility.Hidden;
                if (cmbUniqueID.Items.Count != 5)
                {
                    cmbUniqueID.Items.Clear();
                    cmbUniqueID.Items.Add("Auto");
                    for (int id = 0; id < 4; id++)
                        cmbUniqueID.Items.Add(id.ToString());
                }

            }
            else if ((cardName == "3910PCI") || (cardName == "EXC_BOARD_3910PCI"))
            {
                groupBoxUI.Visibility = Visibility.Visible;
                groupBoxEthernetUSB.Visibility = Visibility.Hidden;
                tBlkUniqIDInfo.Text = "If you are using a single 3910PCI card, you may set this field to auto.  However, if you are using multiple cards, you must set a unique ID for each one, using jumpers JP10-11 on the card.";
                lblUniqueID.Content = "Unique ID: ";
                lblSerialNumber.Visibility = Visibility.Hidden;
                txtSerialNumber.Visibility = Visibility.Hidden;
                if (cmbUniqueID.Items.Count != 5)
                {
                    cmbUniqueID.Items.Clear();
                    cmbUniqueID.Items.Add("Auto");
                    for (int id = 0; id < 4; id++)
                        cmbUniqueID.Items.Add(id.ToString());
                }

            }
            else if ((cardName == "DAS-429UNET/RTx") || (cardName == "EXC-9800MACC-II") || (cardName == "EXC-1553UNET/Px"))
            {
                groupBoxUI.Visibility = Visibility.Hidden;
                groupBoxEthernetUSB.Visibility = Visibility.Visible;
                groupBoxEthernetUSB.Header = "Ethernet or USB Connection";
                radUSB.Visibility = Visibility.Visible;
                Set_IP_USBChecks(mRegDevices);


            }
            else if ((cardName == "ES-1553RUNET/Px") || (cardName == "ES-1553UNET/Px"))
            {
                groupBoxUI.Visibility = Visibility.Hidden;
                groupBoxEthernetUSB.Visibility = Visibility.Visible;
                groupBoxEthernetUSB.Header = "Ethernet Connection";
                tBlkUSBInfo.Visibility = Visibility.Hidden;
                lbIPAddress.Visibility = Visibility.Visible;
                txtBoxIPAddress.Visibility = Visibility.Visible;
                Set_IP_USBChecks(mRegDevices);
                radUSB.Visibility = Visibility.Hidden;
            }
            else if ((cardName == "miniPCIe Card") || (cardName == "EXC_BOARD_MINIPCIE"))
            {
                groupBoxUI.Visibility = Visibility.Visible;
                groupBoxEthernetUSB.Visibility = Visibility.Hidden;
                tBlkUniqIDInfo.Text = "If you are using a single miniPCIe card, you may set this field to auto (which is setting 0).  However, if you are using multiple cards, you must set a unique ID (0-3) for each one, using the on-board jumpers.";
                lblUniqueID.Content = "Unique ID: ";
                if (cmbUniqueID.Items.Count != 5)
                {
                    cmbUniqueID.Items.Clear();
                    cmbUniqueID.Items.Add("Auto");
                    for (int id = 0; id < 4; id++)
                        cmbUniqueID.Items.Add(id.ToString());
                }
            }

        }

        private void radEthernet_Click(object sender, RoutedEventArgs e)
        {
            tBlkUSBInfo.Visibility = Visibility.Hidden;
            groupBoxEthernetUSB.Header = "Ethernet Connection";
            lbIPAddress.Content = "IP Address : ";
            lblSerialNumber.Visibility = Visibility.Hidden;
            txtSerialNumber.Visibility = Visibility.Hidden;
            txtBoxIPAddress.Visibility = Visibility.Visible;
            lbIPAddress.Visibility = Visibility.Visible;
            if (txtBoxIPAddress.Text == "")
                txtBoxIPAddress.Text = "010.072.063.045";
        }

        private void radUSB_Click(object sender, RoutedEventArgs e)
        {
            groupBoxEthernetUSB.Header = "USB Connection";
            lbIPAddress.Visibility = Visibility.Hidden;
            txtBoxIPAddress.Visibility = Visibility.Hidden;
            lblSerialNumber.Visibility = Visibility.Visible;
            txtSerialNumber.Visibility = Visibility.Visible;
            tBlkUSBInfo.Visibility = Visibility.Visible;
            tBlkUSBInfo.Text = "If you are using only a single 429 - UNET device, using the USB connection, you may set this field to *to indicate to use any 429 - UNET irrespective of Serial Number.  However, if you are using multiple 429 - UNET devices, use this field to specify the Serial Number.";
            if (txtSerialNumber.Text == "")
                txtSerialNumber.Text = "*";

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int selectedID;
            string serialNumber="";

            MainWindow mainWin = (MainWindow)this.Owner;
            mainWin.RegDevs[mDevice].DeviceType = GetCardID(lstCards.SelectedItem.ToString());
            mainWin.RegDevs[mDevice].strDeviceType = lstCards.SelectedItem.ToString();
            string strDeviceType = lstCards.SelectedItem.ToString();

            if ( (strDeviceType == "DAS-429UNET/RTx") || (strDeviceType == "EXC-9800MACC-II") || (strDeviceType == "EXC-1553UNET/Px") 
                  || (strDeviceType == "ES-1553RUNET/Px") || (strDeviceType == "ES-1553UNET/Px") )
            {
                if (radUSB.IsChecked == true)
                {
                    mainWin.regDevices[mDevice].ConnectType = "USB";
                    mainWin.regDevices[mDevice].IPAdd = "";
                    mainWin.regDevices[mDevice].MACAdress = ""; 
                    mainWin.regDevices[mDevice].ProductID = "";
                    mainWin.regDevices[mDevice].VendorID = "";

                    if (txtSerialNumber.Text != "*") 
                    {

                        if (txtSerialNumber.Text.Length < 6)
                            serialNumber = txtSerialNumber.Text.ToString().PadLeft(5, '0');
                        else
                            serialNumber = txtSerialNumber.Text;
                        if (strDeviceType == "DAS-429UNET/RTx")
                            mainWin.regDevices[mDevice].USBSN = "CEXC04290" + serialNumber;
                        else if (strDeviceType == "EXC-1553UNET/Px")
                            mainWin.regDevices[mDevice].USBSN = "CEXC15530" + serialNumber;
                        else if (strDeviceType == "EXC-9800MACC-II")
                            mainWin.regDevices[mDevice].USBSN = "CEXC98000" + serialNumber;
                    }
                    else
                    {
                        mainWin.regDevices[mDevice].USBSN = txtSerialNumber.Text;
                    }

                }
                else if (radEthernet.IsChecked == true)
                {
                    mainWin.regDevices[mDevice].ConnectType = "NET";
                    mainWin.regDevices[mDevice].IPAdd = txtBoxIPAddress.Text;
                    mainWin.regDevices[mDevice].MACAdress = ""; 

                    if (strDeviceType == "EXC-1553UNET/Px")
                        mainWin.regDevices[mDevice].UDPPort = 0x1553;
                    else if (strDeviceType == "DAS-429UNET/RTx")
                        mainWin.regDevices[mDevice].UDPPort = 0x429;
                    else if (strDeviceType == "ES-1553RUNET/Px")
                        mainWin.regDevices[mDevice].UDPPort = 0x1553;
                    else if (strDeviceType == "EXC-9800MACC-II")
                        mainWin.regDevices[mDevice].UDPPort = 0x2710;  // 10000 decimal
                }

                mainWin.RegDevs[mDevice].isRemote = 1;
                mainWin.RegDevs[mDevice].DebugTheseModules = 0;

            }
            else
            {
                if ((strDeviceType == "Undefined") || (strDeviceType == ""))
                    return;


                if ( (strDeviceType == "EXC-4000PCI") || (strDeviceType == "EXC-4000cPCI") || (strDeviceType == "EXC-4000PCIe")
                 || (strDeviceType == "EXC-4000PCIe64") || (strDeviceType == "EXC-4000P104plus") 
                 || (strDeviceType == "EXC-4500ccVPX") )
                {
                    mainWin.RegDevs[mDevice].ExcConfigDeviceType = "4000PCI Series";
                    mainWin.RegDevs[mDevice].strDeviceType = "4000PCI";
                }
               else if ((strDeviceType == "2000PCI Series") || (strDeviceType == "EXC-2000PCI") || (strDeviceType == "EXC-2000PCIe"))
                {
                    mainWin.RegDevs[mDevice].ExcConfigDeviceType = "2000PCI Series";
                    mainWin.RegDevs[mDevice].strDeviceType = "4000PCI";
                }
                else
                    mainWin.RegDevs[mDevice].ExcConfigDeviceType = mainWin.RegDevs[mDevice].strDeviceType;

                if (cmbUniqueID.SelectedIndex == 0)
                    selectedID = 0xffff;
                else
                    selectedID = cmbUniqueID.SelectedIndex - 1;

                mainWin.RegDevs[mDevice].SlotID = selectedID;
                mainWin.RegDevs[mDevice].Mem = 0xffff;
                mainWin.RegDevs[mDevice].MemExtra = 0xffff;
                mainWin.RegDevs[mDevice].IO = "0xffff";
                mainWin.RegDevs[mDevice].IRQ = 0xffff;
                mainWin.regDevices[mDevice].USBSN = "";

            }
   
            mainWin.UpdateRegDevices(mDevice);

            Close();
      
        }


        private ushort GetCardID(string strDeviceType)
        {
            if (strDeviceType == "EXC-4000PCI")
                return MainWindow.EXC4000_BRDTYPE_PCI;
            else if (strDeviceType == "EXC-4000cPCI")
                return MainWindow.EXC4000_BRDTYPE_CPCI;
            else if (strDeviceType == "1553PCI/MCH")
                return MainWindow.EXC4000_BRDTYPE_MCH_PCI;
            else if (strDeviceType == "EXC-1553cPCI/MCH")
                return MainWindow.EXC4000_BRDTYPE_MCH_CPCI;
            else if (strDeviceType == "EXC-1553PMC/MCH")
                return MainWindow.EXC4000_BRDTYPE_MCH_PMC;
            else if (strDeviceType == "DAS-429PMC/RTx")
                return MainWindow.EXC4000_BRDTYPE_429_PMC;
            else if (strDeviceType == "EXC-4000PCI/HC")
                return MainWindow.EXC4000_BRDTYPE_PCIHC;
            else if (strDeviceType == "EXC-2000PCI")
                return MainWindow.EXC2000_BRDTYPE_PCI;
            else if (strDeviceType == "EXC-4000P104plus")
                return MainWindow.EXC4000_BRDTYPE_P104P;
            else if (strDeviceType == "EXC-708ccPMC")
                return MainWindow.EXC4000_BRDTYPE_708_PMC;
            else if (strDeviceType == "EXC-1553PMC/Px")
                return MainWindow.EXC4000_BRDTYPE_1553PX_PMC;
            else if (strDeviceType == "EXC-DiscretePCI/Dx")
                return MainWindow.EXC4000_BRDTYPE_DISCR_PCI;
            else if (strDeviceType == "EXC-4000PCIe")
                return MainWindow.EXC4000_BRDTYPE_PCIE;
            else if (strDeviceType == "EXC-4000PCIe64")
                return MainWindow.EXC4000_BRDTYPE_PCIE64;
            else if (strDeviceType == "EXC-2000PCIe")
                return MainWindow.EXC2000_BRDTYPE_PCIE;
            else if (strDeviceType == "EXC-1553ExCARD/Px")
                return MainWindow.EXCARD_BRDTYPE_1553PX;
            else if (strDeviceType == "DAS-429ExCARD/RTx")
                return MainWindow.EXCARD_BRDTYPE_429RTX;
            else if (strDeviceType == "EXC-1553mPCIe/Px")
                return MainWindow.MINIPCIE_BRDTYPE_1553PX;
            else if (strDeviceType == "DAS-429mPCIe/RTx")
                return MainWindow.MINIPCIE_BRDTYPE_429RTX;
            else if (strDeviceType == "EXC-4500ccVPX")
                return MainWindow.EXC4500_BRDTYPE_PCIE_VPX;
            else if (strDeviceType == "EXC-664PCIe")
                return MainWindow.EXC_BRDTYPE_664PCIE;
            else if (strDeviceType == "EXC-1394PCI")
                return MainWindow.EXC_BRDTYPE_1394PCI;
            else if (strDeviceType == "EXC-1394PCIe")
                return MainWindow.EXC_BRDTYPE_1394PCIE;
            else if (strDeviceType == "EXC-UNET")
                return MainWindow.EXC_BRDTYPE_UNET;
            else if (strDeviceType == "ES-1553RUNET/Px")
                return MainWindow.EXC_BRDTYPE_RNET;
            else
                return 0;
        }

        private void Set_IP_USBChecks(MainWindow.sRegisteryDevices[] regDevices)
        {

            if (regDevices[mDevice].IPAdd != "")
            {
                txtBoxIPAddress.Text = regDevices[mDevice].IPAdd;
                radEthernet.IsChecked = true;
                lblSerialNumber.Visibility = Visibility.Hidden;
                txtSerialNumber.Visibility = Visibility.Hidden;
                txtBoxIPAddress.Visibility = Visibility.Visible;
                lbIPAddress.Visibility = Visibility.Visible;
                tBlkUSBInfo.Visibility = Visibility.Hidden;
            }
            else
            {
                txtSerialNumber.Text = regDevices[mDevice].USBSN.Remove(0, 9);
                radUSB.IsChecked = true;
                lblSerialNumber.Visibility = Visibility.Visible;
                txtSerialNumber.Visibility = Visibility.Visible;
                txtBoxIPAddress.Visibility = Visibility.Hidden;
                lbIPAddress.Visibility = Visibility.Hidden;
                tBlkUSBInfo.Visibility = Visibility.Visible;
                tBlkUSBInfo.Text = "If you are using only a single 429 - UNET device, using the USB connection, you may set this field to *to indicate to use any 429 - UNET irrespective of Serial Number.  However, if you are using multiple 429 - UNET devices, use this field to specify the Serial Number.";

            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    } // public partial class Window2 : Window

} // namespace ExcaliburLauncher


