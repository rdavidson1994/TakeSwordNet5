using static TakeSword.ActionOutcome;
namespace TakeSword
{
    public record CampAction(Entity Actor) : PreparedAction
    {
        protected override uint PreparationNeeded => 10;

        protected override ActionOutcome GetCompletionOutcome(bool dryRun = false)
        {
            Entity? location = Actor.GetParent<Location>();
            if (location is null)
            {
                return Failure("you cannot camp when you lack a physical location");
            }

            var wilderness = location.Get<Wilderness>();
            if (wilderness is null)
            {
                return Failure("you can only camp in the wilderness");
            }

            if (wilderness.HasCampsite)
            {
                return Failure("there is already a campsite here");
            }

            if (!dryRun)
            {
                location.Set(wilderness with { HasCampsite = true });
            }
            return Success();
        }
    }
}
