using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XrmEarth.Logger.Target;

namespace XrmEarth.Logger.Configuration
{
    public class TargetCollection : List<LogTarget>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("Targets");
            while (reader.IsStartElement("LogTarget"))
            {
                var type = Type.GetType(reader.GetAttribute("AssemblyName"));
                var serial = new XmlSerializer(type);

                reader.ReadStartElement("LogTarget");
                Add((LogTarget)serial.Deserialize(reader));
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var dispatcher in this)
            {
                writer.WriteStartElement("LogTarget");
                writer.WriteAttributeString("AssemblyName", dispatcher.GetType().AssemblyQualifiedName);
                var xmlSerializer = new XmlSerializer(dispatcher.GetType());
                xmlSerializer.Serialize(writer, dispatcher);
                writer.WriteEndElement();
            }
        }
    }
}
