using System;
using System.Collections.Generic;

namespace ProjectCeleste.Misc.Utils.RandomGenerator
{
    public class ThreadSafeUnikRnd : IDisposable
    {
        private readonly CryptoRandomGenerator _cryptoRandom = new CryptoRandomGenerator();
        private readonly object _rndSyncLock = new object();
        private Random _random;

        /// <summary>
        ///     Creates an instance of ThreadSafeUnikRnd.
        /// </summary>
        public ThreadSafeUnikRnd()
        {
            ReNewSeed();
        }

        public void Dispose()
        {
            _cryptoRandom.Dispose();
        }

        private void ReNewSeed()
        {
            lock (_rndSyncLock)
            {
                _random = new Random(_cryptoRandom.Next(int.MinValue, int.MaxValue));
            }
        }

        private long NextLong(long minValue, long maxValue)
        {
            var uRange = (ulong)(maxValue - minValue);
            ulong ulongRand;
            do
            {
                var buf = new byte[8];
                lock (_rndSyncLock)
                {
                    _random.NextBytes(buf);
                }
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - (ulong.MaxValue % uRange + 1) % uRange);
            return (long)(ulongRand % uRange) + minValue;
        }

        public long NextUnikLong(IList<long> excludeList, long minValue, long maxValue, long numOfMaxTry = 10000,
            bool autoReNewSeed = true)
        {
            var retVal = NextLong(minValue, maxValue);
            var numOfTry = 0;

            while (excludeList.Contains(retVal) && numOfTry <= numOfMaxTry)
            {
                numOfTry++;
                retVal = NextLong(minValue, maxValue);
                if (!autoReNewSeed || numOfTry <= numOfMaxTry / 2) continue;
                ReNewSeed();
            }
            //
            if (excludeList.Contains(retVal))
                throw new Exception(
                    $"Unable to generate an rnd long using the exclude list (numOfTry = {numOfTry} ; numOfMaxTry = {numOfMaxTry} ; autoReNewSeed = {autoReNewSeed})");

            return retVal;
        }

        public int NextUnikInt(IList<int> excludeList, int minValue, int maxValue, int numOfMaxTry = 10000,
            bool autoReNewSeed = true)
        {
            int retVal;

            lock (_rndSyncLock)
            {
                retVal = _random.Next(minValue, maxValue);
            }

            var numOfTry = 0;

            while (excludeList.Contains(retVal) && numOfTry <= numOfMaxTry)
            {
                numOfTry++;
                lock (_rndSyncLock)
                {
                    retVal = _random.Next(minValue, maxValue);
                }
                if (!autoReNewSeed || numOfTry <= numOfMaxTry / 2) continue;
                ReNewSeed();
            }
            //
            if (excludeList.Contains(retVal))
                throw new Exception(
                    $"Unable to generate an rnd int using the exclude list (numOfTry = {numOfTry} ; numOfMaxTry = {numOfMaxTry} ; autoReNewSeed = {autoReNewSeed})");

            return retVal;
        }
    }
}
