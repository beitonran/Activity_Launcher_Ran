using System.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Navigation;
using LauncherMvvmLight.View.ConfigViews;


namespace LauncherMvvmLight.View.PageViews.ConfigView
{
    /// <summary>
    /// Interaction logic for HelpView.xaml
    /// </summary>
    public partial class Config : Page
    {
        private object navigationService;

        public Config()
        {
            InitializeComponent();

            
          
        }


        private void UniqueIdentifierUCView_Loaded(object sender, RoutedEventArgs e)
        {

            var viewModel = (ConfigViewModel)DataContext;
            viewModel.CmbSelectedIdTypes = viewModel.selectedCard.SlotID;
            viewModel.UpdateConfigPageDynamicDisplay(viewModel.LstCardsSelectedItem.DeviceTypeName, viewModel.LstCardsSelectedItem.DeviceTypeDisplayConfig, 
                                                     viewModel.LstCardsSelectedItem.DipConfig, viewModel.selectedCard.IPAdd,  viewModel.selectedCard.SlotID, viewModel.selectedCard.USBSN, viewModel.selectedCard.ConnectType);
              
            

            viewModel.ipADDCommand = viewModel.selectedCard.IPAdd;
            

     /*       if (viewModel.selectedCard.ConnectType == "USB")
            {
                viewModel.radUSB1_Checked = true;
                
            }
            else if (viewModel.selectedCard.ConnectType == "NET")
            {
                viewModel.radEthernet1_Checked = true;
            }
            

            //viewModel.rtad
            */

        }

        
    }


}
