using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeSword;
using NUnit.Framework;

namespace TakeSwordTests
{
    public class PreparedActionTests
    {
        private record MockPreparedAction : PreparedAction
        {
            private ActionOutcome scriptedDryRunOutcome;
            private ActionOutcome scriptedActualOutcome;
            private readonly uint preparationNeeded;

            public MockPreparedAction(ActionOutcome scriptedDryRunOutcome, ActionOutcome scriptedActualOutcome, uint prepartionNeeded)
            {
                this.scriptedDryRunOutcome = scriptedDryRunOutcome;
                this.scriptedActualOutcome = scriptedActualOutcome;
                this.preparationNeeded = prepartionNeeded;
            }

            protected override uint PreparationNeeded => this.preparationNeeded;
            protected override ActionOutcome GetCompletionOutcome(bool dryRun = false)
            {
                if (dryRun)
                {
                    return this.scriptedDryRunOutcome;
                }
                else
                {
                    return this.scriptedActualOutcome;
                }
            }
        }

        // A dry run should fail if the ultimate action is invalid
        [TestCase(true)]
        // Non dry runs should also fail immediately in the same circumstance
        [TestCase(false)]
        public void FailedDryRunCausesImmediateFailure(bool doDryRun)
        {
            ActionOutcome irrelevantEventualOutcome = ActionOutcome.Success("This successful outcome should never occur");
            ActionOutcome expectedEarlyFailure = ActionOutcome.Failure("This failure from a dry run should be immediately returned");
            var testAction = new MockPreparedAction(
                scriptedActualOutcome: irrelevantEventualOutcome,
                scriptedDryRunOutcome: expectedEarlyFailure,
                prepartionNeeded: 2
            );

            Assert.That(testAction.Execute(dryRun: doDryRun), Is.EqualTo(expectedEarlyFailure));
        }

        [Test]
        public void SuccessfulDryRunsAllowProgressUntilUltimateFailure()
        {
            ActionOutcome eventualFailure = ActionOutcome.Failure("This failure should occur after 3 preparations are completed.");
            ActionOutcome successfulDryRunOutcome = ActionOutcome.Success("This success should silently allow the action to progress.");
            ActionOutcome expectedProgressOutcome = ActionOutcome.Progress();
            var testAction = new MockPreparedAction(
                scriptedActualOutcome: eventualFailure,
                scriptedDryRunOutcome: successfulDryRunOutcome,
                prepartionNeeded: 3
            );

            var result1 = testAction.Execute(dryRun: false);
            var result2 = testAction.Execute(dryRun: false);
            var result3 = testAction.Execute(dryRun: false);
            var result4 = testAction.Execute(dryRun: false);

            Assert.That(result1, Is.EqualTo(expectedProgressOutcome));
            Assert.That(result2, Is.EqualTo(expectedProgressOutcome));
            Assert.That(result3, Is.EqualTo(expectedProgressOutcome));
            Assert.That(result4, Is.EqualTo(eventualFailure));
        }

        [Test]
        public void SuccessfulDryRunsAllowProgressUntilUltimateSuccess()
        {
            ActionOutcome eventualSuccess = ActionOutcome.Success("This success should occur after 3 preparations are completed.");
            ActionOutcome successfulDryRunOutcome = ActionOutcome.Success("This success should silently allow the action to progress.");
            ActionOutcome expectedProgressOutcome = ActionOutcome.Progress();
            var testAction = new MockPreparedAction(
                scriptedActualOutcome: eventualSuccess,
                scriptedDryRunOutcome: successfulDryRunOutcome,
                prepartionNeeded: 4
            );

            var result1 = testAction.Execute(dryRun: false);
            var result2 = testAction.Execute(dryRun: false);
            var result3 = testAction.Execute(dryRun: false);
            var result4 = testAction.Execute(dryRun: false);
            var result5 = testAction.Execute(dryRun: false);

            Assert.That(result1, Is.EqualTo(expectedProgressOutcome));
            Assert.That(result2, Is.EqualTo(expectedProgressOutcome));
            Assert.That(result3, Is.EqualTo(expectedProgressOutcome));
            Assert.That(result4, Is.EqualTo(expectedProgressOutcome));
            Assert.That(result5, Is.EqualTo(eventualSuccess));
        }

        [Test]
        public void InterspersedDryRunsDoNotAffectOutcome()
        {
            ActionOutcome eventualSuccess = ActionOutcome.Success("This success should occur after 3 preparations are completed.");
            ActionOutcome successfulDryRunOutcome = ActionOutcome.Success("This success should only be returned by dry runs immediately before completion.");
            ActionOutcome expectedProgressOutcome = ActionOutcome.Progress();
            var testAction = new MockPreparedAction(
                scriptedActualOutcome: eventualSuccess,
                scriptedDryRunOutcome: successfulDryRunOutcome,
                prepartionNeeded: 4
            );

            var result1 = testAction.Execute(dryRun: false);
            var dryRun1 = testAction.Execute(dryRun: true);
            var dryRun2 = testAction.Execute(dryRun: true);
            var result2 = testAction.Execute(dryRun: false);
            var result3 = testAction.Execute(dryRun: false);
            var result4 = testAction.Execute(dryRun: false);
            var dryRun3 = testAction.Execute(dryRun: true);
            var result5 = testAction.Execute(dryRun: false);

            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.EqualTo(expectedProgressOutcome));
                Assert.That(dryRun1, Is.EqualTo(expectedProgressOutcome));
                Assert.That(dryRun2, Is.EqualTo(expectedProgressOutcome));
                Assert.That(result2, Is.EqualTo(expectedProgressOutcome));
                Assert.That(result3, Is.EqualTo(expectedProgressOutcome));
                Assert.That(result4, Is.EqualTo(expectedProgressOutcome));
                Assert.That(dryRun3, Is.EqualTo(successfulDryRunOutcome));
                Assert.That(result5, Is.EqualTo(eventualSuccess));
            });
        }
    }
}
