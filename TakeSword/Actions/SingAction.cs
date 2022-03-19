namespace TakeSword
{
    public record SingAction(Entity Actor) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            return ActionOutcome.Success("you sing a little song");
        }
    }
}
