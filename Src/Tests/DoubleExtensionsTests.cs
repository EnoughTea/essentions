using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class DoubleExtensionsTests
    {
        [Test]
        public void TestClamp()
        {
            double pos = 10;
            double neg = -10;
            Assert.That(pos.Clamp(10, 9), Is.EqualTo(9));
            Assert.That(pos.Clamp(9.999, 10), Is.EqualTo(pos));
            Assert.That(neg.Clamp(-9, -10), Is.EqualTo(-9));
            Assert.That(neg.Clamp(-10, -9.999), Is.EqualTo(neg));
        }

        [Test]
        public void TestEqualsZero()
        {
            double zero = (10 * 0.1) - 1;
            double zero2 = (10 / 10) - 1;
            double zero3 = 1 / 5 + 1 / 5 - 1 / 10 - 1 / 10 - 1 / 10 - 1 / 10;
            Assert.IsTrue(0d.EqualsZero());
            Assert.IsTrue(zero.EqualsZero());
            Assert.IsTrue(zero2.EqualsZero());
            Assert.IsTrue(zero3.EqualsZero());
        }

        [Test]
        public void TestFrac()
        {
            Assert.AreEqual(0.999d, 123.999d.Frac(), 0.00001f);
            Assert.AreEqual(0.999d, (-12345.999d).Frac(), 0.0001f);
            Assert.AreEqual(0, (-1234567.0d).Frac());
            Assert.AreEqual(0, 1234567.0d.Frac());
            Assert.AreEqual(0, 0d.Frac());
        }

        [Test]
        public void TestIsBetween()
        {
            double value = 1;
            Assert.IsTrue(value.IsInRange(0, 10));
            Assert.IsTrue(value.IsInRange(0.9, 1.1));

            Assert.IsFalse(value.IsInRange(1, 1.1, 0, false));
            Assert.IsFalse(value.IsInRange(2, 10));
            Assert.IsFalse(value.IsInRange(-0.9, 0.89, 0.1));
        }

        [Test, TestCaseSource(nameof(GetModuloClassicTestData))]
        public void TestModuloClassic(double a, double b, double expectedOutput)
        {
            Assert.That(a.Modulo(b), Is.EqualTo(expectedOutput));
        }

        [Test, TestCaseSource(nameof(GetModuloModernTestData))]
        public void TestModuloModern(double a, double b, double expectedOutput)
        {
            Assert.That(a.Modulo(b, false), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void TestModuloZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 1d.Modulo(0d));
        }

        [Test]
        public void TestNearlyEquals()
        {
            double small = 0.00001;
            double large = 12345;
            Assert.IsTrue(large.NearlyEquals(large + small, 0.0001));
            Assert.IsTrue(small.NearlyEquals(small * 2 - small));

            Assert.IsTrue(small.NearlyEquals(small - 0.000000000001));
            Assert.IsFalse(small.NearlyEquals(small - 0.00000001, 0.000000001));

            Assert.IsTrue(1d.NearlyEquals(1));
            Assert.IsTrue(1d.NearlyEquals(1 + double.Epsilon));
            Assert.IsTrue(1d.NearlyEquals(1 - double.Epsilon));
            Assert.IsTrue((-1d).NearlyEquals(-1 + double.Epsilon));
            Assert.IsTrue((-1d).NearlyEquals(-1 - double.Epsilon));
            Assert.IsTrue((-1d).NearlyEquals(-1));
            Assert.That((+0d).NearlyEquals(-0d), Is.True);
            Assert.That(double.MaxValue.NearlyEquals(double.MaxValue), Is.True);
            Assert.That(double.MinValue.NearlyEquals(double.MinValue), Is.True);
            Assert.That((-1d).NearlyEqualsUlps(1d), Is.False);
        }

        [Test]
        public void TestNearlyEqualsUlps()
        {
            double e = double.Epsilon;
            double bigValue = 1234567891234f;
            Assert.IsTrue(bigValue.NearlyEqualsUlps(bigValue + e * 2, 1));
            Assert.That((1d).NearlyEqualsUlps(1d), Is.True);
            Assert.That((+0d).NearlyEqualsUlps(-0d), Is.True);
            Assert.That(double.MaxValue.NearlyEqualsUlps(double.MaxValue), Is.True);
            Assert.That(double.MinValue.NearlyEqualsUlps(double.MinValue), Is.True);
        }

        [Test]
        public void TestNormalize()
        {
            Assert.AreEqual(1, 100d.Normalize(0, 100));
            Assert.AreEqual(0, 0d.Normalize(0, 100));
            Assert.AreEqual(0.5, 50d.Normalize(0, 100));

            Assert.AreEqual(1, 100000d.Normalize(0, 100));
            Assert.AreEqual(0, (-100000d).Normalize(0, 100));
        }

        [Test]
        public void TestProbabilisticRound()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++) {
                Assert.That(0d.ProbabilisticRound(rand), Is.Zero);
            }

            double half = 0.5;
            double average = 0;
            int limit = 10000;
            for (int i = 0; i < limit; i++) {
                double rounded = half.ProbabilisticRound(rand);
                average += rounded;
                if (rounded != 1d && rounded != 0d) {
                    throw new InvalidOperationException();
                }
            }

            average /= limit;
            double acceptableError = half / 30;
            Assert.That(average, Is.InRange(half - acceptableError, half + acceptableError));
        }

        [Test]
        public void TestRemap()
        {
            Assert.AreEqual(5, 100d.Remap(0, 200, 0, 10));

            Assert.AreEqual(0, 100d.Remap(200, 1000, 0, 10));
        }


        [Test]
        public void TestToDegrees()
        {
            Assert.That(Math.PI.ToDegrees(), Is.EqualTo(180));
            Assert.That((2 * Math.PI).ToDegrees(), Is.EqualTo(360));
            Assert.That((4 * Math.PI).ToDegrees(), Is.EqualTo(720));
            Assert.That((-Math.PI / 2).ToDegrees(), Is.EqualTo(-90));
            Assert.That(0d.ToDegrees(), Is.EqualTo(0));
            Assert.That(-0d.ToDegrees(), Is.EqualTo(0));
        }

        [Test]
        public void TestToRadians()
        {
            Assert.That(180d.ToRadians(), Is.EqualTo(Math.PI));
            Assert.That(360d.ToRadians(), Is.EqualTo(2 * Math.PI));
            Assert.That(720d.ToRadians(), Is.EqualTo(4 * Math.PI));
            Assert.That(0d.ToRadians(), Is.EqualTo(0));
            Assert.That(-0d.ToRadians(), Is.EqualTo(0));
        }

        [Test]
        public void TestToInvString()
        {
            // Decimals are separated by ',' in russian.
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            Assert.That(0.01d.ToString(CultureInfo.CurrentCulture), Is.EqualTo("0,01"));
            Assert.That(0.01d.ToInvString(), Is.EqualTo("0.01"));
        }

        [Test]

        public void TestWrapAngle()
        {
            Assert.That(180d.WrapAngle(true), Is.EqualTo(Math.PI));
            Assert.That(360d.WrapAngle(true), Is.Zero);
            Assert.That((-90d).WrapAngle(true), Is.EqualTo(-Math.PI / 2));
            Assert.That((-360d).WrapAngle(true), Is.Zero);

            Assert.AreEqual(Math.PI, 540d.WrapAngle(true), 0.00001d);
            Assert.AreEqual(-Math.PI / 2, 630d.WrapAngle(true), 0.00001d);

            Assert.That(0d.WrapAngle(true), Is.Zero);
            Assert.That((-0d).WrapAngle(true), Is.Zero);
            Assert.That(0d.WrapAngle(), Is.Zero);
            Assert.That((-0d).WrapAngle(), Is.Zero);

            Assert.That(Math.PI.WrapAngle(), Is.EqualTo(Math.PI));
            Assert.That((2 * Math.PI).WrapAngle(), Is.Zero);
            Assert.That((-Math.PI / 2).WrapAngle(), Is.EqualTo((-Math.PI / 2)));
            Assert.That((-Math.PI * 3 / 2).WrapAngle(), Is.EqualTo(Math.PI / 2));
            Assert.AreEqual(Math.PI, (3 * Math.PI).WrapAngle(), 0.00001d);
        }

        [Test]
        public void TestWrapClamp()
        {
            Assert.That(0d.WrapClamp(1, 5), Is.EqualTo(4));
            Assert.That(3d.WrapClamp(1, 5), Is.EqualTo(3));

            Assert.That(10d.WrapClamp(1, 5), Is.EqualTo(2));
            Assert.That(20d.WrapClamp(1, 5), Is.EqualTo(4));
            Assert.That(45d.WrapClamp(1, 5), Is.EqualTo(1));

            Assert.That((-10d).WrapClamp(-5, -1), Is.EqualTo(-2));
            Assert.That((-20d).WrapClamp(-5, -1), Is.EqualTo(-4));
            Assert.That((-45d).WrapClamp(-5, -1), Is.EqualTo(-5));
        }

        public static IEnumerable<double[]> GetModuloModernTestData {
            get {
                yield return new[] { 1d, 1, 0 };
                yield return new[] { 0d, 1, 0 };
                yield return new[] { 2d, 10, 2 };
                yield return new[] { 12d, 10, 2 };
                yield return new[] { 22d, 10, 2 };
                yield return new[] { -2d, 10, 8 };
                yield return new[] { -12d, 10, 8 };
                yield return new[] { -22d, 10, 8 };
                yield return new[] { 2d, -10, -8 };
                yield return new[] { 12d, -10, -8 };
                yield return new[] { 22d, -10, -8 };
                yield return new[] { -2d, -10, -2 };
                yield return new[] { -12d, -10, -2 };
                yield return new[] { -22d, -10, -2 };
            }
        }

        public static IEnumerable<double[]> GetModuloClassicTestData {
            get {
                yield return new[] { 1d, 1, 0 };
                yield return new[] { 0d, 1, 0 };
                yield return new[] { 2d, 10, 2 };
                yield return new[] { 12d, 10, 2 };
                yield return new[] { 22d, 10, 2 };
                yield return new[] { -2d, 10, 8 };
                yield return new[] { -12d, 10, 8 };
                yield return new[] { -22d, 10, 8 };
                yield return new[] { 2d, -10, 2 };
                yield return new[] { 12d, -10, 2 };
                yield return new[] { 22d, -10, 2 };
                yield return new[] { -2d, -10, 8 };
                yield return new[] { -12d, -10, 8 };
                yield return new[] { -22d, -10, 8 };
            }
        }
    }
}
