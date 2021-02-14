using System;

namespace TakeSwordNet5
{
    internal interface IWrapperFactory
    {
        /// <summary>
        /// Creates a <see cref="Edit{T}"/> containing the provided <paramref name="initialValue"/>
        /// This method will fail with <see cref="InvalidCastException"/> if the runtime type of
        /// <paramref name="initialValue"/> does not match the generic type parameter of the underlying
        /// <see cref="WrapperFactory{T}"/> instance.
        /// </summary>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        public object CreateWritable(object initialValue);

        public object CreateOptional(object? initialValue);
        public object CreateWritableOptional(object? initialValue);
    }
}