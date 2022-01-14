namespace TakeSword
{
    public interface IActor<T>
    {
        ActionOutcome Act(T self);
    }
}

