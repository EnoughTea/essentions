using System.IO;

namespace Essentions.IO
{
    /// <summary>Represents a file.</summary>
    /// <remarks>
    /// <example>
    /// <code>
    ///    internal sealed class File : IFile
    ///    {
    ///        private readonly FileInfo _file;
    ///        private readonly FilePath _path;
    ///
    ///        public FilePath Path =&gt; _path;
    ///
    ///        Path IFileSystemInfo.Path =&gt; _path;
    ///
    ///        public bool Exists =&gt; _file.Exists;
    ///
    ///        public bool Hidden =&gt; (_file.Attributes &amp; FileAttributes.Hidden) == FileAttributes.Hidden;
    ///
    ///        public long Length =&gt; _file.Length;
    ///
    ///        /// &lt;exception cref="SecurityException"&gt;The caller does not have the required permission. &lt;/exception&gt;
    ///        /// &lt;exception cref="UnauthorizedAccessException"&gt;Access to &lt;paramref name="path" /&gt; is denied. &lt;/exception&gt;
    ///        public File(FilePath path)
    ///        {
    ///            Debug.Assert(path != null);
    ///            _path = path;
    ///            _file = new FileInfo(path.FullPath);
    ///        }
    ///
    ///        /// &lt;exception cref="ArgumentNullException"&gt;&lt;paramref name="destination"/&gt; is &lt;see langword="null"/&gt;&lt;/exception&gt;
    ///        public void Copy(FilePath destination, bool overwrite)
    ///        {
    ///            if (destination == null) {
    ///                throw new ArgumentNullException(nameof(destination));
    ///            }
    ///            _file.CopyTo(destination.FullPath, overwrite);
    ///        }
    ///
    ///        public void Move(FilePath destination)
    ///        {
    ///            if (destination == null) {
    ///                throw new ArgumentNullException(nameof(destination));
    ///            }
    ///            _file.MoveTo(destination.FullPath);
    ///        }
    ///
    ///        public void Delete()
    ///        {
    ///            _file.Delete();
    ///        }
    ///
    ///        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
    ///        {
    ///            return _file.Open((System.IO.FileMode)fileMode,
    ///                              (System.IO.FileAccess)fileAccess,
    ///                              (System.IO.FileShare)fileShare);
    ///        }
    ///    }
    /// </code>
    /// </example>
    /// </remarks>
    public interface IFile : IFileSystemInfo
    {
        /// <summary>Gets the path to the file.</summary>
        /// <value>The path.</value>
        new FilePath Path { get; }

        /// <summary>Gets the length of the file.</summary>
        /// <value>The length of the file.</value>
        long Length { get; }

        /// <summary>Copies the file to the specified destination path.</summary>
        /// <param name="destination">The destination path.</param>
        /// <param name="overwrite">Will overwrite existing destination file if set to <c>true</c>.</param>
        void Copy(FilePath destination, bool overwrite);

        /// <summary>Moves the file to the specified destination path.</summary>
        /// <param name="destination">The destination path.</param>
        void Move(FilePath destination);

        /// <summary>Deletes the file.</summary>
        void Delete();

        /// <summary>Opens the file using the specified options.</summary>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="fileAccess">The file access.</param>
        /// <param name="fileShare">The file share.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
    }
}