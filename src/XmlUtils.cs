using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class XmlUtils
    {
        [UsedImplicitly]
        public static void SerializeToXmlFile<T>([NotNull] T serializableObject, [NotNull] string filePath,
            [NotNull] Encoding encoding,
            bool backup = true, [NotNull] string backupExt = ".bak") where T : class
        {
            serializableObject.ThrowIfNull(nameof(serializableObject));
            filePath.ThrowIfNullOrWhiteSpace(nameof(filePath));
            backupExt.ThrowIfNullOrWhiteSpace(nameof(backupExt));
            encoding.ThrowIfNull(nameof(encoding));

            var xml = SerializeToXml(serializableObject, encoding);

            if (File.Exists(filePath))
            {
                if (backup)
                {
                    var backupFile = $"{filePath}.bak";

                    if (File.Exists(backupFile))
                        File.Delete(backupFile);

                    File.Move(filePath, backupFile);
                }

                File.Delete(filePath);
            }

            File.WriteAllText(filePath, xml, encoding);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string SerializeToXml<T>([NotNull] T serializableObject, [NotNull] Encoding encoding)
            where T : class
        {
            serializableObject.ThrowIfNull(nameof(serializableObject));
            encoding.ThrowIfNull(nameof(encoding));

            string output;
            var serializer = new XmlSerializer(serializableObject.GetType());
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true,
                NewLineHandling = NewLineHandling.None
            };
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            using (var stringWriter = new StringWriterWithEncoding(encoding))
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, serializableObject, ns);
                }

                output = stringWriter.ToString();
            }

            return output;
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T DeserializeFromXmlFile<T>([NotNull] string filePath, [NotNull] Encoding encoding)
            where T : class
        {
            filePath.ThrowIfNullOrWhiteSpace(nameof(filePath));
            encoding.ThrowIfNull(nameof(encoding));

            return DeserializeFromXml<T>(File.ReadAllText(filePath, encoding), encoding);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T DeserializeFromXml<T>([NotNull] string xml, [NotNull] Encoding encoding) where T : class
        {
            xml.ThrowIfNullOrWhiteSpace(nameof(xml));
            encoding.ThrowIfNull(nameof(encoding));

            T output;
            var serializer = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream(encoding.GetBytes(xml)))
            {
                output = (T) serializer.Deserialize(ms);
            }

            return output;
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string PrettyXml([NotNull] string xml, [NotNull] Encoding encoding)
        {
            xml.ThrowIfNullOrWhiteSpace(nameof(xml));
            encoding.ThrowIfNull(nameof(encoding));

            string output;
            var xmlDoc = XDocument.Parse(xml);
            using (var stringWriter = new StringWriterWithEncoding(encoding))
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    OmitXmlDeclaration = false,
                    NewLineHandling = NewLineHandling.None
                }))
                {
                    xmlDoc.Save(xmlWriter);
                }

                output = stringWriter.ToString();
            }

            return output;
        }
    }
}