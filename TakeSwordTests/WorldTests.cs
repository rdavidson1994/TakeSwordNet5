using Newtonsoft.Json;
using NUnit.Framework;
using TakeSword;

namespace TakeSwordTests
{
    public class WorldTests
    {
        private record FooComponent(string FooContent);

        private record BarComponent(int BarRadius, int BarLength);
        private record NumberComponent(int Number);

        private World world;
        [SetUp]
        public void Setup()
        {
            world = new World();
        }

        [Test]
        public void CreateEntity_UnregistedComponent_ThrowsException()
        {
            Assert.Throws<ComponentException>(() => world.CreateEntityId(new FooComponent("some words")));
        }

        [Test]
        public void RegisterComponent_WithComponentAlreadyRegistered_ThrowsException()
        {
            world.RegisterComponent<FooComponent>();
            Assert.Throws<ComponentException>(() => world.RegisterComponent<FooComponent>());
        }

        [Test]
        public void SetComponent_UnregisteredComponent_ThrowsException()
        {
            world.RegisterComponent<FooComponent>();
            var entity = world.CreateEntityId(new FooComponent("hello"));
            Assert.Throws<ComponentException>(() => world.SetComponent(entity, new BarComponent(10, 20)));
        }

        [Test]
        public void GetComponent_UnregisteredComponent_ThrowsException()
        {
            var entity = world.CreateEntityId();
            Assert.Throws<ComponentException>(() => world.GetComponent<FooComponent>(entity));
        }

        [TestCase(ComponentStorage.Dictionary)]
        [TestCase(ComponentStorage.List)]
        public void GetComponent_AbsentComponent_ReturnsNull(ComponentStorage storageType)
        {
            world.RegisterComponent<FooComponent>(storageType);
            var entity = world.CreateEntityId();
            Assert.IsNull(world.GetComponent<FooComponent>(entity));
        }

        [TestCase(ComponentStorage.Dictionary)]
        [TestCase(ComponentStorage.List)]
        public void GetComponent_PresentComponent_IsReturned(ComponentStorage storageType)
        {
            world.RegisterComponent<NumberComponent>(storageType);
            var entity = world.CreateEntityId();
            world.SetComponent(entity, new NumberComponent(42));
            var numberComponent = world.GetComponent<NumberComponent>(entity);
            Assert.AreEqual(42, numberComponent.Number);
        }

        [TestCase(ComponentStorage.Dictionary)]
        [TestCase(ComponentStorage.List)]
        public void GetComponent_OnDestroyedEntity_ReturnsNull(ComponentStorage storageType)
        {
            world.RegisterComponent<NumberComponent>(storageType);
            var entity = world.CreateEntityId();
            world.SetComponent(entity, new NumberComponent(42));
            world.DestroyEntity(entity);
            Assert.IsNull(world.GetComponent<NumberComponent>(entity));
        }

        [TestCase(ComponentStorage.Dictionary)]
        [TestCase(ComponentStorage.List)]
        public void SetComponent_OnDestroyedEntity_ThrowsException(ComponentStorage storageType)
        {
            world.RegisterComponent<NumberComponent>(storageType);
            var entity = world.CreateEntityId();
            world.DestroyEntity(entity);
            Assert.Throws<ComponentException>(() => world.SetComponent(entity, new NumberComponent(42)));
        }

        [Test]
        public void CreateEntity_AfterEntityIsDestroyed_ReclaimsEmptyIndex()
        {
            EntityId entity0_0 = world.CreateEntityId();
            EntityId entity1_0 = world.CreateEntityId();
            world.DestroyEntity(entity0_0);
            EntityId entity0_1 = world.CreateEntityId();
            Assert.AreEqual(entity0_0, new EntityId(0, 0));
            Assert.AreEqual(entity1_0, new EntityId(1, 0));
            Assert.AreEqual(entity0_1, new EntityId(0, 1));
        }

        [TestCase(ComponentStorage.Dictionary)]
        [TestCase(ComponentStorage.List)]
        public void GetComponent_OnEntityOccupyingReclaimedIndex_DoesNotReturnStaleComponentData(ComponentStorage storageType)
        {
            world.RegisterComponent<NumberComponent>(storageType);
            EntityId entity0_0 = world.CreateEntityId();
            EntityId entity1_0 = world.CreateEntityId();
            world.SetComponent<NumberComponent>(entity0_0, new(5));
            world.SetComponent<NumberComponent>(entity1_0, new(10));
            world.DestroyEntity(entity0_0);
            EntityId entity0_1 = world.CreateEntityId();
            Assert.IsNull(world.GetComponent<NumberComponent>(entity0_1));
        }

        [TestCase(ComponentStorage.Dictionary)]
        [TestCase(ComponentStorage.List)]
        public void GetComponent_AfterComponentRemoved_ReturnsNull(ComponentStorage storageType)
        {
            world.RegisterComponent<NumberComponent>(storageType);
            EntityId entity = world.CreateEntityId(new NumberComponent(10));
            world.RemoveComponent<NumberComponent>(entity);
            Assert.IsNull(world.GetComponent<NumberComponent>(entity));
        }

        [Test]
        public void DestroyEntity_AfterEntityHasAlreadyBeenDestroyed_ThrowsExcpetion()
        {
            EntityId entity = world.CreateEntityId();
            world.DestroyEntity(entity);
            Assert.Throws<ComponentException>(() => world.DestroyEntity(entity));
        }

        [Test]
        public void SerializationYieldsExpectedJson()
        {
            world.RegisterComponent<NumberComponent>(ComponentStorage.List);
            EntityId entity0_0 = world.CreateEntityId();
            EntityId entity1_0 = world.CreateEntityId();
            world.SetComponent<NumberComponent>(entity0_0, new(5));
            world.SetComponent<NumberComponent>(entity1_0, new(10));
            world.DestroyEntity(entity0_0);
            EntityId _entity0_1 = world.CreateEntityId();
            string json = world.ToJson();
            Assert.Inconclusive(json);
        }

        [Test]
        public void SerializationYieldsExpectedJson2()
        {
            world.RegisterComponent<NumberComponent>(ComponentStorage.List);
            world.RegisterComponent<FooComponent>(ComponentStorage.List);
            world.RegisterComponent<BarComponent>(ComponentStorage.List);
            EntityId entity0_0 = world.CreateEntityId();
            EntityId entity1_0 = world.CreateEntityId();
            world.SetComponent<NumberComponent>(entity0_0, new(5));
            world.SetComponent(entity0_0, new FooComponent("sure hope I don't die before this gets printed!"));
            world.SetComponent<NumberComponent>(entity1_0, new(10));
            world.DestroyEntity(entity0_0);
            EntityId entity0_1 = world.CreateEntityId();
            world.SetComponent(entity0_1, new BarComponent(10, 20));
            world.SetComponent(entity0_1, new FooComponent("look, it's entity0_1!"));
            string json = world.ToJson();
            Assert.Inconclusive(json);
        }

        private class FakePlayerIO : IPlayerIO
        {
            public string ReadLine()
            {
                return "wait";
            }

            public void Write(string message)
            {
                
            }

            public void WriteLine(string message)
            {
                
            }
        }


        [Test]
        public void SerializationOfFullWorldYieldsExpectedJson()
        {
            WorldSetup.Apply(world, new FakePlayerIO(), out Entity _, out Entity _);
            string json = world.ToJson();
            Assert.Inconclusive(json);
        }

        [Test]
        [Ignore("Ignored WIP test")]
        public void RoundTripSerializationOfFullWorldIsSound()
        {
            WorldSetup.Apply(world, new FakePlayerIO(), out Entity _, out Entity _);
            string json = world.ToJson();
            World deserialized = World.FromJson(json);
            Assert.Inconclusive(json);
        }
    }
}