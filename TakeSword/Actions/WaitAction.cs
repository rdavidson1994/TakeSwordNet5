namespace TakeSword
{
    public record WaitAction(Entity Actor) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            // Do nothing
            return ActionOutcome.Success();
        }
    }
}
