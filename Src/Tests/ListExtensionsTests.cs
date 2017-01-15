using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class ListExtensionsTests
    {
        [Test]
        public void When_Swap_is_used_with_correct_indexes_Elements_are_swapped()
        {
            var list = new List<int>(Enumerable.Range(0, 10));
            list.Swap(1, 2);
            Assert.That(list[1], Is.EqualTo(2));
            Assert.That(list[2], Is.EqualTo(1));
        }
    }
}