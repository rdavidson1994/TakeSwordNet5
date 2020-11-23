namespace TakeSwordNet5
{
    public struct EntityId
    {
        internal int index;
        internal int generation;

        internal EntityId(int index, int generation)
        {
            this.index = index;
            this.generation = generation;
        }
    }
}