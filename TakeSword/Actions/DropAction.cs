namespace TakeSword
{
    using static ActionOutcome;

    public record DropAction(Entity Actor, Entity Target) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            if (!Actor.IsMember(out Entity? actorLocation, out Location _))
                return Failure("you do not have a physical location in the world");

            if (!Target.IsMember<Location>(Actor))
                return Failure("you do not have the item you are trying to drop");

            if (dryRun)
                return Success();

            Target.Enter<Location>(actorLocation);
            return Success();
        }
    }
}
