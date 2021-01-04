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
            var xml = XmlUtils.SerializeToString(value);
            return XmlUtils.DeserializeFromString<T>(xml);
        }

        [UsedImplicitly]
        public static void SerializeToXmlFile<T>([NotNull] this T serializableObject, [NotNull] string xmlFilePath,
            bool backup = true) where T : class
        {
            XmlUtils.SerializeToXmlFile(serializableObject, xmlFilePath, backup);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string SerializeToXml<T>([NotNull] this T serializableObject)
            where T : class
        {
            return XmlUtils.SerializeToString(serializableObject);
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