using static TakeSword.ActionOutcome;

namespace TakeSword
{
    public record HitAction(Entity Actor, Entity Victim) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            if (!Location.DoesMatch(Actor, Victim))
                return Failure("the target is not present");

            var attackAbility = Actor.Get<AttackAbility>();
            if (attackAbility == null)
                return Failure("you have no natural methods of attacking.");

            // End of dry run
            if (dryRun)
                return Success();

            (AttackTraits attackTraits, _) = attackAbility;
            (int damage, _) = attackTraits;
            var health = Victim.Get<Health>();

            if (health == null)
                return Failure("the target takes no damange from your attack");
            

            Victim.Set(health with { Amount = health - damage });
            return Success($"the victim takes {damage} damage.");
        }
    }
}
