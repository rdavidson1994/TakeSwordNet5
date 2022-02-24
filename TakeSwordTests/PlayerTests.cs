using NUnit.Framework;
using System.Collections.Generic;
using TakeSword;
using DummyEntity = System.Int32;

namespace TakeSwordTests
{
    class SpyPlayerIO : IPlayerIO
    {
        public SpyPlayerIO(List<string> inputs)
        {
            this.inputs = inputs;
        }

        private List<string> inputs;
        public string OutputData { get; private set; } = "";

        public string ReadLine()
        {
            string result = inputs[0];
            inputs.RemoveAt(0);
            return result;
        }

        public void Write(string message)
        {
            OutputData += message;
        }

        public void WriteLine(string message)
        {
            OutputData += message + "\n";
        }
    }

    public class PlayerTests
    {
        private VerbSuite<DummyEntity> verbSuite;

        const DummyEntity PLAYER = 1;
        const DummyEntity IRON_SWORD = 2;
        const DummyEntity BRONZE_SWORD = 3;
        const DummyEntity DRAGON = 4;
        public static IReadOnlyList<DummyEntity> DummyLookup(DummyEntity entity, string name)
        {


            List<int> output = name switch
            {
                "dragon" => new() { DRAGON },
                "sword" => new() { IRON_SWORD, BRONZE_SWORD },
                "iron sword" => new() { IRON_SWORD },
                "bronze sword" => new() { BRONZE_SWORD },
                "player" => new() { PLAYER },
                _ => new()
            };

            return output;
        }



        public PlayerTests()
        {
            System.Func<string, IGameAction> ok = (string m) => ActionOutcome.Success(m).AsAction();
            var verbList = new List<IVerb<DummyEntity>>()
            {
                new ZeroTargetVerb<DummyEntity>((e)=>ok($"entity#{e}.meditate()"), "meditate"),
                new SingleTargetVerb<DummyEntity>(DummyLookup, (e, t)=>ok($"entity#{e}.contemplate(entity#{t})"), "contemplate"),
                new SingleTargetVerbWithTool<DummyEntity>(DummyLookup, (e, v, t)=>ok($"entity#{e}.shave(entity#{v}, entity#{t})"), "shave"),
            };
            verbSuite = new(verbList);
        }

        [Test]
        public void Meditate()
        {
            Player<DummyEntity> player = new(verbSuite, new SpyPlayerIO(new()
            {
                "meditate"
            }));

            ActionOutcome outcome = player.Act(PLAYER);
            Assert.AreEqual("entity#1.meditate()", outcome.Message);
        }
    }
}