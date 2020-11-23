using System.Collections.Generic;

namespace TakeSwordNet5
{
    public class DictionaryComponentStorage : IComponentStorage
    {
        private Dictionary<int, object?> innerDictionary;

        public DictionaryComponentStorage(Dictionary<int, object?> innerDictionary)
        {
            this.innerDictionary = innerDictionary;
        }

        public void Expand()
        {
            // Nothing to do - absent keys will already return null.
        }
        public object? this[int index]
        {
            get => innerDictionary[index];
            set => innerDictionary[index] = value;
        }

        public void Remove(int index)
        {
            innerDictionary.Remove(index);
        }
    }
}