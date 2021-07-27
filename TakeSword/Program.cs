using System;
using System.Runtime.CompilerServices;
using static TakeSword.ActionStatus;

[assembly: InternalsVisibleTo("TakeSwordTests")]
namespace TakeSword
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            World world = new();

            // Components
            world.RegisterComponent<Senses>();
            world.RegisterComponent<ItemTraits>();
            world.RegisterComponent<Visibility>();
            world.RegisterComponent<Name>();
            world.RegisterComponent<FoodTraits>();
            world.RegisterComponent<SceneDescription>();

            // Collections
            world.RegisterCollection<Location>();

            // Verbs
            var verbSuite = new VerbSuite<Entity>(VerbUtil.GenerateVerbs());

            // Entity creation
            Entity player = world.CreateEntity(
                new Name("player"),
                new Visibility(),
                new Senses()
            );

            Entity nameless = world.CreateEntity(
                new Visibility()
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

            Entity place = world.CreateEntity(
                new Name("plains"),
                new SceneDescription(new()
                {
                    "You stand among tall grasses in a field of gently rolling hills."
                })
            );

            player.Enter<Location>(place);
            sword.Enter<Location>(place);
            apple.Enter<Location>(place);


            OutputEntry description = DescriptionUtilities.GetDescription(place, player);
            Console.WriteLine(description.AsPlainText());


            while (true)
            {
                Console.Write(">");
                string input = Console.ReadLine()!;
                ActionOutcome? backupFailure = null;
                IGameAction? validAction = null;
                foreach (IGameAction action in verbSuite.GetMatches(player, input))
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

