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