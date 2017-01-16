using System;
using System.Collections.Generic;

namespace Essentions.IO
{
    /// <summary>
    /// Compares <see cref="Path"/> instances.
    /// </summary>
    public sealed class PathComparer : IEqualityComparer<Path>
    {
        /// <summary>The default path comparer.</summary>
        private static readonly Lazy<PathComparer> _Default =
            new Lazy<PathComparer>(() => new PathComparer(Machine.IsUnix()));

        /// <summary>
        /// Gets a value indicating whether comparison is case sensitive.
        /// </summary>
        /// <value>
        /// <c>true</c> if comparison is case sensitive; otherwise, <c>false</c>.
        /// </value>
        public bool IsCaseSensitive { get; }

        /// <summary>The default path comparer.</summary>
        public static PathComparer Default => _Default.Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathComparer"/> class.
        /// </summary>
        /// <param name="isCaseSensitive">if set to <c>true</c>, comparison is case sensitive.</param>
        public PathComparer(bool isCaseSensitive)
        {
            IsCaseSensitive = isCaseSensitive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathComparer"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <exception cref="ArgumentNullException"><paramref name="environment"/> is <see langword="null"/></exception>
        public PathComparer(IFileSystemEnvironment environment)
        {
            if (environment == null) {
                throw new ArgumentNullException(nameof(environment));
            }

            IsCaseSensitive = environment.IsUnix();
        }

        /// <summary>
        /// Determines whether the specified <see cref="Path"/> instances are equal.
        /// </summary>
        /// <param name="x">The first <see cref="Path"/> to compare.</param>
        /// <param name="y">The second <see cref="Path"/> to compare.</param>
        /// <returns>
        /// True if the specified <see cref="Path"/> instances are equal; otherwise, false.
        /// </returns>
        public bool Equals(Path x, Path y)
        {
            if (x == null && y == null) {
                return true;
            }
            if (x == null || y == null) {
                return false;
            }

            if (IsCaseSensitive) {
                return x.FullPath.Equals(y.FullPath);
            }
            return x.FullPath.Equals(y.FullPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="Path"/>.
        /// </summary>
        /// <param name="obj">The path.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/></exception>
        public int GetHashCode(Path obj)
        {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }
            if (IsCaseSensitive) {
                return obj.FullPath.GetHashCode();
            }

            return obj.FullPath.ToUpperInvariant().GetHashCode();
        }
    }
}