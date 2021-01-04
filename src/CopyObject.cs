using Newtonsoft.Json;

namespace Celeste.Misc.Utils
{
    public static class CopyObject
    {
        public static T DeepCopy<T>(this T value)
        {
            var json = JsonConvert.SerializeObject(value);

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
