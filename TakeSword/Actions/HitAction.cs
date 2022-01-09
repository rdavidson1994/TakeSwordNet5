using static TakeSword.ActionOutcome;

namespace TakeSword
{
    public record HitAction(Entity Actor, Entity Victim, Entity? Weapon = null) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            if (!Location.DoesMatch(Actor, Victim))
                return Failure("the target is not present");

            // Determine effective weapon traits
            WeaponTraits attackWeaponTraits;
            if (Weapon is not null)
            {
                // If a weapon is used, get it's traits directly
                var weaponTraits = Weapon.Get<WeaponTraits>();
                if (weaponTraits == null)
                    // Fail if the selected weapon doesn't have weapon traits
                    return Failure("that is not a suitable weapon.");
                attackWeaponTraits = weaponTraits;
            }
            else
            {
                // Otherwise, use weapon traits from the actor's natural weapon
                var attackAbility = Actor.Get<NaturalAttack>();
                if (attackAbility == null)
                    // Fail without it
                    return Failure("you have no natural methods of attacking.");

                (attackWeaponTraits, _) = attackAbility;
            }

            // End of dry run
            if (dryRun)
                return Success();



            (int damage, _) = attackWeaponTraits;
            var health = Victim.Get<Health>();

            if (health == null)
                return Failure("the target takes no damange from your attack");

            Victim.Set(health with { Amount = health - damage });
            return Success($"the victim takes {damage} damage. New health is {health - damage}");
        }
    }
}
