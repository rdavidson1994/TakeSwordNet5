using NUnit.Framework;
using TakeSword;
namespace TakeSwordTests
{
    [TestFixture]
    public class CampActionTests
    {
        private IWorld world;
        private Entity actor;
        private Entity wilderness;
        private Entity building;

        [SetUp]
        public void SetUp()
        {
            world = new World();
            world.RegisterComponent<Wilderness>();
            world.RegisterCollection<Location>();
            wilderness = world.CreateEntity(new Wilderness(HasCampsite: false));
            building = world.CreateEntity();
            actor = world.CreateEntity();
        }

        [Test]
        public void CanCampInWilderness()
        {
            actor.Enter<Location>(wilderness);
            var action = new CampAction(actor);
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(ActionOutcome.Progress(), action.Execute(dryRun: false));
            }
            Assert.IsFalse(wilderness.Get<Wilderness>().HasCampsite);
            Assert.AreEqual(ActionOutcome.Success(), action.Execute(dryRun: false));
            Assert.IsTrue(wilderness.Get<Wilderness>().HasCampsite);
        }

        [Test]
        public void CampActionValidInWilderness()
        {
            actor.Enter<Location>(wilderness);
            var action = new CampAction(actor);
            Assert.AreEqual(ActionOutcome.Progress(), action.Execute(dryRun: true));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CannotCampOutsideOfWilderness(bool dryRun)
        {
            actor.Enter<Location>(building);
            var action = new CampAction(actor);
            Assert.AreEqual(ActionStatus.Failed, action.Execute(dryRun).Status);
        }
    }
}