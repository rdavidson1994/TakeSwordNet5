using static TakeSword.ActionOutcome;

namespace TakeSword
{
    /// <summary>
    /// Convenience class for implementing actions that never take any time to execute,
    /// and have no impact on the game state. The action never completes, and the output
    /// is returned as a failure message.
    /// </summary>
    public interface IInfoAction : IGameAction
    {
        public string Output();
        ActionOutcome IGameAction.Execute(bool dryRun)
        {
            return Failure(Output());
        }
    }
}
