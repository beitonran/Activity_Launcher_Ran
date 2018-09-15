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
using GalaSoft.MvvmLight.Messaging;

namespace LauncherMvvmLight.View.PageViews.ConfigView.ConfigUC
{
    /// <summary>
    /// Interaction logic for ConnectionConfigUCView.xaml
    /// </summary>
    public partial class ConnectionConfigUCView : UserControl
    {
        public ConnectionConfigUCView()
        {
            InitializeComponent();
            //Register any message pipes
            RegisterMessenger();
        }

        private void RegisterMessenger()
        {
            //Register any message pipes
            Messenger.Default.Register<NotificationMessage>(this, NotifyMe);

        }

        public void NotifyMe(NotificationMessage notificationMessage)
        {
            String notificationString = notificationMessage.Notification;
            String[] notificationParams = notificationString.Split('_');

            string DeviceTypeName = notificationParams[0];
            string DeviceTypeDisplayConfig = notificationParams[1];
            string DipConfig = notificationParams[2];
            string ipADD = notificationParams[3];
            string SlotID = notificationParams[4];
            string USBSN = notificationParams[5];
            string ConnectType = notificationParams[6];


          //  radEthernet1.IsChecked = true;
            //           radUSB1.IsChecked = false;
            //txtBoxIPAddress.Text = ipADD;
            //txtSerialNumber.Text = USBSN;




            if (DeviceTypeDisplayConfig == "custom2")
            {
                this.Visibility = Visibility.Visible;
                radEthernet1.Visibility = Visibility.Collapsed;
                radUSB1.Visibility = Visibility.Collapsed;
                tBlkUSBInfo.Visibility = Visibility.Collapsed;
                txtSerialNumber.Visibility = Visibility.Collapsed;


            }
            else if (DeviceTypeDisplayConfig == "custom3")
            {
                this.Visibility = Visibility.Visible;
                radEthernet1.Visibility = Visibility.Visible;
                radUSB1.Visibility = Visibility.Visible;
                tBlkUSBInfo.Visibility = Visibility.Collapsed;
                txtSerialNumber.Visibility = Visibility.Collapsed;

            }
            else if (DeviceTypeDisplayConfig == "custom4")
            {

                this.Visibility = Visibility.Visible;
                radEthernet1.Visibility = Visibility.Visible;
                radUSB1.Visibility = Visibility.Visible;
                tBlkUSBInfo.Visibility = Visibility.Collapsed;
                txtSerialNumber.Visibility = Visibility.Collapsed;
                if (ConnectType == "NET")
                {
                    radEthernet1.IsChecked = true;
                    radUSB1.IsChecked = false;
                    txtBoxIPAddress.Text = ipADD;
                }
                else if (ConnectType == "USB")
                {
                    radEthernet1.IsChecked = false;
                    radUSB1.IsChecked = true;
                    txtSerialNumber.Text = USBSN;

                }

            }
            else if (DeviceTypeDisplayConfig == "custom5")
            {

                this.Visibility = Visibility.Visible;
                radEthernet1.Visibility = Visibility.Visible;
                radUSB1.Visibility = Visibility.Visible;
                tBlkUSBInfo.Visibility = Visibility.Collapsed;
                txtSerialNumber.Visibility = Visibility.Collapsed;
                txtBoxIPAddress.Text = ipADD;  // "010.072.063.045";
                
            }
            else //all other configurations
            {
                this.Visibility = Visibility.Collapsed;
            }

            
        }

        private void radEthernet1_Checked(object sender, RoutedEventArgs e)
        {
            txtSerialNumber.Visibility = Visibility.Collapsed;
            txtBoxIPAddress.Visibility = Visibility.Visible;
            txtSerialNumber.Visibility = Visibility.Collapsed;
            lbIPAddress.Content = "IP Address";
            


        }

        private void radUSB1_Checked(object sender, RoutedEventArgs e)
        {
            txtSerialNumber.Visibility = Visibility.Visible;
            txtBoxIPAddress.Visibility = Visibility.Collapsed;
            txtSerialNumber.Visibility = Visibility.Visible;
            lbIPAddress.Content = "Serial Number";

        }
    }
}
