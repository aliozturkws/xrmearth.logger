using System;

namespace XrmEarth.Logger.Entity
{
    public class Application : IIdentifier
    {
        public Application()
        {
            CreatedAt = DateTime.Now;
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Description { get; set; }
        public Guid AssemblyGuid { get; set; }
        public string AssemblyVersion { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
