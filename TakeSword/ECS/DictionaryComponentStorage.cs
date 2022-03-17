using System.Collections.Generic;

namespace TakeSword
{
    public class DictionaryComponentStorage : IComponentStorage
    {
        private readonly Dictionary<int, object?> innerDictionary = new();

        public void Expand()
        {
            // Nothing to do - absent keys will already return null.
        }
        public object? this[int index]
        {
            get => innerDictionary.GetValueOrDefault(index);
            set => innerDictionary[index] = value;
        }

        public void Remove(int index)
        {
            innerDictionary.Remove(index);
        }
    }
}