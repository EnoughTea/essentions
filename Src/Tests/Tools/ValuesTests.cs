using System;
using Essentions.Tools;
using NUnit.Framework;

namespace Essentions.Tests.Tools
{
    [TestFixture]
    public class ValuesTests
    {
        [Test]
        public void When_Using_used_with_null_Exception_is_thrown()
        {
            bool called = false;
            Assert.Throws<ArgumentNullException>(() => Values.Using<IDisposable>(null, _ => called = true));
            Assert.That(called, Is.False);
        }

        [Test]
        public void When_Using_used_with_idisposable_object_Action_is_called_and_disposable_is_disposed()
        {
            bool called = false;
            var disposable = new TestDisposable();
            Values.Using(disposable, _ => called = true);
            Assert.That(disposable.DisposeCalled, Is.True);
            Assert.That(called, Is.True);
        }

        [Test]
        public void When_Using_used_with_non_idisposable_object_Action_is_still_called()
        {
            bool called = false;
            object anyObject = new object();
            Values.Using(anyObject, _ => called = true);
            Assert.That(called, Is.True);
        }

        [Test]
        public void When_checking_value_types_for_null_Then_true_should_be_returned()
        {
            Assert.That(Values.NotNull(1), Is.True);
            Assert.That(Values.NotNull(new TestStaticBlittable()), Is.True);
            Assert.That(Values.NotNull(new decimal(1f)), Is.True);
        }

        [Test]
        public void When_checking_reference_types_for_null_Then_true_should_be_returned_for_non_nulls()
        {
            Assert.That(Values.NotNull(new object()), Is.True);
            Assert.That(Values.NotNull(new int?(1)), Is.True);
            Assert.That(Values.NotNull(new int?()), Is.False);
            Assert.That(Values.NotNull<object>(null), Is.False);
        }

        public class TestDisposable : IDisposable
        {
            public bool DisposeCalled;

            public void Dispose()
            {
                DisposeCalled = true;
            }
        }
    }
}