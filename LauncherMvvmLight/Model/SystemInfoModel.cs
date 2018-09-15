using System;
using System.Collections.Generic;

namespace LauncherMvvmLight.Model
{
    [Serializable]
    public class SystemInfoModel
    {
        public List<DeviceManagerDataModel> DeviceDriverDataList;
        public List<ModuleDataModel> ModuleDataList;
        public OsDataModel osDataModel;
    }
    [Serializable]
    public class ModuleDataModel
    {
        public string Name;
        public string FileVersion;
        public string FilePath;


    }

    [Serializable]
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

    [Serializable]
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
