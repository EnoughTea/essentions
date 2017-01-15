using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void TestCalculateHashCode()
        {
            List<int> testValueTypes = new List<int> { 1, 2, 3 };
            int firstHashCode = testValueTypes.CalculateHashCode();
            Assert.That(firstHashCode, Is.Not.EqualTo(testValueTypes.GetHashCode()));

            testValueTypes[0] = 4;
            int secondHashCode = testValueTypes.CalculateHashCode();
            Assert.That(secondHashCode, Is.Not.EqualTo(firstHashCode));

            List<object> testRefs = new List<object> { 1, 2, 3 };
            firstHashCode = testRefs.CalculateHashCode();
            Assert.That(firstHashCode, Is.Not.EqualTo(testRefs.GetHashCode()));

            testRefs[0] = 4;
            secondHashCode = testRefs.CalculateHashCode();
            Assert.That(secondHashCode, Is.Not.EqualTo(firstHashCode));
        }

        [Test]
        public void TestCartesianProduct()
        {
            var test = new List<int[]> { new[] { 0, 1, 2 }, new[] { 2, 3 } };
            var expected = new List<int[]> {
                new[] { 0, 2 }, new[] { 0, 3 },
                new[] { 1, 2 }, new[] { 1, 3 },
                new[] { 2, 2 }, new[] { 2, 3 }
            };
            Assert.That(test.CartesianProduct().ToList(), Is.EqualTo(expected));
            Assert.That(new List<int[]>().CartesianProduct(), Is.EqualTo(new List<int[]>()));
            Assert.That(new List<int[]> { new[] { 0 } }.CartesianProduct(),
                        Is.EqualTo(new List<int[]> { new[] { 0 } }));
            Assert.That(new List<int[]> { new[] { 0, 1 } }.CartesianProduct(),
                        Is.EqualTo(new List<int[]> { new[] { 0 }, new[] { 1 } }));
        }

        [DataContract(IsReference = true, Name = "a", Namespace = "")]
        private class A
        {
            public int Value;
        }

        private class B
        {
            public B(int value)
            {
                Value = value;
            }

            public int Value;

            public static explicit operator A(B instance)
            {
                return instance != null ? new A { Value = instance.Value } : null;
            }
        }

        [Test]
        public void TestCastWithOperators()
        {
            List<B> toCast = new List<B> { new B(1), new B(2), new B(3) };
            var casted = toCast.CastWithOperators<B, A>().ToArray();

            Assert.That(casted, Is.Not.Null);
            Assert.That(casted.Length, Is.EqualTo(3));
            Assert.That(casted[0].Value, Is.EqualTo(1));
            Assert.That(casted[1].Value, Is.EqualTo(2));
            Assert.That(casted[2].Value, Is.EqualTo(3));
        }

        [Test]
        public void TestCount()
        {
            Assert.That(new[] {1, 2, 3}.Count(), Is.EqualTo(3));
            Assert.That(new int[0].Count(), Is.Zero);

            Assert.That(new HashSet<int> { 1, 2, 3}.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestEquivalent()
        {
            Assert.That(new[] { 1, 2 }.Equivalent(new[] { 2, 1 }), Is.True);
            Assert.That(new[] { 1, 5, 5 }.Equivalent(new[] { 5, 1, 5 }), Is.True);
            Assert.That(new[] { 1, 5, 5 }.Equivalent(new[] { 1, 1, 5 }), Is.False);
            Assert.That(new[] { 1, 5, 5, 1 }.Equivalent(new[] { 5, 1 }), Is.False);
            Assert.That(new[] { 2, 5, 5, 1 }.Equivalent(new[] { 5, 5, 1 }), Is.False);

            Assert.That(new[] { 1, 2 }.Equivalent(new[] { 2, 1 }, true), Is.True);
            Assert.That(new[] { 1, 5, 5 }.Equivalent(new[] { 5, 1, 5 }, true), Is.True);
            Assert.That(new[] { 1, 5, 5 }.Equivalent(new[] { 1, 1, 5 }, true), Is.True);
            Assert.That(new[] { 1, 5, 5, 1 }.Equivalent(new[] { 5, 1 }, true), Is.True);
            Assert.That(new[] { 1, 5, 5, 2 }.Equivalent(new[] { 1, 5, 5 }, true), Is.False);

            Assert.That(new[] { 0 }.Equivalent(new[] { 0 }), Is.True);
            Assert.That(new[] { 0 }.Equivalent(new int[0]), Is.False);
            Assert.That(new int[] { }.Equivalent(new int[] { }), Is.True);
            int[] same = { 0, 1, 2 };
            Assert.That(same.Equivalent(same), Is.True);
            Assert.That(same.Equivalent(null), Is.False);
            Assert.That(((IEnumerable<int>)null).Equivalent(null), Is.True);
        }

        [Test]
        public void TestIndexOfFirst()
        {
            var alpha = new List<int> { 100, 150, 200, 250, 200, 150, 100 };
            Assert.AreEqual(2, alpha.IndexOfFirst(item => item == 200));
        }

        [Test]
        public void TestInterleave()
        {
            Assert.That(new int[0].Interleave(1), Is.EqualTo(Enumerable.Empty<int>()));
            Assert.That(new[] { 0, 1, 2 }.Interleave(99), Is.EqualTo(new[] { 0, 99, 1, 99, 2 }));
            Assert.That(new[] { 0 }.Interleave(99), Is.EqualTo(new[] { 0 }));
        }

        [Test]
        public void TestForEach()
        {
            int[] seq = { 0, 1, 2 };
            int[] created = new int[3];
            var commit = seq.ForEach(n => created[n] = n * 2).ToArray();
            Assert.That(created, Is.EqualTo(new[] { 0, 2, 4 }));
        }

        [Test]
        public void TestMarkedWith()
        {
            var a1 = new A();
            var a2 = new A();
            var test = new List<object> {a1, new B(1), new B(2), a2 };
            Assert.That(test.MarkedWith(typeof(DataContractAttribute)), Is.EqualTo(new object[] { a1, a2 }));

            var testTypes = new List<Type> { typeof(A), typeof(B), typeof(int) };
            Assert.That(testTypes.MarkedWith(typeof(DataContractAttribute)).ToArray(), Is.EqualTo(new[] { typeof(A) }));
        }

        [Test]
        public void TestMerge()
        {
            Assert.That(new[] { 0, 1, 2, 3 }.Merge(new[] { 4, 5 }, (n1, n2) => n1 + n2),
                        Is.EqualTo(new[] { 4, 6, 2, 3 }));
        }

        [Test]
        public void TestPrepend()
        {
            var alpha = new List<int> { 1, 2, 3, 4, 5, 6 };
            var expected = new List<int> { -2, -1, 0, 1, 2, 3, 4, 5, 6 };
            var result = alpha.Prepend(new[] { -2, -1, 0 }).ToList();

            CollectionAssert.AreEqual(expected, result);
            CollectionAssert.AreEqual(alpha, alpha.Prepend(new int[0]).ToArray());
            Assert.That(new int[] { }.Prepend(new int[] { }), Is.EqualTo(new int[0]));
            Assert.That(new[] { 0 }.Prepend(null), Is.EqualTo(new[] { 0 }));
        }

        [Test]
        public void TestSlice()
        {
            var alpha = new List<int> { 1, 2, 3, 4, 5, 6 };
            var result = alpha.Slice(2).ToArray();
            var expected = new List<int> { 1, 2 };

            // Test slicing without remainders.
            Assert.AreEqual(3, result.Length);
            CollectionAssert.AreEqual(expected, result[0] as ICollection);

            // Test that remainder will get into partial slice.
            var beta = alpha.Slice(100).ToArray();
            Assert.AreEqual(1, beta.Length);
            CollectionAssert.AreEqual(alpha, beta[0] as ICollection);
        }

        [Test]
        public void TestSliceWeighted()
        {
            var alpha = new List<int> { 1, 2, 3, 4, 5, 6 };
            var result = alpha.Slice(3, elem => elem, 6).ToArray();
            var expected = new[] { 1, 2, 3 };

            Assert.AreEqual(4, result.Length);
            CollectionAssert.AreEqual(expected, result[0]);
            CollectionAssert.AreEqual(new[] { 4 }, result[1]);
            CollectionAssert.AreEqual(new[] { 5 }, result[2]);
            CollectionAssert.AreEqual(new[] { 6 }, result[3]);
        }

        [Test]
        public void TestToSet()
        {
            Assert.That(new[] { 1, 1, 2, 3, 1 }.ToSet(), Is.EquivalentTo(new[] { 3, 1, 2 }));
            Assert.That(new[] { 1.5f, 2f, 3.5f }.ToSet(n => (int)n), Is.EquivalentTo(new[] { 1, 2, 3 }));

            var nulls = new int?[] { null, null }.ToSet();
            Assert.That(nulls, Is.EqualTo(new int?[] { null }));
        }

        [Test]
        public void TestToStrings()
        {
            List<int?> test = new List<int?> { 1, null, 3, 4 };
            Assert.That(test.ToStrings(), Is.EqualTo(new[] { "1", "3", "4" }));
            Assert.That(test.ToStrings(true), Is.EqualTo(new[] { "1", "null", "3", "4" }));
            Assert.That(test.ToStrings(true, "—"), Is.EqualTo(new[] { "1", "—", "3", "4" }));
        }
    }
}