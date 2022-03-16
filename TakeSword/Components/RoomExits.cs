using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeSword
{
    public record RoomExits(
        EntityId? North = null,
        EntityId? South = null,
        EntityId? East = null,
        EntityId? West = null,
        EntityId? Up = null,
        EntityId? Down = null
    ) : IDirectional<EntityId?>;
}
