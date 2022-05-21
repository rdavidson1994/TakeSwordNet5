using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TakeSword
{
    internal class DeadIndexList : IEnumerable<int>
    {
        private readonly List<bool> isDeadByIndex = new();
        private readonly List<int> deadIndexes = new();
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

        internal class CustomJsonConverter : JsonConverter<DeadIndexList>
        {
            public override DeadIndexList? ReadJson(
                JsonReader reader,
                Type objectType,
                DeadIndexList? existingValue,
                bool hasExistingValue,
                JsonSerializer serializer)
            {
                if (hasExistingValue)
                {
                    throw new Exception("Reading onto existing DeadIndexList values isn't supported. Didn't think it would come up. Sorry.");
                }

                Dictionary<string, object>? foundDictionary = serializer.Deserialize<Dictionary<string, object>>(reader);
                if (foundDictionary is null)
                {
                    throw new Exception("Expected non null dictionary representation of DeadIndexList");
                }

                if (!foundDictionary.TryGetValue("_count", out object? countObject))
                {
                    throw new Exception("Expected _count field was not found.");
                }

                if (countObject is not long count)
                {
                    throw new Exception($"Expected count to be an integer, but was {countObject.GetType()}");
                }

                if (!foundDictionary.TryGetValue("_contents", out object? deadIndexesObject))
                {
                    throw new Exception("Expected _contents field was absent");
                }

                if (deadIndexesObject is not Newtonsoft.Json.Linq.JArray deadIndexes)
                {
                    throw new Exception($"Expected deadIndexes to be List<int>, but was {deadIndexesObject.GetType()}");
                }

                DeadIndexList output = new();
                for (int i = 0; i < count; i++)
                {
                    output.AddLivingMember();
                }

                foreach (int index in deadIndexes)
                {
                    output.MarkDead(index);
                }

                return output;
            }

            public override void WriteJson(
                JsonWriter writer,
                DeadIndexList? value,
                JsonSerializer serializer
            )
            {
                if (value == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    StringWriter deadIndexesWriter = new();
                    serializer.Serialize(deadIndexesWriter, value.deadIndexes);
                    string deadIndexesString = deadIndexesWriter.ToString();

                    writer.WriteRawValue("{"
                        + $"\"_count\": {value.isDeadByIndex.Count}, "
                        + $"\"_contents\": {deadIndexesString}"
                    + "}");
                }

            }
        }
    }
}