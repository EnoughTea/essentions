using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class AssemblyExtensionsTests
    {
        [Test]
        public void TestFindDerivedInterface()
        {
            var currAssembly = Assembly.GetExecutingAssembly();
            var foundTypes = currAssembly.FindDerived(typeof(ITestBase), false).ToList();

            Assert.IsTrue(foundTypes.Contains(typeof(TestBaseClass)));
            Assert.IsTrue(foundTypes.Contains(typeof(TestDerivedClass)));
            Assert.IsTrue(foundTypes.Contains(typeof(TestBaseClassGeneric<>)));
            Assert.IsTrue(foundTypes.Contains(typeof(TestDerivedClassGeneric<>)));
            Assert.IsTrue(foundTypes.Contains(typeof(TestStruct)));

            foundTypes = currAssembly.FindDerived(typeof(ITestBase)).ToList();

            Assert.IsTrue(foundTypes.Contains(typeof(TestBaseClass)));
            Assert.IsTrue(foundTypes.Contains(typeof(TestDerivedClass)));
            Assert.IsTrue(foundTypes.Contains(typeof(TestBaseClassGeneric<>)));
            Assert.IsTrue(foundTypes.Contains(typeof(TestDerivedClassGeneric<>)));
            Assert.IsFalse(foundTypes.Contains(typeof(TestStruct)));
        }

        [Test]
        public void TestFindDerivedClass()
        {
            var currAssembly = Assembly.GetExecutingAssembly();
            var foundTypes = currAssembly.FindDerived(typeof(TestBaseClass)).ToList();

            Assert.IsTrue(foundTypes.Contains(typeof(TestDerivedClass)));
        }

        [Test]
        public void TestFindDerivedGenericClass()
        {
            var currAssembly = Assembly.GetExecutingAssembly();
            var foundTypes = currAssembly.FindDerived(typeof(TestBaseClassGeneric<>)).ToList();

            Assert.IsTrue(foundTypes.Contains(typeof(TestDerivedClassBasedOnGeneric)), "Failed to find non-generic based on generic.");
            Assert.IsTrue(foundTypes.Contains(typeof(TestDerivedClassGeneric<>)), "Failed to find raw generic.");

            Assert.IsFalse(foundTypes.Contains(typeof(ITestBase)));
            Assert.IsFalse(foundTypes.Contains(typeof(TestDerivedClass)));
        }

        [Test]
        public void TestFindDerivedStruct()
        {
            var currAssembly = Assembly.GetExecutingAssembly();
            var foundTypes = currAssembly.FindDerived(typeof(ITestStructBase), false).ToList();

            Assert.IsTrue(foundTypes.Contains(typeof(TestStruct)));
        }

        // Simple inheritance
        public class TestBaseClass : ITestBase { }
        public class TestDerivedClass : TestBaseClass { }

        // Generic test classes
        // ReSharper disable once UnusedTypeParameter
        public class TestBaseClassGeneric<T> : ITestBase { }
        public class TestDerivedClassGeneric<T> : TestBaseClassGeneric<T> { }
        public class TestDerivedClassBasedOnGeneric : TestBaseClassGeneric<TestBaseClass> { }

        // Struct tests
        public struct TestStruct : ITestBase, ITestStructBase { }

        public interface ITestBase { }
        public interface ITestStructBase { }
    }
}
