namespace TakeSword
{
    public abstract class PreparedAction : IGameAction
    {
        private uint preparationCompleted = 0;
        protected virtual uint PreparationNeeded => 0;
        public ActionOutcome Execute(bool dryRun = false)
        {
            if (dryRun)
            {
                return GetCompletionOutcome(dryRun: true);
            }
            else
            {
                if (preparationCompleted < PreparationNeeded)
                {
                    var preliminaryOutcome = GetCompletionOutcome(dryRun: true);
                    if (preliminaryOutcome.Status == ActionStatus.Failed)
                    {
                        return preliminaryOutcome;
                    }
                    else
                    {
                        preparationCompleted += 1;
                        return ActionOutcome.Progress();
                    }
                }
                else
                {
                    return GetCompletionOutcome();
                }
            }
        }

        protected abstract ActionOutcome GetCompletionOutcome(bool dryRun = false);
    }
}
