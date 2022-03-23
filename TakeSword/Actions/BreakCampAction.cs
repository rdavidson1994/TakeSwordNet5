using static TakeSword.ActionOutcome;

namespace TakeSword
{
    public record BreakCampAction(Entity Actor) : PreparedAction
    {
        protected override uint PreparationNeeded => 10;

        protected override ActionOutcome GetCompletionOutcome(bool dryRun = false)
        {
            Entity? location = Actor.GetParent<Location>();
            Wilderness? wilderness = location?.Get<Wilderness>();
            if (location is null || wilderness is null || !wilderness.HasCampsite)
            {
                return Failure("there is no camp here");
            }
            if (!dryRun)
            {
                location.Set(wilderness with { HasCampsite = false });
            }
            return Success();
        }
    }
}
