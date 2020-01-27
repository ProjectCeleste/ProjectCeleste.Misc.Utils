using System;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.RandomGenerator
{
    public class CryptoRandomGenerator : IDisposable
    {
        private RandomNumberGenerator _r;

        /// <summary>
        ///     Creates an instance of the default implementation of a cryptographic random number generator that can be used to
        ///     generate random data.
        /// </summary>
        [UsedImplicitly]
        public CryptoRandomGenerator()
        {
            _r = RandomNumberGenerator.Create();
        }

        /// <summary>
        ///     Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// <exception cref="ArgumentNullException">buffer is a null reference (Nothing in Visual Basic).</exception>
        [UsedImplicitly]
        public void NextBytes([NotNull] byte[] buffer)
        {
            _r.GetBytes(buffer);
        }

        /// <summary>
        ///     Returns a random number between 0.0 and 1.0.
        /// </summary>
        [UsedImplicitly]
        [Pure]
        public double NextDouble()
        {
            var b = new byte[4];
            _r.GetBytes(b);
            return BitConverter.ToUInt32(b, 0) / ((double) uint.MaxValue + 1);
        }

        /// <summary>
        ///     Returns a random number within the specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">
        ///     The exclusive upper bound of the random number returned. maxValue must be greater than or equal
        ///     to minValue.
        /// </param>
        [UsedImplicitly]
        [Pure]
        public long NextLong(long minValue, long maxValue)
        {
            var range = maxValue - minValue;
            return (long) Math.Floor(NextDouble() * range) + minValue;
        }

        /// <summary>
        ///     Returns a nonnegative random number.
        /// </summary>
        [UsedImplicitly]
        [Pure]
        public long NextLong()
        {
            return NextLong(0, long.MaxValue);
        }

        /// <summary>
        ///     Returns a nonnegative random number less than the specified maximum
        /// </summary>
        /// <param name="maxValue">
        ///     The inclusive upper bound of the random number returned. maxValue must be greater than or equal
        ///     0
        /// </param>
        [UsedImplicitly]
        [Pure]
        public long NextLong(long maxValue)
        {
            return NextLong(0, maxValue);
        }

        /// <summary>
        ///     Returns a random number within the specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">
        ///     The exclusive upper bound of the random number returned. maxValue must be greater than or equal
        ///     to minValue.
        /// </param>
        [UsedImplicitly]
        [Pure]
        public int Next(int minValue, int maxValue)
        {
            var range = (long) maxValue - minValue;
            return (int) ((long) Math.Floor(NextDouble() * range) + minValue);
        }

        /// <summary>
        ///     Returns a nonnegative random number.
        /// </summary>
        [UsedImplicitly]
        [Pure]
        public int Next()
        {
            return Next(0, int.MaxValue);
        }

        /// <summary>
        ///     Returns a nonnegative random number less than the specified maximum
        /// </summary>
        /// <param name="maxValue">
        ///     The inclusive upper bound of the random number returned. maxValue must be greater than or equal
        ///     0
        /// </param>
        [UsedImplicitly]
        [Pure]
        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                _r.Dispose();
                _r = null;
            }

            _disposedValue = true;
        }

        [UsedImplicitly]
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}