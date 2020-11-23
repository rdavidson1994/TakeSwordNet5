using System;

namespace TakeSwordNet5
{
    internal interface IWriteWrapperFactory
    {
        /// <summary>
        /// Creates a <see cref="Writable{T}"/> containing the provided <paramref name="initialValue"/>
        /// This method will fail with <see cref="InvalidCastException"/> if the runtime type of
        /// <paramref name="initialValue"/> does not match the generic type parameter of the underlying
        /// <see cref="WriteWrapperFactory{T}"/> instance.
        /// </summary>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        public object TrustedCreate(object initialValue);
    }
}