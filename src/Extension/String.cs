using System;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class StringExtensions
    {
        public static string WrapIfLengthIsLongerThan(this string value,
            int maxLength,
            string prefix = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (value.Length <= maxLength)
            {
                return value;
            }

            var cutIndex = value.Length - maxLength;
            return prefix + value.Substring(cutIndex);
        }

        public static SecureString ToSecureString(this string value)
        {
            value.ThrowIfNullOrWhiteSpace(nameof(value));

            var secureString = new SecureString();
            foreach (var t in value)
                secureString.AppendChar(t);

            return secureString;
        }

        public static T StringToEnum<T>(this string input, T defaultValue = default,
            bool isNullable = false)
        {
            if (string.IsNullOrWhiteSpace(input) && isNullable &&
                Nullable.GetUnderlyingType(typeof(T))?.GetElementType() == null)
            {
                return default;
            }

            return (input ?? throw new ArgumentNullException(nameof(input))).EnumTryParse(out T outType)
                ? outType
                : defaultValue;
        }

        public static bool EnumTryParse<T>(this string input, out T theEnum)
        {
            input.ThrowIfNullOrWhiteSpace(nameof(input));

            var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (Enum.GetNames(type).Any(en => en.Equals(input, StringComparison.OrdinalIgnoreCase)))
            {
                theEnum = (T) Enum.Parse(type, input, true);
                return true;
            }

            theEnum = default;

            return false;
        }

        #region ThrowIf

        /// <summary>
        ///     Throws an ArgumentNullException if the given string item is null or empty.
        /// </summary>
        /// <param name="data">The item to check for nullity.</param>
        /// <param name="name">The name to use when throwing an exception, if necessary</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ThrowIfNullOrEmpty(this string data, string name = null)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw name == null ? new ArgumentNullException() : new ArgumentNullException(name);
            }
        }

        /// <summary>
        ///     Throws an ArgumentNullException if the given string item is null or empty or white space.
        /// </summary>
        /// <param name="data">The item to check for nullity.</param>
        /// <param name="name">The name to use when throwing an exception, if necessary</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ThrowIfNullOrWhiteSpace(this string data, string name = null)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw name == null ? new ArgumentNullException() : new ArgumentNullException(name);
            }
        }

        /// <summary>
        ///     Throws an ArgumentException if the given string not match.
        /// </summary>
        /// <param name="data">The item to check for nullity.</param>
        /// <param name="pattern"></param>
        /// <param name="name">The name to use when throwing an exception, if necessary</param>
        /// <param name="regexOptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void ThrowIfNotMatchRegEx(this string data, string pattern,
            string name = null, RegexOptions regexOptions = RegexOptions.None)
        {
            if (Regex.IsMatch(data, pattern, regexOptions))
            {
                throw name == null ? new ArgumentException() : new ArgumentException(name);
            }
        }

        #endregion
    }
}