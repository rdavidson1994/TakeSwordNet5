using System.Collections.Generic;

namespace TakeSwordNet5
{
    public interface ICollectionComponent
    {
        List<EntityId> Members { get; }

        IEnumerable<Entity> EnumerateMembers(World world);
    }
}