namespace TakeSword
{
    public record ActionOutcome(
        ActionStatus Status,
        string? Message
    )
    {
        private record DummyAction(ActionOutcome Outcome) : IGameAction
        {
            public ActionOutcome Execute(bool dryRun = false)
            {
                return Outcome;
            }
        }
        public static ActionOutcome Failure(string? message = null)
        {
            return new(ActionStatus.Failed, message);
        }

        public static ActionOutcome Success(string? message = null)
        {
            return new(ActionStatus.Successful, message);
        }

        public static ActionOutcome Progress(string? message = null)
        {
            return new(ActionStatus.InProgress, message);
        }

        public IGameAction AsAction()
        {
            return new DummyAction(this);
        }
    }
}
