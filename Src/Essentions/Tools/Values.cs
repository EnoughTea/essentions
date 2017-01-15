using System;
using JetBrains.Annotations;

namespace Essentions.Tools
{
    /// <summary> Some misc methods that don't belong anywhere else. </summary>
    public static class Values
    {
        /// <summary>Checks the specified for being non-null without boxing.
        /// Supports <see cref="Nullable{T}"/>.</summary>
        /// <returns><c>true</c> for non-null values; otherwise, <c>false</c>.</returns>
        public static bool NotNull<T>(T value)
        {
            return NullCheck<T>.NullOp.HasValue(value);
        }

        /// <summary>Version of a using statement that is less fussy at compile time.</summary>
        /// <param name="resource">Instance which implements <see cref="IDisposable" />.</param>
        /// <param name="action">Action to perform with resource.</param>
        public static void Using<T>([NotNull] T resource, [NotNull] Action<T> action)
            where T : class
        {
            Check.NotNull(resource)
                 .NotNull(action);

            try {
                action(resource);
            }
            finally {
                (resource as IDisposable)?.Dispose();
            }
        }
    }
}