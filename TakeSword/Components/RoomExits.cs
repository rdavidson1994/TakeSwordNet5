using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeSword
{
    public class RoomExits : IDictionary<Direction, EntityId>
    {
        private readonly Dictionary<Direction, EntityId> data;

        public RoomExits(Dictionary<Direction, EntityId> data)
        {
            this.data = data;
        }

        #region Implement IDictionary by delegation to this.data

        public EntityId this[Direction key] { get => ((IDictionary<Direction, EntityId>)data)[key]; set => ((IDictionary<Direction, EntityId>)data)[key] = value; }

        public ICollection<Direction> Keys => ((IDictionary<Direction, EntityId>)data).Keys;

        public ICollection<EntityId> Values => ((IDictionary<Direction, EntityId>)data).Values;

        public int Count => ((ICollection<KeyValuePair<Direction, EntityId>>)data).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<Direction, EntityId>>)data).IsReadOnly;

        public void Add(Direction key, EntityId value)
        {
            ((IDictionary<Direction, EntityId>)data).Add(key, value);
        }

        public void Add(KeyValuePair<Direction, EntityId> item)
        {
            ((ICollection<KeyValuePair<Direction, EntityId>>)data).Add(item);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<Direction, EntityId>>)data).Clear();
        }

        public bool Contains(KeyValuePair<Direction, EntityId> item)
        {
            return ((ICollection<KeyValuePair<Direction, EntityId>>)data).Contains(item);
        }

        public bool ContainsKey(Direction key)
        {
            return ((IDictionary<Direction, EntityId>)data).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<Direction, EntityId>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<Direction, EntityId>>)data).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<Direction, EntityId>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<Direction, EntityId>>)data).GetEnumerator();
        }

        public bool Remove(Direction key)
        {
            return ((IDictionary<Direction, EntityId>)data).Remove(key);
        }

        public bool Remove(KeyValuePair<Direction, EntityId> item)
        {
            return ((ICollection<KeyValuePair<Direction, EntityId>>)data).Remove(item);
        }

        public bool TryGetValue(Direction key, [MaybeNullWhen(false)] out EntityId value)
        {
            return ((IDictionary<Direction, EntityId>)data).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)data).GetEnumerator();
        }

        #endregion
    }
}
