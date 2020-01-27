using System;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.Extension
{
    [UsedImplicitly]
    public static class RandomExtension
    {
        /// <summary>
        ///     Returns a random number within a specified range.
        /// </summary>
        /// <param name="random">The random generator</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned. </param>
        /// <param name="maxValue">
        ///     The exclusive upper bound of the random number returned.
        ///     maxValue must be greater than or equal to minValue.
        /// </param>
        /// <returns>
        ///     A 64-bit signed integer greater than or equal to minValue and less than maxValue;
        ///     that is, the range of return values includes minValue but not maxValue.
        ///     If minValue equals maxValue, minValue is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">minValue is greater than maxValue.</exception>
        [UsedImplicitly]
        [Pure]
        public static long NextLong([NotNull] this Random random, long minValue, long maxValue)
        {
            random.ThrowIfNull(nameof(random));

            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue, "minValue is greater than maxValue");

            var uRange = (ulong) (maxValue - minValue);
            ulong ulongRand;
            do
            {
                var buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong) BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - (((ulong.MaxValue % uRange) + 1) % uRange));

            return (long) (ulongRand % uRange) + minValue;
        }
    }
}