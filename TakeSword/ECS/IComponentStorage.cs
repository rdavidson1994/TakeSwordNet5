using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TakeSword
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

        public class CustomJsonConverter : JsonConverter<IComponentStorage>
        {
            public override IComponentStorage? ReadJson(
                JsonReader reader,
                Type objectType,
                IComponentStorage? existingValue,
                bool hasExistingValue,
                JsonSerializer serializer)
            {
                // For now, lets just worry about ListComponentStorage...
                return serializer.Deserialize<ListComponentStorage>(reader);
            }

            public override void WriteJson(JsonWriter writer, IComponentStorage? value, JsonSerializer serializer)
            {
                throw new NotImplementedException(
                    "I don't *think* I need to implement serialization for you," +
                    " since the runtime type of a given reference shouldn't ever " +
                    "be an interface type...");
            }
        }
    }
}