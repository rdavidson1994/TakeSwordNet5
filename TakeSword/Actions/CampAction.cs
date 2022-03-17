namespace TakeSword
{
    public record CampAction(Entity Actor) : PreparedAction
    {
        protected override uint PreparationNeeded => 10;

        protected override ActionOutcome GetCompletionOutcome(bool dryRun = false)
        {
            // Todo - make sure you're outside
            if (dryRun)
            {
                return ActionOutcome.Success();
            }

            return ActionOutcome.Success();
        }
    }
}
