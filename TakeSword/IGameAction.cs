namespace TakeSword
{
    public interface IGameAction
    {
        ActionOutcome Execute(bool dryRun = false);
    }
}
