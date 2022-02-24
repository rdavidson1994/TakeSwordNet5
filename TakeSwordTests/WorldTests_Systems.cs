using NUnit.Framework;
using System.Collections.Generic;
using TakeSword;

namespace TakeSwordTests
{
    record Lycanthropy(int Severity);
    record FullMoonVisible(int Brightness);
    record Violence(int Amount);
    public class WorldTests_Systems
    {
        private World world;
        private EntityId alice;
        private EntityId bob;
        private EntityId charlie;
        private EntityId debby;

        [SetUp]
        public void SetUp()
        {
            world = new World();
            world.RegisterComponent<Lycanthropy>();
            world.RegisterComponent<FullMoonVisible>();
            world.RegisterComponent<Violence>();
            // Alice and bob have lycanthropy, but bob and charlie can see the moon.
            // Debby is a control.
            alice = world.CreateEntityId(new Lycanthropy(10));
            bob = world.CreateEntityId(new Lycanthropy(20), new FullMoonVisible(20));
            charlie = world.CreateEntityId(new FullMoonVisible(30));
            debby = world.CreateEntityId();

        }

        [Test]
        public void Run_SystemWithMandatoryParameter_RequiresMatchingComponent()
        {
            world.InstallSystem((
                EntityId id,
                Lycanthropy lycanthropy,
                FullMoonVisible fullMoon
            ) =>
            {
                // Only bob should enter this system
                Assert.AreEqual(bob, id);
                Assert.Pass();
            });
            world.Run();
            // If we get here without hitting the Assert.Pass in the system,
            // the test is failed.
            Assert.Fail();
        }

        [Test]
        public void Run_SystemWithCreateParameter_CanCreateTheSuchComponent()
        {
            world.InstallSystem((
                EntityId id,
                Lycanthropy lycanthropy,
                FullMoonVisible fullMoon,
                Create<Violence> violence
            ) =>
            {
                // Only bob should enter this system
                Assert.AreEqual(bob, id);
                violence.Write(new(9001));
            });
            world.Run();
            Assert.AreEqual(9001, world.GetComponent<Violence>(bob).Amount);
        }

        [Test]
        public void Run_SystemWithOptionalParameter_CanHitEntitiesWithoutSuchComponent()
        {
            Dictionary<EntityId, (int?, int)> results = new();
            world.InstallSystem((
                EntityId id,
                Optional<Lycanthropy> lycanthropy,
                FullMoonVisible fullMoon
            ) =>
            {
                results.Add(id, (lycanthropy.Value?.Severity, fullMoon.Brightness));
            });
            world.Run();
            Dictionary<EntityId, (int?, int)> expectedResults = new()
            {
                [bob] = (20, 20),
                [charlie] = (null, 30)
            };
            CollectionAssert.AreEquivalent(expectedResults, results);
        }

        [Test]
        public void Run_SystemWithEditParameter_OnlyHitsEntitiesWithThatComponent()
        {
            world.InstallSystem((
                EntityId id,
                Edit<Lycanthropy> lycanthropy,
                FullMoonVisible fullMoon
            ) =>
            {
                Assert.AreEqual(bob, id);
                lycanthropy.Write(lycanthropy.Value with { Severity = 100 });
            });
            world.Run();
            Assert.AreEqual(100, world.GetComponent<Lycanthropy>(bob).Severity);
        }

        [Test]
        public void Run_WithDeadEntities_SkipsThoseEntities()
        {
            List<EntityId> affectedIds = new();
            world.InstallSystem((
                EntityId id,
                Lycanthropy lycanthropy
            ) =>
            {
                affectedIds.Add(id);
            });
            world.DestroyEntity(bob);
            world.Run();
            CollectionAssert.AreEqual(new[] { alice }, affectedIds);
        }
    }
}