using System.Collections.Generic;

namespace TakeSword
{
    using static ActionOutcome;

    public record RelaxAction(Entity Actor, double DiceRoll) : SequenceAction
    {
        protected override IEnumerable<IGameAction> GetActions()
        {
            var location = Actor.GetParent<Location>();
            if (location is null)
            {
                yield return Failure("you have no physical location to rest in").AsAction();
                yield break;
            }

            var wilderness = location.Get<Wilderness>();
            if (wilderness is not null && !wilderness.HasCampsite)
            {
                yield return Failure("you cannot relax in the wilderness until you set camp").AsAction();
                yield break;
            }

            for (int i = 0; i < 14400; i++)
            {
                yield return Progress().AsAction();
            }

            yield return DowntimeUtilities.GetDowntimeActivity(Actor, DiceRoll);
        }
    }
}

