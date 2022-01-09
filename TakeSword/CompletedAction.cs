namespace TakeSword
{
    public record CompletedAction(
        IGameAction Action,
        ActionOutcome Outcome
    );
}

