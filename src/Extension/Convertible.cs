using System;
using System.Globalization;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class ConvertibleExtensions
    {
        public static string ToBytesSize<T>(this T value2, bool useThreeNonZeroDigits = true)
            where T : IConvertible
        {
            var value = value2.ToDecimal(new NumberFormatInfo());
            if (!useThreeNonZeroDigits)
            {
                if (value < FileUtils.Kb)
                    return $"{value} bytes";
                if (value < FileUtils.Mb)
                    return $"{value / FileUtils.Kb:f2} KB";
                return value < FileUtils.Gb ? $"{value / FileUtils.Mb:f2} MB" : $"{value / FileUtils.Gb:f2} GB";
            }

            string[] suffixes =
            {
                "bytes", "KB", "MB", "GB",
                "TB", "PB", "EB", "ZB", "YB"
            };

            for (var i = 0; i < suffixes.Length; i++)
            {
                if (value <= (decimal) Math.Pow(FileUtils.Kb, i + 1))
                    return ThreeNonZeroDigits(value / (decimal) Math.Pow(FileUtils.Kb, i)) + " " + suffixes[i];
            }

            return ThreeNonZeroDigits(value / (decimal) Math.Pow(FileUtils.Kb, suffixes.Length - 1)) + " " +
                   suffixes[suffixes.Length - 1];
        }

        private static string ThreeNonZeroDigits<T>(T value2) where T : IConvertible
        {
            var value = value2.ToDecimal(new NumberFormatInfo());
            return value >= 100 ? value.ToString("0,0") : value.ToString(value >= 10 ? "0.0" : "0.00");
        }
    }
}