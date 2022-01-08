namespace TakeSword
{
    public class Actor : IActor
    {
        private IActor implementor;

        public Actor(IActor implementor)
        {
            this.implementor = implementor;
        }

        public ActionOutcome Act(Entity self)
        {
            return implementor.Act(self);
        }
    }
}

