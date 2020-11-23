namespace TakeSwordNet5
{
    internal class WriteWrapperFactory<T> : IWriteWrapperFactory
        where T : notnull
    {
        public Writable<T> Create(T initialValue)
        {
            return new Writable<T>(initialValue);
        }

        public object TrustedCreate(object initialValue)
        {
            return Create((T)initialValue);
        }
    }
}