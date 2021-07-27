using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public record DropAllAction(Entity Actor) : SequenceAction
    {
        protected override IEnumerable<IGameAction> GetActions()
        {
            var items = Actor.GetMembers<Location>().ToList();
            foreach (var item in items)
            {
                yield return new DropAction(Actor, item);
            }
        }
    }
}
