
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;

namespace LauncherMvvmLight.View.ShellViews
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Page
    {

        public Shell()
        {
            InitializeComponent();

            string ExecutableFilePath = @"C:\Excalibur\MerlinPlus\MerlinPlus.exe";
            if (File.Exists(ExecutableFilePath))
                btnExalt.Foreground = Brushes.Green;
            else
                btnExalt.Foreground = Brushes.Red;
        }

        private void DeviceDataGridView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ModuleDataGridView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void FileExportSettingsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void DeviceDataGridView_Loaded_1(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}