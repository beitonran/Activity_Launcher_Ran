
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LauncherMvvmLight.Model;
using System.Threading.Tasks;


namespace LauncherMvvmLight.Domain.Services.DeviceScannerService
{


    /// <summary>
    /// The Interface defining methods for Create Employee and Read All Employees  
    /// </summary>
    public interface IDeviceScannerService
    {
        ObservableCollection<DeviceInfoModel> GetDevices(String getType);
        ObservableCollection<DeviceOnlyInfoModel> GetDevicesOnly();
        ObservableCollection<DeviceInfoModel> GetDevices();
    }

    /// <summary>
    /// Class implementing IDataAccessService interface and implementing
    /// its methods by making call to the Entities using CompanyEntities object
    /// </summary>
    public class DeviceScannerService : IDeviceScannerService
    {
        // scanner service
        private DeviceScanner deviceScannerSrv;
        //for Registery option
        private ObservableCollection<DeviceOnlyInfoModel> _devicesOnly;
        private ObservableCollection<DeviceInfoModel> _devices;
        //for File option
        private ObservableCollection<DeviceInfoModel> _devicesFile;
        //for Excell option
        private ObservableCollection<DeviceInfoModel> _devicesExcellFile;
        //for AccessDB(OLE) option
        private ObservableCollection<DeviceInfoModel> _devicesAccessDB;
        //for SQL DB option
        private ObservableCollection<DeviceInfoModel> _devicesSqlDB;

       
        //for Registery option
        public ObservableCollection<DeviceOnlyInfoModel> DevicesOnly
        {
            get
            {
                return _devicesOnly;
            }
            set
            {
                _devicesOnly = value;
            }
        }
        public ObservableCollection<DeviceInfoModel> Devices
        {
            get
            {
                return _devices;
            }
            set
            {
                _devices = value;
            }
        }
        //for File option
        public ObservableCollection<DeviceInfoModel> DevicesFile
        {
            get
            {
                return _devicesFile;
            }
            set
            {
                _devicesFile = value;
            }
        }
        //for File option
        public ObservableCollection<DeviceInfoModel> DevicesExcellFile
        {
            get
            {
                return _devicesExcellFile;
            }
            set
            {
                _devicesExcellFile = value;
            }
        }
        //for Access DB option
        public ObservableCollection<DeviceInfoModel> DevicesAccessDB
        {
            get
            {
                return _devicesAccessDB;
            }
            set
            {
                _devicesAccessDB = value;
            }
        }
        //for SQL DB option - Entity Framework
        public ObservableCollection<DeviceInfoModel> DevicesSqlDB
        {
            get
            {
                return _devicesSqlDB;
            }
            set
            {
                _devicesSqlDB = value;
            }
        }


        //constructor
        public DeviceScannerService()
        {
            /* BinaryFile based==> initialize device info repository!!!! and get all listed devices !!!!!
            _deviceInfoRepository = new DeviceInfoRepository("devconfig");            
            _devices = new ObservableCollection<DeviceInfoModel>(_deviceInfoRepository.FindAll());
            */
            /* DB based
            DB based==> initialize device info repository!!!! and get all listed devices !!!!!
            var builder = new DbContextOptionsBuilder<DeviceContext>();
            builder.UseInMemoryDatabase("ExcDevicesDB");
            options = builder.Options;
            */
            Devices = new ObservableCollection<DeviceInfoModel>();
            DevicesOnly = new ObservableCollection<DeviceOnlyInfoModel>();

            //Initialize scanner
            deviceScannerSrv = DeviceScanner.Instance;

            Devices = deviceScannerSrv.Devices;
            DevicesOnly = deviceScannerSrv.DevicesOnly;


        }

        public ObservableCollection< DeviceInfoModel> GetDevices(String getType)
        {
            /*File-based-->get from info repository
            _devices = new ObservableCollection<DeviceInfoModel>(_deviceInfoRepository.FindAll());
            */
            /*DB-based-->get from info repository
            using (var deviceContext = new DeviceContext())
            {
                //Perform data access using the context 
                deviceContext.ExcDeviceDbSet.Add()
            }
            */
            if(getType == "registery")            
                return Devices;
            else if (getType == "file")
                return DevicesFile;
            else if (getType == "excell")
                return DevicesExcellFile;
            else if (getType == "access")
                return DevicesAccessDB;
            else if (getType == "sql")
                return DevicesSqlDB;
            else
            {
                return null;
            }
        }

        public ObservableCollection<DeviceInfoModel> GetDevices()
        {
            /*File-based-->get from info repository
            _devices = new ObservableCollection<DeviceInfoModel>(_deviceInfoRepository.FindAll());
            */
            /*DB-based-->get from info repository
            using (var deviceContext = new DeviceContext())
            {
                //Perform data access using the context 
                deviceContext.ExcDeviceDbSet.Add()
            }
            */

            return Devices;
            /*
            if (getType == "registery")
                return Devices;
            else if (getType == "file")
                return DevicesFile;
            else if (getType == "excell")
                return DevicesExcellFile;
            else if (getType == "access")
                return DevicesAccessDB;
            else if (getType == "sql")
                return DevicesSqlDB;
            else
            {
                return null;
            }
            */
        }
        public ObservableCollection<DeviceOnlyInfoModel> GetDevicesOnlyAsync()
        {
           
            return DevicesOnly;

        }
        public ObservableCollection<DeviceOnlyInfoModel> GetDevicesOnly()
        {
            /*File-based-->get from info repository
            _devices = new ObservableCollection<DeviceInfoModel>(_deviceInfoRepository.FindAll());
            */
            /*DB-based-->get from info repository
            using (var deviceContext = new DeviceContext())
            {
                //Perform data access using the context 
                deviceContext.ExcDeviceDbSet.Add()
            }
            */
           
            return DevicesOnly;
           
        }
    }
}
