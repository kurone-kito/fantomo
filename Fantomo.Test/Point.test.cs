using NUnit.Framework;
using Fantomo.Core;

namespace Fantomo.Test
{
    [TestFixture]
    public sealed class PointTest
    {
        [Test]
        public void Constructor()
        {
            var point = new Point();
            Assert.That(point.X, Is.EqualTo(0));
            Assert.That(point.Y, Is.EqualTo(0));
        }

        [Test]
        public void Specific([Random(2)]int x, [Random(2)]int y)
        {
            var point = new Point(x, y);
            Assert.That(point.X, Is.EqualTo(x));
            Assert.That(point.Y, Is.EqualTo(y));
        }

        [Test]
        public void GetNeighbor([Random(2)]int x, [Random(2)]int y, [Values]Direction direction)
        {
            var point = new Point(x, y);
            var neighbor = new Point(x, y).GetNeighbor(direction);
            Assert.That(neighbor, Is.Not.EqualTo(point));
            Assert.That(neighbor, Is.EqualTo(point + direction.Offset()));
        }
    }
}
