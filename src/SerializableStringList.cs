using System.Collections.Generic;
using System.Linq;

namespace Celeste.Misc.Utils
{
    public class SerializableStringList
    {
        private readonly IList<string> _items;

        public SerializableStringList(IList<string> items)
        {
            _items = items;
        }

        public SerializableStringList(string str)
        {
            _items = str
                .Split(',')
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .ToList();
        }

        public string SerializeList() => _items.ToStringList();
    }
}
