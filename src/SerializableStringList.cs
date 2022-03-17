using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Celeste.Misc.Utils
{
    public class SerializableStringList : IXmlSerializable
    {
        private IList<string> _items;

        public SerializableStringList()
        {
            _items = new List<string>();
        }

        public SerializableStringList(IList<string> items)
        {
            _items = items;
        }

        public SerializableStringList(string str)
        {
            _items = Parse(str);
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            _items = Parse(reader.ReadString());
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
            writer.WriteString(_items.ToStringList());
        }

        public string SerializeList() => _items.ToStringList();
    }
}
