using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ProjectCeleste.Misc.Utils.Extension;
using ProjectCeleste.Misc.Utils.RandomGenerator;

namespace ProjectCeleste.Misc.Utils
{
    public static class RandomUtils
    {
        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string RandomString(int length, bool allowDigit = true, bool allowLowerCase = false,
            bool allowSpecialChar = false)
        {
            int seed;
            using (var r = new CryptoRandomGenerator())
            {
                seed = r.Next(int.MinValue, int.MaxValue);
            }

            return RandomString(length, seed, allowDigit, allowLowerCase, allowSpecialChar);
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static string RandomString(int length, int seed, bool allowDigit = true, bool allowLowerCase = false,
            bool allowSpecialChar = false)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (allowLowerCase)
                chars += chars.ToLower();

            if (allowDigit)
                chars += "0123456789";

            if (allowSpecialChar)
                chars += @"$%-_#@&*-+=!?.,:;<>[]{}()/\|";

            var random = new Random(seed);

            return new string(Enumerable.Repeat(chars, length)
                .OrderBy(_ => random.Next()).Select(s => s[random.Next(s.Length - 1)]).ToArray());
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T RandomWithChance<T>([NotNull] params KeyValuePair<T, double>[] args)
        {
            args.ThrowIfNull(nameof(args));

            Random random;
            using (var r = new CryptoRandomGenerator())
            {
                random = new Random(r.Next(int.MinValue, int.MaxValue));
            }

            var rndList = args.OrderBy(_ => random.Next()).ToArray();
            var rndNumber = random.NextDouble();
            var cumulative = 0.0;

            foreach (var arg in rndList)
            {
                cumulative += arg.Value;
                if (rndNumber <= cumulative)
                    return arg.Key;
            }

            return rndList[0].Key;
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T RandomWithWeight<T>([NotNull] params KeyValuePair<T, int>[] args)
        {
            args.ThrowIfNull(nameof(args));

            if (args.Any(key => key.Value < 0))
                return args.Where(key => key.Value < 0).OrderByDescending(key => key).First().Key;

            args = args.Where(key => key.Value >= 0).ToArray();
            var sum = args.Sum(key => Convert.ToInt64(key.Value));
            return RandomWithChance((from value in args
                let chance = (double) value.Value / sum
                select new KeyValuePair<T, double>(value.Key, chance)).ToArray());
        }

        [UsedImplicitly]
        [NotNull]
        [Pure]
        public static T RandomWithoutChance<T>([NotNull] params T[] args)
        {
            args.ThrowIfNull(nameof(args));

            return RandomWithChance((from value in args
                let chance = 1.00 / args.Length
                select new KeyValuePair<T, double>(value, chance)).ToArray());
        }
    }
}