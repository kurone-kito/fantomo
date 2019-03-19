using NUnit.Framework;
using Fantomo.Core;

namespace Fantomo.Test
{
    [TestFixture]
    public class DoorTest
    {
        private Door door;

        [Test]
        public void EmptyConstructor()
        {
            var door = new Door();
            Assert.IsNull(door.Rooms);
            Assert.IsFalse(door.IsLocked);
        }
    }
}
