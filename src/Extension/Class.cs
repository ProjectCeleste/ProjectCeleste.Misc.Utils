using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class ClassExtensions
    {
        #region Xml

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T DeepCopyUsingXml<T>([NotNull] this T value) where T : class
        {
            var xml = XmlUtils.SerializeToXml(value, Encoding.UTF8);
            return XmlUtils.DeserializeFromXml<T>(xml, Encoding.UTF8);
        }

        [UsedImplicitly]
        public static void SerializeToXmlFile<T>([NotNull] this T serializableObject, [NotNull] string xmlFilePath,
            [NotNull] Encoding encoding,
            bool backup = true, [NotNull] string backupExt = ".bak") where T : class
        {
            XmlUtils.SerializeToXmlFile(serializableObject, xmlFilePath, encoding, backup, backupExt);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string SerializeToXml<T>([NotNull] this T serializableObject, [NotNull] Encoding encoding)
            where T : class
        {
            return XmlUtils.SerializeToXml(serializableObject, encoding);
        }

        #endregion

        #region Json

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T DeepCopyUsingJson<T>([NotNull] this T value) where T : class
        {
            value.ThrowIfNull(nameof(value));

            var json = JsonConvert.SerializeObject(value);
            return JsonConvert.DeserializeObject<T>(json);
        }

        #endregion
    }
}