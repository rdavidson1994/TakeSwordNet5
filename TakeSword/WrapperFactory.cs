namespace TakeSword
{
    internal class WrapperFactory<T> : IWrapperFactory
        where T : class
    {
        public Edit<T> CreateWritable(T initialValue)
        {
            return new Edit<T>(initialValue);
        }
        
        public Optional<T> CreateOptional(T? value)
        {
            return new Optional<T>(value);
        }

        public Create<T?> CreateWritableOptional(T? value)
        {
            return new Create<T?>(value);
        }
        public object CreateWritable(object initialValue)
        {
            return CreateWritable((T)initialValue);
        }

        public object CreateOptional(object? initialValue)
        {
            return CreateOptional((T?)initialValue);
        }

        public object CreateWritableOptional(object? initialValue)
        {
            return CreateWritableOptional((T?)initialValue);
        }
    }
}