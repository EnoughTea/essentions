using System;
using System.Linq;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class FunctionExtensionsTests
    {
        [Test]
        public void MemoizeUnaryTest()
        {
            int callCount = 0;
            Func<int, int> square = i => {
                callCount++;
                return i * i;
            };
            Func<int, int> memoizedSquare = square.Memoize();

            Enumerable.Range(0, 3).ForEach(i => memoizedSquare(i)).ToList();
            Assert.That(callCount, Is.EqualTo(3));
            memoizedSquare(1);
            Assert.That(callCount, Is.EqualTo(3));
            memoizedSquare(4);
            Assert.That(callCount, Is.EqualTo(4));
        }

        [Test]
        public void MemoizeBinaryTest()
        {
            int callCount = 0;
            Func<int, int, int> sum = (a, b) => {
                callCount++;
                return a + b;
            };
            Func<int, int, int> memoizedSum = sum.Memoize();

            Enumerable.Range(0, 3).ForEach(i => memoizedSum(i, i)).ToList();
            Assert.That(callCount, Is.EqualTo(3));
            memoizedSum(2, 2);
            Assert.That(callCount, Is.EqualTo(3));
            memoizedSum(1, 2);
            Assert.That(callCount, Is.EqualTo(4));
        }

        [Test]
        public void CurryTest()
        {
            Func<int, int, int> sum = (a, b) => a + b;
            Func<int, Func<int, int>> curriedSum = sum.Curry();

            Assert.That(curriedSum(2)(4), Is.EqualTo(6));
        }

        [Test]
        public void TestPartial()
        {
            Func<int, int, int> sum = (a, b) => a + b;
            Func<int, int> plusFive = sum.Partial(5);

            Assert.That(plusFive(5), Is.EqualTo(10));
        }

        [Test]
        public void TestTuplify()
        {
            Func<int, int, int> sum = (a, b) => a + b;
            Func<Tuple<int, int>, int> tuplifiedSum = sum.Tuplify();

            Func<int, int, int, int> ternarySum = (a, b, c) => a + b + c;
            Func<Tuple<int, int, int>, int> tuplifiedTernarySum = ternarySum.Tuplify();

            Assert.That(tuplifiedSum(Tuple.Create(3, 5)), Is.EqualTo(8));
            Assert.That(tuplifiedTernarySum(Tuple.Create(3, 5, 10)), Is.EqualTo(18));
        }

        [Test]
        public void TestUntuplify()
        {
            Func<Tuple<int, int>, int> sum = tuple => tuple.Item1 + tuple.Item2;
            Func<Tuple<int, int, int>, int> ternarySum = tuple => tuple.Item1 + tuple.Item2 + tuple.Item3;
            Func<int, int, int> untuplifiedSum = sum.Untuplify();
            Func<int, int, int, int> untuplifiedTernarySum = ternarySum.Untuplify();

            Assert.That(untuplifiedSum(3, 5), Is.EqualTo(8));
            Assert.That(untuplifiedTernarySum(3, 5, 10), Is.EqualTo(18));
        }
    }
}