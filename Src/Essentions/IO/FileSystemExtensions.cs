using System;
using System.IO;

namespace Essentions.IO
{
    /// <summary>
    /// Contains extensions for <see cref="IFileSystem"/>.
    /// </summary>
    public static class FileSystemExtensions
    {
        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="path">The path.</param>
        /// <returns>Created or existing directory.</returns>
        public static IDirectory CreateDirectory(this IFileSystem fileSystem, DirectoryPath path)
        {
            if (fileSystem == null) {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            var directory = fileSystem.GetDirectory(path);
            if (!directory.Exists) {
                directory.Create();
            }
            return directory;
        }

        /// <summary>
        /// Creates a file at the specified path or gets an existing file.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="path">The path.</param>
        /// <returns>Created or existing file.</returns>
        public static IFile CreateFile(this IFileSystem fileSystem, FilePath path)
        {
            if (fileSystem == null) {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            CreateDirectory(fileSystem, path.GetDirectory());

            var file = fileSystem.GetFile(path);
            if (!file.Exists) {
                file.OpenWrite().Dispose();
            }

            return file;
        }

        /// <summary>
        /// Creates a file at the specified path or gets an existing file. If file was created,
        ///  writes given content to it.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="path">The path.</param>
        /// <param name="contentsBytes">The file contents.</param>
        /// <returns>Created or existing file.</returns>
        public static IFile CreateFile(this IFileSystem fileSystem, FilePath path, byte[] contentsBytes)
        {
            if (fileSystem == null) {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            CreateDirectory(fileSystem, path.GetDirectory());

            var file = fileSystem.GetFile(path);
            if (!file.Exists) {
                using (var stream = file.OpenWrite()) {
                    using (var ms = new MemoryStream(contentsBytes)) {
                        ms.CopyTo(stream);
                    }
                }
            }

            return file;
        }

        /// <summary>
        /// Determines if a specified <see cref="FilePath"/> exist.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="path">The path.</param>
        /// <returns>Whether or not the specified file exist.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileSystem"/> is <see langword="null"/></exception>
        public static bool Exists(this IFileSystem fileSystem, FilePath path)
        {
            if (fileSystem == null) {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            var file = fileSystem.GetFile(path);
            return file != null && file.Exists;
        }

        /// <summary>
        /// Determines if a specified <see cref="DirectoryPath"/> exist.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="path">The path.</param>
        /// <returns>Whether or not the specified directory exist.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileSystem"/> is <see langword="null"/></exception>
        public static bool Exists(this IFileSystem fileSystem, DirectoryPath path)
        {
            if (fileSystem == null) {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            var directory = fileSystem.GetDirectory(path);
            return directory != null && directory.Exists;
        }
    }
}