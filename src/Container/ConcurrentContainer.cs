#region Using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

#endregion

namespace ProjectCeleste.Misc.Utils.Container
{
    public class ConcurrentContainer<T1, T2>
    {
        [XmlIgnore] [JsonIgnore] protected internal readonly ConcurrentDictionary<T1, T2> Values;

        public ConcurrentContainer()
        {
            Values = new ConcurrentDictionary<T1, T2>();
        }

        public ConcurrentContainer(IDictionary<T1, T2> values)
        {
            Values = new ConcurrentDictionary<T1, T2>(values);
        }

        public ConcurrentContainer(IEqualityComparer<T1> comparer)
        {
            Values = new ConcurrentDictionary<T1, T2>(comparer);
        }

        public ConcurrentContainer(IDictionary<T1, T2> values, IEqualityComparer<T1> comparer)
        {
            Values = new ConcurrentDictionary<T1, T2>(values, comparer);
        }

        [XmlIgnore]
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

        public event EventHandler<T2> OnRemoved;

        protected internal bool Remove(T1 key)
        {
            return Remove(key, out _);
        }

        protected internal bool Remove(T1 key, out T2 @out)
        {
            if (!Values.TryRemove(key, out @out))
                return false;

            OnRemoved?.Invoke(this, @out);

            return true;
        }

        public event EventHandler<T2> OnAdd;

        protected internal bool Add(T2 value, Func<T2, T1> keySelector)
        {
            var key = keySelector(value);
            if (!Values.TryAdd(key, value))
                return false;

            OnAdd?.Invoke(this, value);

            return true;
        }

        public event EventHandler<T2> OnUpdated;

        protected internal bool Update(T2 value, Func<T2, T1> keySelector)
        {
            var key = keySelector(value);

            if (!Values.TryGetValue(key, out T2 item))
                throw new KeyNotFoundException($"KeyNotFoundException '{key}'");

            if (!ReferenceEquals(value, item) && !Values.TryUpdate(key, value, item))
                return false;

            OnUpdated?.Invoke(this, value);

            return true;
        }

        protected internal void Clear()
        {
            Values.Clear();
        }

        public int Count()
        {
            return Gets().Count();
        }
    }
}