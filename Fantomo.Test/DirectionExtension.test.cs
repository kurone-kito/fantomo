using Fantomo.Core;
using NUnit.Framework;

namespace Fantomo.Test
{
    [TestFixture]
    public sealed class DirectionExtensionTest
    {
        [TestCase(Direction.Down, 0, 1)]
        [TestCase(Direction.Left, -1, 0)]
        [TestCase(Direction.Right, 1, 0)]
        [TestCase(Direction.Up, 0, -1)]
        [TestCase(Direction.Down | Direction.Left, -1, 1)]
        [TestCase(Direction.Down | Direction.Right, 1, 1)]
        [TestCase(Direction.Down | Direction.Up, 0, 0)]
        [TestCase(Direction.Left | Direction.Right, 0, 0)]
        [TestCase(Direction.Left | Direction.Up, -1, -1)]
        [TestCase(Direction.Right | Direction.Up, 1, -1)]
        public void Offset(Direction direction, int x, int y)
        {
            Assert.That(direction.Offset(), Is.EqualTo(new Point(x, y)));
        }
    }
}
