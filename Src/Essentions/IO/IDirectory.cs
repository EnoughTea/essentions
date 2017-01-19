using System.Collections.Generic;

namespace Essentions.IO
{
    /// <summary>Represents a directory.</summary>
    /// <remarks>
    /// <example>
    /// <code>
    ///    internal sealed class Directory : IDirectory
    ///    {
    ///        private readonly DirectoryInfo _directory;
    ///        private readonly DirectoryPath _path;
    ///
    ///        public DirectoryPath Path =&gt; _path;
    ///
    ///        Path IFileSystemInfo.Path =&gt; _path;
    ///
    ///        public bool Exists =&gt; _directory.Exists;
    ///
    ///        public bool Hidden =&gt; (_directory.Attributes &amp; FileAttributes.Hidden) == FileAttributes.Hidden;
    ///
    ///        /// &lt;exception cref="SecurityException"&gt;The caller does not have the required permission. &lt;/exception&gt;
    ///        public Directory(DirectoryPath path)
    ///        {
    ///            Debug.Assert(path != null);
    ///            _path = path;
    ///            _directory = new DirectoryInfo(_path.FullPath);
    ///        }
    ///
    ///        /// &lt;exception cref="IOException"&gt;The directory cannot be created. &lt;/exception&gt;
    ///        public void Create()
    ///        {
    ///            _directory.Create();
    ///        }
    ///
    ///        /// &lt;exception cref="UnauthorizedAccessException"&gt;The directory contains a read-only file.&lt;/exception&gt;
    ///        /// &lt;exception cref="DirectoryNotFoundException"&gt;The directory described by this
    ///        ///  &lt;see cref="T:System.IO.DirectoryInfo" /&gt; object does not exist or could not be found.&lt;/exception&gt;
    ///        /// &lt;exception cref="SecurityException"&gt;The caller does not have the required permission. &lt;/exception&gt;
    ///        public void Delete(bool recursive)
    ///        {
    ///            var fileSearchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    ///            foreach (var info in _directory.GetFileSystemInfos("*", fileSearchOption)) {
    ///                info.Attributes &amp;= ~FileAttributes.ReadOnly;
    ///            }
    ///
    ///            _directory.Delete(recursive);
    ///        }
    ///
    ///        public IEnumerable&lt;IDirectory&gt; GetDirectories(string filter, SearchScope scope)
    ///        {
    ///            var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
    ///            return _directory.GetDirectories(filter, option)
    ///                .Select(directory =&gt; new Directory(directory.FullName));
    ///        }
    ///
    ///        public IEnumerable&lt;IFile&gt; GetFiles(string filter, SearchScope scope)
    ///        {
    ///            var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
    ///            return _directory.GetFiles(filter, option)
    ///                .Select(file =&gt; new File(file.FullName));
    ///        }
    ///    }
    /// </code>
    /// </example>
    /// </remarks>
    public interface IDirectory : IFileSystemInfo
    {
        /// <summary>Gets the path to the directory.</summary>
        /// <value>The path.</value>
        new DirectoryPath Path { get; }

        /// <summary>Creates the directory.</summary>
        void Create();

        /// <summary>Deletes the directory.</summary>
        /// <param name="recursive">Will perform a recursive delete if set to <c>true</c>.</param>
        void Delete(bool recursive);

        /// <summary>Gets directories matching the specified filter and scope.</summary>
        /// <param name="filter">The filter.</param>
        /// <param name="scope">The search scope.</param>
        /// <returns>Directories matching the filter and scope.</returns>
        IEnumerable<IDirectory> GetDirectories(string filter, SearchScope scope);

        /// <summary>Gets files matching the specified filter and scope.</summary>
        /// <param name="filter">The filter.</param>
        /// <param name="scope">The search scope.</param>
        /// <returns>Files matching the specified filter and scope.</returns>
        IEnumerable<IFile> GetFiles(string filter, SearchScope scope);
    }
}