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

        public void IsLocked([Values]bool locked)
        {
            var door = new Door
            {
                IsLocked = locked
            };
            Assert.That(door.IsLocked, Is.EqualTo(locked));
        }

        [Test]
        public void Toggle([Values]bool locked)
        {
            var door = new Door
            {
                IsLocked = locked
            };
            var toggled = door.Toggle();
            Assert.That(toggled, Is.Not.SameAs(door));
            Assert.That(toggled.IsLocked, Is.EqualTo(!locked));
        }
    }
}
