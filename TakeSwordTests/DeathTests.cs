using NUnit.Framework;
using TakeSword;
namespace TakeSwordTests
{
    [TestFixture]
    public class DeathTests
    {
        private IWorld world;

        [SetUp]
        public void SetUp()
        {
            world = new World();
            world.RegisterComponent<Health>();
            world.RegisterComponent<Name>();
            world.RegisterComponent<Actor<Entity>>();
            Death.Install(world);
        }

        [Test]
        public void CreaturesWithNoHealthDie()
        {
            Entity alice = world.CreateEntity(
                new Health(0),
                new Actor<Entity>(null),
                new Name("alice")
            );
            world.Run();
            Assert.AreEqual("corpse of alice", alice.Get<Name>().Value);
            Assert.IsNull(alice.Get<Health>());
            Assert.IsNull(alice.Get<Actor<Entity>>());
        }

        [Test]
        public void CreaturesWithPositiveHealthSurvive()
        {
            var aliceActor = new Actor<Entity>(null);
            Entity alice = world.CreateEntity(
                new Health(100),
                aliceActor,
                new Name("alice")
            );
            world.Run();
            Assert.AreEqual("alice", alice.Get<Name>().Value);
            Assert.AreEqual(100, alice.Get<Health>().Amount);
            Assert.AreEqual(aliceActor, alice.Get<Actor<Entity>>());
        }
    }
}