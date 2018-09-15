using System;
using LauncherMvvmLight.Model;

namespace LauncherMvvmLight.Domain.Messages
{
    public class UpdateSystemReportMessage
    {
        public SystemInfoModel systemInfoModelReport { get; set; }
    }
}
