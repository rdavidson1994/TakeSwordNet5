using static TakeSword.ActionOutcome;

namespace TakeSword
{
    public record ConsumeAction(
        Entity Actor,
        Entity Target,
        ConsumeMode Mode = ConsumeMode.None
    ) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            var foodTraits = Target.Get<FoodTraits>();
            if (foodTraits is null)
                return Failure("the target is not consumable");

            if (foodTraits.IsBeverage && Mode == ConsumeMode.Eat)
                return Failure("you can't eat a beverage");

            if (!foodTraits.IsBeverage && Mode == ConsumeMode.Drink)
                return Failure("you can't drink solid food");

            var location = Target.GetParent<Location>();
            if (!Actor.Equals(location))
                return Failure("you must be holding the target in order to consume it");

            if (dryRun)
                return Success();

            Target.Destroy();
            return Success($"yum! the target tasted like {foodTraits.Amount} nutrition points");
        }
    }
}
