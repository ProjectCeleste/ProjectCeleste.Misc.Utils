#region Using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Newtonsoft.Json;

#endregion

namespace ProjectCeleste.Misc.Utils.Container
{
    public class ConcurrentContainerCached<T1, T2> : IDisposable
    {
        [XmlIgnore] [JsonIgnore] protected internal readonly ConcurrentDictionary<T1, T2> Values;

        [XmlIgnore] [JsonIgnore] private Timer _snapshotTimer;

        [XmlIgnore] [JsonIgnore] private ReadOnlyCollection<T2> _valuesCache;

        public ConcurrentContainerCached() : this(new ConcurrentDictionary<T1, T2>())
        {
        }

        public ConcurrentContainerCached(IEqualityComparer<T1> comparer) : this(
            new ConcurrentDictionary<T1, T2>(comparer))
        {
        }

        public ConcurrentContainerCached(IDictionary<T1, T2> values) : this(new ConcurrentDictionary<T1, T2>(values))
        {
        }

        public ConcurrentContainerCached(IDictionary<T1, T2> values, IEqualityComparer<T1> comparer) : this(
            new ConcurrentDictionary<T1, T2>(values, comparer))
        {
        }

        public ConcurrentContainerCached(ConcurrentDictionary<T1, T2> values, int cacheDelay = 5)
        {
            Values = values;
            _valuesCache = new ReadOnlyCollection<T2>(Values.ToArray().Select(p => p.Value).ToArray());

            var interval = cacheDelay * 1000;
            _snapshotTimer = new Timer(TakeSnapshot, new object(), interval, interval);
        }

        [XmlIgnore]
        [JsonIgnore]
        public virtual T2 this[T1 key] => Values.TryGetValue(key, out T2 value)
            ? value
            : throw new KeyNotFoundException($"KeyNotFoundException '{key}'");

        public virtual void Dispose()
        {
            if (_snapshotTimer == null)
                return;

            _snapshotTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _snapshotTimer.Dispose();
            _snapshotTimer = null;
        }

        public T2 Get(T1 key)
        {
            return Values.TryGetValue(key, out T2 value)
                ? value
                : default(T2);
        }

        public T2 Get(Func<T2, bool> critera, bool useCache = true)
        {
            return useCache
                ? _valuesCache.FirstOrDefault(critera)
                : Values.ToArray().Select(p => p.Value).FirstOrDefault(critera);
        }

        public IEnumerable<T2> Gets(Func<T2, bool> critera, bool useCache = true)
        {
            return useCache ? _valuesCache.Where(critera) : Values.ToArray().Select(p => p.Value).Where(critera);
        }

        public IEnumerable<T2> Gets(bool useCache = true)
        {
            return useCache ? _valuesCache : Values.ToArray().Select(p => p.Value);
        }

        public event EventHandler<T2> OnRemoved;

        protected internal bool Remove_(T1 key)
        {
            return Remove_(key, out _);
        }

        protected internal bool Remove_(T1 key, out T2 @out)
        {
            if (!Values.TryRemove(key, out @out))
                return false;

            OnRemoved?.Invoke(this, @out);

            return true;
        }

        public event EventHandler<T2> OnAdd;

        protected internal bool Add_(T2 value, Func<T2, T1> keySelector)
        {
            var key = keySelector(value);
            if (!Values.TryAdd(key, value))
                return false;

            OnAdd?.Invoke(this, value);

            return true;
        }

        public event EventHandler<T2> OnUpdated;

        protected internal bool Update_(T2 value, Func<T2, T1> keySelector)
        {
            var key = keySelector(value);

            if (!Values.TryGetValue(key, out T2 item))
                throw new KeyNotFoundException($"KeyNotFoundException '{key}'");

            if (!ReferenceEquals(value, item) && !Values.TryUpdate(key, value, item))
                return false;

            OnUpdated?.Invoke(this, value);

            return true;
        }

        protected internal void Clear_()
        {
            Values.Clear();
        }

        public virtual int Count(bool useCache = true)
        {
            return Gets(useCache).Count();
        }

        private void TakeSnapshot(object state)
        {
            if (!Monitor.TryEnter(state))
                return;

            try
            {
                Interlocked.Exchange(ref _valuesCache,
                    new ReadOnlyCollection<T2>(Values.ToArray().Select(p => p.Value).ToArray()));
            }
            finally
            {
                Monitor.Exit(state);
            }
        }
    }
}