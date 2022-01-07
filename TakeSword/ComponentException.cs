using System;

namespace TakeSword
{
    public class ComponentException : Exception
    {
        internal ComponentException(string message) : base(message)
        {

        }
    }
}