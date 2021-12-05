using System;
using System.Collections.Generic;
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
        private static Dictionary<string, XmlSerializer> cachedXmlSerializers = new Dictionary<string, XmlSerializer>();

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
            using var xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings);
            Serialize(xmlWriter, objectToSerialize);

            return stringWriter.ToString();
        }

        public static void Serialize(XmlWriter xmlWriter, object objectToSerialize)
        {
            if (objectToSerialize == null)
                return;

            var serializer = GetXmlSerializer(objectToSerialize.GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            serializer.Serialize(xmlWriter, objectToSerialize, ns);
        }

        public static T DeserializeFromFile<T>(string xmlFilePath) where T : class
        {
            try
            {
                if (!File.Exists(xmlFilePath))
                    return null;

                var xmlFileInfo = new FileInfo(xmlFilePath);
                if (xmlFileInfo.Length == 0)
                    return null;

                var xmls = GetXmlSerializer(typeof(T));
                using var fr = xmlFileInfo.OpenRead();

                return (T)xmls.Deserialize(fr);
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception was thrown while reading the XML file {xmlFilePath}", ex);
            }
        }

        public static T DeserializeFromString<T>(string xmlString) where T : class
        {
            if (string.IsNullOrEmpty(xmlString))
                return null;

            var xmls = GetXmlSerializer(typeof(T));
            using var sr = new StringReader(xmlString);

            return (T)xmls.Deserialize(sr);
        }

        public static T DeserializeFromStream<T>(XmlTextReader reader) where T : class
        {
            var xmls = GetXmlSerializer(typeof(T));
            return (T)xmls.Deserialize(reader);
        }

        private static XmlSerializer GetXmlSerializer(Type type)
        {
            var key = type.ToString();

            lock (cachedXmlSerializers)
            {
                if (cachedXmlSerializers.TryGetValue(key, out var cachedSerializer))
                {
                    return cachedSerializer;
                }
                else
                {
                    var serializer = new XmlSerializer(type);
                    cachedXmlSerializers.Add(key, serializer);
                    return serializer;
                }
            }
        }
    }
}