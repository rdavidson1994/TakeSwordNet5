namespace TakeSword
{
    using System.Collections.Generic;
    using static ActionOutcome;
    using static ActionStatus;

    public abstract record SequenceAction : IGameAction
    {
        protected abstract IEnumerable<IGameAction> GetActions();

        private IEnumerator<IGameAction>? activeEnumerator;
        private IGameAction? peekedAction;

        public ActionOutcome Execute(bool dryRun = false)
        {
            if (activeEnumerator == null)
            {
                if (dryRun)
                {
                    return PreviewOutcome();
                }
                else
                {
                    activeEnumerator = GetActions().GetEnumerator();
                }
            }

            var action = GetCurrentAction(activeEnumerator);
            if (action == null)
            {
                // Once we are out of actions, the sequence is complete
                return Success();
            }

            var outcome = action.Execute(dryRun);
            if (outcome.Status == InProgress || outcome.Status == Failed)
            {
                return outcome;
            }

            else if (dryRun)
            {
                return outcome with { Status = InProgress };
            }

            else if (activeEnumerator.MoveNext())
            {
                // If not, we return InProgress instead of success
                peekedAction = activeEnumerator.Current;
                return outcome with { Status = InProgress };
            }

            else
            {
                return outcome;
            }
        }

        private ActionOutcome PreviewOutcome()
        {
            var temporaryEnumerator = GetActions().GetEnumerator();
            if (!temporaryEnumerator.MoveNext())
            {
                // If there are no actions to perform,
                // we are done.
                return Success();
            }
            var previewOutcome = temporaryEnumerator.Current.Execute(dryRun: true);
            ActionStatus filteredStatus = previewOutcome.Status switch
            {
                Successful => InProgress,
                InProgress => InProgress,
                Failed => Failed,
                _ => throw new System.NotImplementedException(),
            };
            return previewOutcome with { Status = filteredStatus };
        }

        private IGameAction? GetCurrentAction(IEnumerator<IGameAction> enumerator)
        {
            if (peekedAction != null)
            {
                return peekedAction;
            }
            else if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            else
            {
                return null;
            }
        }
    }
}
