namespace TakeSword
{
    using System.Linq;
    using System.Text;

    public record InventoryAction(Entity Actor) : IInfoAction
    {
        public string Output()
        {
            var items = Actor.GetMembers<Location>().ToList();
            if (items.Count == 0)
            {
                return "You aren't carrying anything.";
            }

            var output = new StringBuilder();
            foreach (var item in items)
            {
                string name = item.Get<Name>() ?? "unnamed object";
                output.AppendLine($"* {name}");
            }

            return output.ToString();
        }
    }
}
