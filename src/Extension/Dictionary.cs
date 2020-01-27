using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class DictionaryExtensions
    {
        [UsedImplicitly]
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            [NotNull] Func<TValue> valueProvider)
        {
            dictionary.ThrowIfNull(nameof(dictionary));
            key.ThrowIfNull(nameof(key));
            valueProvider.ThrowIfNull(nameof(valueProvider));

            if (dictionary.TryGetValue(key, out var ret))
                return ret;
            ret = valueProvider();
            dictionary[key] = ret;
            return ret;
        }

        [UsedImplicitly]
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            [NotNull] TValue value)
        {
            dictionary.ThrowIfNull(nameof(dictionary));
            key.ThrowIfNull(nameof(key));
            value.ThrowIfNull(nameof(value));

            if (dictionary.TryGetValue(key, out var ret))
                return ret;
            ret = value;
            dictionary[key] = ret;
            return ret;
        }

        [UsedImplicitly]
        public static bool ChangeKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey oldKey, [NotNull] TKey newKey, out Exception exception)
        {
            dictionary.ThrowIfNull(nameof(dictionary));
            oldKey.ThrowIfNull(nameof(oldKey));
            newKey.ThrowIfNull(nameof(newKey));

            if (dictionary.IsReadOnly)
            {
                exception = new ReadOnlyException();
                return false;
            }

            if (!dictionary.TryGetValue(oldKey, out var value))
                throw new KeyNotFoundException($"Key '{oldKey} not found'");

            if (dictionary.ContainsKey(newKey))
                throw new ArgumentException($"Key '{newKey}' already used", nameof(newKey));

            dictionary.Remove(oldKey);

            try
            {
                dictionary.Add(newKey, value);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                exception = e;
                dictionary.Add(oldKey, value);
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            exception = null;
            return true;
        }
    }
}