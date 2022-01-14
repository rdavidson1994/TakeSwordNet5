using System;

namespace TakeSword
{
    public static class Death
    {
        public static void Install(World world)
        {
            world.InstallSystem((EntityId self, Edit<Health> health, Edit<Actor<Entity>> actor, Edit<Name> name) => {
                if (health.Value > 0)
                    return;
                string nameString = name.Value;
                Console.WriteLine($"{nameString} has died.");
                name.Write(new($"corpse of {nameString}"));
                health.Destroy();
                actor.Destroy();
            });
        }
    }
}
