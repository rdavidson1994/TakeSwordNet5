using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public static class VerbUtil
    {
        static IReadOnlyList<Entity> LookupFunction(Entity subject, string name)
        {
            return subject.NearbyObjects().EntitiesWithMatchingName(name).ToList();
        }

        static SingleTargetVerb<Entity> OneTarget(Func<Entity, Entity, IGameAction> actionFactory, params string[] synonyms)
        {
            return new SingleTargetVerb<Entity>(LookupFunction, actionFactory, synonyms);
        }

        static ZeroTargetVerb<Entity> ZeroTarget(Func<Entity, IGameAction> actionFactory, params string[] synonyms)
        {
            return new ZeroTargetVerb<Entity>(actionFactory, synonyms);
        }

        static SingleTargetVerbWithTool<Entity> OneTargetWithTool(Func<Entity, Entity, Entity, IGameAction> actionFactory, params string[] synonyms)
        {
            return new SingleTargetVerbWithTool<Entity>(LookupFunction, actionFactory, synonyms);
        }

        public static IReadOnlyList<IVerb<Entity>> GenerateVerbs()
        {
            List<IVerb<Entity>> output = new List<IVerb<Entity>>()
            {
                ZeroTarget(a => new QuitGameCommand(), "quit"),
                ZeroTarget(a => new DropAllAction(a), "drop all", "put down all", "discard all"),
                ZeroTarget(a => new TakeAllAction(a), "take all", "get all", "pick up all"),
                ZeroTarget(a => new InventoryAction(a), "inventory", "i", "items"),
                ZeroTarget(a => new LookAction(a), "look", "examine", "x"),
                ZeroTarget(a => new WaitAction(a), "wait"),
                ZeroTarget(a => new CampAction(a), "camp", "make camp"),
                OneTargetWithTool((actor, target, weapon) => new HitAction(actor, target, weapon), "hit", "strike", "attack"),
                OneTarget((a, t) => new DropAction(a, t), "drop", "put down", "discard"),
                OneTarget((a, t) => new TakeAction(a, t), "take", "get", "pick up"),
                OneTarget((a, t) => new ConsumeAction(a, t), "consume"),
                OneTarget((a, t) => new ConsumeAction(a, t, ConsumeMode.Eat), "eat"),
                OneTarget((a, t) => new ConsumeAction(a, t, ConsumeMode.Drink), "drink"),
                OneTarget((a, t) => new HitAction(a, t), "hit", "strike", "attack"),
            };

            foreach (Direction d in DirectionExtensions.EnumerateDirections())
            {
                string name = d.Name();
                string letter = name[0].ToString();

                var moveVerb = ZeroTarget(a => new DirectionMoveAction(a, d), name, letter, "go "+name, "go "+letter);
                output.Add(moveVerb);

            }


            return output;
        }
    }
}
