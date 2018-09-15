using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Windows.Controls;
using LauncherMvvmLight.Model;
using LauncherMvvmLight.ViewModel;


namespace LauncherMvvmLight.View.ShellViews.ModuleDataView
{
    /// <summary>
    /// Interaction logic for ModuleDataView.xaml
    /// </summary>
    public partial class ModuleDataGridView : UserControl
    {
        //private DeviceDataGridViewModel viewModel = new ModuleDataGridViewModel();

        public ModuleDataGridView()
        {
            InitializeComponent();
            //ModuleDataGridViewModel ModuleDataGridViewModel = new ModuleDataGridViewModel();
            //this.DataContext = viewModel;

            //ApplicationManager.Instance.LongRunningOperations = new ModuleScannerLongOpHandler(WaitSign);

            //this.Loaded += (s, e) =>
            //{
                //this.DataContext = this;
                //users.Add(new User() { Id = 2, Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
                //users.Add(new User() { Id = 3, Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });

            //};
            
/*
        private Guid _id;
        public string strCardType;
        public ushort cardType;
        public ushort dev;
        public string IPAdd;
        public string USBSN;
        public string SlotID;
        public string IRQ;
        public string Mem1;
        public string Mem2;
        public List<ModuleInfoModel> Modules;
        public bool deviceStatus;
*/

            //if (this.dgdev.Columns[0].Header != null)
            //{
            //    this.dgdev.Columns[0].Header = "ddd";
            //}


        }

        private void dgModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
