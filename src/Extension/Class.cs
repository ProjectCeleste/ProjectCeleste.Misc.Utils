using Newtonsoft.Json;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class ClassExtensions
    {
        #region Xml

        public static T DeepCopyUsingXml<T>(this T value) where T : class
        {
            var xml = XmlUtils.SerializeToString(value);
            return XmlUtils.DeserializeFromString<T>(xml);
        }

        public static void SerializeToXmlFile<T>(this T serializableObject, string xmlFilePath,
            bool backup = true) where T : class
        {
            XmlUtils.SerializeToXmlFile(serializableObject, xmlFilePath, backup);
        }

        public static string SerializeToXml<T>(this T serializableObject)
            where T : class
        {
            return XmlUtils.SerializeToString(serializableObject);
        }

        #endregion

        #region Json

        public static T DeepCopyUsingJson<T>(this T value) where T : class
        {
            value.ThrowIfNull(nameof(value));

            var json = JsonConvert.SerializeObject(value);
            return JsonConvert.DeserializeObject<T>(json);
        }

        #endregion
    }
}