using System;
using System.Collections.Generic;
using System.IO;

namespace Essentions.IO
{
    /// <summary>
    /// Contains extension methods for <see cref="IFile"/>.
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// Sets the content of the provided file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="content">The content.</param>
        /// <returns>The same <see cref="IFile"/> instance so that multiple calls can be chained.</returns>
        public static IFile SetText(this IFile file, string content)
        {
            using (var stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream)) {
                writer.Write(content);
                return file;
            }
        }

        /// <summary>
        /// Gets the text content of the file (UTF-8 encoding).
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The text content of the file.</returns>
        /// <exception cref="FileNotFoundException">File could not be found.</exception>
        public static string GetText(this IFile file)
        {
            if (!file.Exists) {
                throw new FileNotFoundException("File could not be found.", file.Path.FullPath);
            }

            using (var stream = file.OpenRead())
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Opens the file using the specified options.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is <see langword="null"/></exception>
        public static Stream Open(this IFile file, FileMode mode)
        {
            if (file == null) {
                throw new ArgumentNullException(nameof(file));
            }
            return file.Open(mode,
                             mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite,
                             FileShare.None);
        }

        /// <summary>
        /// Opens the file using the specified options.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is <see langword="null"/></exception>
        public static Stream Open(this IFile file, FileMode mode, FileAccess access)
        {
            if (file == null) {
                throw new ArgumentNullException(nameof(file));
            }
            return file.Open(mode, access, FileShare.None);
        }

        /// <summary>
        /// Opens the file for reading.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is <see langword="null"/></exception>
        public static Stream OpenRead(this IFile file)
        {
            if (file == null) {
                throw new ArgumentNullException(nameof(file));
            }
            return file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Opens the file for writing.
        /// If the file already exists, it will be overwritten.
        /// </summary>
        /// <param name="file">The file to be opened.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is <see langword="null"/></exception>
        public static Stream OpenWrite(this IFile file)
        {
            if (file == null) {
                throw new ArgumentNullException(nameof(file));
            }
            return file.Open(FileMode.Create, FileAccess.Write, FileShare.None);
        }

        /// <summary>
        /// Enumerates line in file
        /// </summary>
        /// <param name="file">The file to be read from.</param>
        /// <param name="encoding">The encoding that is applied to the content of the file.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of file line content</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is <see langword="null"/></exception>
        public static IEnumerable<string> ReadLines(this IFile file, System.Text.Encoding encoding)
        {
            if (file == null) {
                throw new ArgumentNullException(nameof(file));
            }

            using (var stream = file.OpenRead())
            using (var reader = new StreamReader(stream, encoding)) {
                var result = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null) {
                    result.Add(line);
                }

                return result;
            }
        }
    }
}