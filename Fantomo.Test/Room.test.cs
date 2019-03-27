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
            Assert.That(room.Address, Is.EqualTo(new Point()));
            Assert.That(room.IsTrap, Is.False);
            Assert.That(room.Neighbors, Is.Null);
            Assert.That(room.InnerNeighbors, Is.Null);
        }

        [Test]
        public void Address([Random(2)]int x, [Random(2)]int y)
        {
            var address = new Point(x, y);
            var room = new Room
            {
                Address = address
            };
            Assert.That(room.Address, Is.EqualTo(address));
        }

        public void IsTrap([Values]bool trap)
        {
            var room = new Room
            {
                IsTrap = trap
            };
            Assert.That(room.IsTrap, Is.EqualTo(trap));
        }

        [Test]
        public void Neighbors()
        {
            var dic = new Dictionary<Direction, Door>();
            var room = new Room
            {
                InnerNeighbors = dic
            };
            Assert.That(room.Neighbors, Is.SameAs(dic));
            Assert.That(room.InnerNeighbors, Is.SameAs(dic));
        }
    }
}
