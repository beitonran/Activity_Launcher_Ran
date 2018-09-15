using System.Windows;
using LauncherMvvmLight.ViewModel;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Windows.Navigation;

namespace LauncherMvvmLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
           // Closing += (s, e) => ViewModelLocator.Cleanup();
            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #region <!-- Metro Window Style -->
        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PART_MAXIMIZE_RESTORE_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        //private void PART_HELP_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show("Window Metro Theme v1.0\nDesigned by Heiswayi Nrird (http://heiswayi.github.io) 2015.\nReleased under MIT license.");
        //}
        #endregion

    }
}