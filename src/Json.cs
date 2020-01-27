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

            var xml = serializableObject.SerializeToJson(encoding);

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

            File.WriteAllText(filePath, xml, Encoding.UTF8);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string SerializeToJson<T>([NotNull] this T value, Encoding encoding) where T : class
        {
            value.ThrowIfNull(nameof(value));
            encoding.ThrowIfNull(nameof(encoding));

            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T DeserializeFromJson<T>([NotNull] string json, Encoding encoding) where T : class
        {
            json.ThrowIfNullOrWhiteSpace(nameof(json));
            encoding.ThrowIfNull(nameof(encoding));

            return JsonConvert.DeserializeObject<T>(json);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T DeserializeFromJsonFile<T>([NotNull] string filePath, Encoding encoding) where T : class
        {
            filePath.ThrowIfNullOrWhiteSpace(nameof(filePath));
            encoding.ThrowIfNull(nameof(encoding));

            return DeserializeFromJson<T>(File.ReadAllText(filePath, encoding), encoding);
        }
    }
}