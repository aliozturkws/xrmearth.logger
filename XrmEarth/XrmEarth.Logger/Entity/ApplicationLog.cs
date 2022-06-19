using System;
using XrmEarth.Logger.Enums;

namespace XrmEarth.Logger.Entity
{
    public class ApplicationLog : IIdentifier, ICallerMember
    {
        public ApplicationLog()
        {
            CreatedAt = DateTime.Now;
        }

        public Guid ID { get; set; }
        public Guid ApplicationInstanceID { get; set; }
        public string ParentCallerMember { get; set; }
        public string CallerMember { get; set; }
        public string Message { get; set; }
        public LogType Type { get; set; }
        public int? LogLevel { get; set; }
        public string Tag1 { get; set; }
        public string Tag2 { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
