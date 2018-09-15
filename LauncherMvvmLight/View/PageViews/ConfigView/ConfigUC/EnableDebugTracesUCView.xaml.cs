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
using LauncherMvvmLight.Domain.Messages;

namespace LauncherMvvmLight.View.PageViews.ConfigView.ConfigUC
{
    /// <summary>
    /// Interaction logic for UniqueIdentifierUCView.xaml
    /// </summary>
    public partial class EnableDebugTracesUCView : UserControl
    {
        public EnableDebugTracesUCView()
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

            if (DeviceTypeDisplayConfig == "custom2")
            {
                this.Visibility = Visibility.Visible;

                chkbDbgDetail0.Visibility = Visibility.Visible;
                chkbDbgDetail0.Content = "1553 Module 0";
                chkbDbgDetail1.Visibility = Visibility.Visible;
                chkbDbgDetail1.Content = "Discrete Module";
                chkbDbgDetail2.Visibility = Visibility.Collapsed;
                chkbDbgDetail2.Content = "";
                chkbDbgDetail3.Visibility = Visibility.Collapsed;
                chkbDbgDetail3.Content = "";

            }
            else if (DeviceTypeDisplayConfig == "custom3")
            {
                this.Visibility = Visibility.Visible;

                chkbDbgDetail0.Visibility = Visibility.Visible;
                chkbDbgDetail0.Content = "429 Module";
                chkbDbgDetail1.Visibility = Visibility.Visible;
                chkbDbgDetail1.Content = "Discrete Module";
                chkbDbgDetail2.Visibility = Visibility.Collapsed;
                chkbDbgDetail2.Content = "";
                chkbDbgDetail3.Visibility = Visibility.Collapsed;
                chkbDbgDetail3.Content = "";
            }
            else if (DeviceTypeDisplayConfig == "custom4")
            {
                this.Visibility = Visibility.Visible;

                chkbDbgDetail0.Visibility = Visibility.Visible;
                chkbDbgDetail0.Content = "1553 Module 0";
                chkbDbgDetail1.Visibility = Visibility.Visible;
                chkbDbgDetail1.Content = "1553 Module 1";
                chkbDbgDetail2.Visibility = Visibility.Visible;
                chkbDbgDetail2.Content = "Discrete Module";
                chkbDbgDetail3.Visibility = Visibility.Collapsed;
                chkbDbgDetail3.Content = "";
            }
            else if (DeviceTypeDisplayConfig == "custom5")
            {
                this.Visibility = Visibility.Visible;

                chkbDbgDetail0.Visibility = Visibility.Visible;
                chkbDbgDetail0.Content = "1553 Module 0";
                chkbDbgDetail1.Visibility = Visibility.Visible;
                chkbDbgDetail1.Content = "1553 Module 1";
                chkbDbgDetail2.Visibility = Visibility.Visible;
                chkbDbgDetail2.Content = "1553 Module 2";
                chkbDbgDetail3.Visibility = Visibility.Visible;
                chkbDbgDetail3.Content = "1553 Module 3";
            }
            else//all other configurations
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

    }

}   

