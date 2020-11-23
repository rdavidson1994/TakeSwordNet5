namespace TakeSwordNet5
{
    internal interface IWritable
    {
        object GetWrittenValue();
        bool WasDestroyed();
    }
}