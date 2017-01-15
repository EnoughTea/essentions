using Essentions.Collections;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Essentions.Tests.Collections
{
    [TestFixture]
    public class RequestReclaimPoolTests
    {
        [Test]
        public void When_pool_is_created_Then_used_and_unused_counts_should_be_valid()
        {
            int capacity = 32;
            var pool = new RequestReclaimPool<Item>(i => true, capacity);
            Assert.That(pool.UsedCount, Is.Zero);
            Assert.That(pool.UnusedCount, Is.EqualTo(capacity));
            Assert.That(pool.Count(), Is.Zero);
        }

        [Test]
        public void When_item_is_requested_and_is_valid_Then_Reclaim_do_not_return_it_to_the_pool()
        {
            var pool = new RequestReclaimPool<Item>(i => i.Life > 0);

            var item = pool.Request();
            item.Life = 100;
            pool.Reclaim();

            Assert.That(pool.UsedCount, Is.EqualTo(1));
            Assert.That(pool.UnusedCount, Is.EqualTo(31));
            Assert.That(pool.Count(), Is.EqualTo(1));
        }

        [Test]
        public void When_item_is_requested_and_is_invalid_Then_Reclaim_returns_it_to_the_pool()
        {
            var pool = new RequestReclaimPool<Item>(i => i.Life > 0);

            var item = pool.Request();
            item.Life = 0;
            pool.Reclaim();

            Assert.That(pool.UsedCount, Is.EqualTo(0));
            Assert.That(pool.UnusedCount, Is.EqualTo(32));
            Assert.That(pool.Count(), Is.EqualTo(0));

            var item2 = pool.Request();
            Assert.That(item, Is.EqualTo(item2));
        }

        [Test]
        public void When_pool_capacity_is_exhausted_Then_it_is_resized()
        {
            int capacity = 8;
            var pool = new RequestReclaimPool<Item>(i => i.Life > 0, capacity);

            for (int i = 0; i < capacity; i++) {
                pool.Request();
            }

            pool.Request();

            Assert.That(pool.UsedCount, Is.EqualTo(capacity+1));
        }

        [Test]
        public void When_used_instances_also_reclaimed_Then_they_are_available_for_requesting()
        {
            var pool = new RequestReclaimPool<Item>(i => true);
            var item = pool.Request();
            var item2 =pool.Request();

            pool.Reclaim();

            Assert.That(pool.Request(), Is.Not.EqualTo(item));
            Assert.That(pool.Request(), Is.Not.EqualTo(item2));

            pool.Reclaim(true);

            Assert.That(pool.Request(), Is.EqualTo(item));
            Assert.That(pool.Request(), Is.EqualTo(item2));
        }

        [UsedImplicitly]
        private class Item
        {
            public int Life { get; set; }
        }
    }
}