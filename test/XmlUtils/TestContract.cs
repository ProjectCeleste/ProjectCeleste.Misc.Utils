using System.Xml.Serialization;

namespace ProjectCeleste.Misc.Utils.Tests.XmlUtils
{
    [XmlRoot(ElementName = "data")]
    public class SampleXmlData
    {
        [XmlAttribute(AttributeName = "content")]
        public string Content { get; set; }

        [XmlElement(ElementName = "element")]
        public SampleXmlElement Element { get; set; }
    }

    [XmlRoot(ElementName = "element")]
    public class SampleXmlElement
    {
        [XmlAttribute(AttributeName = "amount")]
        public int Amount { get; set; }
    }
}
