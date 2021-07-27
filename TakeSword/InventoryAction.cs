namespace TakeSword
{
    using System.Linq;
    using System.Text;
    using static ActionOutcome;

    public record InventoryAction(Entity Actor) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            if (dryRun)
                return Success();

            var items = Actor.GetMembers<Location>().ToList();
            if (items.Count == 0)
            {
                return Success("You aren't carrying anything.");
            }

            var output = new StringBuilder();
            foreach (var item in items)
            {
                string name = item.Get<Name>() ?? "unnamed object";
                output.AppendLine($"* {name}");
            }

            return Success(output.ToString());
        }
    }
}
