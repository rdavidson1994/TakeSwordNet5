using NUnit.Framework;
using System;
using TakeSword;

namespace TakeSwordTests
{
    public class WorldTests
    {
        private record FooComponent();
        private record BarComponent();
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
            Assert.Throws<ComponentException>(()=>world.CreateEntity(new FooComponent()));
        }

        [Test]
        public void SetComponent_UnregisteredComponent_ThrowsException()
        {
            world.RegisterComponent<FooComponent>();
            var entity = world.CreateEntity(new FooComponent());
            Assert.Throws<ComponentException>(() => world.SetComponent(entity, new BarComponent()));
        }

        [Test]
        public void GetComponent_UnregisteredComponent_ThrowsException()
        {
            var entity = world.CreateEntity();
            Assert.Throws<ComponentException>(() => world.GetComponent<FooComponent>(entity));
        }

        [Test]
        public void GetComponent_AbsentComponent_ReturnsNull()
        {
            world.RegisterComponent<FooComponent>();
            var entity = world.CreateEntity();
            Assert.IsNull(world.GetComponent<FooComponent>(entity));
        }

        [Test]
        public void GetComponent_PresentComponent_IsReturned()
        {
            world.RegisterComponent<NumberComponent>();
            var entity = world.CreateEntity();
            world.SetComponent(entity, new NumberComponent(42));
            var numberComponent = world.GetComponent<NumberComponent>(entity);
            Assert.AreEqual(42, numberComponent.Number);
        }

        public void GetComponent_OnDestroyedEntity_ReturnsNull()
        {
            world.RegisterComponent<NumberComponent>();
            var entity = world.CreateEntity();
            world.SetComponent(entity, new NumberComponent(42));
            world.DestroyEntity(entity);
            Assert.IsNull(world.GetComponent<NumberComponent>(entity));
        }
    }
}