namespace TakeSword
{
    public record DanceAction(Entity Actor) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            return ActionOutcome.Success("you do a little dance");
        }
    }
}
