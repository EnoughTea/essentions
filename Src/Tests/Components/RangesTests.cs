using System.Collections.Generic;
using System.Linq;
using Essentions.Components;
using NUnit.Framework;

namespace Essentions.Tests.Components
{
    [TestFixture]
    public class RangesTests
    {
        [Test]
        public void When_contiguous_ranges_are_coalesced_Then_resulting_range_is_encompassing()
        {
            var ranges = GetContiguousRanges();
            Assert.That(ranges.Coalesce().ToArray(), Is.EqualTo(new[] { new Range<float>(-10, 10) }));
        }

        [Test]
        public void When_non_contiguous_ranges_are_coalesced_Then_same_ranges_returned()
        {
            var ranges = GetNonContiguousRanges();
            Assert.That(ranges.Coalesce().ToArray(), Is.EqualTo(ranges));
        }

        [Test]
        public void When_both_contiguous_and_non_contiguous_ranges_are_coalesced_Then_consecutively_contiguous_are_coalesced_in_min_to_max_order()
        {
            var ranges = GetContiguousAndNonContiguousRanges();
            var d = ranges.Coalesce().ToArray();
            Assert.That(d, Is.EqualTo(
                            new[] {
                                new Range<float>(-20, 0), new Range<float>(-5, 5), new Range<float>(0, 10),
                                new Range<float>(5, 100)
                            }));
        }

        [Test]
        public void When_checking_if_ranges_contain_another_range_Then_correct_result_is_returned()
        {
            var ranges = GetContiguousAndNonContiguousRanges().ToArray();
            Assert.That(ranges.Contains(new Range<float>(0, 10)), Is.True);
            Assert.That(ranges.Contains(new Range<float>(-5, 5.0001f)), Is.False);
        }

        [Test]
        public void When_checking_if_ranges_contain_another_range2_Then_correct_result_is_returned()
        {
            var ranges = GetContiguousAndNonContiguousRanges().ToArray();
            Assert.That(ranges.Contains(0, 10), Is.True);
            Assert.That(ranges.Contains(-5, 5.0001f), Is.False);
        }

        [Test]
        public void When_checking_if_ranges_contain_a_value_Then_correct_result_is_returned()
        {
            var ranges = GetContiguousAndNonContiguousRanges().ToArray();
            Assert.That(ranges.Contains(0), Is.True);
            Assert.That(ranges.Contains(-21.0001f), Is.False);
        }

        [Test]
        public void When_getting_overlapped_ranges_Then_correct_result_is_returned()
        {
            var ranges = GetContiguousAndNonContiguousRanges().ToArray();
            Assert.That(ranges.Overlapped(new Range<float>(0, 10)), Is.EqualTo(
                            new[] {
                                new Range<float>(-10, 0),
                                new Range<float>(0, 10),
                                new Range<float>(-5, 5),
                                new Range<float>(5, 20)
                            }));
            Assert.That(ranges.Overlapped(new Range<float>(-5, 5.0001f)), Is.EqualTo(new[] {
                                new Range<float>(-10, 0),
                                new Range<float>(0, 10),
                                new Range<float>(-5, 5),
                                new Range<float>(5, 20)
                            }));
        }

        [Test]
        public void When_searching_with_where_Then_correct_ranges_are_returned()
        {
            var ranges = GetContiguousAndNonContiguousRanges().ToArray();
            Assert.That(ranges.Where(r => r.Max == 10), Is.EqualTo(new[] { new Range<float>(0, 10 )} ));
            Assert.That(ranges.Where(r => r.Max == 21), Is.EqualTo(new Range<float>[] {}));

        }

        private static IEnumerable<Range<float>> GetContiguousRanges()
        {
            yield return new Range<float>(-10, 0);
            yield return new Range<float>(0, 10);
        }

        private static IEnumerable<Range<float>> GetContiguousAndNonContiguousRanges()
        {
            yield return new Range<float>(20, 100);
            yield return new Range<float>(-10, 0);
            yield return new Range<float>(-20, -10);
            yield return new Range<float>(0, 10);
            yield return new Range<float>(-5, 5);
            yield return new Range<float>(5, 20);
        }

        private static IEnumerable<Range<float>> GetNonContiguousRanges()
        {
            yield return new Range<float>(-10, -5);
            yield return new Range<float>(5, 10);
        }
    }
}