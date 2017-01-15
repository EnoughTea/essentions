using System.Collections.Generic;
using Essentions.Collections;
using NUnit.Framework;

namespace Essentions.Tests.Collections
{
    [TestFixture]
    public class RequestReturnPoolTests
    {
        [Test]
        public void When_pool_is_created_Then_instances_up_to_capacity_should_be_created()
        {
            int capacity = 32;
            var pool = new RequestReturnPool<Item>(capacity);
            Assert.That(pool.UnusedCount, Is.EqualTo(capacity));
            Assert.That(pool.Count(), Is.EqualTo(capacity));
        }

        [Test]
        public void When_item_is_requested_Then_it_is_provided_and_pool_count_is_decreased()
        {
            int capacity = 32;
            var pool = new RequestReturnPool<Item>(capacity);
            var item = pool.Request();

            Assert.That(item, Is.Not.Null);
            Assert.That(pool.UnusedCount, Is.EqualTo(capacity - 1));
        }

        [Test]
        public void When_pool_capacity_is_exhausted_Then_new_items_are_allocated()
        {
            int capacity = 32;
            var pool = new RequestReturnPool<Item>(capacity);
            for (int i = 0; i < capacity; i++) {
                pool.Request();
            }

            Assert.That(pool.UnusedCount, Is.Zero);

            var item = pool.Request();
            Assert.That(item, Is.Not.Null);
            Assert.That(pool.UnusedCount, Is.Zero);
        }

        [Test]
        public void When_items_are_returned_to_pool_Then_they_are_available_for_request()
        {
            var requested = new List<Item>();
            int capacity = 32;
            var pool = new RequestReturnPool<Item>(capacity);
            for (int i = 0; i < capacity; i++) {
                requested.Add(pool.Request());
            }

            foreach (var requestedItem in requested) {
                pool.Return(requestedItem);
            }

            Assert.That(pool.UnusedCount, Is.EqualTo(capacity));

            int index = capacity - 1;
            foreach (var item in pool) {
                Assert.That(item, Is.EqualTo(requested[index--]));
            }
        }

        private class Item
        {
        }
    }
}