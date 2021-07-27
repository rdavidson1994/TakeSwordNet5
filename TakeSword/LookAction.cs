using static TakeSword.ActionOutcome;

namespace TakeSword
{
    public record LookAction(Entity Actor) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            if (dryRun)
                return Success();
            if (Actor.GetParent<Location>() is not Entity location)
                return Failure("you do not have a physical location to examine.");
            OutputEntry description = DescriptionUtilities.GetDescription(location, Actor);
            return Success(description.AsPlainText());
        }
    }
}
