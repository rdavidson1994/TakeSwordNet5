namespace TakeSwordNet5
{
    public interface IComponentStorage
    {
        object? this[int index]
        {
            get;
            set;
        }

        public void Expand();
        public void Remove(int index);
    }
}