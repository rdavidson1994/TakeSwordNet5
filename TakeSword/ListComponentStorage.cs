using System.Collections.Generic;

namespace TakeSword
{
    public class ListComponentStorage : IComponentStorage
    {
        private readonly List<object?> innerList;

        public ListComponentStorage(int count)
        {
            innerList = new List<object?>(new object?[count]);
        }

        public object? this[int index]
        {
            get => innerList[index];
            set => innerList[index] = value;
        }

        public void Expand()
        {
            innerList.Add(null);
        }

        public void Remove(int index)
        {
            innerList[index] = null;
        }
    }
}