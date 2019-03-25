using NUnit.Framework;
using Fantomo.Core;

namespace Fantomo.Test
{
    [TestFixture]
    public sealed class RoomTest
    {
        [Test]
        public void Constructor()
        {
            var room = new Room();
            Assert.AreEqual(room.Address, new Point());
            Assert.IsFalse(room.IsTrap);
            Assert.IsNull(room.Neighbors);
            Assert.IsNull(room.InnerNeighbors);
        }

        [TestCase(0, 1)]
        [TestCase(-1, 0)]
        public void Address(int x, int y)
        {
            var room = new Room();
            var address = new Point(x, y);
            room.Address = address;
            Assert.AreEqual(room.Address, address);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsTrap(bool trap)
        {
            var room = new Room();
            room.IsTrap = trap;
            Assert.AreEqual(room.IsTrap, trap);
        }
    }
}

