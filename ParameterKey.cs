namespace TakeSwordNet5
{
    internal struct ParameterKey
    {
        internal int componentId;
        internal bool needsWritable;

        internal ParameterKey(int componentId, bool needsWritable)
        {
            this.componentId = componentId;
            this.needsWritable = needsWritable;
        }
    }
}