namespace TakeSword
{
    public static class CharacterActions
    {
        public static void Install(World world)
        {
            world.InstallSystem<Actor>((entityId, actor) =>
            {
                if (world.RetrieveEntity(entityId) is Entity entity)
                {
                    actor.Act(entity);
                }
            });
        }
    }
}
