using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LauncherMvvmLight.Domain.Services.DeviceScannerService;
using LauncherMvvmLight.View.ConfigViews;
using LauncherMvvmLight.View.PageViews.HelpView;
using LauncherMvvmLight.View.ShellViews.DeviceDataView;
using LauncherMvvmLight.ViewModel;
using Microsoft.Practices.Unity;

namespace LauncherMvvmLight
{
    /// <summary>
    /// Here the DI magic come on.
    /// </summary>
    public class Bootstrapper
    {
        public IUnityContainer Container { get; set; }

        public Bootstrapper()
        {
            Container = new UnityContainer();gdgdgdgffd

           

            if (ViewModelBase.IsInDesignModeStatic)
            {
                ConfigureDesignContainer();

            }
            else
            {
                ConfigureContainer();

            }


        }
        /// <summary>
        /// We register here every service / interface / viewmodel.
        /// </summary>
        private void ConfigureDesignContainer()
        {
            
        }
        /// <summary>
        /// We register here every service / interface / viewmodel.
        /// </summary>
        private void ConfigureContainer()
        {

            var navigationService = new FrameNavigationService();
            navigationService.Configure("Shell", new Uri("../View/ShellViews/Shell.xaml", UriKind.Relative));
            navigationService.Configure("Help", new Uri("../View/PageViews/HelpView/HelpView.xaml", UriKind.Relative));
            navigationService.Configure("Config", new Uri("../View/PageViews/ConfigView/ConfigView.xaml", UriKind.Relative));

            Container.RegisterInstance<IFrameNavigationService>(navigationService);


            //Container.RegisterInstance<IDeviceInfoRepository>(new DeviceInfoRepository("devices"));
            Container.RegisterType<IDeviceScannerService>();

            Container.RegisterType<MainViewModel>();
            Container.RegisterType<HelpViewModel>();
            Container.RegisterType<ConfigViewModel>();
            Container.RegisterType<DeviceDataGridViewModel>();
            Container.RegisterType<FileExportSettingsViewModel>();




        }
    }
}
