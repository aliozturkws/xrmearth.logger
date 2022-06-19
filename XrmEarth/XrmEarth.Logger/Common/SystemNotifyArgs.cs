using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmEarth.Logger.Enums;

namespace XrmEarth.Logger.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemNotifyArgs
    {
        public string Message { get; set; }
        public LogType Type { get; set; }
        public int Level { get; set; }

        public bool InitializeCompleted { get; set; }
    }
}
