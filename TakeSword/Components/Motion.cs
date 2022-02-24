namespace TakeSword
{
    public record Motion(EntityId Destination)
    {
        public record System(World World)
        {
            public void Run(EntityId self, Motion motion)
            {
                World.SetMembership<Location>(self, new(), motion.Destination);
            }
        }

        public static void SystemInstall(World world)
        {
            world.InstallSystem((EntityId self, Motion motion) =>
            {
                world.SetMembership<Location>(self, new(), motion.Destination);
            });
        }
    }
}

