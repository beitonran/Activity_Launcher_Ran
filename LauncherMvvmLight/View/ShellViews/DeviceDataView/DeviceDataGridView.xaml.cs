using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LauncherMvvmLight.Model;
using LauncherMvvmLight.ViewModel;


namespace LauncherMvvmLight.View.ShellViews.DeviceDataView
{
    /// <summary>
    /// Interaction logic for DeviceDataView.xaml
    /// </summary>
    public partial class DeviceDataGridView : UserControl
    {
        //Access the view model
        //protected DeviceDataGridViewModel m_ViewModel
        //{
        //    get { return (DeviceDataGridViewModel)Resources["ViewModel"]; }
        //}

        public DeviceDataGridView()
        {
            InitializeComponent();
           // m_ViewModel.Initialize();
           // Loaded += delegate { m_ViewModel.OnLoaded(); };
           // Unloaded += delegate { m_ViewModel.OnUnloaded(); };
          
            //InitStyles();

           


        }

        void OnUserControlLoaded(Object sender, RoutedEventArgs e)
        {
            dgdev.CommitEdit(DataGridEditingUnit.Row, true);
        }
        void OnUserControlUnloaded(Object sender, RoutedEventArgs e)
        {
            dgdev.CancelEdit(DataGridEditingUnit.Row);
        }


        #region styles
        private void InitStyles()
        {
            //Style rowStyle = new Style(typeof(DataGridRow));
           // rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
           //                          new MouseButtonEventHandler(Row_DoubleClick)));
          //  dgdev.RowStyle = rowStyle;
        }
        #endregion

        #region event_handlers
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {

            //DataGridRow row = sender as DataGridRow;
            // Some operations with this row
            //m_ViewModel.Row_DoubleClickHandler(row);

        }
        #endregion

        private void dgdev_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}
