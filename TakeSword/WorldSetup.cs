using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TakeSwordTests")]
namespace TakeSword
{
    public static class WorldSetup
    {
        public static void Apply(World world, out Entity player, out Entity startLocation)
        {
            // Component registration
            void register<T>() where T : class
            {
                world.RegisterComponent<T>();
            }
            register<Senses>();
            register<ItemTraits>();
            register<Visibility>();
            register<Name>();
            register<FoodTraits>();
            register<SceneDescription>();
            register<Actor<Entity>>();
            register<Health>();
            register<NaturalAttack>();
            register<WeaponTraits>();

            // Collections
            world.RegisterCollection<Location>();

            // Systems
            CharacterActions.Install(world);
            Death.Install(world);

            // Entity creation
            VerbSuite<Entity> verbSuite = new(VerbUtil.GenerateVerbs());
            IActor<Entity> playerAI = new Player<Entity>(verbSuite, new ConsolePlayerIO());
            player = world.CreateEntity(
                new Name("player"),
                new Visibility(),
                new Senses(),
                new Actor<Entity>(playerAI),
                new Health(100),
                new NaturalAttack(
                    Attack: new(Damage: 10, DamageType.Blunt),
                    Name: "punch"
                )
            );

            var weirdo = world.CreateEntity(
                new Name("weirdo"),
                new Senses(),
                new Visibility(),
                new Health(50),
                new Actor<Entity>(new Script((self) =>
                {
                    System.Console.WriteLine("WEIRDO - doing the thing!");
                    return ActionOutcome.Progress("Doing the thing!");
                }))
            );

            Entity sword = world.CreateEntity(
                new Name("iron sword"),
                new ItemTraits(200),
                new Visibility(),
                new WeaponTraits(25, DamageType.Sharp)
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

