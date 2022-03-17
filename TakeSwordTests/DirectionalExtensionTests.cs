using NUnit.Framework;
using TakeSword;

namespace TakeSwordTests
{
    class MockDirectional : IDirectional<string>
    {
        public string North => "north";
        public string South => "south";
        public string East => "east";
        public string West => "west";
        public string Up => "up";
        public string Down => "down";
    }
    [TestFixture]
    public class DirectionalExtensionTests
    {
        [TestCase(Direction.North, "north")]
        [TestCase(Direction.South, "south")]
        [TestCase(Direction.East, "east")]
        [TestCase(Direction.West, "west")]
        [TestCase(Direction.Up, "up")]
        [TestCase(Direction.Down, "down")]
        public void Get_ReturnsMatchingPropertyForEachDirection(Direction direction, string expectedPropertyValue)
        {
            IDirectional<string> directional = new MockDirectional();
            Assert.AreEqual(expectedPropertyValue, directional.GetFacing(direction));
        }
    }
}