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

            string time = TimeUtilities.ClockTime(Actor.Time);
            string message = $"at {time} you decide to finish your adventures for the day";

            yield return Success(message).AsAction();

            yield return DowntimeUtilities.GetDowntimeActivity(Actor, DiceRoll);

            while (TimeUtilities.IsDay(Actor.Time))
            {
                yield return Success().AsAction();
            }
            time = TimeUtilities.ClockTime(Actor.Time);
            yield return Success($"you go to sleep at {time}").AsAction();
            
            while (TimeUtilities.IsNight(Actor.Time))
            {
                yield return Success().AsAction();
            }
            time = TimeUtilities.ClockTime(Actor.Time);
            yield return Success($"you wake up at {time}").AsAction();

        }
    }
}

