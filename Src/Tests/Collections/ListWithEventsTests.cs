using System.Linq;
using Essentions.Collections;
using NUnit.Framework;

namespace Essentions.Tests.Collections
{
    [TestFixture]
    public class ListWithEventsTests
    {
        [Test]
        public void When_list_is_created_Then_it_is_empty()
        {
            var list = new ListWithEvents<int>();
            Assert.That(list.Count, Is.Zero);
        }

        [Test]
        public void When_item_is_added_Then_count_is_changed_and_appropriate_events_are_called()
        {
            var list = Create();
            var item = new Item();

            list.Add(item);

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(item.BeforeAdding, Is.EqualTo(1));
            Assert.That(item.Added, Is.EqualTo(1));
        }

        [Test]
        public void When_item_is_added_unsuccessfully_Then_count_is_not_changed_and_appropriate_events_are_not_called()
        {
            var list = Create(successfulAdds: false);
            var item = new Item();

            list.Add(item);

            Assert.That(list.Count, Is.Zero);
            Assert.That(item.BeforeAdding, Is.EqualTo(1));
            Assert.That(item.Added, Is.EqualTo(0));
        }

        [Test]
        public void When_item_range_is_added_Then_count_is_changed_and_appropriate_events_are_called()
        {
            var list = Create();
            var items = new[] { new Item(), new Item() };

            list.AddRange(items);

            Assert.That(list.Count, Is.EqualTo(2));
            foreach (var item in items) {
                Assert.That(item.BeforeAdding, Is.Zero);
                Assert.That(item.Added, Is.Zero);
                Assert.That(item.BeforeRangeAdding, Is.EqualTo(1));
                Assert.That(item.RangeAdded, Is.EqualTo(1));
            }
        }

        [Test]
        public void When_item_range_is_added_unsuccessfully_Then_count_is_changed_and_appropriate_events_are_called()
        {
            var list = Create(successfulRangeAdds: false);
            var items = new[] { new Item(), new Item() };

            list.AddRange(items);

            Assert.That(list.Count, Is.Zero);
            foreach (var item in items) {
                Assert.That(item.BeforeRangeAdding, Is.EqualTo(1));
                Assert.That(item.RangeAdded, Is.Zero);
            }
        }

        [Test]
        public void When_item_range_is_removed_Then_count_is_changed_and_appropriate_events_are_called()
        {
            var list = Create();
            var items = new[] { new Item(), new Item(), new Item(), new Item() };
            list.AddRange(items);

            list.RemoveRange(1, 2);

            Assert.That(list.Count, Is.EqualTo(2));
            for (int i = 1; i < 3; i++) {
                Assert.That(items[i].BeforeRemoval, Is.Zero);
                Assert.That(items[i].Removed, Is.Zero);
                Assert.That(items[i].BeforeRangeRemoval, Is.EqualTo(1));
                Assert.That(items[i].RangeRemoved, Is.EqualTo(1));
            }
        }

        [Test]
        public void When_item_range_is_removed_unsuccessfully_Then_count_is_changed_and_appropriate_events_are_called()
        {
            var list = Create(successfulRangeRemovals: false);
            var items = new[] { new Item(), new Item(), new Item(), new Item() };
            list.AddRange(items);

            list.RemoveRange(1, 2);

            Assert.That(list.Count, Is.EqualTo(4));
            for (int i = 1; i < 3; i++) {
                Assert.That(items[i].BeforeRangeRemoval, Is.EqualTo(1));
                Assert.That(items[i].RangeRemoved, Is.Zero);
            }
        }

        [Test]
        public void When_item_is_removed_Then_count_is_changed_and_removal_events_are_called()
        {
            var list = Create();
            var item = new Item();
            list.Add(item);

            bool success = list.Remove(item);

            Assert.That(success, Is.True);
            Assert.That(list.Count, Is.Zero);
            Assert.That(item.BeforeRemoval, Is.EqualTo(1));
            Assert.That(item.Removed, Is.EqualTo(1));
        }

        [Test]
        public void When_item_is_removed_unsuccessfully_Then_count_is_not_changed_and_appropriate_events_are_called()
        {
            var list = Create(successfulRemoves: false);
            var item = new Item();
            list.Add(item);
            list.Remove(item);

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(item.BeforeRemoval, Is.EqualTo(1));
            Assert.That(item.Removed, Is.Zero);
        }

        [Test]
        public void When_item_is_replaced_Then_count_is_unchanged_and_replacing_events_are_called()
        {
            var list = Create();
            var replaced = new Item();
            var replacee = new Item();
            list.Add(replaced);

            list[0] = replacee;

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(replacee));
            Assert.That(replaced.BeforeReplacing, Is.EqualTo(1));
            Assert.That(replaced.Replaced, Is.EqualTo(1));
            Assert.That(replacee.BeforeReplacing, Is.EqualTo(1));
            Assert.That(replacee.Replaced, Is.EqualTo(1));
        }

        [Test]
        public void When_item_is_replaced_unsuccessfully_Then_count_is_not_changed_and_appropriate_events_are_called
            ()
        {
            var list = Create(successfulReplaces: false);
            var replaced = new Item();
            var replacee = new Item();
            list.Add(replaced);

            list[0] = replacee;

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(replaced));
            Assert.That(replaced.BeforeReplacing, Is.EqualTo(1));
            Assert.That(replaced.Replaced, Is.Zero);
            Assert.That(replacee.BeforeReplacing, Is.EqualTo(1));
            Assert.That(replacee.Replaced, Is.Zero);
        }

        [Test]
        public void When_list_is_cleared_successfully_Then_it_becomes_empty_and_appropriate_events_are_called()
        {
            var list = Create();
            var item = new Item();
            var item2 = new Item();
            list.Add(item);
            list.Add(item2);

            list.Clear();

            Assert.That(list.Count, Is.Zero);
            Assert.That(item.BeforeClearing, Is.EqualTo(1));
            Assert.That(item2.BeforeClearing, Is.EqualTo(1));
        }

        [Test]
        public void When_list_is_cleared_unsuccessfully_Then_its_count_is_not_changed_and_appropriate_events_are_called()
        {
            var list = Create(successfulClears: false);
            var item = new Item();
            var item2 = new Item();
            list.Add(item);
            list.Add(item2);

            list.Clear();

            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(item.BeforeClearing, Is.EqualTo(1));
            Assert.That(item2.BeforeClearing, Is.EqualTo(1));
        }

        [Test]
        public void When_contained_item_is_checked_for_containment_Then_true_is_returned_when_item_is_in_the_list()
        {
            var list = new ListWithEvents<int>(new [] { 1, 2, 3, 4 });

            Assert.That(list.Contains(3), Is.True);
        }

        [Test]
        public void When_absent_item_is_checked_for_containment_Then_true_is_returned_when_item_is_in_the_list()
        {
            var list = new ListWithEvents<int>(new[] { 1, 2, 3, 4 });

            Assert.That(list.Contains(0), Is.False);
        }

        private static ListWithEvents<Item> Create(bool successfulAdds = true, bool successfulRemoves = true,
                                                   bool successfulClears = true, bool successfulReplaces = true,
                                                   bool successfulRangeAdds = true, bool successfulRangeRemovals = true)
        {
            var list = new ListWithEvents<Item>();
            list.Added += (items, i, item) => { item.Added++; };
            list.BeforeAdding += (items, i, item) => {
                item.BeforeAdding++;
                return successfulAdds;
            };

            list.BeforeRangeAddition += (items, i, range) => {
                foreach (var item in range) {
                    item.BeforeRangeAdding++;
                }

                return successfulRangeAdds;
            };

            list.BeforeRangeRemoval += (items, i, length) => {
                foreach (var item in items.Skip(i).Take(length)) {
                    item.BeforeRangeRemoval++;
                }

                return successfulRangeRemovals;
            };

            list.RangeAdded += (items, i, length) => {
                foreach (var item in items.Skip(i).Take(length)) {
                    item.RangeAdded++;
                }
            };

            list.RangeRemoved += (items, range) => {
                foreach (var item in range) {
                    item.RangeRemoved++;
                }
            };

            list.BeforeRemoval += (items, i, item) => {
                item.BeforeRemoval++;
                return successfulRemoves;
            };

            list.BeforeReplacing += (items, i, replaced, replacee) => {
                replaced.BeforeReplacing++;
                replacee.BeforeReplacing++;
                return successfulReplaces;
            };

            list.BeforeClearing += items => {
                foreach (var item in items) {
                    item.BeforeClearing++;
                }

                return successfulClears;
            };

            list.Removed += (items, i, item) => { item.Removed++; };

            list.Replaced += (items, i, replaced, replacee) => {
                replaced.Replaced++;
                replacee.Replaced++;
            };

            return list;
        }

        private class Item
        {
            public int Added { get; set; }

            public int BeforeAdding { get; set; }

            public int BeforeRangeAdding { get; set; }

            public int BeforeClearing { get; set; }

            public int BeforeRemoval { get; set; }

            public int BeforeRangeRemoval { get; set; }

            public int BeforeReplacing { get; set; }

            public int RangeAdded { get; set; }

            public int Removed { get; set; }

            public int RangeRemoved { get; set; }

            public int Replaced { get; set; }

            public void Reset()
            {
                Added = BeforeAdding = BeforeRangeAdding = BeforeClearing = BeforeRemoval = BeforeRangeRemoval =
                    BeforeReplacing = RangeAdded = Removed = RangeRemoved = Replaced = 0;
            }
        }
    }
}