using NUnit.Framework;
using System.Linq;
using TakeSword;

namespace TakeSwordTests
{
    public record HitPoints(int Number);
    public record TeamMemberTraits(int Number);
    public class WorldTests_CollectionComponents
    {
        private World world;
        private EntityId redTeam;
        private EntityId blueTeam;
        private EntityId alice;
        private EntityId bob;
        private EntityId charlie;
        private EntityId debby;

        [SetUp]
        public void SetUp()
        {
            world = new World();
            world.RegisterCollection<TeamMemberTraits>();
            world.RegisterComponent<HitPoints>();
            blueTeam = world.CreateEntityId();
            redTeam = world.CreateEntityId();
            alice = world.CreateEntityId();
            bob = world.CreateEntityId();
            charlie = world.CreateEntityId();
            debby = world.CreateEntityId();
        }

        [Test]
        public void RegisterCollection_WithTypeAlreadyRegisteredAsComponent_ThrowsException()
        {
            Assert.Throws<ComponentException>(() => world.RegisterCollection<HitPoints>());
        }

        [Test]
        public void GetMembers_WithMembersInCollection_ReportsThoseMembers()
        {
            world.SetMembership(alice, new TeamMemberTraits(12), redTeam);
            var members = world.GetMembers<TeamMemberTraits>(redTeam);
            int count = 0;
            foreach (var entity in members)
            {
                Assert.AreEqual(alice, entity.Id);
                count += 1;
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Getmembership_WithMembershipAlreadySet_ReturnsExistingValue()
        {
            world.SetMembership(alice, new TeamMemberTraits(12), redTeam);
            var tuple = world.GetMembership<TeamMemberTraits>(alice);
            Assert.AreEqual(tuple.Item1, new TeamMemberTraits(12));
            Assert.AreEqual(tuple.Item2, redTeam);
        }

        [Test]
        public void GetMembers_WhenCollectionIsEmpty_ReportsEmptyEnumerable()
        {
            var results = world.GetMembers<TeamMemberTraits>(redTeam);
            foreach (var _ in results)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetMembers_FromMultipleCollections_YieldDistinctAndCorrectResults()
        {
            world.SetMembership(alice, new TeamMemberTraits(10), redTeam);
            world.SetMembership(bob, new TeamMemberTraits(11), blueTeam);
            world.SetMembership(charlie, new TeamMemberTraits(20), redTeam);
            world.SetMembership(debby, new TeamMemberTraits(30), blueTeam);
            var redIds = world.GetMembers<TeamMemberTraits>(redTeam).Select((e) => e.Id);
            var blueIds = world.GetMembers<TeamMemberTraits>(blueTeam).Select((e) => e.Id);
            CollectionAssert.AreEqual(redIds, new[] { alice, charlie });
            CollectionAssert.AreEqual(blueIds, new[] { bob, debby });
        }

        [Test]
        public void GetMembers_FromDestroyedCollection_ReturnsEmptyEnumerable()
        {
            world.SetMembership(alice, new TeamMemberTraits(10), redTeam);
            world.DestroyEntity(redTeam);
            CollectionAssert.IsEmpty(world.GetMembers<TeamMemberTraits>(redTeam));
        }

        [Test]
        public void GetMembership_AfterCollectionIsDestroyed_ReturnsNull()
        {
            world.SetMembership(alice, new TeamMemberTraits(10), redTeam);
            world.DestroyEntity(redTeam);
            Assert.IsNull(world.GetMembership<TeamMemberTraits>(alice));
        }

        [Test]
        public void GetMembers_AfterMembersAreDestroyed_ReturnsEmptyEnumerable()
        {
            world.SetMembership(alice, new TeamMemberTraits(10), redTeam);
            world.DestroyEntity(alice);
            CollectionAssert.IsEmpty(world.GetMembers<TeamMemberTraits>(redTeam));
        }

        [Test]
        public void GetMembers_AfterSomeMembersAreDestroyed_StillReturnsRemainingMembers()
        {
            world.SetMembership(alice, new TeamMemberTraits(10), redTeam);
            world.SetMembership(bob, new TeamMemberTraits(20), redTeam);
            world.DestroyEntity(bob);
            var redIds = world.GetMembers<TeamMemberTraits>(redTeam).Select((e) => e.Id);
            CollectionAssert.AreEqual(redIds, new[] { alice });
        }

        [Test]
        public void GetMembers_AfterSomeMembersAreRemoved_StillReturnsRemainingMembers()
        {
            world.SetMembership(alice, new TeamMemberTraits(10), redTeam);
            world.SetMembership(bob, new TeamMemberTraits(20), redTeam);
            world.RemoveMembership<TeamMemberTraits>(bob);
            var redIds = world.GetMembers<TeamMemberTraits>(redTeam).Select((e) => e.Id);
            CollectionAssert.AreEqual(redIds, new[] { alice });
        }

        [Test]
        public void GetComponent_WhenUsedOnMembershipDataType_ThrowsException()
        {
            Assert.Throws<ComponentException>(() => world.GetComponent<TeamMemberTraits>(alice));
        }

        [Test]
        public void SetComponent_WhenUsedOnMembershipDataType_ThrowsException()
        {
            Assert.Throws<ComponentException>(() => world.SetComponent<TeamMemberTraits>(alice, new(10)));
        }

        [Test]
        public void GetMembership_ForEntityThatIsNotAMember_ReturnsNull()
        {
            Assert.IsNull(world.GetMembership<TeamMemberTraits>(alice));
        }

        [Test]
        public void SetMembership_OnEntityThatAlreadyHasMembership_MovesItBetweenCollections()
        {
            world.SetMembership(alice, new TeamMemberTraits(10), redTeam);
            world.SetMembership(bob, new TeamMemberTraits(20), redTeam);
            world.SetMembership(alice, new TeamMemberTraits(30), blueTeam);
            var redMembers = world.GetMembers<TeamMemberTraits>(redTeam).Select(x=>x.Id);
            var blueMembers = world.GetMembers<TeamMemberTraits>(blueTeam).Select(x=>x.Id);
            CollectionAssert.AreEqual(new[] { bob }, redMembers);
            CollectionAssert.AreEqual(new[] { alice }, blueMembers);
        }

        [Test]
        public void SetMembership_OnEntityWithRecentlyDestroyedCollection_MovesToNewCollection()
        {
            world.SetMembership(alice, new TeamMemberTraits(10), redTeam);
            world.SetMembership(bob, new TeamMemberTraits(20), redTeam);
            world.DestroyEntity(redTeam);
            world.SetMembership(alice, new TeamMemberTraits(30), blueTeam);
            var blueMembers = world.GetMembers<TeamMemberTraits>(blueTeam).Select(x => x.Id);
            CollectionAssert.AreEqual(new[] { alice }, blueMembers);
        }

        [Test]
        public void SetMembership_WithSameCollectionEntity_UpdatesMembershipDataOnly()
        {
            world.SetMembership(alice, new TeamMemberTraits(10), redTeam);
            world.SetMembership(bob, new TeamMemberTraits(20), redTeam);
            world.SetMembership(alice, new TeamMemberTraits(30), redTeam);
            var aliceMembership = world.GetMembership<TeamMemberTraits>(alice);
            var redMembers = world.GetMembers<TeamMemberTraits>(redTeam).Select(x => x.Id);
            CollectionAssert.AreEqual(new[] { alice, bob }, redMembers);
            Assert.AreEqual(30, aliceMembership.Item1.Number);
        }
    }
}