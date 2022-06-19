using System.Configuration;
using System.Xml;

namespace XrmEarth.Logger.Configuration
{
    public class LogConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("TargetCollection", DefaultValue = null, IsRequired = false)]
        public TargetCollection TargetCollection { get; set; }

        protected override void DeserializeSection(XmlReader reader)
        {
            base.DeserializeSection(reader);
        }

        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            return base.SerializeSection(parentElement, name, saveMode);
        }
    }
}
