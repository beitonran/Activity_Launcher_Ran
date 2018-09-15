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
using LauncherMvvmLight.Model;

namespace LauncherMvvmLight.View.PageViews.ConfigView.ConfigUC
{
    /// <summary>
    /// Interaction logic for UniqueIdentifierUCView.xaml
    /// </summary>
    public partial class UniqueIdentifierUCView : UserControl
    {
        public UniqueIdentifierUCView()
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

   //         int index = Convert.ToInt32(SlotID);
     //       cmbIdType.SelectedIndex = index;
       //     int i = cmbIdType.SelectedIndex;

            if (DeviceTypeDisplayConfig == "custom0")
            {
                this.Visibility = Visibility.Visible;
                tblkUniqIDInfo.Text = "";
                lblUniqueIdInfo.Content = "";

                cmbIdType.SelectedIndex = 0;
                cmbIdType.Items.Add("N/A");
                cmbIdType.IsEnabled = false;
            }
            else if (DeviceTypeDisplayConfig == "custom1")
            {
                this.Visibility = Visibility.Visible;
                tblkUniqIDInfo.Text = "If you are using a single card, you may set this field to auto.  However, if you are using multiple cards, you must set a unique ID for each one, using dip switch SW1 on the card.";
                lblUniqueIdInfo.Content = "Unique ID: ";
                cmbIdType.IsEnabled = true;

                int DipNum = Int16.Parse(DipConfig);
                if (cmbIdType.Items.Count != DipNum)
                {
                    cmbIdType.Items.Clear();
                    cmbIdType.Items.Add("Auto");
                    for (int id = 0; id < (DipNum-1); id++)
                        cmbIdType.Items.Add(id.ToString());

                    if (SlotID != "")
                        cmbIdType.SelectedIndex = Convert.ToInt32(SlotID) + 1;
                    else
                        cmbIdType.SelectedIndex = 0;
                }
            }
            else if (DeviceTypeDisplayConfig == "custom2")
            {
                this.Visibility = Visibility.Collapsed;                
            }
            else if (DeviceTypeDisplayConfig == "custom3")
            {
                this.Visibility = Visibility.Collapsed;

                /*
                 * tblkUniqIDInfo.Text = "If you are using a single 3910PCI card, you may set this field to auto.  However, if you are using multiple cards, you must set a unique ID for each one, using jumpers JP10-11 on the card.";
                lblUniqueIdInfo.Content = "Unique ID: ";                

                cmbIdType.IsEnabled = true;

                int DipNum = Int16.Parse(DipConfig);
                if (cmbIdType.Items.Count != DipNum)
                {
                    cmbIdType.Items.Clear();
                    cmbIdType.Items.Add("Auto");
                    for (int id = 0; id < (DipNum - 1); id++)
                        cmbIdType.Items.Add(id.ToString());

                    cmbIdType.SelectedIndex = 0;

                }
                */
            }
            else if (DeviceTypeDisplayConfig == "custom4")
            {
                this.Visibility = Visibility.Collapsed;
                tblkUniqIDInfo.Text = "";
                lblUniqueIdInfo.Content = "";                          
            }
            else if (DeviceTypeDisplayConfig == "custom5")
            {
                this.Visibility = Visibility.Collapsed;
                tblkUniqIDInfo.Text = "";
                lblUniqueIdInfo.Content = "";                             
            }
            else if (DeviceTypeDisplayConfig == "custom6")
            {
                this.Visibility = Visibility.Visible;
                tblkUniqIDInfo.Text = "If you are using a single miniPCIe card, you may set this field to auto (which is setting 0).  However, if you are using multiple cards, you must set a unique ID (0-3) for each one, using the on-board jumpers.";
                lblUniqueIdInfo.Content = "Unique ID: ";               

                int DipNum = Int16.Parse(DipConfig);
                if (cmbIdType.Items.Count != DipNum)
                {
                    cmbIdType.Items.Clear();
                    cmbIdType.Items.Add("Auto");
                    for (int id = 0; id < (DipNum - 1); id++)
                        cmbIdType.Items.Add(id.ToString());

                    cmbIdType.SelectedIndex = 0;

                }

            }

        }
    }
}
