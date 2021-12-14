using System;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class ComparableExtensions
    {
        public static bool IsWithinInclusive<T>(this T value, T minimum, T maximum) where T : IComparable<T>
        {
            return value.CompareTo(minimum) >= 0 && value.CompareTo(maximum) <= 0;
        }

        public static bool IsWithinExclusive<T>(this T value, T minimum, T maximum) where T : IComparable<T>
        {
            return value.CompareTo(minimum) >= 0 && value.CompareTo(maximum) <= 0;
        }

        #region ThrowIf

        public static void ThrowIfNotWithinInclusive<T>(this T value, T minimum, T maximum, string name = null)
            where T : IComparable<T>
        {
            if (!value.IsWithinInclusive(minimum, maximum))
            {
                throw name == null
                    ? new ArgumentOutOfRangeException()
                    : new ArgumentOutOfRangeException(name, value, null);
            }
        }

        public static void ThrowIfNotWithinExclusive<T>(this T value, T minimum, T maximum, string name = null)
            where T : IComparable<T>
        {
            if (!value.IsWithinExclusive(minimum, maximum))
            {
                throw name == null
                    ? new ArgumentOutOfRangeException()
                    : new ArgumentOutOfRangeException(name, value, null);
            }
        }

        public static void ThrowIfNotEquals<T>(this T value, T compareValue, string name = null)
            where T : IComparable<T>
        {
            switch (value)
            {
                case null when compareValue == null:
                    return;
                case null:
                    throw name == null
                        ? new ArgumentException()
                        : new ArgumentException(string.Empty, name);
            }

            if (value.Equals(compareValue))
            {
                throw name == null
                    ? new ArgumentException()
                    : new ArgumentException(string.Empty, name);
            }
        }

        public static void ThrowIfEquals<T>(this T value, T compareValue, string name = null)
            where T : IComparable<T>
        {
            switch (value)
            {
                case null when compareValue == null:
                    throw name == null
                        ? new ArgumentException()
                        : new ArgumentException(string.Empty, name);
                case null:
                    return;
            }

            if (value.Equals(compareValue))
            {
                throw name == null
                    ? new ArgumentException()
                    : new ArgumentException(string.Empty, name);
            }
        }

        #endregion
    }
}