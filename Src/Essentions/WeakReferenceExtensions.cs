using System;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="WeakReference" />. </summary>
    public static class WeakReferenceExtensions
    {
        /// <summary>
        ///     Tries to retrieve the target object that is referenced by the current
        ///     <see cref="WeakReference{T}" /> object.
        /// </summary>
        /// <typeparam name="T">Referenced object type.</typeparam>
        /// <param name="weakReference">Weak reference to get target of.</param>
        /// <returns>The target object, if it is available, or null.</returns>
        [CanBeNull]
        public static T GetTarget<T>([NotNull] this WeakReference<T> weakReference) where T : class
        {
            Check.NotNull(weakReference);

            T instance;
            weakReference.TryGetTarget(out instance);
            return instance;
        }
    }
}