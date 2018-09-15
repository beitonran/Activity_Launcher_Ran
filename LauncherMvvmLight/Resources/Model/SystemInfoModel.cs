using System;
using System.Collections.Generic;

namespace LauncherMvvmLight.Model
{    
    public class SystemInfoModel
    {
        public List<DeviceManagerDataModel> DeviceDriverDataList;
        public OsDataModel osDataModel;

    }

    public class DeviceManagerDataModel
    {
        public string Name;
        public string Manufacturer;
        public string HardwareIds;
        public string DriverVersion;
        public string DriverProvider;
        public string LocationInfo;
        public string IsPresent;
        public string IsEnabled;
        public string HasProblem;

    }
    public class OsDataModel
    {

        public string osVersionFriendlyName;

        public string osVersion;
        public string osPlatform;
        public string osServicePack;
        public string osVersionString;
        public string verMajor;
        public string verMajorRevision;
        public string verMinor;
        public string verMinorRevision;
        public string verBuild;

        public bool isOS64;
        public bool isArch64;
        public int  processorNum;

    }

    public class UserProcessDataModel
    {

    }
}
