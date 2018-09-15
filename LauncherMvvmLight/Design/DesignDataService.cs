using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using LauncherMvvmLight.Domain;
using LauncherMvvmLight.Domain.Services.DeviceScannerService;
using LauncherMvvmLight.Model;


namespace LauncherMvvmLight.Design
{
    public class DesignDataService : IDeviceScannerService
    {
        public bool SearchDevices()
        {
            throw new NotImplementedException();
        }

        ObservableCollection<DeviceInfoModel> IDeviceScannerService.GetDevices(string getType)
        {
            throw new NotImplementedException();
        }
        ObservableCollection<DeviceOnlyInfoModel> IDeviceScannerService.GetDevicesOnly()
        {
            throw new NotImplementedException();
        }

        ObservableCollection<DeviceInfoModel> IDeviceScannerService.GetDevices()
        {
            throw new NotImplementedException();
        }

    }
}