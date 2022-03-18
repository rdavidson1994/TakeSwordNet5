namespace TakeSword
{
    public class Actor<T> : IActor<T>
    {
        private readonly IActor<T> implementor;

        public Actor(IActor<T> implementor)
        {
            this.implementor = implementor;
        }

        public ActionOutcome Act(T self)
        {
            return implementor.Act(self);
        }
    }
}

