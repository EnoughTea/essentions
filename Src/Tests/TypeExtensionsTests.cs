using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void TestGetDefaultValue()
        {
            Assert.That(typeof(int).GetDefaultValue<int>(), Is.EqualTo(default(int)));
            Assert.That(typeof(BigInteger).GetDefaultValue<BigInteger>(), Is.EqualTo(default(BigInteger)));
            var genericValueType = typeof(KeyValuePair<int, int>);
            var unconstructedGenericType = Type.GetType("System.Collections.Generic.KeyValuePair`2");
            Assert.That(genericValueType.GetDefaultValue<KeyValuePair<int, int>>(),
                        Is.EqualTo(default(KeyValuePair<int, int>)));

            Assert.That(typeof(int?).GetDefaultValue<int?>(), Is.EqualTo(default(int?)));

            Assert.That(typeof(PritaveClassTest).GetDefaultValue<PritaveClassTest>(), Is.Null);
            Assert.That(typeof(PritaveStructTest).GetDefaultValue<PritaveStructTest>(),
                        Is.EqualTo(default(PritaveStructTest)));

            Assert.That(typeof(List<>).GetDefaultValue<object>(), Is.Null);
            Assert.That(typeof(void).GetDefaultValue<object>(), Is.Null);

            Assert.Throws<ArgumentException>(() => unconstructedGenericType.GetDefaultValue<KeyValuePair<int, int>>());
        }

        [Test]
        public void TestHasInterface()
        {
            Assert.IsTrue(typeof(List<>).DerivedFrom(typeof(IEnumerable<>)),
                          "Failed DerivedFrom raw concrete generic type on raw generic interface.");
            Assert.IsTrue(typeof(List<int>).DerivedFrom(typeof(IEnumerable<>)),
                          "Failed DerivedFrom concrete generic type on raw generic interface.");
            Assert.IsTrue(typeof(List<int>).DerivedFrom(typeof(IEnumerable<int>)),
                          "Failed DerivedFrom concrete generic type against concrete interface.");

            Assert.IsTrue(typeof(IList<>).DerivedFrom(typeof(IEnumerable<>)),
                          "Failed DerivedFrom raw generic interface on raw generic interface.");
            Assert.IsTrue(typeof(IList<int>).DerivedFrom(typeof(IEnumerable<>)),
                          "Failed DerivedFrom generic interface on raw generic interface.");
            Assert.IsTrue(typeof(IList<int>).DerivedFrom(typeof(IEnumerable<int>)),
                          "Failed DerivedFrom generic interface against concrete interface.");

            Assert.IsFalse(typeof(List<int>).DerivedFrom(typeof(IEnumerable<float>)),
                           "Failed DerivedFrom concrete generic type against non-existing concrete interface.");
            Assert.IsFalse(typeof(List<>).DerivedFrom(typeof(IEnumerable<int>)),
                           "Failed DerivedFrom raw generic type against concrete interface.");

            Assert.IsFalse(typeof(IList<int>).DerivedFrom(typeof(IEnumerable<float>)),
                           "Failed DerivedFrom concrete generic interface against non-existing concrete interface.");
            Assert.IsFalse(typeof(IList<>).DerivedFrom(typeof(IEnumerable<int>)),
                           "Failed DerivedFrom raw generic interface against concrete interface.");

            Assert.IsTrue(typeof(TestList<>).DerivedFrom(typeof(List<>)),
                          "Failed DerivedFrom concrete generic interface on raw generic type.");
            Assert.IsFalse(typeof(TestIntList).DerivedFrom(typeof(TestList<>)),
                           "Failed DerivedFrom concrete type on raw generic type.");

            Assert.IsTrue(typeof(List<>).DerivedFrom(typeof(IEnumerable<>)),
                          "Failed DerivedFrom raw generic type on raw generic interface.");
            Assert.IsTrue(typeof(TestList<>).DerivedFrom(typeof(List<>)),
                          "Failed DerivedFrom raw generic type on raw generic type.");
            Assert.IsTrue(typeof(TestIntList).DerivedFrom(typeof(List<>)),
                          "Failed DerivedFrom concrete type on raw generic type.");

            Assert.IsTrue(typeof(IList<int>).DerivedFrom(typeof(IEnumerable<int>)),
                          "Failed DerivedFrom generic interface against concrete interface.");
            Assert.IsTrue(typeof(List<int>).DerivedFrom(typeof(IEnumerable<int>)),
                          "Failed DerivedFrom concrete generic type against concrete interface.");

            Assert.IsFalse(typeof(IList<>).DerivedFrom(typeof(TestList<>)),
                           "Failed DerivedFrom raw generic interface on raw generic type.");

            Assert.IsFalse(typeof(TestIntList).DerivedFrom(typeof(TestList<int>)),
                           "Sanity DerivedFrom for completely unrelated classes.");
            Assert.IsFalse(typeof(List<int>).DerivedFrom(typeof(TestIntList)),
                           "Sanity for hierarchy levels.");

            Assert.IsTrue(typeof(TestIntListDerived).DerivedFrom(typeof(TestIntList)),
                          "Nust work for non-generic types as well.");
        }

        [Test]
        public void TestInstantiate()
        {
            Assert.That(typeof(DateTime).Instantiate(), Is.EqualTo(default(DateTime)));
            Assert.That(typeof(TestIntList).Instantiate(), Is.EqualTo(new TestIntList()));
            Assert.That(typeof(PritaveClassTest).Instantiate(), Is.Not.Null);
            Assert.That(typeof(PritaveStructTest).Instantiate(), Is.EqualTo(default(PritaveStructTest)));
            Assert.That(typeof(int[]).Instantiate(), Is.EqualTo(new int[0]));
            Assert.That(typeof(List<int>[]).Instantiate(), Is.EqualTo(new List<int>()));

            Assert.That(typeof(void).Instantiate(), Is.Null);
            Assert.Throws<ArgumentException>(() => typeof(List<>).Instantiate());
            var unconstructedGenericValueType = Type.GetType("System.Collections.Generic.KeyValuePair`2");
            Assert.Throws<ArgumentException>(() => unconstructedGenericValueType.Instantiate());
        }

        [Test]
        public void TestIsIntegerType()
        {
            Assert.That(typeof(object).IsIntegerType(), Is.False);
            Assert.That(typeof(double).IsIntegerType(), Is.False);

            Assert.That(typeof(byte).IsIntegerType(), Is.True);
            Assert.That(typeof(sbyte).IsIntegerType(), Is.True);
            Assert.That(typeof(short).IsIntegerType(), Is.True);
            Assert.That(typeof(ushort).IsIntegerType(), Is.True);
            Assert.That(typeof(int).IsIntegerType(), Is.True);
            Assert.That(typeof(uint).IsIntegerType(), Is.True);
            Assert.That(typeof(long).IsIntegerType(), Is.True);
            Assert.That(typeof(ulong).IsIntegerType(), Is.True);
            Assert.That(typeof(BigInteger).IsIntegerType(), Is.True);

            Assert.That(typeof(byte?).IsIntegerType(), Is.True);
            Assert.That(typeof(sbyte?).IsIntegerType(), Is.True);
            Assert.That(typeof(short?).IsIntegerType(), Is.True);
            Assert.That(typeof(ushort?).IsIntegerType(), Is.True);
            Assert.That(typeof(int?).IsIntegerType(), Is.True);
            Assert.That(typeof(uint?).IsIntegerType(), Is.True);
            Assert.That(typeof(long?).IsIntegerType(), Is.True);
            Assert.That(typeof(ulong?).IsIntegerType(), Is.True);
            Assert.That(typeof(BigInteger?).IsIntegerType(), Is.True);
        }

        [Test]
        public void TestIsRealNumberType()
        {
            Assert.That(typeof(object).IsRealNumberType(), Is.False);
            Assert.That(typeof(int).IsRealNumberType(), Is.False);

            Assert.That(typeof(float).IsRealNumberType(), Is.True);
            Assert.That(typeof(double).IsRealNumberType(), Is.True);
            Assert.That(typeof(decimal).IsRealNumberType(), Is.True);
            Assert.That(typeof(Complex).IsRealNumberType(), Is.True);
        }

        private class PritaveClassTest
        {
        }

        private struct PritaveStructTest
        {
        }

        public class TestList<T> : List<T>
        {
        }

        [Serializable]
        public class TestIntList : List<int>
        {
        }

        [Serializable]
        public class TestIntListDerived : TestIntList
        {
        }
    }
}