/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:LauncherMvvmLight.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LauncherMvvmLight.Domain.Services.DeviceCollectService;
using LauncherMvvmLight.Domain.Services.DeviceScannerService;
using Microsoft.Practices.ServiceLocation;
using LauncherMvvmLight.View.ConfigViews;
using LauncherMvvmLight.View.PageViews.HelpView;
using LauncherMvvmLight.View.ShellViews;
using LauncherMvvmLight.View.ShellViews.DeviceDataView;
using LauncherMvvmLight.View.ShellViews.ModuleDataView;
using LauncherMvvmLight.Domain.Services.TaskService;
using LauncherMvvmLight.View.PageViews.TestView;
using LauncherMvvmLight.Domain.Services.SyncService;

namespace LauncherMvvmLight.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            //working with bootstrapper option
            //private static Bootstrapper _bootStrapper;
            //if (_bootStrapper == null)
            //    _bootStrapper = new Bootstrapper();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);


            //SimpleIoc.Default.Register<ModuleInfoRepo>(() => {
            //    return new ModuleInfoRepo("modules");
            //});

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDeviceScannerService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDeviceScannerService, DeviceScannerService>();
                SimpleIoc.Default.Register<IDataCollectSrv, DataCollectSrv>();
                SimpleIoc.Default.Register<IDBSrv, DBSrv>();
                SimpleIoc.Default.Register<ITaskSrv, TaskSrv>();
                SimpleIoc.Default.Register<IAsyncSrv, AsyncSrv>();
                SimpleIoc.Default.Register<ISyncSrv, SyncSrv>();

            }


            //SimpleIoc.Default.Register<IDeviceInfoRepository>(() => new DeviceInfoRepository("devconfig"), "devices", true);


            SetupNavigation();
            SimpleIoc.Default.Register<ShellViewModel>();
            SimpleIoc.Default.Register<ConfigViewModel>();
            SimpleIoc.Default.Register<HelpViewModel>();
            SimpleIoc.Default.Register<TestViewModel>();
            SimpleIoc.Default.Register<ShellViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();


            SimpleIoc.Default.Register<ModuleDataGridViewModel>();
            SimpleIoc.Default.Register<DeviceDataGridViewModel>();
            SimpleIoc.Default.Register<FileExportSettingsViewModel>();



        }

        private static void SetupNavigation()
        {
            var navigationService = new FrameNavigationService();
            navigationService.Configure("Shell", new Uri("../View/ShellViews/Shell.xaml", UriKind.Relative));
            navigationService.Configure("Help", new Uri("../View/PageViews/HelpView/HelpView.xaml", UriKind.Relative));
            navigationService.Configure("Test", new Uri("../View/PageViews/TestView/TestView.xaml", UriKind.Relative));
            navigationService.Configure("Config", new Uri("../View/PageViews/ConfigView/ConfigView.xaml", UriKind.Relative));

            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);

        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public HelpViewModel Help
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HelpViewModel>();
            }
        }
        public TestViewModel Test
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TestViewModel>();
            }
        }
        public ConfigViewModel Config
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConfigViewModel>();
            }
        }
        public DeviceDataGridViewModel DeviceDataGrid
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DeviceDataGridViewModel>();
            }
        }

        public ModuleDataGridViewModel ModuleDataGrid
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ModuleDataGridViewModel>();
            }
        }

        public FileExportSettingsViewModel FileExportSettings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FileExportSettingsViewModel>();
            }
        }

        public ShellViewModel Shell
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ShellViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}