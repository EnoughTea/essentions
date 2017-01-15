using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class RandomExtensionsTests
    {
        private static readonly Random _Rand = new Random();

        [Test]
        public void TestNextByte()
        {
            Assert.That(Average(() => _Rand.NextByte()), Is.InRange(125, 131));
        }

        [Test]
        public void TestNextDate()
        {
            DateTime min = new DateTime(1999, 2, 15, 8, 30, 0);
            DateTime max = new DateTime(2999, 8, 29, 16, 0, 0);
            for (int i = 0; i < 1000; i++) {
                Assert.That(_Rand.NextDate(min, max), Is.InRange(min, max));
            }
        }

        [Test]
        public void TestNextExponential()
        {
            Assert.That(Average(() => _Rand.NextExponential()), Is.InRange(0, 1.1));
        }

        [Test]
        public void TestNextNormal()
        {
            Assert.That(Average(() => _Rand.NextNormal(0, 1)), Is.InRange(-0.05d, 0.05d));
        }

        [Test]
        public void TestRoll()
        {
            Assert.That(Average(() => _Rand.Roll(2, 6)), Is.InRange(6.8f, 7.2f));
        }

        [Test]
        public void TestNextItemWithProbability()
        {
            Dictionary<int, double> itemsWithChances = new Dictionary<int, double> {
                { 1, 0.25f },
                { 2, 0.25f },
                { 3, 0.5f }
            };

            Assert.That(Average(() => _Rand.NextItem(itemsWithChances, e => e.Value).Key), Is.InRange(2.2d, 2.3d));
        }

        [Test]
        public void TestNextItemWithProbabilityAndAnotherScale()
        {
            Dictionary<int, double> itemsWithChances = new Dictionary<int, double> {
                { 1, 0.5f },
                { 2, 0.5f },
                { 3, 1 }
            };

            Assert.That(Average(() => _Rand.NextItem(itemsWithChances, e => e.Value).Key), Is.InRange(2.2d, 2.3d));
        }

        [Test]
        public void TestNextItemList()
        {
            var items = new List<int> {1, 2, 3, 3};

            Assert.That(Average(() => _Rand.NextItem(items)), Is.InRange(2.2d, 2.3d));
        }

        [Test]
        public void TestNextItemArray()
        {
            var items = new[] { 1, 2, 3, 3 };

            Assert.That(Average(() => _Rand.NextItem(items)), Is.InRange(2.2d, 2.3d));
        }

        [Test]
        public void TestNextItemReadOnlyCollection()
        {
            var items = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 3 });
            Assert.That(Average(() => _Rand.NextItem(items)), Is.InRange(2.2d, 2.3d));
        }

        [Test]
        public void TestNextShort()
        {
            Assert.That(Average(() => _Rand.NextShort()), Is.InRange(15383, 17383));
        }

        [Test]
        public void TestNextSingle()
        {
            Assert.That(Average(() => _Rand.NextSingle(1.0f)), Is.InRange(0.4f, 0.6f));
        }

        [Test]
        public void TestTaper()
        {
            Assert.That(Average(() => _Rand.Taper(0, 1, 9, 10)), Is.InRange(8.7f, 9.3f));
        }

        [Test]
        public void TestWalk()
        {
            // Averages are uselless here, we need to test frequencies.
            // Walk up
            TestTools.TestFrequencies(
                new [] { 1.0f / 2.0f, 1.0f / 4.0f, 1.0f / 8.0f, 1.0f / 16.0f },
                () => _Rand.Walk(0, 0, 2));

            // walk down
            TestTools.TestFrequencies(
                new [] { 3.0f / 64.0f, 3.0f / 16.0f, 3.0f / 4.0f },
                () => _Rand.Walk(2, 4, 0));

            // walk both
            TestTools.TestFrequencies(
                new [] { 1 / 32.0f, 1 / 16.0f, 1 / 8.0f, 1 / 2.0f, 1 / 8.0f, 1 / 16.0f, 1 / 32.0f },
                () => _Rand.Walk(3, 2, 2));
        }

        [Test]
        public void TestNextTriangularInt()
        {
            Assert.That(Average(() => _Rand.NextTriangularInt(5, 1)), Is.InRange(4.8f, 5.2f));
        }

        [Test]
        public void TestNextTriangular()
        {
            Assert.That(Average(() => _Rand.NextTriangular(0, 10, 5)), Is.InRange(4.8f, 5.2f));
        }

        private static double Average(Func<double> calc, int runs = 10000)
        {
            double avg = 0;
            for (int i = 0; i < runs; i++) {
                avg += calc();
            }

            return avg / runs;
        }
    }
}