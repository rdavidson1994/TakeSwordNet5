using System;

namespace TakeSword
{
    public static class Death
    {
        public static void Install(World world)
        {
            world.InstallSystem((EntityId self, Edit<Health> health, Edit<Actor> actor, Edit<Name> name) => {
                if (health.Value > 0)
                    return;

                Console.WriteLine($"{name.Value} has died.");
                name.Write(new($"corpse of {name.Value}"));
                health.Destroy();
                actor.Destroy();
            });
        }
    }
}
