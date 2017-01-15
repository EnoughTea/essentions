using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class CollectionExtensionsTests
    {
        [Test]
        public void When_AddFluent_is_used_with_element_Then_element_is_added_and_collection_is_returned()
        {
            int element = 10;
            var list = new List<int>();
            Assert.That(CollectionExtensions.Add(list, element), Is.EqualTo(list));
            Assert.That(list.FirstOrDefault(), Is.EqualTo(element));
        }

        [Test]
        public void TestAddUnique()
        {
            var alpha = new List<int> { 1, 2, 3 };

            alpha.AddUnique(3);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, alpha);

            alpha.AddUnique(4);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4 }, alpha);

            alpha = new List<int> { 1, 2, 3 };
            alpha.AddUnique(alpha);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, alpha);

            alpha.AddUnique(new []{ 3, 4, 5});
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 5}, alpha);
        }

        [Test]
        public void TestAddRange()
        {
            var alpha = new List<int> { 1, 2, 3 };
            var empty = new List<int>();

            alpha.AddRange(new List<int> { 1, 2, 3 }, true);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, alpha);

            empty.AddRange(alpha);
            CollectionAssert.AreEqual(alpha, empty);

            empty.Clear();
            var alphaClone = new List<int>(alpha);
            alpha.AddRange(empty);
            CollectionAssert.AreEqual(alphaClone, alpha);

            alpha.AddRange(new List<int> { 1, 2, 3 }, false);
            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 1, 2, 3}, alpha);

            Assert.Throws<ArgumentNullException>(() => alpha.AddRange(null));
        }

        [Test]
        public void TestRemoveRange()
        {
            var alpha = new List<int> { 1, 2, 3, 4 };
            var beta = new List<int> { 1, 2, 3, 4 };
            var omega = new List<int> { 1, 2, 3 };
            var empty = new List<int>();
            var remainder = new List<int> { 4 };

            alpha.RemoveRange(empty);
            CollectionAssert.AreEqual(beta, alpha);

            empty.RemoveRange(alpha);
            CollectionAssert.AreEqual(new List<int>(), empty);

            alpha.RemoveRange(omega);
            CollectionAssert.AreEqual(remainder, alpha);
        }
    }
}
