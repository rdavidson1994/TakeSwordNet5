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
