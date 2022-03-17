using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Celeste.Misc.Utils
{
    public class SerializableStringList : IXmlSerializable
    {
        [XmlIgnore]
        public IList<string> Items { get; private set; }

        public SerializableStringList()
        {
            Items = new List<string>();
        }

        public SerializableStringList(IList<string> items)
        {
            Items = items;
        }

        public SerializableStringList(string str)
        {
            Items = Parse(str);
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            Items = Parse(reader.ReadString());
        }

        private IList<string> Parse(string value)
        {
            return value
                .Split(',')
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToList();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString(Items.ToStringList());
        }

        public string SerializeList() => Items.ToStringList();
    }
}
