using System;
using System.Collections;
using System.Numerics;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class CheckTests
    {
        [Test]
        public void When_checking_that_value_is_NotNull_Then_throws_ANE_in_case_of_null_value()
        {
            Assert.Throws<ArgumentNullException>(() => Check.NotNull(null));
            Assert.DoesNotThrow(() => Check.NotNull(1));
            Assert.DoesNotThrow(() => Check.NotNull(new object()));
        }

        [Test]
        public void When_checking_that_value_is_NotNullOrEmpty_Then_throws_ANE_in_case_of_null_value_and_AE_in_case_of_empty_value()
        {
            Assert.Throws<ArgumentNullException>(() => Check.NotNullOrEmpty(null));
            Assert.Throws<ArgumentException>(() => Check.NotNullOrEmpty(""));
            Assert.Throws<ArgumentException>(() => Check.NotNullOrEmpty(new int[0]));
            Assert.DoesNotThrow(() => Check.NotNullOrEmpty("!"));
            Assert.DoesNotThrow(() => Check.NotNullOrEmpty(new[] {1}));
        }

        [Test]
        public void When_checking_that_value_is_NotEmpty_Then_throws_ANE_in_case_of_null_value_and_AE_in_case_of_empty_value()
        {
            Assert.Throws<ArgumentNullException>(() => Check.NotNullOrWhiteSpace(null));
            Assert.Throws<ArgumentException>(() => Check.NotNullOrWhiteSpace(""));
            Assert.Throws<ArgumentException>(() => Check.NotNullOrWhiteSpace("\t\r\n "));
            Assert.DoesNotThrow(() => Check.NotNullOrWhiteSpace("!"));
        }


        [Test]
        public void When_checking_that_values_are_EqualCount_Then_throws_AE_in_case_of_collections_with_different_count()
        {
            Assert.Throws<ArgumentException>(() => Check.EqualCount(new[] {1}, new[] {1, 2}));
            Assert.Throws<ArgumentException>(() => Check.EqualCount((IEnumerable)new[] { 1 }, new[] { 1, 2 }));
            Assert.DoesNotThrow(() => Check.EqualCount(new int[0], new int[0]));
        }

        [Test]
        public void When_checking_that_values_are_NotEqualCount_Then_throws_AE_in_case_of_collections_with_equal_count()
        {
            Assert.Throws<ArgumentException>(() => Check.NotEqualCount(new[] { 1, 2 }, new[] { 1, 2 }));
            Assert.Throws<ArgumentException>(() => Check.NotEqualCount((IEnumerable)new[] { 1, 2 }, new[] { 1, 2 }));
            Assert.DoesNotThrow(() => Check.NotEqualCount(new[] {1}, new int[0]));
        }

        [Test]
        public void When_checking_that_values_are_Equal_Then_throws_AE_in_case_of_different_values()
        {
            Assert.Throws<ArgumentException>(() => Check.Equals(new[] { 1 }, new[] { 1 }));
            Assert.Throws<ArgumentException>(() => Check.Equals(1, 2));
            Assert.DoesNotThrow(() => Check.Equals(1, 1));
            object test = new object();
            Assert.DoesNotThrow(() => Check.Equals(test, test));
        }

        [Test]
        public void When_checking_that_values_are_NotEqual_Then_throws_AE_in_case_of_equal_values()
        {
            var testRef = new[] { 1 };
            Assert.Throws<ArgumentException>(() => Check.NotEqual(testRef, testRef));
            Assert.Throws<ArgumentException>(() => Check.NotEqual(1, 1));
            Assert.DoesNotThrow(() => Check.NotEqual(1, 2));
            Assert.DoesNotThrow(() => Check.NotEqual(new object(), new object()));
        }

        [Test]
        public void When_checking_that_value_is_False_Then_throws_AE_in_case_of_true()
        {
            Assert.Throws<ArgumentException>(() => Check.False(true));
            Assert.DoesNotThrow(() => Check.False(false));
        }

        [Test]
        public void When_checking_that_value_is_True_Then_throws_AE_in_case_of_false()
        {
            Assert.Throws<ArgumentException>(() => Check.True(false));
            Assert.DoesNotThrow(() => Check.True(true));
        }

        [Test]
        public void When_checking_that_value_is_InRange_Then_throws_AOORE_when_it_is_out_of_inclusive_range()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.InRange(11, 0, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.InRange(-11, -10, 0));
            Assert.DoesNotThrow(() => Check.InRange(10, 10, 10));
            Assert.DoesNotThrow(() => Check.InRange(true, true, true));
        }

        [Test]
        public void When_checking_that_value_is_InRangeExclusive_Then_throws_AOORE_when_it_is_out_of_exclusive_range()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.InRangeExclusive(11, 0, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.InRangeExclusive(-11, -10, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.InRangeExclusive(true, true, true));
            Assert.DoesNotThrow(() => Check.InRangeExclusive(10, 9, 11));
        }

        [Test]
        public void When_checking_that_value_is_Greater_Then_throws_AOORE_in_case_of_lesser_or_equal()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Greater(-3, -2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Greater(0, 0));
            Assert.DoesNotThrow(() => Check.Greater(11, 10));
        }

        [Test]
        public void When_checking_that_value_is_GreaterOrEqual_Then_throws_AOORE_in_case_of_lesser()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.GreaterOrEqual(-3, -2));
            Assert.DoesNotThrow(() => Check.GreaterOrEqual(10, 10));
            Assert.DoesNotThrow(() => Check.GreaterOrEqual(-1, -2));
        }

        [Test]
        public void When_checking_that_value_is_Less_Then_throws_AOORE_in_case_of_greater_or_equal()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Less(-2, -3));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Less(0, 0));
            Assert.DoesNotThrow(() => Check.Less(10, 11));
        }

        [Test]
        public void When_checking_that_value_is_LessOrEqual_Then_throws_AOORE_in_case_of_lesser()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.LessOrEqual(-2, -3));
            Assert.DoesNotThrow(() => Check.LessOrEqual(10, 10));
            Assert.DoesNotThrow(() => Check.LessOrEqual(-2, -1));
        }

        [Test]
        public void When_checking_that_value_is_Positive_Then_throws_AOORE_in_case_of_zero_or_negative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0d));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0L));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0UL));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0m));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(0u));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive((short)0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive((ushort)0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(new BigInteger(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Positive(-1));
            Assert.DoesNotThrow(() => Check.Positive(1));
        }

        [Test]
        public void When_checking_that_value_is_Negative_Then_throws_AOORE_in_case_of_zero_or_positive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Negative(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Negative(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Negative(1d));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Negative(1f));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Negative(1L));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Negative(1m));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Negative((short)1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Negative(new BigInteger(1)));
            Assert.DoesNotThrow(() => Check.Negative(-1));
        }

        [Test]
        public void When_checking_that_value_is_Zero_Then_throws_AOORE_in_case_of_non_zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero(1d));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero(1f));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero(1L));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero(1UL));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero(1u));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero((short)1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero((ushort)1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero(1m));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.Zero(new BigInteger(1)));
            Assert.DoesNotThrow(() => Check.Zero(0));
        }

        [Test]
        public void When_checking_that_value_is_NonZero_Then_throws_AOORE_in_case_of_zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero(0d));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero(0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero(0L));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero(0UL));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero(0U));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero((short)0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero((ushort)0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero(0m));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NonZero(new BigInteger(0)));
            Assert.DoesNotThrow(() => Check.NonZero(1));
            Assert.DoesNotThrow(() => Check.NonZero(-1));
        }

        [Test]
        public void When_checking_that_value_is_NotNan_Then_throws_AOORE_in_case_of_NaN()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotNaN(double.NaN));
            Assert.Throws<ArgumentOutOfRangeException>(() => Check.NotNaN(float.NaN));
            Assert.DoesNotThrow(() => Check.NotNaN(1f));
            Assert.DoesNotThrow(() => Check.NotNaN(-1d));
            Assert.DoesNotThrow(() => Check.NotNaN(null));
        }
    }
}