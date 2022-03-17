namespace TakeSword
{
    public static class CharacterActions
    {
        public static void Install(IWorld world)
        {
            world.InstallSystem<Actor<Entity>>((entityId, actor) =>
            {
                if (world.RetrieveEntity(entityId) is Entity entity)
                {
                    actor.Act(entity);
                }
            });
        }
    }
}
