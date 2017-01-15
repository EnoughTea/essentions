using System;
using Essentions.Tests.Tools;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class ByteArrayExtensionsTests
    {
        [Test]
        public void When_IndexOf_is_called_on_null_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentNullException>(() => ((byte[])null).IndexOf(new byte[] { 4, 5 }));
        }

        [Test]
        public void When_IndexOf_is_called_on_empty_Then_minus_one_is_returned()
        {
            byte[] seq = new byte[0];

            Assert.That(seq.IndexOf(new byte[] { 4, 5 }), Is.EqualTo(-1));
        }

        [Test]
        public void When_IndexOf_is_called_with_contained_pattern_Then_index_where_pattern_starts_is_returned()
        {
            byte[] seq = {1, 2, 3, 4, 5, 6, 7, 8};

            Assert.That(seq.IndexOf(new byte[] { 4, 5 }), Is.EqualTo(3));
        }

        [Test]
        public void When_IndexOf_is_called_with_absent_pattern_Then_minus_one_is_returned()
        {
            byte[] seq = { 1, 2, 3, 4, 5, 6, 7, 8 };

            Assert.That(seq.IndexOf(new byte[] { 5, 4 }), Is.EqualTo(-1));
        }

        [Test]
        public void When_IndexOf_is_called_with_empty_pattern_Then_minus_one_is_returned()
        {
            byte[] seq = { 1, 2, 3, 4, 5, 6, 7, 8 };

            Assert.That(seq.IndexOf(new byte[0]), Is.EqualTo(-1));
        }

        [Test]
        public void When_ToMemoryStream_is_called_with_empty_bytes_Then_empty_stream_is_returned()
        {
            var ms = new byte[0].ToMemoryStream();
            Assert.That(ms, Is.Not.Null);
            Assert.That(ms.Length, Is.Zero);
        }

        [Test]
        public void When_ToMemoryStream_is_called_with_non_empty_bytes_Then_stream_containg_them_is_returned()
        {
            var bytes = new byte[] { 255, 255, 128, 64 };
            var readBytes = new byte[4];
            var ms = bytes.ToMemoryStream();
            Assert.That(ms, Is.Not.Null);
            Assert.That(ms.Length, Is.EqualTo(4));
            ms.Read(readBytes, 0, bytes.Length);
            Assert.That(readBytes, Is.EqualTo(bytes));
        }

        [Test]
        public void When_ToStruct_called_on_empty_array_Then_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(() => new byte[0].ToStruct<TestStaticBlittable>());
        }

        [Test]
        public void When_ToStruct_restores_static_blittable_struct_Then_conversion_should_succeed()
        {
            byte[] persistentStructBytes = {
                0, 1, 0, 0, 43, 135, 22, 217, 206, 247, 239, 63, 72, 34, 189, 32, 0, 0
            };

            TestStaticBlittable restoredPersistentStruct = persistentStructBytes.ToStruct<TestStaticBlittable>();
            Assert.That(restoredPersistentStruct.Value, Is.EqualTo(256));
            Assert.That(restoredPersistentStruct.Value2, Is.EqualTo(0.999));
            Assert.That(restoredPersistentStruct.Value3, Is.EqualTo("≈₽"));
        }
    }
}