namespace TakeSword
{
    public static class GameActionExtensions
    {
        public static bool IsValid(this IGameAction gameAction)
        {
            return gameAction.Execute(dryRun: true).Status != ActionStatus.Failed;
        }
    }
}
