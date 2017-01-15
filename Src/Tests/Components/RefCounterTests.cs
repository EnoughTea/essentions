using Essentions.Components;
using NUnit.Framework;

namespace Essentions.Tests.Components
{
    [TestFixture]
    public class RefCounterTests
    {
        [Test]
        public void When_RefCounter_is_created_Then_it_is_empty()
        {
            var counter = new RefCounter<Item>();
            Assert.That(counter.Count, Is.Zero);
        }

        [Test]
        public void When_item_counter_is_not_zero_Then_it_is_tracked()
        {
            var item = new Item();
            var counter = Create();

            counter.Retain(item);
            Assert.That(counter.Tracked(item), Is.True);

            counter.Release(item);
            Assert.That(counter.Tracked(item), Is.False);
        }

        [Test]
        public void When_a_new_item_is_retained_Then_it_is_counted_and_appropriate_events_are_called()
        {
            var item = new Item();
            var counter = Create();

            counter.Retain(item);

            Assert.That(counter.Count, Is.EqualTo(1));
            Assert.That(item.FirstTimeRetained, Is.EqualTo(1));
            Assert.That(item.Incremented, Is.EqualTo(1));
        }

        [Test]
        public void When_item_is_retained_again_Then_it_doesnt_increment_count_and_appropriate_events_are_called()
        {
            var item = new Item();
            var counter = Create();

            counter.Retain(item);
            counter.Retain(item);
            Assert.That(counter.Count, Is.EqualTo(1));
            Assert.That(item.FirstTimeRetained, Is.EqualTo(1));
            Assert.That(item.Incremented, Is.EqualTo(2));
        }

        [Test]
        public void When_item_is_retained_and_released_Then_it_decrements_count_and_appropriate_events_are_called()
        {
            var item = new Item();
            var counter = Create();

            counter.Retain(item);
            counter.Retain(item);
            counter.Release(item);
            counter.Release(item);
            Assert.That(counter.Count, Is.Zero);
            Assert.That(item.FirstTimeRetained, Is.EqualTo(1));
            Assert.That(item.Incremented, Is.EqualTo(2));
            Assert.That(item.Decremented, Is.EqualTo(2));
            Assert.That(item.Released, Is.EqualTo(1));
            counter.Release(item);
            Assert.That(item.Decremented, Is.EqualTo(2));
            Assert.That(item.Released, Is.EqualTo(1));
        }

        [Test]
        public void When_few_items_are_retained_Then_count_is_incremented_appropriately()
        {
            var counter = new RefCounter<Item>();
            counter.Retain(new Item());
            counter.Retain(new Item());
            counter.Retain(new Item());
            Assert.That(counter.Count, Is.EqualTo(3));
        }

        [Test]
        public void When_counter_is_cleared_Then_no_items_are_left_and_their_events_are_called()
        {
            Item i1 = new Item(), i2 = new Item();
            var counter = Create();
            counter.Retain(i1);
            counter.Retain(i2);
            counter.Retain(i2);
            counter.Release(i2);

            counter.Clear();

            Assert.That(counter.Count, Is.Zero);
            Assert.That(i1.Decremented, Is.EqualTo(1));
            Assert.That(i2.Decremented, Is.EqualTo(2));
            Assert.That(i1.Released, Is.EqualTo(1));
            Assert.That(i2.Released, Is.EqualTo(1));
        }


        [Test]
        public void When_ToString_is_called_Then_it_contains_a_number_of_counted_items()
        {
            var counter = new RefCounter<Item>();
            Assert.That(counter.ToString().Contains("0"), Is.True);
            counter.Retain(new Item());
            Assert.That(counter.ToString().Contains("1"), Is.True);
        }

        private static RefCounter<Item> Create()
        {
            var counter = new RefCounter<Item>();
            counter.FirstTimeRetained += i => {
                i.FirstTimeRetained++;
            };

            counter.Incremented += i => {
                i.Incremented++;
            };

            counter.Decremented += i => {
                i.Decremented++;
            };

            counter.Released += i => {
                i.Released++;
            };

            return counter;
        }

        private class Item
        {
            public int FirstTimeRetained;
            public int Incremented;
            public int Decremented;
            public int Released;

            public void ResetCounters()
            {
                Released = FirstTimeRetained = Decremented = Incremented = 0;
            }
        }
    }
}