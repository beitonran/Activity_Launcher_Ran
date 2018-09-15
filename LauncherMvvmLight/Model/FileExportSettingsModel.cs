using System;
using LauncherMvvmLight.Domain.UtilServices;
using LauncherMvvmLight.Infrastructure.Util;

namespace LauncherMvvmLight.Model
{
    [Serializable]
    public class FileExportSettingsModel : SingletonBase<FileExportSettingsModel>
    {
        public FileExportSettingsModel()
        {

        }

        public string[] getFileExtensions = { ".TXT", ".CSV" };
    }
}
