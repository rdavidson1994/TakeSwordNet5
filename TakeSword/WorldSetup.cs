using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TakeSwordTests")]
namespace TakeSword
{
    public static class WorldSetup
    {
        public static void Apply(World world, IPlayerIO io, out Entity player, out Entity startLocation)
        {
            // Component registration
            world.RegisterComponent<Senses>();
            world.RegisterComponent<ItemTraits>();
            world.RegisterComponent<Visibility>();
            world.RegisterComponent<Name>();
            world.RegisterComponent<FoodTraits>();
            world.RegisterComponent<SceneDescription>();
            world.RegisterComponent<Actor<Entity>>();
            world.RegisterComponent<Health>();
            world.RegisterComponent<NaturalAttack>();
            world.RegisterComponent<WeaponTraits>();
            world.RegisterComponent<RoomExits>();
            world.RegisterComponent<Motion>();
            world.RegisterComponent<Wilderness>();

            // Collections
            world.RegisterCollection<Location>();

            // Systems
            CharacterActions.Install(world);
            Death.Install(world);
            Motion.SystemInstall(world);

            // Entity creation
            VerbSuite<Entity> verbSuite = new(VerbUtil.GenerateVerbs());
            IActor<Entity> playerAI = new Player<Entity>(verbSuite, io);
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

            var secondLocation = world.CreateEntity(
                new Name("shady alley"),
                new SceneDescription(new()
                {
                    "You are in a shady back alley."
                })
            );

            startLocation = world.CreateEntity(
                new Name("plains"),
                new SceneDescription(new()
                {
                    "You stand among tall grasses in a field of gently rolling hills."
                }),
                new Wilderness(
                    HasCampsite: false
                ),
                new RoomExits() with
                {
                    North = secondLocation.Id
                }
            );

            secondLocation.Set(new RoomExits() with
            {
                South = startLocation.Id
            });

            player.Enter<Location>(startLocation);
            sword.Enter<Location>(startLocation);
            apple.Enter<Location>(startLocation);
        }
    }
}

