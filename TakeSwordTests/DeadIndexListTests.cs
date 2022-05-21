using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeSword;
using NUnit.Framework;
using Newtonsoft.Json;

namespace TakeSwordTests
{
    [TestFixture]
    public class DeadIndexListTests
    {
        [TestCase(10, 2,3,4)]
        [TestCase(5)]
        [TestCase(3, 0,1,2)]
        [TestCase(4, 0,2)]
        [TestCase(4, 0,1,2)]
        [TestCase(4, 0,1,2,3)]
        public void RoundTripSerializationIsSound(int memberCount, params int[] deadMembers)
        {
            DeadIndexList subject = new();
            for (int i = 0; i < memberCount; i++)
            {
                subject.AddLivingMember();
            }
            foreach (int deadMember in deadMembers)
            {
                subject.MarkDead(deadMember);
            }
            JsonConverter converter = new DeadIndexList.CustomJsonConverter();
            string serialized = JsonConvert.SerializeObject(subject, Formatting.None, new[] { converter });
            DeadIndexList deserialized = JsonConvert.DeserializeObject<DeadIndexList>(serialized, converter);
            Assert.Multiple(() => { 
                CollectionAssert.AreEquivalent(subject, deserialized);
            });
        }

    }
}
