using System;

namespace TakeSwordNet5
{
    public struct EntityId
    {
        internal int index;
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

        public Entity? RetrieveEntity(World world)
        {
            if (!world.EntityIsCurrent(this))
            {
                return null;
            }
            else
            {
                return new Entity(this, world);
            }
        }
    }
}