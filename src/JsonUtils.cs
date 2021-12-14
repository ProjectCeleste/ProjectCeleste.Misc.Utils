using System.IO;
using System.Text;
using Newtonsoft.Json;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class JsonUtils
    {
        public static void SerializeToJsonFile(this object serializableObject, string filePath,
            Encoding encoding, bool backup = true, string backupExt = ".bak")
        {
            serializableObject.ThrowIfNull(nameof(serializableObject));
            filePath.ThrowIfNullOrWhiteSpace(nameof(filePath));
            backupExt.ThrowIfNullOrWhiteSpace(nameof(backupExt));
            encoding.ThrowIfNull(nameof(encoding));

            if (File.Exists(filePath))
            {
                if (backup)
                {
                    var backupFile = filePath + backupExt;

                    if (File.Exists(backupFile))
                        File.Delete(backupFile);

                    File.Move(filePath, backupFile);
                }

                File.Delete(filePath);
            }

            using var file = File.CreateText(filePath);

            var serializer = new JsonSerializer();
            serializer.Serialize(file, serializableObject);
        }

        public static string SerializeToJson<T>(this T value) where T : class
        {
            value.ThrowIfNull(nameof(value));

            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public static T DeserializeFromJson<T>(string json) where T : class
        {
            json.ThrowIfNullOrWhiteSpace(nameof(json));

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T DeserializeFromJsonFile<T>(string filePath) where T : class
        {
            filePath.ThrowIfNullOrWhiteSpace(nameof(filePath));

            using var file = File.OpenText(filePath);
            var jsonSerializer = new JsonSerializer();

            return (T)jsonSerializer.Deserialize(file, typeof(T));
        }
    }
}