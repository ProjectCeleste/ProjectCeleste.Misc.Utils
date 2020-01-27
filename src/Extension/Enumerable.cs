using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class EnumerableExtensions
    {
        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static IEnumerable<T> EmptyIfNull<T>([NoEnumeration] [ItemNotNull] this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string ToStringList([NotNull] [ItemNotNull] this IEnumerable<string> source,
            [NotNull] string separator = ",")
        {
            separator.ThrowIfNullOrEmpty(nameof(separator));

            var enumerable = source as string[] ?? source.ToArray();
            if (enumerable.Length == 0)
            {
                return string.Empty;
            }

            var list = enumerable.Aggregate(string.Empty, (current, str) => current + str + separator);
            if (list.EndsWith(separator))
            {
                list = list.Substring(0, list.Length - 1);
            }

            return list;
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string ToStringFormat<T>([NotNull] [ItemNotNull] this IEnumerable<T> source)
        {
            var enumerable = source as T[] ?? source.ToArray();
            if (enumerable.Length == 0)
            {
                return $"new {typeof(T)}[0] " + "{}";
            }

            var sb = new StringBuilder($"new {typeof(T)}[" + enumerable.Length + "] { ");
            for (var i = 0; i < enumerable.Length; i++)
            {
                var type = typeof(T);
                if (type == typeof(string))
                {
                    sb.Append(i + 1 < enumerable.Length ? $"{enumerable[i]}, " : $"{enumerable[i]}");
                }
                else if (type == typeof(byte))
                {
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X2}, " : $"0x{enumerable[i]:X2}");
                }
                else if (type == typeof(short))
                {
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X4}, " : $"0x{enumerable[i]:X4}");
                }
                else if (type == typeof(ushort))
                {
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X4}, " : $"0x{enumerable[i]:X4}");
                }
                else if (type == typeof(int))
                {
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X4}, " : $"0x{enumerable[i]:X4}");
                }
                else if (type == typeof(uint))
                {
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X4}, " : $"0x{enumerable[i]:X4}");
                }
                else if (type == typeof(long))
                {
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X8}, " : $"0x{enumerable[i]:X8}");
                }
                else if (type == typeof(ulong))
                {
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X8}, " : $"0x{enumerable[i]:X8}");
                }
                else if (type == typeof(decimal))
                {
                    sb.Append(i + 1 < enumerable.Length ? $"0x{enumerable[i]:X16}, " : $"0x{enumerable[i]:X16}");
                }
                else
                {
                    sb.Append(i + 1 < enumerable.Length ? $"{enumerable[i]}, " : $"{enumerable[i]}");
                }
            }

            sb.Append(" }");

            return sb.ToString();
        }
    }
}