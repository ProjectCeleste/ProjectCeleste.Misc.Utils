using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectCeleste.Misc.Utils.RandomGenerator
{
    public class UniqueRandomIntGeneratorAlt
    {
        private  IList<int> _excludeNumbers;
        private readonly UniqueRandomIntGenerator _uniqueRandomIntGenerator;

        public UniqueRandomIntGeneratorAlt(
            int min = int.MinValue,
            int max = int.MaxValue,
            IEnumerable<int> excludeNumbers = null)
        {
            _uniqueRandomIntGenerator = new UniqueRandomIntGenerator(min, max);
            _excludeNumbers = excludeNumbers as List<int> ?? excludeNumbers?.ToList() ?? new List<int>();
        }

        public int Next()
        {
           return _uniqueRandomIntGenerator.Next(ref _excludeNumbers);
        }
    }

    public class UniqueRandomIntGenerator
    {
        private readonly int _randomMaxReTry = 25000 * Environment.ProcessorCount;
        private readonly object _syncLock = new object();
        private readonly int _minValue;
        private readonly int _maxValue;
        private Random _random;

        public UniqueRandomIntGenerator(
            int min = int.MinValue,
            int max = int.MaxValue)
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

        public int Next(ref IList<int> excludeNumbers)
        {
            lock (_syncLock)
            {
                var countReTry = 0L;
                var number = _random.Next(_minValue, _maxValue);
                while (excludeNumbers.Contains(number) && countReTry <= _randomMaxReTry)
                {
                    countReTry++;
                    number = _random.Next(_minValue, _maxValue);
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