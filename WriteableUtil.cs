namespace TakeSwordNet5
{
    public static class WriteableUtil
    {
        public static void Destroy<T>(ref Writable<T>? writableT) where T : class
        {
            if (writableT != null)
            {
                writableT.Destroyed = true;
                writableT = null;
            }
        }
    }
}