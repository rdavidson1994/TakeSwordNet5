using static TakeSword.ActionOutcome;

namespace TakeSword
{

    public record LookAction(Entity Actor) : IInfoAction
    {
        public string Output()
        {
            if (Actor.GetParent<Location>() is not Entity location)
                return "you do not have a physical location to examine.";
            OutputEntry description = DescriptionUtilities.GetDescription(location, Actor);
            return description.AsPlainText();
        }
    }
}
