using System.Collections.Generic;
using System.Linq;
using static TakeSword.ActionOutcome;

namespace TakeSword
{
    public record TakeAllAction(Entity Actor) : SequenceAction
    {
        protected override IEnumerable<IGameAction> GetActions()
        {
            if (!Actor.IsMember<Location>(out Entity? location))
            {
                yield return Failure("you do not have a physical" +
                    " location in space, so there is nothing for" +
                    " you to take").AsAction();
                yield break;
            }

            foreach (var item in location.GetMembers<Location>().ToList())
            {
                if (item.Has<ItemTraits>())
                {
                    yield return new TakeAction(Actor, item);
                }
            }

        }
    }
}
