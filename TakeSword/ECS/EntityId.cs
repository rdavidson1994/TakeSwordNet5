using Newtonsoft.Json;
using System;

namespace TakeSword
{
    public struct EntityId
    {
        [JsonProperty]
        internal int index;
        [JsonProperty]
        internal int generation;

        internal EntityId(int index, int generation)
        {
            this.index = index;
            this.generation = generation;
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityId id &&
                   index == id.index &&
                   generation == id.generation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(index, generation);
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityId left, EntityId right)
        {
            return !(left == right);
        }
    }
}