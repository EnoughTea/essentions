using System;
using Essentions.Tools;
using NUnit.Framework;

namespace Essentions.Tests.Tools
{
    [TestFixture]
    public class EaseTests
    {
        [Test]
        public void When_unknown_interpolation_is_used_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(() => Ease.Do(0, 100, 0.5, (InterpolationKind)int.MaxValue));
        }


        [Test]
        public void When_TimePercent_is_calculated_on_zero_time_Then_one_should_be_returned()
        {
            Assert.That(Ease.TimePercent(0d, 0d), Is.EqualTo(1));
            Assert.That(Ease.TimePercent(0f, 0f), Is.EqualTo(1));

            Assert.That(Ease.TimePercent(10d, 0d), Is.EqualTo(1));
            Assert.That(Ease.TimePercent(10f, 0f), Is.EqualTo(1));

            Assert.That(Ease.TimePercent(-10d, 0d), Is.EqualTo(1));
            Assert.That(Ease.TimePercent(-10f, 0f), Is.EqualTo(1));
        }

        [Test]
        public void When_TimePercent_is_calculated_on_greater_than_one_time_Then_one_should_be_returned()
        {
            Assert.That(Ease.TimePercent(1.01d, 0d), Is.EqualTo(1));
            Assert.That(Ease.TimePercent(10d, 0d), Is.EqualTo(1));
            Assert.That(Ease.TimePercent(1.01f, 0f), Is.EqualTo(1));
            Assert.That(Ease.TimePercent(10f, 0f), Is.EqualTo(1));

            Assert.That(Ease.TimePercent(1.01d, 0.5d), Is.EqualTo(1));
            Assert.That(Ease.TimePercent(10d, 10d), Is.EqualTo(1));
            Assert.That(Ease.TimePercent(1.01f, 0.5f), Is.EqualTo(1));
            Assert.That(Ease.TimePercent(10f, 10f), Is.EqualTo(1));
        }

        [Test]
        public void When_linear_interpolation_is_done_Then_correct_value_is_returned()
        {
            Assert.That(Ease.Do(0f, 100f, 0f), Is.EqualTo(0f));
            Assert.That(Ease.Do(0f, 100f, 0.25f), Is.EqualTo(25f));
            Assert.That(Ease.Do(0f, 100f, 0.5f), Is.EqualTo(50f));
            Assert.That(Ease.Do(0f, 100f, 0.75f), Is.EqualTo(75f));
            Assert.That(Ease.Do(0f, 100f, 1f), Is.EqualTo(100f));

            Assert.That(Ease.Do(0, 100, 0), Is.EqualTo(0));
            Assert.That(Ease.Do(0, 100, 0.25d), Is.EqualTo(25));
            Assert.That(Ease.Do(0, 100, 0.5d), Is.EqualTo(50));
            Assert.That(Ease.Do(0, 100, 0.75d), Is.EqualTo(75));
            Assert.That(Ease.Do(0, 100, 1d), Is.EqualTo(100));
        }

        [Test]
        public void When_quadratic_interpolation_is_done_Then_correct_value_is_returned()
        {
            Assert.That(Ease.Do(0f, 100f, 0f, InterpolationKind.Quadratic), Is.EqualTo(0f));
            Assert.That(Ease.Do(0f, 100f, 0.25f, InterpolationKind.Quadratic), Is.EqualTo(6.25f));
            Assert.That(Ease.Do(0f, 100f, 0.5f, InterpolationKind.Quadratic), Is.EqualTo(25f));
            Assert.That(Ease.Do(0f, 100f, 0.75f, InterpolationKind.Quadratic), Is.EqualTo(56.25f));
            Assert.That(Ease.Do(0f, 100f, 1f, InterpolationKind.Quadratic), Is.EqualTo(100f));

            Assert.That(Ease.Do(0, 100, 0, InterpolationKind.Quadratic), Is.EqualTo(0));
            Assert.That(Ease.Do(0, 100, 0.25d, InterpolationKind.Quadratic), Is.EqualTo(6.25));
            Assert.That(Ease.Do(0, 100, 0.5d, InterpolationKind.Quadratic), Is.EqualTo(25));
            Assert.That(Ease.Do(0, 100, 0.75d, InterpolationKind.Quadratic), Is.EqualTo(56.25));
            Assert.That(Ease.Do(0, 100, 1d, InterpolationKind.Quadratic), Is.EqualTo(100));
        }

        [Test]
        public void When_cubic_interpolation_is_done_Then_correct_value_is_returned()
        {
            Assert.That(Ease.Do(0f, 100f, 0f, InterpolationKind.Cubic), Is.EqualTo(0f));
            Assert.That(Ease.Do(0f, 100f, 0.25f, InterpolationKind.Cubic), Is.EqualTo(1.5625f));
            Assert.That(Ease.Do(0f, 100f, 0.5f, InterpolationKind.Cubic), Is.EqualTo(12.5f));
            Assert.That(Ease.Do(0f, 100f, 0.75f, InterpolationKind.Cubic), Is.EqualTo(42.1875f));
            Assert.That(Ease.Do(0f, 100f, 1f, InterpolationKind.Cubic), Is.EqualTo(100f));

            Assert.That(Ease.Do(0, 100, 0, InterpolationKind.Cubic), Is.EqualTo(0));
            Assert.That(Ease.Do(0, 100, 0.25d, InterpolationKind.Cubic), Is.EqualTo(1.5625));
            Assert.That(Ease.Do(0, 100, 0.5d, InterpolationKind.Cubic), Is.EqualTo(12.5));
            Assert.That(Ease.Do(0, 100, 0.75d, InterpolationKind.Cubic), Is.EqualTo(42.1875));
            Assert.That(Ease.Do(0, 100, 1d, InterpolationKind.Cubic), Is.EqualTo(100));
        }

        [Test]
        public void When_sin_interpolation_is_done_Then_correct_value_is_returned()
        {
            Assert.That(Ease.Do(0f, 100f, 0f, InterpolationKind.Sin), Is.EqualTo(0f));
            Assert.That(Ease.Do(0f, 100f, 0.25f, InterpolationKind.Sin), Is.InRange(38f, 39f));
            Assert.That(Ease.Do(0f, 100f, 0.5f, InterpolationKind.Sin), Is.InRange(70f, 71f));
            Assert.That(Ease.Do(0f, 100f, 0.75f, InterpolationKind.Sin), Is.InRange(92f, 93f));
            Assert.That(Ease.Do(0f, 100f, 1f, InterpolationKind.Sin), Is.EqualTo(100f));

            Assert.That(Ease.Do(0, 100, 0, InterpolationKind.Sin), Is.EqualTo(0));
            Assert.That(Ease.Do(0, 100, 0.25d, InterpolationKind.Sin), Is.InRange(38, 39));
            Assert.That(Ease.Do(0, 100, 0.5d, InterpolationKind.Sin), Is.InRange(70, 71));
            Assert.That(Ease.Do(0, 100, 0.75d, InterpolationKind.Sin), Is.InRange(92, 93));
            Assert.That(Ease.Do(0, 100, 1d, InterpolationKind.Sin), Is.InRange(99.9d, 100d));
        }

        [Test]
        public void When_cos_interpolation_is_done_Then_correct_value_is_returned()
        {
            Assert.That(Ease.Do(0f, 100f, 0f, InterpolationKind.Cos), Is.EqualTo(0f));
            Assert.That(Ease.Do(0f, 100f, 0.25f, InterpolationKind.Cos), Is.InRange(7f, 8f));
            Assert.That(Ease.Do(0f, 100f, 0.5f, InterpolationKind.Cos), Is.InRange(29f, 30f));
            Assert.That(Ease.Do(0f, 100f, 0.75f, InterpolationKind.Cos), Is.InRange(61f, 62f));
            Assert.That(Ease.Do(0f, 100f, 1f, InterpolationKind.Cos), Is.EqualTo(100f));

            Assert.That(Ease.Do(0, 100, 0, InterpolationKind.Cos), Is.EqualTo(0));
            Assert.That(Ease.Do(0, 100, 0.25d, InterpolationKind.Cos), Is.InRange(7, 8));
            Assert.That(Ease.Do(0, 100, 0.5d, InterpolationKind.Cos), Is.InRange(29, 30));
            Assert.That(Ease.Do(0, 100, 0.75d, InterpolationKind.Cos), Is.InRange(61, 62));
            Assert.That(Ease.Do(0, 100, 1d, InterpolationKind.Cos), Is.InRange(99.9d, 100d));
        }

        [Test]
        public void When_Smooth_interpolation_is_done_Then_correct_value_is_returned()
        {
            Assert.That(Ease.Do(0f, 100f, 0f, InterpolationKind.Smooth), Is.EqualTo(0f));
            Assert.That(Ease.Do(0f, 100f, 0.25f, InterpolationKind.Smooth), Is.InRange(15f, 16f));
            Assert.That(Ease.Do(0f, 100f, 0.5f, InterpolationKind.Smooth), Is.InRange(49.9f, 50.1f));
            Assert.That(Ease.Do(0f, 100f, 0.75f, InterpolationKind.Smooth), Is.InRange(84f, 85f));
            Assert.That(Ease.Do(0f, 100f, 1f, InterpolationKind.Smooth), Is.EqualTo(100f));

            Assert.That(Ease.Do(0, 100, 0, InterpolationKind.Smooth), Is.EqualTo(0));
            Assert.That(Ease.Do(0, 100, 0.25d, InterpolationKind.Smooth), Is.InRange(15, 16));
            Assert.That(Ease.Do(0, 100, 0.5d, InterpolationKind.Smooth), Is.InRange(49.9d, 50.1d));
            Assert.That(Ease.Do(0, 100, 0.75d, InterpolationKind.Smooth), Is.InRange(84, 85));
            Assert.That(Ease.Do(0, 100, 1d, InterpolationKind.Smooth), Is.InRange(99.9d, 100d));
        }


        [Test]
        public void When_Smoothest_interpolation_is_done_Then_correct_value_is_returned()
        {
            Assert.That(Ease.Do(0f, 100f, 0f, InterpolationKind.Smoothest), Is.EqualTo(0f));
            Assert.That(Ease.Do(0f, 100f, 0.25f, InterpolationKind.Smoothest), Is.InRange(10f, 11f));
            Assert.That(Ease.Do(0f, 100f, 0.5f, InterpolationKind.Smoothest), Is.InRange(49.9f, 50.1f));
            Assert.That(Ease.Do(0f, 100f, 0.75f, InterpolationKind.Smoothest), Is.InRange(89f, 90f));
            Assert.That(Ease.Do(0f, 100f, 1f, InterpolationKind.Smoothest), Is.EqualTo(100f));

            Assert.That(Ease.Do(0, 100, 0, InterpolationKind.Smoothest), Is.EqualTo(0));
            Assert.That(Ease.Do(0, 100, 0.25d, InterpolationKind.Smoothest), Is.InRange(10, 11));
            Assert.That(Ease.Do(0, 100, 0.5d, InterpolationKind.Smoothest), Is.InRange(49.9d, 50.1d));
            Assert.That(Ease.Do(0, 100, 0.75d, InterpolationKind.Smoothest), Is.InRange(89, 90));
            Assert.That(Ease.Do(0, 100, 1d, InterpolationKind.Smoothest), Is.InRange(99.9d, 100d));
        }
    }
}