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
            Assert.IsFalse(room.IsTrap);
            Assert.IsNull(room.Neighbors);
            Assert.IsNull(room.InnerNeighbors);
        }
    }
}

