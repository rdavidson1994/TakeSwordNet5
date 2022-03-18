namespace TakeSword
{
    public static class Death
    {
        public static void DeathSystem(EntityId self, Edit<Health> health, Edit<Actor<Entity>> actor, Edit<Name> name)
        {
            if (health.Value > 0)
                return;
            string nameString = name.Value;
            name.Write(new($"corpse of {nameString}"));
            health.Destroy();
            actor.Destroy();
        }
        public static void Install(IWorld world)
        {
            world.InstallSystem<
                Edit<Health>,
                Edit<Actor<Entity>>,
                Edit<Name>>(
                (self, health, actor, name) => DeathSystem(self, health, actor, name));
        }
    }
}
