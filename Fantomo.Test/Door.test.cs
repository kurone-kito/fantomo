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
            Assert.That(new Door().IsLocked, Is.False);
        }

        [Test]
        public void Toggle()
        {
            var door = new Door();
            var toggled = door.Toggle();
            Assert.AreNotSame(door, toggled);
            Assert.IsFalse(door.IsLocked);
            Assert.IsTrue(toggled.IsLocked);
        }
    }
}
