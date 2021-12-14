using System;
using System.Collections.Generic;
using System.Linq;
using ProjectCeleste.Misc.Utils.Extension;

namespace ProjectCeleste.Misc.Utils.RandomGenerator
{
    public class UniqueRandomLongGeneratorAlt
    {
        private IList<long> _excludeNumbers;
        private readonly UniqueRandomLongGenerator _uniqueRandomLongGenerator;

        public UniqueRandomLongGeneratorAlt(
            long min = long.MinValue,
            long max = long.MaxValue,
            IEnumerable<long> excludeNumbers = null)
        {
            _uniqueRandomLongGenerator = new UniqueRandomLongGenerator(min, max);
            _excludeNumbers = excludeNumbers as List<long> ?? excludeNumbers?.ToList() ?? new List<long>();
        }

        public long Next()
        {
            return _uniqueRandomLongGenerator.Next(ref _excludeNumbers);
        }
    }

    public class UniqueRandomLongGenerator
    {
        private readonly long _randomMaxReTry = 25000 * Environment.ProcessorCount;
        private readonly object _syncLock = new object();
        private readonly long _minValue;
        private readonly long _maxValue;
        private Random _random;

        public UniqueRandomLongGenerator(
            long min = long.MinValue,
            long max = long.MaxValue)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), min, "minValue is greater than maxValue");

            _minValue = min;
            _maxValue = max;

            ReNewSeed();
        }

        private void ReNewSeed()
        {
            using var r = new CryptoRandomGenerator();
            _random = new Random(r.Next(int.MinValue, int.MaxValue));
        }

        public long Next(ref IList<long> excludeNumbers)
        {
            lock (_syncLock)
            {
                var countReTry = 0L;
                var number = _random.NextLong(_minValue, _maxValue);
                while (excludeNumbers.Contains(number) && countReTry <= _randomMaxReTry)
                {
                    countReTry++;
                    number = _random.NextLong(_minValue, _maxValue);
                }

                if (excludeNumbers.Contains(number))
                {
                    ReNewSeed();
                    throw new IndexOutOfRangeException("Out of number, seed has been renewed");
                }

                excludeNumbers.Add(number);
                return number;
            }
        }
    }
}