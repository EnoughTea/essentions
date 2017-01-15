using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class IntExtensionsTests
    {
        [Test]
        public void TestClamp()
        {
            int pos = 10;
            int neg = -10;
            Assert.That(pos.Clamp(10, 9), Is.EqualTo(9));
            Assert.That(pos.Clamp(9, 10), Is.EqualTo(pos));
            Assert.That(neg.Clamp(-9, -10), Is.EqualTo(-9));
            Assert.That(neg.Clamp(-10, -9), Is.EqualTo(neg));
        }

        [Test]
        public void TestDigitAtPosition()
        {
            Assert.That(0.DigitAtPosition(2), Is.EqualTo(0));
            Assert.That(1234567890.DigitAtPosition(0), Is.EqualTo(1));
            Assert.That(1234567890.DigitAtPosition(1), Is.EqualTo(2));
            Assert.That(1234567890.DigitAtPosition(9), Is.EqualTo(0));

            Assert.That(1234567890.DigitAtPosition(0, false), Is.EqualTo(0));
            Assert.That(1234567890.DigitAtPosition(1, false), Is.EqualTo(9));
            Assert.That(1234567890.DigitAtPosition(9, false), Is.EqualTo(1));

            Assert.That((-1234567890).DigitAtPosition(0), Is.EqualTo(1));
            Assert.That((-1234567890).DigitAtPosition(1), Is.EqualTo(2));
            Assert.That((-1234567890).DigitAtPosition(9), Is.EqualTo(0));
            Assert.That((-1234567890).DigitAtPosition(2, true, false), Is.EqualTo(-3));

            Assert.That(1234567890.DigitAtPosition(-1), Is.EqualTo(0));
            Assert.That(1234567890.DigitAtPosition(100), Is.EqualTo(0));
        }

        [Test]
        public void TestDigitCount()
        {
            Assert.That(1234567890.DigitCount(), Is.EqualTo(10));
            Assert.That((-1234567890).DigitCount(), Is.EqualTo(10));
            Assert.That(0.DigitCount(), Is.EqualTo(1));
            Assert.That((-1).DigitCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestGetHighWord()
        {

            // 0x0FABBEEF is [0000 1111 1010 1011][1011 1110 1110 1111]
            // -0x0FABBEEF is [1111 0000 0101 0100][0100 0001 0001 0001]
            Assert.That(0x0FABBEEF.GetHighWord(), Is.EqualTo(0x0FAB));
            Assert.That((-0x0FABBEEF).GetHighWord(), Is.EqualTo(-0x0FAC));

            IntUnion confirm = new IntUnion { Number = -0x0FABBEEF };
            Assert.That((-0x0FABBEEF).GetHighWord(), Is.EqualTo(confirm.High));
        }

        [Test]
        public void TestGetLowWord()
        {
            // 0x0FABBEEF is [0000 1111 1010 1011][1011 1110 1110 1111]
            // -0x0FABBEEF is [1111 0000 0101 0100][0100 0001 0001 0001]
            Assert.That(0x0FABBEEF.GetLowWord(), Is.EqualTo(0xBEEF));
            Assert.That((-0x0FABBEEF).GetLowWord(), Is.EqualTo(0x4111));

            IntUnion confirm = new IntUnion { Number = -0x0FABBEEF };
            Assert.That((-0x0FABBEEF).GetLowWord(), Is.EqualTo(confirm.Low));
        }

        [Test]
        public void TestWrapClamp()
        {
            Assert.That(0.WrapClamp(1, 5), Is.EqualTo(4));
            Assert.That(3.WrapClamp(1, 5), Is.EqualTo(3));

            Assert.That(10.WrapClamp(1, 5), Is.EqualTo(2));
            Assert.That(20.WrapClamp(1, 5), Is.EqualTo(4));
            Assert.That(45.WrapClamp(1, 5), Is.EqualTo(1));

            Assert.That((-10).WrapClamp(-5, -1), Is.EqualTo(-2));
            Assert.That((-20).WrapClamp(-5, -1), Is.EqualTo(-4));
            Assert.That((-45).WrapClamp(-5, -1), Is.EqualTo(-5));
        }

        [Test]
        public void TestIsBitSet()
        {
            // 0x0FABBEEF is [0000 1111 1010 1011][1011 1110 1110 1111]
            // -0x0FABBEEF is [1111 0000 0101 0100][0100 0001 0001 0001]
            Assert.That(0x0FABBEEF.IsBitSet(0), Is.True);
            Assert.That(0x0FABBEEF.IsBitSet(1), Is.True);
            Assert.That(0x0FABBEEF.IsBitSet(29), Is.False);
            Assert.That((-0x0FABBEEF).IsBitSet(0), Is.True);
            Assert.That((-0x0FABBEEF).IsBitSet(1), Is.False);
            Assert.That((-0x0FABBEEF).IsBitSet(29), Is.True);
        }

        [Test]
        public void TestIsEven()
        {
            Assert.IsTrue(0.IsEven());
            Assert.IsTrue(2.IsEven());
            Assert.IsTrue(((int)Math.Pow(2, 11)).IsEven());
            Assert.IsTrue(((int)Math.Pow(2, 30)).IsEven());
            Assert.IsTrue(((int)Math.Pow(2, 31)).IsEven());

            Assert.IsFalse((-1).IsEven());
            Assert.IsFalse(3.IsEven());
            Assert.IsFalse(((int)Math.Pow(5, 11)).IsEven());
            Assert.IsFalse(((int)Math.Pow(3, 11)).IsEven());
        }

        [Test]
        public void TestIsInRange()
        {
            Assert.That(10.IsInRange(10, 10), Is.True);
            Assert.That(10.IsInRange(10, 10, 0, false), Is.False);
            Assert.That(10.IsInRange(10, 10, 1, false), Is.True);
            Assert.That(15.IsInRange(1, 10, 5), Is.True);

            Assert.That((-10).IsInRange(-10, -10), Is.True);
            Assert.That((-10).IsInRange(-10, -10, 0, false), Is.False);
            Assert.That((-10).IsInRange(-10, -10, 1, false), Is.True);
            Assert.That((-15).IsInRange(-1, -10, 5), Is.True);
        }

        [Test]
        public void TestIsPoT()
        {
            Assert.IsTrue(1.IsPoT());
            Assert.IsTrue(2.IsPoT());
            Assert.IsTrue(1073741824.IsPoT());

            Assert.IsFalse((-2).IsPoT());
            Assert.IsFalse((-1073741824).IsPoT());
            Assert.IsFalse(0.IsPoT());
            Assert.IsFalse(3.IsPoT());
            Assert.IsFalse(34.IsPoT());
        }

        [Test, TestCaseSource(nameof(GetModuloClassicTestData))]
        public void TestModuloClassic(int a, int b, int expectedOutput)
        {
            Assert.That(a.Modulo(b), Is.EqualTo(expectedOutput));
        }

        [Test, TestCaseSource(nameof(GetModuloModernTestData))]
        public void TestModuloModern(int a, int b, int expectedOutput)
        {
            Assert.That(a.Modulo(b, false), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void TestModuloZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 1.Modulo(0));
        }

        [Test]
        public void TestRemap()
        {
            Assert.AreEqual(5, 100.Remap(0, 200, 0, 10));

            Assert.AreEqual(0, 100.Remap(200, 1000, 0, 10));
        }

        [Test]
        public void TestRoundToMultiple()
        {
            Assert.AreEqual(32, 24.ToMultiple(16));
            Assert.AreEqual(16, 23.ToMultiple(16));
            Assert.AreEqual(100, 50.ToMultiple(100));
            Assert.AreEqual(0, 49.ToMultiple(100));

            Assert.AreEqual(20, 20.ToMultiple(20));
            Assert.AreEqual(0, 0.ToMultiple(100000));
        }

        [Test]
        public void TestScaleToPoT()
        {
            Assert.AreEqual(2, 2.ToNextPoT());
            Assert.AreEqual(32, 25.ToNextPoT());
            Assert.AreEqual(1024, 522.ToNextPoT());
            Assert.AreEqual(1, 0.ToNextPoT());
            Assert.AreEqual(1, (-10).ToNextPoT());
            int big = 1073741824;
            Assert.AreEqual(1, (big + 1).ToNextPoT());
        }

        [Test]
        public void TestSetBit()
        {
            Assert.That(0.SetBit(0, true), Is.EqualTo(1));
            Assert.That(24.SetBit(5, true), Is.EqualTo(56));
            Assert.That((-1).SetBit(0, false), Is.EqualTo(-2));
        }

        [Test]
        public void TestSetHighWord()
        {
            Assert.That(0x0000BEEF.SetHighWord(0x0FAB), Is.EqualTo(0x0FABBEEF));
            Assert.That((-0x0000BEEF).SetHighWord(-0x0FAC), Is.EqualTo(-0x0FABBEEF));
        }

        [Test]
        public void TestSetLowWord()
        {
            Assert.That(0x0FAB0000.SetLowWord(0x0000BEEF), Is.EqualTo(0x0FABBEEF));
            Assert.That((-0x0FAC0000).SetLowWord(0x4111), Is.EqualTo(-0x0FABBEEF));
        }

        [Test]
        public void TestToEven()
        {
            Assert.That(0.ToEven(), Is.EqualTo(0));

            Assert.That(1.ToEven(), Is.EqualTo(2));
            Assert.That(2.ToEven(), Is.EqualTo(2));
            Assert.That(15.ToEven(), Is.EqualTo(16));

            Assert.That((-1).ToEven(), Is.EqualTo(0));
            Assert.That((-2).ToEven(), Is.EqualTo(-2));
            Assert.That((-15).ToEven(), Is.EqualTo(-14));
        }

        [Test]
        public void TestToMultiple()
        {
            Assert.That(1.ToMultiple(2), Is.EqualTo(2));
            Assert.That(103.ToMultiple(64), Is.EqualTo(128));
            Assert.That((-1).ToMultiple(2), Is.EqualTo(-2));
            Assert.That((-103).ToMultiple(64), Is.EqualTo(-128));

            Assert.That(8.ToMultiple(8), Is.EqualTo(8));
            Assert.That(100.ToMultiple(10), Is.EqualTo(100));

            Assert.That(0.ToMultiple(10), Is.EqualTo(0));
            Assert.That(0.ToMultiple(0), Is.EqualTo(0));
        }

        [Test]
        public void TestToNextPoT()
        {
            Assert.That(0.ToNextPoT(), Is.EqualTo(1));
            Assert.That(1.ToNextPoT(), Is.EqualTo(1));
            Assert.That(2.ToNextPoT(), Is.EqualTo(2));
            Assert.That(128.ToNextPoT(), Is.EqualTo(128));

            Assert.That(3.ToNextPoT(), Is.EqualTo(4));
            Assert.That(321.ToNextPoT(), Is.EqualTo(512));

            Assert.That((-128).ToNextPoT(), Is.EqualTo(1));
            Assert.That((-1).ToNextPoT(), Is.EqualTo(1));
        }

        [Test]
        public void TestToSignString()
        {
            Assert.That(0.ToSignString(), Is.EqualTo("0"));
            Assert.That(0.ToSignString(true), Is.EqualTo("+0"));
            Assert.That(123456.ToSignString(), Is.EqualTo("+123456"));
            Assert.That((-123456).ToSignString(), Is.EqualTo("-123456"));
        }

        [Test]
        public void TestToInvString()
        {
            // Thousands are separated by ' ' in russian.
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            Assert.That(1000000.ToString("N0", CultureInfo.CurrentCulture), Is.EqualTo("1 000 000"));
            Assert.That(1000000.ToInvString("N0"), Is.EqualTo("1,000,000"));
        }


        public static IEnumerable<int[]> GetModuloModernTestData {
            get {
                yield return new[] { 1, 1, 0 };
                yield return new[] { 0, 1, 0 };
                yield return new[] { 2, 10, 2 };
                yield return new[] { 12, 10, 2 };
                yield return new[] { 22, 10, 2 };
                yield return new[] { -2, 10, 8 };
                yield return new[] { -12, 10, 8 };
                yield return new[] { -22, 10, 8 };
                yield return new[] { 2, -10, -8 };
                yield return new[] { 12, -10, -8 };
                yield return new[] { 22, -10, -8 };
                yield return new[] { -2, -10, -2 };
                yield return new[] { -12, -10, -2 };
                yield return new[] { -22, -10, -2 };
            }
        }

        public static IEnumerable<int[]> GetModuloClassicTestData {
            get {
                yield return new[] { 1, 1, 0 };
                yield return new[] { 0, 1, 0 };
                yield return new[] { 2, 10, 2 };
                yield return new[] { 12, 10, 2 };
                yield return new[] { 22, 10, 2 };
                yield return new[] { -2, 10, 8 };
                yield return new[] { -12, 10, 8 };
                yield return new[] { -22, 10, 8 };
                yield return new[] { 2, -10, 2 };
                yield return new[] { 12, -10, 2 };
                yield return new[] { 22, -10, 2 };
                yield return new[] { -2, -10, 8 };
                yield return new[] { -12, -10, 8 };
                yield return new[] { -22, -10, 8 };
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct IntUnion
        {

            [FieldOffset(0)]
            public int Number;

            [FieldOffset(0)]
            public short Low;

            [FieldOffset(2)]
            public short High;
        }
    }
}
