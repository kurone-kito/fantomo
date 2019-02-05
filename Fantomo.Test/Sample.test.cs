using System;
using NUnit.Framework;
using Fantomo.Core;

namespace Fantomo.Test
{
    [TestFixture]
    public class DoorTest
    {
        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("setupA");

        }
        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TeardownA");

        }

        [Test]
        public void ItA()
        {
            Console.WriteLine("itA");
            Assert.IsFalse(true, "1 should not be prime");
        }
    }

    [TestFixture]
    sealed class DoorTestSub : DoorTest
    {
        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("setupB");

        }
        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TeardownB");

        }

        [Test]
        public void ItA()
        {
            Console.WriteLine("itB");
            Assert.IsFalse(true, "1 should not be prime");
        }
    }
}
