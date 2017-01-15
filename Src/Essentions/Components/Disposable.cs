using System;

namespace Essentions.Components
{
    /// <summary>
    ///     Implements the standard .NET disposal pattern. Does not implement a finalizer,
    ///     since it is needed only on classes with actual unmanaged resources to finalize. Therefore,
    ///     <see cref="Dispose()" /> won't be called after garbage collection if there are only managed
    ///     resources to dispose.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        /// <summary> Gets a value indicating whether this instance is disposed. </summary>
        /// <value> <c>true</c> if this instance is disposed; otherwise, <c>false</c>. </value>
        public bool IsDisposed { get; private set; }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources;
        ///     <c>false</c> to release only unmanaged resources.
        /// </param>
        protected void Dispose(bool disposing)
        {
            if (!IsDisposed) {
                OnDispose(disposing);
                IsDisposed = true;
            }
        }

        /// <summary> If this instance is disposed, throws an exception. </summary>
        /// <exception cref="ObjectDisposedException"> Throws if this instance is disposed. </exception>
        protected void EnsureNotDisposed()
        {
            if (IsDisposed) {
                var name = GetType().FullName;
                throw new ObjectDisposedException(name, $"Object of type '{name}' is disposed already");
            }
        }

        /// <summary> Releases unmanaged and - optionally - managed resources. </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources;
        ///     <c>false</c> to release only unmanaged resources.
        /// </param>
        protected abstract void OnDispose(bool disposing);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or
        ///     resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        ///     Don't call this from finalizer in child classes! Call <see cref="Dispose(bool)" /> with <c>false</c>
        ///     instead.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}