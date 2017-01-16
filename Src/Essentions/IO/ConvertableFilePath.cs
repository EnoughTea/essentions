using System;
using JetBrains.Annotations;

namespace Essentions.IO
{
    /// <summary>
    /// Represents a <see cref="FilePath" /> that can be easily converted.
    /// </summary>
    public sealed class ConvertableFilePath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertableFilePath"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/></exception>
        internal ConvertableFilePath(FilePath path)
        {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            Path = path;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The actual path.</value>
        public FilePath Path { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ConvertableFilePath"/> to <see cref="FilePath"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The result of the conversion. </returns>
        [CanBeNull]
        public static implicit operator FilePath(ConvertableFilePath path)
        {
            return path?.Path;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ConvertableFilePath"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The result of the conversion.</returns>
        [CanBeNull]
        public static implicit operator string(ConvertableFilePath path)
        {
            return path?.Path.FullPath;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Path.FullPath;
        }
    }
}