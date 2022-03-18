using static TakeSword.ActionOutcome;

namespace TakeSword
{
    public record TakeAction(Entity Actor, Entity Target) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            if (!Actor.IsMember(out Entity? actorLocation, out Location _))
                return Failure("you do not have a physical location in the world");

            if (Target.IsMember<Location>(Actor))
                return Failure("you already have the target");

            if (!Target.IsMember<Location>(actorLocation))
                return Failure("the item you are trying to take is not here");

            if (!Target.Has<ItemTraits>())
                return Failure("you cannot take the target because " +
                    "it is not a reasonably sized inanimate object.");

            if (dryRun)
                return Success();

            Target.Enter<Location>(Actor);
            return Success();
        }
    }
}
