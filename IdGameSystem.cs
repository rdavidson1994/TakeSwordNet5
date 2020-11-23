using System;

namespace TakeSwordNet5
{
    public class IdGameSystem
    {
        internal IdGameSystem(Action<EntityId, object[]> effect, ParameterKey[] parameterKeys)
        {
            Effect = effect;
            ParameterKeys = parameterKeys;
        }

        public Action<EntityId, object[]> Effect { get; }
        internal ParameterKey[] ParameterKeys { get; }
    }
}