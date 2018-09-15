using System;
using System.Collections.Generic;
using System.ComponentModel;
using LauncherMvvmLight.Domain.UtilServices;



namespace LauncherMvvmLight.Model
{
    public class DeviceConfigDisplayModel
    {
        public string DeviceStatus { get; set; }
        public string DeviceName { get; set; }      
    }

    public class DeviceTableDbModel
    {
        public string DeviceStatus { get; set; }
        public string DeviceName { get; set; }
        public string DeviceAlias { get; set; }
        public string DeviceFolder { get; set; }
        public string DeviceDisplayConfig { get; set; }
        public string DipConfig { get; set; }

    }

    public class DeviceConfigTableModel
    {
        public string DeviceTypeName { get; set; }
        public string DeviceTypeDisplayConfig { get; set; }
        public string DipConfig { get; set; }

        public static explicit operator DeviceConfigTableModel(string v)
        {
            throw new NotImplementedException();
        }
    }


    public class ModuleTableDbModel
    {
        public string dbModuleStatus { get; set; }
        public string dbModuleNameId { get; set; }
        public string dbModuleAlias { get; set; }
        public string dbModuleFolder { get; set; }
    }

    public class dllDisplayInfoModel
    {
        public string dllName { get; set; }
        public string dllVersion { get; set; }
    }


    public class ModuleInformationModel
    {
        public List<ModuleInfoModel> Modules;
    }

    public class DeviceInfoModel
    {       
        private Guid _id;
        public string strCardType;
        public ushort cardType;
        public string ExtendedDeviceType;
        public string ExcConfigDeviceType;
        public ushort dev;
        public string IPAdd;
        public string USBSN;
        public string ConnectType;
        public string MACAdress;
        public int UDPPort;
        public string SlotID;
        public string IRQ;
        public string Mem1;
        public string Mem2;
        public int DebugTheseModules;
        public int isRemote;
        public string ProductID;
        public string VendorID;

        public List<ModuleInfoModel> Modules;
        public bool deviceStatus;

        public DeviceInfoModel()
        {
            Modules = new List<ModuleInfoModel>();
        }
    }

    public class ModuleInfoModel
    {
        public string ModuleName { get; set; }
        public ushort ModuleType { get; set; }
        public ushort mod { get; set; }
        public bool modStatus { get; set; }
        public string FirmwareVer { get; set; }
        public string HardwareVer { get; set; }
    }

    public class DeviceOnlyInfoModel
    {


        public string strCardType { get; set; }
        public ushort cardType { get; set; }
        public ushort dev { get; set; }
        public string IPAdd { get; set; }
        public string USBSN { get; set; }
        public string SlotID { get; set; }
        public string IRQ { get; set; }
        public string Mem1 { get; set; }
        public string Mem2 { get; set; }
        public string IO { get; set; }
        public bool deviceStatus { get; set; }
        public string ExtendedDeviceType { get; set; }
        public string ExcConfigDeviceType { get; set; }
        public string ConnectType { get; set; }
        public string MACAdress { get; set; }
        public int DebugTheseModules;
        public int isRemote;
        public string ProductID;
        public string VendorID;
        public int UDPPort { get; set; }


    }

}
