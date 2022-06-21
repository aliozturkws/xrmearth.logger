using XrmEarth.Logger.Enums;

namespace XrmEarth.Logger.Common
{
    public class SystemNotifyArgs
    {
        public string Message { get; set; }
        public LogType Type { get; set; }
        public int Level { get; set; }

        public bool InitializeCompleted { get; set; }
    }
}
