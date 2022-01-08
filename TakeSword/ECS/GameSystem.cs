using System;

namespace TakeSword
{
    public class GameSystem
    {
        internal GameSystem(Action<EntityId, object[]> effect, ParameterKey[] parameterKeys)
        {
            Effect = effect;
            ParameterKeys = parameterKeys;
        }

        public Action<EntityId, object[]> Effect { get; }
        internal ParameterKey[] ParameterKeys { get; }
    }
}