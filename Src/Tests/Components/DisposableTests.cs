using System;
using Essentions.Components;
using NUnit.Framework;

namespace Essentions.Tests.Components
{
    [TestFixture]
    public class DisposableTests
    {
        [SetUp]
        public void Init()
        {
            WithoutFinalizer.BothManagedAndUnmanagedWereDisposed = false;
            WithoutFinalizer.DisposeOverrideCalled = false;
        }

        [Test]
        public void When_Disposable_without_finalizer_is_collected_Then_Dispose_method_is_not_called_at_all()
        {
            var disposable = new WeakReference<WithoutFinalizer>(new WithoutFinalizer());
            Assert.That(disposable.GetTarget().IsDisposed, Is.False);
            Assert.DoesNotThrow(() => disposable.GetTarget().CallEnsureNotDisposed());
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.That(disposable.GetTarget(), Is.Null);
            Assert.That(WithoutFinalizer.DisposeOverrideCalled, Is.False);
            Assert.That(WithoutFinalizer.BothManagedAndUnmanagedWereDisposed, Is.False);
        }

        [Test]
        public void When_Disposable_without_finalizer_is_disposed_Then_managed_resources_are_disposed()
        {
            var disposable = new WithoutFinalizer();
            Assert.That(disposable.IsDisposed, Is.False);
            disposable.Dispose();
            Assert.That(WithoutFinalizer.DisposeOverrideCalled, Is.True);
            Assert.That(WithoutFinalizer.BothManagedAndUnmanagedWereDisposed, Is.True);
            Assert.That(disposable.IsDisposed, Is.True);
            Assert.Throws<ObjectDisposedException>(() => disposable.CallEnsureNotDisposed());
        }

        [Test]
        public void When_Disposable_with_finalizer_is_collected_Then_only_unmanaged_resources_are_disposed()
        {
            var disposable = new WeakReference<WithFinalizer>(new WithFinalizer());
            Assert.That(disposable.GetTarget().IsDisposed, Is.False);
            Assert.DoesNotThrow(() => disposable.GetTarget().CallEnsureNotDisposed());
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.That(disposable.GetTarget(), Is.Null);
            Assert.That(WithoutFinalizer.DisposeOverrideCalled, Is.True);
            Assert.That(WithoutFinalizer.BothManagedAndUnmanagedWereDisposed, Is.False);
        }

        [Test]
        public void When_Disposable_with_finalizer_is_disposed_Then_managed_and_unmanaged_resources_are_disposed()
        {
            var disposable = new WithFinalizer();
            Assert.That(disposable.IsDisposed, Is.False);
            disposable.Dispose();
            Assert.That(WithoutFinalizer.DisposeOverrideCalled, Is.True);
            Assert.That(WithoutFinalizer.BothManagedAndUnmanagedWereDisposed, Is.True);
            Assert.That(disposable.IsDisposed, Is.True);
            Assert.Throws<ObjectDisposedException>(() => disposable.CallEnsureNotDisposed());
        }

        private class WithoutFinalizer : Disposable
        {
            public static bool DisposeOverrideCalled { get; set; }

            public static bool BothManagedAndUnmanagedWereDisposed { get; set; }

            public void CallEnsureNotDisposed()
            {
                EnsureNotDisposed();
            }

            /// <summary> Releases unmanaged and - optionally - managed resources. </summary>
            /// <param name="disposing">
            ///     <c>true</c> to release both managed and unmanaged resources;
            ///     <c>false</c> to release only unmanaged resources.
            /// </param>
            protected override void OnDispose(bool disposing)
            {
                DisposeOverrideCalled = true;
                BothManagedAndUnmanagedWereDisposed = disposing;
            }
        }

        private class WithFinalizer : WithoutFinalizer
        {
            ~WithFinalizer()
            {
                Dispose(false);
            }
        }
    }
}