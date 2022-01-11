using System;
using static TakeSword.ActionStatus;

namespace TakeSword
{
    public class Player : IActor
    {
        private readonly VerbSuite<Entity> verbSuite;
        private readonly IPlayerIO io;

        private CompletedAction? lastCompletedAction = null;

        public Player(VerbSuite<Entity> verbSuite, IPlayerIO io)
        {
            this.verbSuite = verbSuite;
            this.io = io;
        }

        public ActionOutcome Act(Entity self)
        {
            if (this.lastCompletedAction != null)
            {
                (var previousAction, var previousOutcome) = this.lastCompletedAction;

                if (previousOutcome.Status == InProgress)
                {
                    if (!string.IsNullOrEmpty(previousOutcome.Message))
                    {
                        io.WriteLine($"({previousOutcome.Message})...");
                    }
                    // Repeate (i.e. continue) the action if it is still in progress
                    ActionOutcome nextOutcome = previousAction.Execute();
                    this.lastCompletedAction = new CompletedAction(previousAction, nextOutcome);
                    return nextOutcome;
                }

                else
                {
                    if (!string.IsNullOrEmpty(previousOutcome.Message))
                    {
                        io.WriteLine($"{previousOutcome.Message}");
                    }
                }
            }
            while (true)
            {
                io.Write(">");
                string input = io.ReadLine();
                ActionOutcome? backupFailure = null;
                IGameAction? validAction = null;
                foreach (IGameAction action in verbSuite.GetMatches(self, input))
                {
                    ActionOutcome testOutcome = action.Execute(dryRun: true);
                    if (testOutcome.Status == Failed)
                    {
                        if (backupFailure == null)
                        {
                            backupFailure = testOutcome;
                        }
                    }
                    else
                    {
                        validAction = action;
                    }
                }

                if (validAction is null)
                {
                    if (string.IsNullOrEmpty(backupFailure?.Message))
                    {
                        Console.WriteLine("Invalid: I couldn't recognize the verb of that sentence.");
                    }
                    else
                    {
                        Console.WriteLine(backupFailure.Message);
                    }

                    continue;
                }

                ActionOutcome outcome = validAction.Execute();
                this.lastCompletedAction = new CompletedAction(validAction, outcome);
                return outcome;
            }
        }
    }
}

