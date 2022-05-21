using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TakeSword
{
    public class ListComponentStorage : IComponentStorage
    {
        public class CustomJsonConverter : JsonConverter<ListComponentStorage>
        {
            public override ListComponentStorage? ReadJson(
                JsonReader reader,
                Type objectType,
                ListComponentStorage? existingValue,
                bool hasExistingValue,
                JsonSerializer serializer)
            {
                if (existingValue is not null)
                {
                    throw new NotImplementedException("Didn't implement json " +
                        "deserialization on top of previous values");
                }
                throw new NotImplementedException("Still have to implement deserialization");
            }

            public override void WriteJson(
                JsonWriter writer,
                ListComponentStorage? value,
                JsonSerializer serializer)
            {
                if (value is null)
                {
                    writer.WriteNull();
                }
                else
                {
                    serializer.Serialize(writer, value.innerList);
                }
            }
        }
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