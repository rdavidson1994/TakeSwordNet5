using System;
using System.Runtime.CompilerServices;
using static TakeSword.ActionStatus;

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
        }
    }
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Verbs
            var verbSuite = new VerbSuite<Entity>(VerbUtil.GenerateVerbs());

            World world = new();
            WorldSetup.Apply(world, out Entity playerEntity, out Entity startLocation);
            OutputEntry description = DescriptionUtilities.GetDescription(startLocation, playerEntity);
            Console.WriteLine(description.AsPlainText());
            
            while (true)
            {
                world.Run();
            }

            while (true)
            {
                Console.Write(">");
                string input = Console.ReadLine()!;
                ActionOutcome? backupFailure = null;
                IGameAction? validAction = null;
                foreach (IGameAction action in verbSuite.GetMatches(playerEntity, input))
                {
                    ActionOutcome testOutcome = action.Execute(dryRun: true);
                    if (testOutcome.Status == Failed)
                    {
                        if (backupFailure == null)
                        {
                            backupFailure = testOutcome;
                        }
                    }
                    else
                    {
                        validAction = action;
                    }
                }

                if (validAction is null)
                {
                    if (string.IsNullOrEmpty(backupFailure?.Message))
                    {
                        Console.WriteLine("Invalid: I couldn't recognize the verb of that sentence.");
                    }
                    else
                    {
                        Console.WriteLine($"Invalid: {backupFailure.Message}");
                    }

                    continue;
                }

                ActionOutcome outcome = validAction.Execute();
                while (outcome.Status == InProgress)
                {
                    if (string.IsNullOrEmpty(outcome.Message))
                    {
                        Console.WriteLine("...");
                    }
                    else
                    {
                        Console.WriteLine($"({outcome.Message})...");
                    }
                    outcome = validAction.Execute();
                }

                if (outcome.Status == Failed)
                {
                    if (string.IsNullOrEmpty(outcome.Message))
                    {
                        Console.WriteLine("Failed.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed: {outcome.Message}");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(outcome.Message))
                    {
                        Console.WriteLine("Done!");
                    }
                    else
                    {
                        Console.WriteLine(outcome.Message);
                    }
                }
                
            }
        }
    }
}

