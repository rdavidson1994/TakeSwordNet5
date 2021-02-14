using System;

namespace TakeSword
{
    public static class WriteableUtil
    {
        public static Action<EntityId, Edit<T>> DecaySystem<T>(Action<Edit<T>> tick, Func<T, bool> done)
        {
            return (id, editT) =>
            {
                if (done(editT.Value))
                {
                    editT.Destroy();
                }
                else
                {
                    tick(editT);
                }
            };
        }
        public static void Destroy<T>(ref Edit<T>? writableT) where T : class
        {
            if (writableT != null)
            {
                writableT.Destroyed = true;
                writableT = null;
            }
        }
    }
}