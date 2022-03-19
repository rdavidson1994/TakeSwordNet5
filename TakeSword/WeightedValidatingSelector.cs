using System;
using System.Collections.Generic;

namespace TakeSword
{
    public class WeightedValidatingSelector 
    {
        private record Entry(
            IGameAction Activity,
            uint Weight
        );
        private List<Entry> entries = new();

        public void AddEntry(IGameAction action, uint weight)
        {
            entries.Add(new Entry(action, weight));
        }

        public IGameAction SelectActivity(Entity Actor, double percentile)
        {
            uint totalWeight = 0;

            List<Entry> relevantEntries = new();
            foreach (Entry entry in entries)
            {
                if (entry.Activity.IsValid())
                {
                    relevantEntries.Add(entry);
                    totalWeight += entry.Weight;
                }
            }

            uint targetWeightFraction = (uint)(Math.Floor(totalWeight * percentile));
            uint currentWeightFraction = 0;
            for (int i = 0; i < relevantEntries.Count; i++)
            {
                if (currentWeightFraction > targetWeightFraction)
                {
                    return relevantEntries[i - 1].Activity;
                }
                currentWeightFraction += relevantEntries[i].Weight;
            }
            return relevantEntries[^1].Activity;
        }
    }
}

