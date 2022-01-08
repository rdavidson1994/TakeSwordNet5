using System.Collections;
using System.Collections.Generic;

namespace TakeSword
{
    internal class DeadIndexList : IEnumerable<int>
    {
        private List<bool> isDeadByIndex = new();
        private List<int> deadIndexes = new();
        public int Count => deadIndexes.Count;
        public void MarkDead(int index)
        {
            deadIndexes.Add(index);
            isDeadByIndex[index] = true;
        }

        public bool Contains(int index)
        {
            return isDeadByIndex[index];
        }
        public void AddLivingMember()
        {
            isDeadByIndex.Add(false);
        }
        public bool ReclaimIndex(out int reclaimedIndex)
        {
            if (deadIndexes.Count == 0)
            {
                reclaimedIndex = -1;
                return false;
            }
            reclaimedIndex = deadIndexes[^1];
            deadIndexes.RemoveAt(deadIndexes.Count - 1);
            isDeadByIndex[reclaimedIndex] = false;
            return true;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>)deadIndexes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)deadIndexes).GetEnumerator();
        }
    }
}