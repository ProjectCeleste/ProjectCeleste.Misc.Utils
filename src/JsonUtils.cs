using System.IO;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils
{
    public static class JsonUtils
    {
        [UsedImplicitly]
        public static void SerializeToJsonFile([NotNull] this object serializableObject, [NotNull] string filePath,
            Encoding encoding, bool backup = true, [NotNull] string backupExt = ".bak")
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

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string SerializeToJson<T>([NotNull] this T value) where T : class
        {
            value.ThrowIfNull(nameof(value));

            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T DeserializeFromJson<T>([NotNull] string json) where T : class
        {
            json.ThrowIfNullOrWhiteSpace(nameof(json));

            return JsonConvert.DeserializeObject<T>(json);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T DeserializeFromJsonFile<T>([NotNull] string filePath) where T : class
        {
            filePath.ThrowIfNullOrWhiteSpace(nameof(filePath));

            using var file = File.OpenText(filePath);
            var jsonSerializer = new JsonSerializer();

            return (T)jsonSerializer.Deserialize(file, typeof(T));
        }
    }
}