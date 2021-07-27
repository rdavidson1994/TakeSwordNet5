using System;
using System.Runtime.CompilerServices;


namespace TakeSword
{
    public static class DescriptionUtilities
    {
        public static OutputEntry GetDescription(Entity scene, Entity viewer)
        {
            Entity you = viewer;
            InteractiveSpan resolver(object symbol)
            {
                if (symbol is Entity entity)
                {
                    if (entity.Id.Equals(viewer.Id))
                    {
                        return new InteractiveSpan("you");
                    }
                    return entity.GenerateMenu(viewer);
                }
                else
                {
                    throw new Exception($"Unexpected entry {symbol}");
                }
            }
            OutputTemplate sceneDescription = new();
            Senses? senses = viewer.Get<Senses>();
            if (senses is null)
            {
                sceneDescription.AddFormat($"{you} cannot perceive anything.");
                return sceneDescription.Render(resolver);
            }

            if (scene.Has(out SceneDescription? desc))
            {
                sceneDescription.AddLines(desc.Lines);
            }
            else if (scene.Has(out Name? sceneName))
            {
                sceneDescription.AddFormat($"{you} are inside {scene}.");
            }
            else
            {
                sceneDescription.AddFormat($"{you} are inside an unnamed object.");
            }

            bool displayedItem = false;
            foreach (Entity item in scene.GetMembers<Location>())
            {
                if (!displayedItem)
                {
                    sceneDescription.AddLine("Nearby objects:");
                }
                if (item.Has<Visibility>())
                {
                    sceneDescription.AddFormat($"- {item}");
                    displayedItem = true;
                }
            }
            if (!displayedItem)
            {
                sceneDescription.AddLine("There are no items here");
            }

            return sceneDescription.Render(resolver);
        }
    }

}

