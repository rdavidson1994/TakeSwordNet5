namespace TakeSword
{
    internal struct ParameterKey
    {
        internal int componentId;
        internal bool needsWritable;
        internal bool optional;

        internal ParameterKey(int componentId, bool needsWritable, bool optional)
        {
            this.componentId = componentId;
            this.needsWritable = needsWritable;
            this.optional = optional;
        }
    }
}