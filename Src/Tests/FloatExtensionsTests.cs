using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class FloatExtensionsTests
    {
        [Test]
        public void TestClamp()
        {
            float pos = 10;
            float neg = -10;
            Assert.That(pos.Clamp(10, 9), Is.EqualTo(9));
            Assert.That(pos.Clamp(9.999f, 10f), Is.EqualTo(pos));
            Assert.That(neg.Clamp(-9, -10), Is.EqualTo(-9));
            Assert.That(neg.Clamp(-10f, -9.999f), Is.EqualTo(neg));
        }

        [Test]
        public void TestEqualsZero()
        {
            float zero = (10f * 0.1f) - 1f;
            float zero2 = (10f / 10f) - 1f;
            float zero3 = 1f / 5f + 1f / 5f - 1f / 10f - 1f / 10f - 1f / 10f - 1f / 10f;
            Assert.IsTrue(0f.EqualsZero());
            Assert.IsTrue(zero.EqualsZero());
            Assert.IsTrue(zero2.EqualsZero());
            Assert.IsTrue(zero3.EqualsZero());
        }

        [Test]
        public void TestFrac()
        {
            Assert.AreEqual(0.999f, 123.999f.Frac(), 0.00001f);
            Assert.AreEqual(0.999f, (-12345.999f).Frac(), 0.0001f);
            Assert.AreEqual(0f, (-1234567.0f).Frac());
            Assert.AreEqual(0f, 1234567.0f.Frac());
            Assert.AreEqual(0f, 0f.Frac());
        }

        [Test]
        public void TestIsBetween()
        {
            float value = 1f;
            Assert.IsTrue(value.IsInRange(0f, 10f));
            Assert.IsTrue(value.IsInRange(0.9f, 1.1f));

            Assert.IsFalse(value.IsInRange(1f, 1.1f, 0, false));
            Assert.IsFalse(value.IsInRange(2f, 10f));
            Assert.IsFalse(value.IsInRange(-0.9f, 0.89f, 0.1f));
        }

        [Test]
        public void TestModuloZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 1f.Modulo(0f));
        }

        [Test, TestCaseSource(nameof(GetModuloClassicTestData))]
        public void TestModuloClassic(float a, float b, float expectedOutput)
        {
            Assert.That(a.Modulo(b), Is.EqualTo(expectedOutput));
        }


        [Test, TestCaseSource(nameof(GetModuloModernTestData))]
        public void TestModuloModern(float a, float b, float expectedOutput)
        {
            Assert.That(a.Modulo(b, false), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void TestNearlyEquals()
        {
            float small = 0.00001f;
            float large = 12345f;
            Assert.IsTrue(large.NearlyEquals(large + small));
            Assert.IsTrue(small.NearlyEquals(small * 2f - small));
            Assert.IsFalse(small.NearlyEquals(small * 1.97f - small, 0.0000001f));

            Assert.IsTrue(small.NearlyEquals(small - 0.0000001f));
            Assert.IsFalse(small.NearlyEquals(small - 0.000001f, 0.0000001f));

            Assert.IsTrue(0f.NearlyEquals(0f));
            Assert.IsTrue(1f.NearlyEquals(1f));
            Assert.IsTrue(1f.NearlyEquals(1f + float.Epsilon));
            Assert.IsTrue(1f.NearlyEquals(1f - float.Epsilon));
            Assert.IsTrue((-1f).NearlyEquals(-1f + float.Epsilon));
            Assert.IsTrue((-1f).NearlyEquals(-1f - float.Epsilon));
            Assert.IsTrue((-1f).NearlyEquals(-1f));
            Assert.That((+0f).NearlyEquals(-0f), Is.True);
            Assert.That(float.MaxValue.NearlyEquals(float.MaxValue), Is.True);
            Assert.That(float.MinValue.NearlyEquals(float.MinValue), Is.True);
        }

        [Test]
        public void TestNearlyEqualsUlps()
        {
            float e = float.Epsilon;
            float bigValue = 12345f;
            Assert.IsTrue(bigValue.NearlyEqualsUlps(bigValue + e * 2, 1));
            Assert.That(1f.NearlyEqualsUlps(1f), Is.True);
            Assert.That((+0f).NearlyEqualsUlps(-0f), Is.True);
            Assert.That(float.MaxValue.NearlyEqualsUlps(float.MaxValue), Is.True);
            Assert.That(float.MinValue.NearlyEqualsUlps(float.MinValue), Is.True);
            Assert.That((-1f).NearlyEqualsUlps(1f), Is.False);
        }

        [Test]
        public void TestNormalize()
        {
            Assert.AreEqual(1, 100f.Normalize(0f, 100f));
            Assert.AreEqual(0, 0f.Normalize(0f, 100f));
            Assert.AreEqual(0.5f, 50f.Normalize(0f, 100f));

            Assert.AreEqual(1, 100000f.Normalize(0f, 100f));
            Assert.AreEqual(0, (-100000f).Normalize(0f, 100f));
        }

        [Test]
        public void TestProbabilisticRound()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++) {
                Assert.That(0d.ProbabilisticRound(rand), Is.Zero);
            }

            float half = 0.5f;
            float average = 0f;
            int limit = 10000;
            for (int i = 0; i < limit; i++) {
                float rounded = half.ProbabilisticRound(rand);
                average += rounded;
                if (rounded != 1f && rounded != 0f) {
                    throw new InvalidOperationException();
                }
            }

            average /= limit;
            float acceptableError = half / 30;
            Assert.That(average, Is.InRange(half - acceptableError, half + acceptableError));
        }

        [Test]
        public void TestRemap()
        {
            Assert.AreEqual(5f, 100f.Remap(0f, 200f, 0f, 10f));

            Assert.AreEqual(0f, 100f.Remap(200f, 1000f, 0f, 10f));
        }

        [Test]
        public void TestToDegrees()
        {
            Assert.That(((float)Math.PI).ToDegrees(), Is.EqualTo(180f));
            Assert.That(((float)(2 * Math.PI)).ToDegrees(), Is.EqualTo(360f));
            Assert.That(((float)(4 * Math.PI)).ToDegrees(), Is.EqualTo(720f));
            Assert.That(((float)(-Math.PI / 2)).ToDegrees(), Is.EqualTo(-90f));
            Assert.That(0f.ToDegrees(), Is.EqualTo(0f));
            Assert.That(-0f.ToDegrees(), Is.EqualTo(0f));
        }

        [Test]
        public void TestToInvString()
        {
            // Decimals are separated by ',' in russian.
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            Assert.That(0.01f.ToString(CultureInfo.CurrentCulture), Is.EqualTo("0,01"));
            Assert.That(0.01f.ToInvString(), Is.EqualTo("0.01"));
        }

        [Test]
        public void TestToRadians()
        {
            Assert.That(180f.ToRadians(), Is.EqualTo((float)Math.PI));
            Assert.That(360f.ToRadians(), Is.EqualTo((float)(2 * Math.PI)));
            Assert.That(720f.ToRadians(), Is.EqualTo((float)(4 * Math.PI)));
            Assert.That((-90f).ToRadians(), Is.EqualTo((float)(-Math.PI / 2)));
            Assert.That(0f.ToRadians(), Is.EqualTo(0f));
            Assert.That((-0f).ToRadians(), Is.EqualTo(0f));
        }

        [Test]
        public void TestWrapAngle()
        {
            Assert.That(180f.WrapAngle(true), Is.EqualTo((float)Math.PI));
            Assert.That(360f.WrapAngle(true), Is.Zero);
            Assert.That((-90f).WrapAngle(true), Is.EqualTo((float)(-Math.PI / 2f)));
            Assert.That((-360f).WrapAngle(true), Is.Zero);

            Assert.AreEqual((float)Math.PI, 540f.WrapAngle(true), 0.00001f);
            Assert.AreEqual(-(float)Math.PI / 2f, 630f.WrapAngle(true), 0.00001f);

            Assert.That(0f.WrapAngle(), Is.Zero);
            Assert.That((-0f).WrapAngle(), Is.Zero);
            Assert.That(0f.WrapAngle(true), Is.Zero);
            Assert.That((-0f).WrapAngle(true), Is.Zero);

            Assert.That(((float)Math.PI).WrapAngle(), Is.EqualTo((float)Math.PI));
            Assert.That(((float)(2 * Math.PI)).WrapAngle(), Is.Zero);
            Assert.That(((float)(-Math.PI / 2)).WrapAngle(), Is.EqualTo((float)(-Math.PI / 2f)));
            Assert.AreEqual((float)Math.PI / 2f, (-(float)Math.PI * 3f / 2f).WrapAngle(), 0.00001f);
            Assert.AreEqual((float)Math.PI, ((float)(3 * Math.PI)).WrapAngle(), 0.00001f);
        }

        [Test]
        public void TestWrapClamp()
        {
            Assert.That(0f.WrapClamp(1, 5), Is.EqualTo(4));
            Assert.That(3f.WrapClamp(1, 5), Is.EqualTo(3));

            Assert.That(10f.WrapClamp(1, 5), Is.EqualTo(2));
            Assert.That(20f.WrapClamp(1, 5), Is.EqualTo(4));
            Assert.That(45f.WrapClamp(1, 5), Is.EqualTo(1));

            Assert.That((-10f).WrapClamp(-5, -1), Is.EqualTo(-2));
            Assert.That((-20f).WrapClamp(-5, -1), Is.EqualTo(-4));
            Assert.That((-45f).WrapClamp(-5, -1), Is.EqualTo(-5));
        }

        public static IEnumerable<float[]> GetModuloModernTestData {
            get {
                yield return new[] { 1f, 1, 0 };
                yield return new[] { 0f, 1, 0 };
                yield return new[] { 2f, 10, 2 };
                yield return new[] { 12f, 10, 2 };
                yield return new[] { 22f, 10, 2 };
                yield return new[] { -2f, 10, 8 };
                yield return new[] { -12f, 10, 8 };
                yield return new[] { -22f, 10, 8 };
                yield return new[] { 2f, -10, -8 };
                yield return new[] { 12f, -10, -8 };
                yield return new[] { 22f, -10, -8 };
                yield return new[] { -2f, -10, -2 };
                yield return new[] { -12f, -10, -2 };
                yield return new[] { -22f, -10, -2 };
            }
        }

        public static IEnumerable<float[]> GetModuloClassicTestData {
            get {
                yield return new[] { 1f, 1, 0 };
                yield return new[] { 0f, 1, 0 };
                yield return new[] { 2f, 10, 2 };
                yield return new[] { 12f, 10, 2 };
                yield return new[] { 22f, 10, 2 };
                yield return new[] { -2f, 10, 8 };
                yield return new[] { -12f, 10, 8 };
                yield return new[] { -22f, 10, 8 };
                yield return new[] { 2f, -10, 2 };
                yield return new[] { 12f, -10, 2 };
                yield return new[] { 22f, -10, 2 };
                yield return new[] { -2f, -10, 8 };
                yield return new[] { -12f, -10, 8 };
                yield return new[] { -22f, -10, 8 };
            }
        }
    }
}