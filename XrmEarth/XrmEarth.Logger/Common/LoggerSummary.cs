using System.Collections.Generic;
using XrmEarth.Logger.Enums;

namespace XrmEarth.Logger.Common
{
    public class LoggerSummary
    {
        public Dictionary<LogType, int> LogTypeCounts { get; set; }
    }
}
