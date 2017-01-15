using System;
using System.Runtime.InteropServices;
using Essentions.Tools;
using NUnit.Framework;

namespace Essentions.Tests.Tools
{
    [TestFixture]
    public class ChangeTests
    {
        [Test]
        public void When_CastTo_called_with_mismatched_types_Exception_is_thrown()
        {
            Assert.Throws<InvalidCastException>(() => Change.CastTo(new Random(), new List()));
        }

        [Test]
        public void When_CastTo_called_with_null_as_example_Null_variable_type_is_used()
        {
            object obj = new Random();
            Random example = null;
            Assert.That(Change.CastTo(obj, example), Is.EqualTo(obj));
        }

        [Test]
        public void When_converting_non_blittable_struct_to_bytes_Exception_is_thrown()
        {
            TestStaticNonBlittable mutableStruct = new TestStaticNonBlittable {
                Value2 = new[] { "1", "2" },
                Value = 128
            };

            Assert.Throws<ArgumentException>(() => Change.StructToBytes(mutableStruct));
        }

        [Test]
        public void When_converting_blittable_struct_to_bytes_Valid_bytes_are_returned()
        {
            TestStaticBlittable persistentStruct = new TestStaticBlittable { Value = 256, Value2 = 0.999, Value3 = "≈₽" };
            byte[] expectedPersistentStructBytes = {
                0, 1, 0, 0, 43, 135, 22, 217, 206, 247, 239, 63, 72, 34, 189, 32, 0, 0
            };

            Assert.That(Change.StructToBytes(persistentStruct), Is.EqualTo(expectedPersistentStructBytes));
        }

        [Test]
        public void When_integers_are_sorted_Then_order_is_correct()
        {
            int max = -10;
            int min = 0;
            Change.Sort(min, max, out min, out max);
            Assert.That(max, Is.EqualTo(0));
            Assert.That(min, Is.EqualTo(-10));

            max = 10;
            min = -5;
            Change.Sort(min, max, out min, out max);
            Assert.That(max, Is.EqualTo(10));
            Assert.That(min, Is.EqualTo(-5));
        }

        [Test]
        public void When_floats_are_sorted_Then_order_is_correct()
        {
            float max = -1f;
            float min = -0.999f;
            Change.Sort(min, max, out min, out max);
            Assert.That(max, Is.EqualTo(-0.999f));
            Assert.That(min, Is.EqualTo(-1f));

            max = 10f;
            min = 9.999f;
            Change.Sort(min, max, out min, out max);
            Assert.That(max, Is.EqualTo(10f));
            Assert.That(min, Is.EqualTo(9.999f));
        }

        [Test]
        public void When_doubles_are_sorted_Then_order_is_correct()
        {
            double max = -1d;
            double min = -0.999d;
            Change.Sort(min, max, out min, out max);
            Assert.That(max, Is.EqualTo(-0.999d));
            Assert.That(min, Is.EqualTo(-1d));

            max = 10d;
            min = 9.999d;
            Change.Sort(min, max, out min, out max);
            Assert.That(max, Is.EqualTo(10d));
            Assert.That(min, Is.EqualTo(9.999d));
        }

        [Test]
        public void When_reftypes_are_swapped_in_array_Then_swap_is_performed()
        {
            var array = new[] { new TestClass(0), new TestClass(1), new TestClass(2) };

            Change.Swap(ref array[0], ref array[2]);
            Assert.That(array[0].Value, Is.EqualTo(2));
            Assert.That(array[2].Value, Is.EqualTo(0));
        }

        [Test]
        public void When_reftypes_are_swapped_in_instances_Then_swap_is_performed()
        {
            var first = new TestClass(0);
            first.Ref = first;
            var second = new TestClass(1);
            second.Ref = second;

            Change.Swap(ref first.Ref, ref second.Ref);
            Assert.That(first.Ref, Is.EqualTo(second));
            Assert.That(second.Ref, Is.EqualTo(first));
        }

        [Test]
        public void When_valuetypes_are_swapped_in_array_Then_swap_is_performed()
        {
            var array = new[] { 0, 1, 2 };

            Change.Swap(ref array[0], ref array[2]);
            Assert.That(array[0], Is.EqualTo(2));
            Assert.That(array[2], Is.EqualTo(0));
        }

        [Test]
        public void When_valuetypes_are_swapped_in_instances_Then_swap_is_performed()
        {
            var first = new TestClass(0);
            var second = new TestClass(1);

            Change.Swap(ref first.Value, ref second.Value);
            Assert.That(first.Value, Is.EqualTo(1));
            Assert.That(second.Value, Is.EqualTo(0));
        }

        public class TestClass
        {
            public TestClass(int value)
            {
                Value = value;
            }

            public TestClass Ref;

            public int Value;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct TestStaticBlittable
    {
        public int Value;
        public double Value2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
        public string Value3;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct TestStaticNonBlittable
    {
        public int Value;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.ByValTStr, SizeConst = 2)]
        public string[] Value2;
    }
}