using System;
using System.Linq;
using Essentions.Components;
using NUnit.Framework;

namespace Essentions.Tests.Components
{
    [TestFixture]
    public class RangeTests
    {
        [Test]
        public void When_Range_is_created_Then_min_and_max_are_valid_and_it_is_impossible_to_create_invalid_range()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.Max, Is.EqualTo(10));
            Assert.That(range.Min, Is.EqualTo(-10));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Range<int>(2, 1));
        }

        [Test]
        public void When_Range_is_compared_to_another_Range_Then_ranges_are_sorted_by_their_minimum()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.CompareTo(new Range<float>(1, 2)), Is.EqualTo(-1));
            Assert.That(range.CompareTo(range), Is.Zero);
            Assert.That(range.CompareTo(new Range<float>(-11, -10)), Is.EqualTo(1));
        }

        [Test]
        public void When_finding_complementing_Range_Then_resulting_range_is_valid()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.Complement(new Range<float>(-15, -5)), Is.EqualTo(new Range<float>(-5, 10)));
            Assert.That(range.Complement(new Range<float>(5, 20)), Is.EqualTo(new Range<float>(-10, 5)));
            Assert.That(range.Complement(new Range<float>(1, 2)), Is.Null);
            Assert.That(range.Complement(range), Is.Null);
        }

        [Test]
        public void When_Contains_is_called_for_ranges_and_numbers_Then_true_if_returned_in_case_of_containment()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.Contains(new Range<float>(-10, -9.999f)), Is.True);
            Assert.That(range.Contains(new Range<float>(4, 10)), Is.True);
            Assert.That(range.Contains(new Range<float>(0, 0)), Is.True);
            Assert.That(range.Contains(1), Is.True);
            Assert.That(range.Contains(new Range<float>(-11, -10.001f)), Is.False);
            Assert.That(range.Contains(new Range<float>(-11, 0)), Is.False);
            Assert.That(range.Contains(10.001f), Is.False);
            Assert.That(range.Contains(range), Is.True);
        }

        [Test]
        public void When_finding_intersecting_Range_Then_resulting_range_is_valid()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.Intersect(new Range<float>(1, 2)), Is.EqualTo(new Range<float>(1, 2)));
            Assert.That(range.Intersect(new Range<float>(5, 15)), Is.EqualTo(new Range<float>(5, 10)));
            Assert.That(range.Intersect(new Range<float>(-15, -5)), Is.EqualTo(new Range<float>(-10, -5)));
            Assert.That(range.Intersect(new Range<float>(-5, 5)), Is.EqualTo(new Range<float>(-5, 5)));
            Assert.That(range.Intersect(new Range<float>(10, 15)), Is.EqualTo(new Range<float>(10, 10)));
            Assert.That(range.Intersect(range), Is.EqualTo(range));
        }

        [Test]
        public void When_checking_for_containment_within_another_range_Then_result_is_true_when_other_range_is_larger()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.IsContainedBy(new Range<float>(-11, 11)), Is.True);
            Assert.That(range.IsContainedBy(new Range<float>(float.MinValue, float.MaxValue)), Is.True);
            Assert.That(range.IsContainedBy(new Range<float>(-15, -10)), Is.False);
            Assert.That(range.IsContainedBy(new Range<float>(-9.99f, 10.999f)), Is.False);
            Assert.That(range.IsContainedBy(range), Is.True);
        }

        [Test]
        public void When_checking_for_continuity_within_another_range_Then_result_is_true_when_ranges_are_contiguous()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.IsContiguousWith(new Range<float>(-11, 11)), Is.False);
            Assert.That(range.IsContiguousWith(new Range<float>(10, 100)), Is.True);
            Assert.That(range.IsContiguousWith(new Range<float>(-100, -10)), Is.True);
            Assert.That(range.IsContiguousWith(new Range<float>(-100, -10.001f)), Is.False);
            Assert.That(range.IsContiguousWith(new Range<float>(-5, 5)), Is.False);
            Assert.That(range.IsContiguousWith(new Range<float>(0, 0)), Is.False);
            Assert.That(range.IsContiguousWith(new Range<float>(10.001f, 100)), Is.False);
        }

        [Test]
        public void When_checking_for_overlap_with_another_range_Then_result_is_true_when_ranges_are_overlapping()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.Overlaps(new Range<float>(-11, 11)), Is.True);
            Assert.That(range.Overlaps(new Range<float>(10, 100)), Is.True);
            Assert.That(range.Overlaps(new Range<float>(-100, -10)), Is.True);
            Assert.That(range.Overlaps(new Range<float>(-100, -10.001f)), Is.False);
            Assert.That(range.Overlaps(new Range<float>(10.001f, 100)), Is.False);
        }

        [Test]
        public void When_making_union_another_range_Then_resulting_range_holds_both()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.Union(new Range<float>(-11, 11)), Is.EqualTo(new Range<float>(-11, 11)));
            Assert.That(range.Union(new Range<float>(-9, 9)), Is.EqualTo(range));
            Assert.That(range.Union(new Range<float>(100, 200)), Is.EqualTo(new Range<float>(-10, 200)));
            Assert.That(range.Union(new Range<float>(-200, -100)), Is.EqualTo(new Range<float>(-200, 10)));
        }

        [Test]
        public void When_range_is_split_Then_resulting_ranges_are_bordering_on_split()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.Split(4.5f), Is.EqualTo(new[] { new Range<float>(-10, 4.5f), new Range<float>(4.5f, 10f) }));
            Assert.That(range.Split(10), Is.EqualTo(new[] { range }));
            Assert.That(range.Split(-10), Is.EqualTo(new[] { range }));
            Assert.That(range.Split(10.001f), Is.EqualTo(Enumerable.Empty<Range<float>>()));
            Assert.That(range.Split(-10.001f), Is.EqualTo(Enumerable.Empty<Range<float>>()));
        }

        [Test]
        public void When_iterating_over_range_Then_iteration_is_complete()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.Iterate(val => val + 5).ToArray(), Is.EqualTo(new [] { -10f, -5f, 0f, 5f, 10f } ));
            Assert.That(range.Iterate(val => val + 100), Is.EqualTo(new[] { -10f }));
            Assert.That(new Range<float>(0, 0).Iterate(val => val + 100f), Is.EqualTo(new[] { 0f }));
        }

        [Test]
        public void When_reverse_iterating_over_range_Then_iteration_is_complete()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.ReverseIterate(val => val - 5).ToArray(), Is.EqualTo(new[] { 10f, 5f, 0f, -5f, -10f }));
            Assert.That(range.ReverseIterate(val => val - 100), Is.EqualTo(new[] { 10f }));
            Assert.That(new Range<float>(0, 0).ReverseIterate(val => val - 100), Is.EqualTo(new[] { 0f }));
        }

        [Test]
        public void When_creating_empty_ranges_Then_range_can_be_created_for_every_type_with_default_ctor()
        {
            Assert.That(Range<int>.Empty, Is.EqualTo(new Range<int>(0, 0)));
            Assert.That(Range<float>.Empty, Is.Not.EqualTo(new Range<int>(0, 0)));
            Assert.That(Range<float>.Empty, Is.Not.EqualTo(new Range<int>(0, 0)));
            Assert.That(Range<RangeItem>.Empty, Is.EqualTo(new Range<RangeItem>(new RangeItem(), new RangeItem())));
        }

        [Test]
        public void When_creating_ranges_with_null_values_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentNullException>(() => new Range<RangeItem>(null, null));
        }

        [Test]
        public void When_ranges_are_compared_Then_range_minimum_value_is_being_used()
        {
            Range<float> range = new Range<float>(-10, 10);
            Assert.That(range.CompareTo((object)new Range<float>(11, 100)), Is.EqualTo(-1));
            Assert.That(range.CompareTo((object)new Range<float>(9, 10)), Is.EqualTo(-1));
            Assert.That(range.CompareTo((object)new Range<float>(-9, -9)), Is.EqualTo(-1));
            Assert.That(range.CompareTo((object)new Range<float>(-10, 0)), Is.Zero);
            Assert.That(range.CompareTo((object)new Range<float>(-9, -9)), Is.EqualTo(-1));

            Assert.That(range.CompareTo((object)-10f), Is.Zero);
            Assert.That(range.CompareTo((object)1f), Is.EqualTo(-1));
            Assert.That(range.CompareTo((object)-100f), Is.EqualTo(1));

            Assert.That(range.CompareTo((object)null), Is.EqualTo(1));
            Assert.That(range.CompareTo((object)range), Is.Zero);
        }

        public class RangeItem : IComparable<RangeItem>, IEquatable<RangeItem>
        {
            public int Value;

            /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object. </summary>
            /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order. </returns>
            /// <param name="other">An object to compare with this instance. </param>
            public int CompareTo(RangeItem other)
            {
                if (ReferenceEquals(other, null)) {
                    return 1;
                }

                return Value.CompareTo(other.Value);
            }

            /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
            /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(RangeItem other)
            {
                if (ReferenceEquals(other, null)) {
                    return false;
                }

                return Value == other.Value;
            }
        }
    }
}