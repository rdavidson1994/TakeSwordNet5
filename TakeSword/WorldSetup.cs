using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TakeSwordTests")]
namespace TakeSword
{
    public static class WorldSetup
    {
        public static void Apply(World world, out Entity player, out Entity startLocation)
        {
            // Components
            world.RegisterComponent<Senses>();
            world.RegisterComponent<ItemTraits>();
            world.RegisterComponent<Visibility>();
            world.RegisterComponent<Name>();
            world.RegisterComponent<FoodTraits>();
            world.RegisterComponent<SceneDescription>();
            world.RegisterComponent<Actor>();

            // Collections
            world.RegisterCollection<Location>();

            // Systems
            world.InstallSystem<Actor>((entityId, actor) =>
            {
                if (world.RetrieveEntity(entityId) is Entity entity)
                {
                    actor.Act(entity);
                }
            });

            VerbSuite<Entity> verbSuite = new(VerbUtil.GenerateVerbs());
            // Entity creation
            player = world.CreateEntity(
                new Name("player"),
                new Visibility(),
                new Senses(),
                new Actor(new Player(verbSuite))
            );

            var weirdo = world.CreateEntity(
                new Name("weirdo"),
                new Senses(),
                new Visibility(),
                new Actor(new Script((self) =>
                {
                    System.Console.WriteLine("WEIRDO - doing the thing!");
                    return ActionOutcome.Progress("Doing the thing!");
                }))
            );

            Entity sword = world.CreateEntity(
                new Name("iron sword"),
                new ItemTraits(200),
                new Visibility()
            );

            Entity apple = world.CreateEntity(
                new Name("apple"),
                new ItemTraits(40),
                new FoodTraits(300),
                new Visibility()
            );

            startLocation = world.CreateEntity(
                new Name("plains"),
                new SceneDescription(new()
                {
                    "You stand among tall grasses in a field of gently rolling hills."
                })
            );

            player.Enter<Location>(startLocation);
            sword.Enter<Location>(startLocation);
            apple.Enter<Location>(startLocation);
            weirdo.Enter<Location>(startLocation);
        }
    }
}

