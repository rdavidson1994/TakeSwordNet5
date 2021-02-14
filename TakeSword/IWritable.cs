namespace TakeSword
{
    internal interface IWritable
    {
        // All implementations should have one generic type argument representing the wrapped type
        object? WrittenValue { get; }

        bool WasDestroyed { get; }
    }
}