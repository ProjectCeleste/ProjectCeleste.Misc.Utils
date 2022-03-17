using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Celeste.Misc.Utils
{
    public static class IEnumerableExtensions
    {
        public static string ToStringList<T>(this IEnumerable<T> value)
        {
            if (value == null || !value.Any())
                return string.Empty;

            var list = value.Aggregate(string.Empty, (current, str) => current + str + ",");
            if (list.EndsWith(","))
                list = list.Substring(0, list.Length - 1);

            return list;
        }

        public static string PrintToFormatString<T>(this IEnumerable<T> value)
        {
            if (value == null)
                return string.Empty;

            var enumerable = value as T[] ?? value.ToArray();
            if (!enumerable.Any())
                return $"new {typeof(T)}[0] " + "{}";

            var sb = new StringBuilder($"new {typeof(T)}[" + enumerable.Length + "] { ");
            for (var i = 0; i < enumerable.Length; i++)
                if (typeof(T) == typeof(string))
                    sb.Append(i + 1 < enumerable.Length ? $"{enumerable[i]}, " : $"{enumerable[i]}");
                else if (typeof(T) == typeof(byte))
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X2}, " : $"0x{enumerable[i]:X2}");
                else if (typeof(T) == typeof(int))
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X4}, " : $"0x{enumerable[i]:X4}");
                else if (typeof(T) == typeof(uint))
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X4}, " : $"0x{enumerable[i]:X4}");
                else if (typeof(T) == typeof(long))
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X8}, " : $"0x{enumerable[i]:X8}");
                else if (typeof(T) == typeof(ulong))
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X8}, " : $"0x{enumerable[i]:X8}");
                else
                    sb.Append(i + 1 < enumerable.Length ? $"{enumerable[i]}, " : $"{enumerable[i]}");

            sb.Append(" }");

            return sb.ToString();
        }
    }
}
