namespace TakeSword
{
    public abstract record PreparedAction : IGameAction
    {
        private uint preparationCompleted = 0;
        protected abstract uint PreparationNeeded { get; }
        public ActionOutcome Execute(bool dryRun = false)
        {
            if (dryRun)
            {
                var preliminaryOutcome = GetCompletionOutcome(dryRun: true);
                if (preliminaryOutcome.Status == ActionStatus.Failed)
                {
                    // If the action's completion ever becomes invalid,
                    // return the failure immediately to interupt the action.
                    return preliminaryOutcome;
                }
                else
                {
                    if (preparationCompleted < PreparationNeeded)
                    {
                        return ActionOutcome.Progress();
                    }
                    else
                    {
                        return preliminaryOutcome;
                    }
                }
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
                    return GetCompletionOutcome(dryRun: false);
                }
            }
        }

        protected abstract ActionOutcome GetCompletionOutcome(bool dryRun = false);
    }
}
