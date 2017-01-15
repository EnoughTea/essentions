using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class LongExtensionsTests
    {
        [Test]
        public void TestGetHighWord()
        {
            Assert.That(0x0FABBEEFL.GetHighInt(), Is.EqualTo(0));
            Assert.That((-0x0FABBEEFL).GetHighInt(), Is.EqualTo(-1));

            Assert.That(0x0FABBEEFFEEDDEAD.GetHighInt(), Is.EqualTo(0x0FABBEEF));
            Assert.That((-0x0FABBEEFFEEDDEAD).GetHighInt(), Is.EqualTo(-0xFABBEF0));

            LongUnion confirm = new LongUnion { Number = -0x0FABBEEFFEEDDEAD };
            Assert.That((-0x0FABBEEFFEEDDEAD).GetHighInt(), Is.EqualTo(confirm.High));
        }

        [Test]
        public void TestGetLowWord()
        {
            Assert.That(0x0FABBEEFL.GetLowInt(), Is.EqualTo(0x0FABBEEF));
            Assert.That((-0x0FABBEEFL).GetLowInt(), Is.EqualTo(-0x0FABBEEF));

            Assert.That(0x0FABBEEF0CAFDEAD.GetLowInt(), Is.EqualTo(0x0CAFDEAD));
            Assert.That((-0x0FABBEEF0CAFDEAD).GetLowInt(), Is.EqualTo(-0x0CAFDEAD));

            LongUnion confirm = new LongUnion { Number = -0x0FABBEEFFEEDDEAD };
            Assert.That((-0x0FABBEEFFEEDDEAD).GetLowInt(), Is.EqualTo(confirm.Low));
        }


        [Test]
        public void TestModuloZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 1d.Modulo(0d));
        }

        [Test, TestCaseSource(nameof(GetModuloClassicTestData))]
        public void TestModuloClassic(long a, long b, long expectedOutput)
        {
            Assert.That(a.Modulo(b), Is.EqualTo(expectedOutput));
        }


        [Test, TestCaseSource(nameof(GetModuloModernTestData))]
        public void TestModuloModern(long a, long b, long expectedOutput)
        {
            Assert.That(a.Modulo(b, false), Is.EqualTo(expectedOutput));
        }

        public static IEnumerable<long[]> GetModuloModernTestData {
            get {
                yield return new[] { 1L, 1, 0 };
                yield return new[] { 0L, 1, 0 };
                yield return new[] { 2L, 10, 2 };
                yield return new[] { 12L, 10, 2 };
                yield return new[] { 22L, 10, 2 };
                yield return new[] { -2L, 10, 8 };
                yield return new[] { -12L, 10, 8 };
                yield return new[] { -22L, 10, 8 };
                yield return new[] { 2L, -10, -8 };
                yield return new[] { 12L, -10, -8 };
                yield return new[] { 22L, -10, -8 };
                yield return new[] { -2L, -10, -2 };
                yield return new[] { -12L, -10, -2 };
                yield return new[] { -22L, -10, -2 };
            }
        }

        public static IEnumerable<long[]> GetModuloClassicTestData {
            get {
                yield return new[] { 1L, 1, 0 };
                yield return new[] { 0L, 1, 0 };
                yield return new[] { 2L, 10, 2 };
                yield return new[] { 12L, 10, 2 };
                yield return new[] { 22L, 10, 2 };
                yield return new[] { -2L, 10, 8 };
                yield return new[] { -12L, 10, 8 };
                yield return new[] { -22L, 10, 8 };
                yield return new[] { 2L, -10, 2 };
                yield return new[] { 12L, -10, 2 };
                yield return new[] { 22L, -10, 2 };
                yield return new[] { -2L, -10, 8 };
                yield return new[] { -12L, -10, 8 };
                yield return new[] { -22L, -10, 8 };
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct LongUnion
        {

            [FieldOffset(0)]
            public long Number;

            [FieldOffset(0)]
            public int Low;

            [FieldOffset(4)]
            public int High;
        }
    }
}