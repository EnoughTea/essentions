using System.Collections.Generic;
using NUnit.Framework;

namespace Essentions.Tests
{

    [TestFixture]
    public class DictionaryExtensionsTests
    {
        [Test]
        public void TestGetValue()
        {
            var alpha = new Dictionary<int, int> { {1, 1}, {2, 2}, { 3, 3} };
            var empty = new Dictionary<int, object>();

            Assert.AreEqual(1, alpha.GetValue(1));
            Assert.AreEqual(0, alpha.GetValue(int.MaxValue));
            Assert.AreEqual(-1, alpha.GetValue(int.MaxValue, -1));
            Assert.AreEqual(null, empty.GetValue(int.MaxValue));
            Assert.AreEqual(-1, empty.GetValue(int.MaxValue, -1));
        }

        [Test]
        public void TestListInnerAdd()
        {
            var test = new Dictionary<int, List<int>>();
            test.AddInner(0, 1);
            var createdList = test[0];
            Assert.That(createdList[0], Is.EqualTo(1));
            test.AddInner(0, 2);
            Assert.That(createdList[1], Is.EqualTo(2));
        }

        [Test]
        public void TestListInnerAddRange()
        {
            var test = new Dictionary<int, List<int>>();

            test.AddInnerRange(0, new[] { 1, 2 });
            var createdList = test[0];
            Assert.That(createdList, Is.EqualTo(new List<int> { 1, 2 }));

            test.AddInnerRange(0, new[] { 3, 4, 5 });
            Assert.That(createdList, Is.EqualTo(new List<int> { 1, 2, 3, 4, 5 }));

            test.AddInnerRange<int, List<int>, int>(0, null);
            Assert.That(createdList, Is.EqualTo(new List<int> { 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void TestListInnerUnique()
        {
            var test = new Dictionary<int, List<int>>();
            test.AddUniqueInner(0, 1);
            var createdList = test[0];
            Assert.That(createdList[0], Is.EqualTo(1));
            test.AddUniqueInner(0, 1);
            Assert.That(createdList, Is.EqualTo(new List<int> { 1 }));
            test.AddUniqueInner(0, 2);
            Assert.That(createdList, Is.EqualTo(new List<int> { 1, 2 }));
        }

        [Test]
        public void TestHashSetInnerAdd()
        {
            var test = new Dictionary<int, HashSet<int>>();
            test.AddInner(0, 1);
            var createdList = test[0];
            Assert.That(createdList, Is.EqualTo(new HashSet<int> { 1 }));
            test.AddInner(0, 2);
            Assert.That(createdList, Is.EqualTo(new HashSet<int> { 1, 2 }));
        }

        [Test]
        public void TestHashSetInnerAddRange()
        {
            var test = new Dictionary<int, HashSet<int>>();

            test.AddInnerRange(0, new[] { 1, 2 });
            var createdList = test[0];
            Assert.That(createdList, Is.EqualTo(new HashSet<int> { 1, 2 }));

            test.AddInnerRange(0, new[] { 3, 4, 5 });
            Assert.That(createdList, Is.EqualTo(new HashSet<int> { 1, 2, 3, 4, 5 }));

            test.AddInnerRange(0, null);
            Assert.That(createdList, Is.EqualTo(new HashSet<int> { 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void TestListInnerContains()
        {
            var test = new Dictionary<int, List<int>>();
            test[0] = new List<int> {1, 2, 3, 4, 5};
            Assert.That(test.ContainsInner(0, 3), Is.True);
            Assert.That(test.ContainsInner(0, 6), Is.False);
            Assert.That(test.ContainsInner(1, 0), Is.False);
        }

        [Test]
        public void TestHashSetInnerContains()
        {
            var test = new Dictionary<int, HashSet<int>>();
            test[0] = new HashSet<int> { 1, 2, 3, 4, 5 };
            Assert.That(test.ContainsInner(0, 3), Is.True);
            Assert.That(test.ContainsInner(0, 6), Is.False);
            Assert.That(test.ContainsInner(1, 0), Is.False);
        }

        [Test]
        public void TestListInnerRemove()
        {
            var test = new Dictionary<int, List<int>>();
            var list = test[0] = new List<int> { 1, 2, 3, 4, 5 };
            Assert.That(test.RemoveInner(0, 3), Is.True);
            Assert.That(list, Is.EqualTo(new List<int> { 1, 2, 4, 5 }));
            Assert.That(test.RemoveInner(0, 6), Is.False);
            Assert.That(list, Is.EqualTo(new List<int> { 1, 2, 4, 5 }));
        }

        [Test]
        public void TestHashSetInnerRemove()
        {
            var test = new Dictionary<int, HashSet<int>>();
            var list = test[0] = new HashSet<int> { 1, 2, 3, 4, 5 };
            Assert.That(test.RemoveInner(0, 3), Is.True);
            Assert.That(list, Is.EqualTo(new HashSet<int> { 1, 2, 4, 5 }));
            Assert.That(test.RemoveInner(0, 6), Is.False);
            Assert.That(list, Is.EqualTo(new HashSet<int> { 1, 2, 4, 5 }));
        }
    }
}
