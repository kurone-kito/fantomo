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
            Assert.AreEqual(point.X, 0);
            Assert.AreEqual(point.Y, 0);
        }
    }
}
