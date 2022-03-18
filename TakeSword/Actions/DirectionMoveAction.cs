namespace TakeSword
{
    public record DirectionMoveAction(Entity Actor, Direction Direction) : PreparedAction
    {
        protected override uint PreparationNeeded => 3;
        protected override ActionOutcome GetCompletionOutcome(bool dryRun = false)
        {
            Entity? room = Actor.GetParent<Location>();
            if (room == null)
            {
                return ActionOutcome.Failure("you cannot move since you have no physical location in space.");
            }

            if (!room.Has(out RoomExits? roomExits))
            {
                return ActionOutcome.Failure("this area has no exits.");
            }

            EntityId? exit = roomExits.GetFacing(this.Direction);
            if (!exit.HasValue)
            {
                return ActionOutcome.Failure("there is no exit facing that direction");
            }

            if (!dryRun)
            {
                Actor.Set(new Motion(exit.Value));
            }

            return ActionOutcome.Success();
        }
    }
}
