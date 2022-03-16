namespace TakeSword
{
    public record Motion(EntityId Destination)
    {
        public static void SystemInstall(World world)
        {
            world.InstallSystem((EntityId self, Edit<Motion> motion) =>
            {
                world.SetMembership<Location>(self, new(), motion.Value.Destination);
                motion.Destroy();
            });
        }
    }
}

