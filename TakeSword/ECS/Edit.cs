using System;

namespace TakeSword
{
    public class Edit<T> : IWritable

    {
        public Edit(T initialValue)
        {
            Value = initialValue;
        }
        public T Value { get; }
        internal bool Destroyed { get; set; } = false;
        internal T? NewValue { get; set; }
        public void Write(T newValue)
        {
            NewValue = newValue;
        }
        public void Destroy()
        {
            Destroyed = true;
        }

        public object? WrittenValue => NewValue ?? Value;

        public bool WasDestroyed => Destroyed;
    }

    public class Create<T> : Edit<T>, IOptional
    {
        public Create(T initialValue) : base(initialValue)
        {
        }
    }
}