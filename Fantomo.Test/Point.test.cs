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

        [Test]
        public void PlusOperator([Random(2)]int x1, [Random(2)]int x2, [Random(2)]int y1, [Random(2)]int y2)
        {
            var a = new Point(x1, y1);
            var b = new Point(x2, y2);
            var calced = a + b;
            Assert.That(calced.X, Is.EqualTo(x1 + x2));
            Assert.That(calced.Y, Is.EqualTo(y1 + y2));
        }

        [Test]
        public void CreateGrid([Random(1, 10, 2)]int w, [Random(1, 10, 2)]int h)
        {
            var list = Point.CreateGrid(new Point(w, h)).GetEnumerator();
            for (var y = 0; y < h; y++)
                for (var x = 0; x < w; x++)
                {
                    list.MoveNext();
                    Assert.That(list.Current.X, Is.EqualTo(x));
                    Assert.That(list.Current.Y, Is.EqualTo(y));
                }
        }
    }
}
