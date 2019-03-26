using System.Collections.Generic;
using Fantomo.Core;
using NUnit.Framework;

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
            var address = new Point(x, y);
            var room = new Room
            {
                Address = address
            };
            Assert.AreEqual(room.Address, address);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsTrap(bool trap)
        {
            var room = new Room
            {
                IsTrap = trap
            };
            Assert.AreEqual(room.IsTrap, trap);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void Neighbors(int times)
        {
            var dic = new Dictionary<Direction, Door>();
            var room = new Room
            {
                InnerNeighbors = dic
            };
            Assert.AreSame(room.InnerNeighbors, dic);
            Assert.AreSame(room.Neighbors, dic);
            Assert.AreSame(room.Neighbors, room.InnerNeighbors);
        }
    }
}
