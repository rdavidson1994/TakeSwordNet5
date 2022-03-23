using NUnit.Framework;
using TakeSword;
namespace TakeSwordTests
{
    [TestFixture]
    public class BreakCampActionTests
    {
        private IWorld world;
        private Entity actor;
        private Entity location;

        [SetUp]
        public void SetUp()
        {
            world = new World();
            world.RegisterComponent<Wilderness>();
            world.RegisterCollection<Location>();
            location = world.CreateEntity();
            actor = world.CreateEntity();
            actor.Enter<Location>(location);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CannotBreakCampWithoutWilderness(bool dryRun)
        {
            var action = new BreakCampAction(actor);
            Assert.AreEqual(ActionStatus.Failed, action.Execute(dryRun).Status);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CannotBreakCampWithoutCampsite(bool dryRun)
        {
            location.Set(new Wilderness(HasCampsite: false));
            var action = new BreakCampAction(actor);
            Assert.AreEqual(ActionStatus.Failed, action.Execute(dryRun).Status);
        }

        [Test]
        public void BreakingCampIsValidWhenCampPresent()
        {
            location.Set(new Wilderness(HasCampsite: true));
            var action = new BreakCampAction(actor);
            Assert.AreEqual(ActionStatus.InProgress, action.Execute(dryRun: true).Status);
        }

        [Test]
        public void BreakingCampRemovesCampWhenCampPresent()
        {
            location.Set(new Wilderness(HasCampsite: true));
            var action = new BreakCampAction(actor);
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(ActionOutcome.Progress(), action.Execute());
            }
            Assert.AreEqual(ActionOutcome.Success(), action.Execute());
            Assert.IsFalse(location.Get<Wilderness>().HasCampsite);
        }

    }
}