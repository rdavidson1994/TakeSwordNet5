using NUnit.Framework;
using TakeSword;

namespace TakeSwordTests
{
    public record HitPoints(int HP);
    public record TeamMemberTraits(int Number);
    public class WorldTests_CollectionComponents
    {
        private World world;
        private EntityId team;
        private EntityId alice;
        private EntityId bob;

        [SetUp]
        public void SetUp()
        {
            world = new World();
            world.RegisterCollection<TeamMemberTraits>();
            world.RegisterComponent<HitPoints>();
            team = world.CreateEntity();
            alice = world.CreateEntity();
            bob = world.CreateEntity();
        }

        [Test]
        public void Collection_ReportsAddedMembers()
        {
            world.SetMembership(alice, new TeamMemberTraits(12), team);
            var members = world.GetMembers<TeamMemberTraits>(team);
            int count = 0;
            foreach (var entity in members)
            {
                Assert.AreEqual(alice, entity.Id);
                count += 1;
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Member_RemembersCollection()
        {
            world.SetMembership(alice, new TeamMemberTraits(12), team);
            var tuple = world.GetMembership<TeamMemberTraits>(alice);
            Assert.AreEqual(tuple.Item1, new TeamMemberTraits(12));
            Assert.AreEqual(tuple.Item2, team);
        }

        [Test]
        public void GetMembers_WhenCollectionIsEmpty_ReportsEmptyEnumerable()
        {
            var results = world.GetMembers<TeamMemberTraits>(team);
            foreach (var result in results)
            {
                Assert.Fail();
            }
        }

        [Test] 
        public void GetMembers_FromMultipleCollections_YieldDistinctAndCorrectResults()
        {
            // todo
        }
    }
}