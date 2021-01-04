#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

#endregion

namespace ProjectCeleste.Misc.Utils.Container
{
    public class ReadOnlyContainer<T1, T2>
    {
        [JsonIgnore] protected internal readonly IReadOnlyDictionary<T1, T2> Values;

        public ReadOnlyContainer(IDictionary<T1, T2> values)
        {
            Values = new ReadOnlyDictionary<T1, T2>(new Dictionary<T1, T2>(values));
        }

        public ReadOnlyContainer(IDictionary<T1, T2> values, IEqualityComparer<T1> comparer)
        {
            Values = new ReadOnlyDictionary<T1, T2>(new Dictionary<T1, T2>(values, comparer));
        }

        [JsonIgnore]
        public T2 this[T1 key] => Values.TryGetValue(key, out T2 value)
            ? value
            : throw new KeyNotFoundException($"KeyNotFoundException '{key}'");

        public T2 Get(T1 key)
        {
            return Values.TryGetValue(key, out T2 value)
                ? value
                : default(T2);
        }

        public T2 Get(Func<T2, bool> critera)
        {
            return Gets().FirstOrDefault(critera);
        }

        public IEnumerable<T2> Gets(Func<T2, bool> critera)
        {
            return Gets().Where(critera);
        }

        public IEnumerable<T2> Gets()
        {
            return Values.ToArray().Select(p => p.Value);
        }
    }
}