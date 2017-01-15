using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        private const string Upper = "ABCD";
        private const string Lower = "abcd";
        private const string Mixed = Upper + Lower;
        private const string WeirdUpper = "ṌḊЁАႢႤЯ";
        private const string WeirdLower = "ổứღძя";
        private const string WeirdMixed = WeirdUpper + WeirdLower;

        private enum TestEnum
        {
            None,
            First,
            Second
        }

        [Test]
        public void TestBeforeFirst()
        {
            Assert.That("".BeforeFirst('c'), Is.EqualTo(""));
            Assert.That("c".BeforeFirst('c'), Is.EqualTo(""));
            Assert.That("abcdef".BeforeFirst('c'), Is.EqualTo("ab"));
            Assert.That("a〈〉bc₽def".BeforeFirst('₽'), Is.EqualTo("a〈〉bc"));
        }

        [Test]
        public void TestBeforeLast()
        {
            Assert.That("".BeforeLast('c'), Is.EqualTo(""));
            Assert.That("c".BeforeLast('c'), Is.EqualTo(""));
            Assert.That("ca ca".BeforeLast('c'), Is.EqualTo("ca "));
            Assert.That("₽a〈〉bc₽ def₽".BeforeLast('₽'), Is.EqualTo("₽a〈〉bc₽ def"));
        }

        [Test]
        public void TestConvert()
        {
            Assert.That("".Convert(TestEnum.None), Is.EqualTo(TestEnum.None));
            Assert.That("First".Convert(TestEnum.None), Is.EqualTo(TestEnum.First));
            Assert.That("100".Convert(0), Is.EqualTo(100));
            Assert.That("0,01".Convert(0f, CultureInfo.GetCultureInfo("RU-ru")), Is.EqualTo(0.01f));
            Assert.That("20".Convert<double?>(), Is.EqualTo(20));
            Assert.That("".Convert<double?>(), Is.EqualTo(null));
        }

        [Test]
        public void TestEndsWith()
        {
            Assert.That("".EndsWith(c => c == '\0'), Is.False);
            Assert.That("\0".EndsWith(c => c == '\0'), Is.True);

            Assert.That("ab ba".EndsWith(c => c == 'a'), Is.True);
            Assert.That("₽".EndsWith(c => c == '₽'), Is.True);
            Assert.That("₽a".EndsWith(c => c == '₽'), Is.False);
        }

        [Test]
        public void TestIsConvertableType()
        {
            // Primitive types and certain built-in types:
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(TestEnum)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(String)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(Char)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(Guid)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(IntPtr)));

            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(Boolean)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(Byte)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(Int16)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(Int32)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(Int64)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(Single)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(Double)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(Decimal)));

            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(sbyte)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(ushort)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(UInt32)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(UInt64)));

            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(DateTime)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(DateTimeOffset)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(TimeSpan)));

            // Custom value and any reference types:
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(KeyValuePair<int, int>)));
            Assert.IsTrue(StringExtensions.IsConvertableType(typeof(object)));

            // Nullables:
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(char?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(Guid?)));

            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(bool?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(byte?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(short?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(int?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(long?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(float?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(double?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(decimal?)));

            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(sbyte?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(ushort?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(uint?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(ulong?)));

            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(DateTime?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(DateTimeOffset?)));
            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(TimeSpan?)));

            Assert.IsFalse(StringExtensions.IsConvertableType(typeof(KeyValuePair<int, int>?)));
        }

        [Test]
        public void TestIsLower()
        {
            Assert.IsFalse(Upper.IsLower());
            Assert.IsTrue(Lower.IsLower());
            Assert.IsFalse(Mixed.IsLower());

            Assert.IsFalse(WeirdUpper.IsLower());
            Assert.IsTrue(WeirdLower.IsLower());
            Assert.IsFalse(WeirdMixed.IsLower());
        }

        [Test]
        public void TestIsNumeric()
        {
            Assert.That("+0".IsNumeric(), Is.True);
            Assert.That(" \t-0.0".IsNumeric(), Is.True);
            Assert.That("1,000,000.00".IsNumeric(), Is.True);
            Assert.That("0x10G".IsNumeric(), Is.False);
            Assert.That("  0xFEEDDEAD  ".IsNumeric(), Is.True);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            Assert.That("1 000 000,00".IsNumeric(), Is.True);
        }

        [Test]
        public void TestIsUpper()
        {
            Assert.IsTrue(Upper.IsUpper());
            Assert.IsFalse(Lower.IsUpper());
            Assert.IsFalse(Mixed.IsUpper());

            Assert.IsTrue(WeirdUpper.IsUpper());
            Assert.IsFalse(WeirdLower.IsUpper());
            Assert.IsFalse(WeirdMixed.IsUpper());
        }

        [Test]
        public void TestParseEnum()
        {
            TestEnum expected = TestEnum.First;
            Assert.AreEqual(expected, "First".ParseEnum<TestEnum>());
            Assert.AreNotEqual(expected, "None".ParseEnum<TestEnum>());
            Assert.Throws<ArgumentException>(() => "Some garbage".ParseEnum<TestEnum>());
        }

        [Test]
        public void TestRemove()
        {
            Assert.Throws<ArgumentException>(() => "".Remove(""));
            Assert.That("babbab".Remove("b"), Is.EqualTo("aa"));
            Assert.That("₽babbab₽".Remove("₽b"), Is.EqualTo("abbab₽"));
            Assert.That("₽babbab₽".Remove("aba"), Is.EqualTo("₽babbab₽"));
        }

        [Test]
        public void TestRemoveChars()
        {
            Assert.Throws<ArgumentException>(() => "".RemoveChars(""));
            Assert.That("babbab".RemoveChars("b"), Is.EqualTo("aa"));
            Assert.That("₽babbab₽".RemoveChars("₽b"), Is.EqualTo("aa"));
            Assert.That("₽babbab₽".RemoveChars("abc"), Is.EqualTo("₽₽"));
        }

        [Test]
        public void TestRepeat()
        {
            Assert.That("".Repeat(10), Is.EqualTo(""));
            Assert.That("any string".Repeat(0), Is.EqualTo(""));
            Assert.That("#".Repeat(5), Is.EqualTo("#####"));
            Assert.That("-₽-".Repeat(2), Is.EqualTo("-₽--₽-"));
        }

        [Test]
        public void TestStartsWith()
        {
            Assert.That("".StartsWith(c => c == '\0'), Is.False);
            Assert.That("\0".StartsWith(c => c == '\0'), Is.True);

            Assert.That("ab ba".StartsWith(c => c == 'a'), Is.True);
            Assert.That("₽".StartsWith(c => c == '₽'), Is.True);
            Assert.That("a₽".StartsWith(c => c == '₽'), Is.False);
        }

        [Test]
        public void TestSubstring()
        {
            string test = "This is test string, repeat, test string";
            Assert.Throws<ArgumentException>(() => "".Substring(""));
            Assert.That(test.Substring("T"), Is.EqualTo(""));
            Assert.That(test.Substring("₽"), Is.EqualTo(test));
            Assert.That(test.Substring("test"), Is.EqualTo("This is "));
            Assert.That(test.Substring(0, "test"), Is.EqualTo("This is "));
            Assert.That(test.Substring(8, "test"), Is.EqualTo(""));
            Assert.That(test.Substring(9, "test"), Is.EqualTo("est string, repeat, "));
        }

        [Test]
        public void TestToBase64()
        {
            string test = "This is test string ₽";
            Assert.That(test.ToBase64(), Is.EqualTo("VGhpcyBpcyB0ZXN0IHN0cmluZyDigr0="));
            Assert.That("".ToBase64(), Is.EqualTo(""));
        }

        [Test]
        public void TestToHexFromHex()
        {
            const string alpha = "Элемент игры Warcraft 3 присутствует.";
            const string alphaHex =
                "%42d%43b%435%43c%435%43d%442%20%438%433%440%44b%20%57%61%72%63%72%61%66%74%20%33%20%43f%440%438%441%443%442%441%442%432%443%435%442%2e";
            string hex = alpha.ToHex();
            Assert.AreEqual(alphaHex, hex);
            string result = hex.FromHex();
            Assert.AreEqual(alpha, result);
        }

        [Test]
        public void When_ToMemoryStream_is_called_with_empty_string_and_default_encoding_Then_empty_stream_is_returned()
        {
            var ms = string.Empty.ToMemoryStream();

            Assert.That(ms, Is.Not.Null);
            Assert.That(ms.Length, Is.Zero);
        }

        [Test]
        public void When_ToMemoryStream_is_called_with_non_empty_string_and_default_encoding_Then_stream_containg_utf8_string_is_returned()
        {
            string test = "Это ₽!";
            byte[] testBytes = Encoding.UTF8.GetBytes(test);
            var readBytes = new byte[testBytes.Length];
            var ms = test.ToMemoryStream();

            Assert.That(ms, Is.Not.Null);
            Assert.That(ms.Length, Is.EqualTo(testBytes.Length));
            ms.Read(readBytes, 0, testBytes.Length);

            Assert.That(readBytes, Is.EqualTo(testBytes));
        }

        [Test]
        public void TestToUri()
        {
            string webUrl = @"http:\\www.google.com";
            string webUrlEndsWithSlash = @"http:\\www.google.com\";
            Assert.That(webUrl.ToUri(), Is.EqualTo(new Uri(webUrl)));
            Assert.That(webUrlEndsWithSlash.ToUri(), Is.Null);
            Assert.That("".ToUri(), Is.Null);
        }

        [Test]
        public void TestTruncate()
        {
            Assert.That("".Truncate(0), Is.EqualTo(""));
            Assert.That("123456789".Truncate(5), Is.EqualTo("12345"));
            Assert.That("123456789".Truncate(9), Is.EqualTo("123456789"));
        }
    }
}