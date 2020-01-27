using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.RandomGenerator
{
    [UsedImplicitly]
    public class UniqueRandomIntGenerator
    {
        private readonly int _randomMaxReTry = 25000 * Environment.ProcessorCount;
        private readonly object _syncLock = new object();
        private readonly IList<int> _excludeNumbers;
        private readonly int _minValue;
        private readonly int _maxValue;
        private Random _random;

        public UniqueRandomIntGenerator([Range(int.MinValue, int.MaxValue - 1)]
            int min = int.MinValue, [Range(int.MinValue + 1, int.MaxValue)]
            int max = int.MaxValue,
            [CanBeNull] IEnumerable<int> excludeNumbers = null)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), min, "minValue is greater than maxValue");

            _minValue = min;
            _maxValue = max;
            _excludeNumbers = excludeNumbers as List<int> ?? excludeNumbers.ToList();

            ReNewSeed();
        }

        private void ReNewSeed()
        {
            using var r = new CryptoRandomGenerator();
            _random = new Random(r.Next(int.MinValue, int.MaxValue));
        }

        [UsedImplicitly]
        [Pure]
        public int Next()
        {
            lock (_syncLock)
            {
                var countReTry = 0L;
                var number = _random.Next(_minValue, _maxValue);
                while (_excludeNumbers.Contains(number) && countReTry <= _randomMaxReTry)
                {
                    countReTry++;
                    number = _random.Next(_minValue, _maxValue);
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