using System.Collections.Generic;

namespace TakeSword
{
    public interface ICollectionComponent
    {
        List<EntityId> Members { get; }

        IEnumerable<Entity> EnumerateMembers(World world);
    }
}