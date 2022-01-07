namespace TakeSword
{
    public record Script(System.Func<Entity, ActionOutcome> Lambda) : IActor
    {
        public ActionOutcome Act(Entity self)
        {
            return Lambda(self);
        }
    }
}

