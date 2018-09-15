using System.Windows.Controls;


namespace LauncherMvvmLight.View.ShellViews.DeviceDataView
{
    /// <summary>
    /// Interaction logic for DeviceDataView.xaml
    /// </summary>
    public partial class DeviceDataView : UserControl
    {
        public DeviceDataView()
        {
            InitializeComponent();
            DeviceDataViewModel deviceDataViewModel = new DeviceDataViewModel();
            this.DataContext = deviceDataViewModel;

            //ApplicationManager.Instance.LongRunningOperations = new ModuleScannerLongOpHandler(WaitSign);

        }
    }
}
