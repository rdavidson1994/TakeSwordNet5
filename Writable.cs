using System;

namespace TakeSwordNet5
{
    public class Writable<T> : IWritable
        where T : notnull
    {
        public Writable(T initialValue)
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

        public object GetWrittenValue()
        {
            return NewValue ?? Value;
        }

        public bool WasDestroyed()
        {
            return Destroyed;
        }
    }
}