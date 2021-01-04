using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectCeleste.Misc.Utils.RandomGenerator
{
    public static class Misc
    {
        private static readonly CryptoRandomGenerator CryptoRandom = new CryptoRandomGenerator();

        public static T RndWithChance<T>(params KeyValuePair<T, double>[] args)
        {
            var random = new Random(CryptoRandom.Next(int.MinValue, int.MaxValue));

            var rnd = random.NextDouble();

            var cumulative = 0.0;

            foreach (var arg in args.OrderBy(key => random.Next()))
            {
                cumulative += arg.Value;
                if (rnd <= cumulative)
                    return arg.Key;
            }

            return args.OrderByDescending(key => key.Value).First().Key;
        }

        public static T RndWithWeight<T>(params KeyValuePair<T, int>[] args)
        {
            return args.Any(key => key.Value == -1)
                ? args.First(key => key.Value == -1).Key
                : RndWithChance((from value in args
                                 let chance = (double)value.Value / args.Sum(key => Convert.ToInt64(key.Value))
                                 select new KeyValuePair<T, double>(value.Key, chance)).ToArray());
        }

        public static T RndWithoutChance<T>(params T[] args)
        {
            return RndWithChance((from value in args
                                  let chance = 1.00 / args.Length
                                  select new KeyValuePair<T, double>(value, chance)).ToArray());
        }

        public static string RandomString(int length, bool allowDigit = true, bool allowLowerCase = false,
            bool allowSpecialChar = false, int seed = -1)
        {
            var chars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (allowLowerCase)
                chars += chars.ToLower();

            if (allowDigit)
                chars += @"0123456789";

            if (allowSpecialChar)
                chars += @"$%-_#@&*-+=!?.,:;<>[]{}()/\|";

            if (seed == -1)
                seed = CryptoRandom.Next(int.MinValue, int.MaxValue);

            var random = new Random(seed);

            return new string(Enumerable.Repeat(chars, length)
                .OrderBy(s => random.Next()).Select(s => s[random.Next(s.Length - 1)]).ToArray());
        }
    }
}
