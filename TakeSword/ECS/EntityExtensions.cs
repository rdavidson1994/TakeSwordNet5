using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;

[assembly: InternalsVisibleTo("TakeSwordTests")]

namespace TakeSword
{
    public static class EntityExtensions
    {
        public static IEnumerable<Entity> EntitiesWithMatchingName(this IEnumerable<Entity> inputs, string name)
        {
            // First, yield any/all exact matches.
            bool foundExactMatch = false;
            foreach (Entity entity in EntitiesWithExactName(inputs, name))
            {
                foundExactMatch = true;
                yield return entity;
            }

            // If any exact matches were yielded, we are done.
            if (foundExactMatch)
            {
                yield break;
            }

            // Otherwise yield all partial matches.
            var desiredNamePieces = name.Split();
            foreach (Entity entity in inputs)
            {
                if (entity.Has(out Name? nameComponent))
                {
                    var actualNamePieces = nameComponent.Value.Split().ToHashSet();
                    if (desiredNamePieces.All(x => actualNamePieces.Contains(x)))
                    {
                        yield return entity;
                    }
                }
            }
        }

        public static IEnumerable<Entity> EntitiesWithExactName(this IEnumerable<Entity> inputs, string name)
        {
            foreach (Entity entity in inputs)
            {
                if (entity.Has(out Name? nameComponent) && nameComponent == name)
                {
                    yield return entity;
                }
            }
        }

        public static string? Name(this Entity subject, Entity viewer)
        {
            return subject.Get<Name>()?.Value;
        }

        public static IList<Entity> NearbyObjects(this Entity viewer)
        {
            var output = new List<Entity>();
            if (viewer.IsMember<Location>(out Entity? location))
            {
                var neighbors = location.GetMembers<Location>();
                if (neighbors is not null)
                {
                    foreach (Entity? neighbor in neighbors)
                    {
                        output.Add(neighbor);
                    }
                }
            }

            var inventory = viewer.GetMembers<Location>();
            if (inventory is not null)
            {
                foreach (Entity? item in inventory)
                {
                    output.Add(item);
                }
            }

            return output;
        }

        public static InteractiveSpan GenerateMenu(this Entity subject, Entity viewer)
        {
            List<MenuOption> options = new();
            string? name = subject.Name(viewer);
            if (name == null)
            {
                return new InteractiveSpan("unnamed object");
            }

            // Local convenience function, to keep things readable.
            void AddOption(string x)
            {
                options.Add(new(x, x + " " + name));
            }


            AddOption("examine");
            if (subject.Has<ItemTraits>())
            {
                if (!subject.IsMember<Location>(viewer))
                {
                    AddOption("take");
                }
                else
                {
                    AddOption("drop");
                }
            }

            if (subject.Has(out FoodTraits? foodTraits))
            {
                if (foodTraits.IsBeverage)
                {
                    AddOption("drink");
                }
                else
                {
                    AddOption("eat");
                }
            }
            return new InteractiveSpan(name, options);
        }

    }

}

