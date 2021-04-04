using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectCeleste.Misc.Utils
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    public static class XmlUtils
    {
        public static string ToXml(this object serializableObject)
        {
            return SerializeToString(serializableObject);
        }

        public static void SerializeToXmlFile(this object objectToSerialize, string xmlFilePath, bool backupOldXmlFile = true)
        {
            if (backupOldXmlFile && File.Exists(xmlFilePath))
            {
                var backupFile = $"{xmlFilePath}.bak";

                File.Delete(backupFile);
                File.Move(xmlFilePath, backupFile);
            }

            var serializer = new XmlSerializer(objectToSerialize.GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using var stringWriter = File.Open(xmlFilePath, FileMode.Create);
            using var xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings);

            serializer.Serialize(xmlWriter, objectToSerialize, ns);
        }

        private static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.None
        };

        public static string SerializeToString(object objectToSerialize)
        {
            if (objectToSerialize == null)
                return null;
            
            using var stringWriter = new Utf8StringWriter();
            using var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.Indentation = 2;
            Serialize(xmlWriter, objectToSerialize);

            return stringWriter.ToString();
        }

        public static void Serialize(XmlWriter stream, object objectToSerialize)
        {
            if (objectToSerialize == null)
                return;

            var serializer = new XmlSerializer(objectToSerialize.GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings);
            serializer.Serialize(xmlWriter, objectToSerialize, ns);
        }

        public static T DeserializeFromFile<T>(string xmlFilePath) where T : class
        {
            if (!File.Exists(xmlFilePath))
                return null;

            var xmlFileInfo = new FileInfo(xmlFilePath);
            if (xmlFileInfo.Length == 0)
                return null;

            var xmls = new XmlSerializer(typeof(T));
            using var fr = xmlFileInfo.OpenRead();

            return (T)xmls.Deserialize(fr);
        }

        public static T DeserializeFromString<T>(string xmlString) where T : class
        {
            if (string.IsNullOrEmpty(xmlString))
                return null;

            var xmls = new XmlSerializer(typeof(T));
            using var sr = new StringReader(xmlString);

            return (T)xmls.Deserialize(sr);
        }

        public static T DeserializeFromStream<T>(XmlTextReader reader) where T : class
        {
            var xmls = new XmlSerializer(typeof(T));
            return (T)xmls.Deserialize(reader);
        }
    }
}