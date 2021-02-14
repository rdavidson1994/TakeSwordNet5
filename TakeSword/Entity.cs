namespace TakeSwordNet5
{
    public class Entity
    {
        public EntityId Id { get; }
        private World world;


        internal Entity(EntityId entityId, World world)
        {
            this.Id = entityId;
            this.world = world;
        }

        public T? Get<T>() where T : class
        {
            return world.GetComponent<T>(Id);
        }

        public void Set<T>(T newValue) where T : class
        {
            world.SetComponent<T>(Id, newValue);
        }

        public void Destroy()
        {
            world.DestroyEntity(Id);
        }
    }
}