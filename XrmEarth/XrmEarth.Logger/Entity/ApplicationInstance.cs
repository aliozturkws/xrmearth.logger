using System;

namespace XrmEarth.Logger.Entity
{
    public class ApplicationInstance : IIdentifier
    {
        public ApplicationInstance()
        {
            CreatedAt = DateTime.Now;
        }

        public Guid ID { get; set; }
        public Guid ApplicationID { get; set; }
        public string Path { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public string Parameters { get; set; }
        public string Result { get; set; }
        public string Summary { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
