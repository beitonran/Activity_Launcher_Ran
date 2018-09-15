using System;
using System.Collections.Generic;
using System.ComponentModel;
using LauncherMvvmLight.Domain.UtilServices;



namespace LauncherMvvmLight.Model
{

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
        public string SlotID;
        public string IRQ;
        public string Mem1;
        public string Mem2;
        public string ConnectType;
        public string MACAdress;
        public int UDPPort;
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

    public class ModuleInformationModel
    {
        public List<ModuleInfoModel> Modules;
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
