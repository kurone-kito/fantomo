using NUnit.Framework;
using Fantomo.Core;

namespace Fantomo.Test
{
    [TestFixture]
    public sealed class DoorTest
    {
        [Test]
        public void Constructor()
        {
            var door = new Door();
            Assert.IsFalse(door.IsLocked);
        }

        [Test]
        public void Toggle()
        {
            var door = new Door();
            var toggled = door.Toggle();
            Assert.AreNotEqual(door, toggled);
            Assert.IsFalse(door.IsLocked);
            Assert.IsTrue(toggled.IsLocked);
        }
    }
}

