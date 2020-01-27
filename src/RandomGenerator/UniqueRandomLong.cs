using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JetBrains.Annotations;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils.RandomGenerator
{
    [UsedImplicitly]
    public class UniqueRandomLongGenerator
    {
        private readonly long _randomMaxReTry = 25000 * Environment.ProcessorCount;
        private readonly object _syncLock = new object();
        private readonly IList<long> _excludeNumbers;
        private readonly long _minValue;
        private readonly long _maxValue;
        private Random _random;

        public UniqueRandomLongGenerator([Range(long.MinValue, long.MaxValue - 1)]
            long min = long.MinValue, [Range(long.MinValue + 1, long.MaxValue)]
            long max = long.MaxValue,
            [CanBeNull] IEnumerable<long> excludeNumbers = null)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), min, "minValue is greater than maxValue");

            _minValue = min;
            _maxValue = max;
            _excludeNumbers = excludeNumbers as List<long> ?? excludeNumbers.ToList();

            ReNewSeed();
        }

        private void ReNewSeed()
        {
            using var r = new CryptoRandomGenerator();
            _random = new Random(r.Next(int.MinValue, int.MaxValue));
        }

        [UsedImplicitly]
        [Pure]
        public long Next()
        {
            lock (_syncLock)
            {
                var countReTry = 0L;
                var number = _random.NextLong(_minValue, _maxValue);
                while (_excludeNumbers.Contains(number) && countReTry <= _randomMaxReTry)
                {
                    countReTry++;
                    number = _random.NextLong(_minValue, _maxValue);
                }

                if (_excludeNumbers.Contains(number))
                {
                    ReNewSeed();
                    throw new IndexOutOfRangeException("Out of number, seed has been renewed");
                }

                _excludeNumbers.Add(number);
                return number;
            }
        }
    }
}