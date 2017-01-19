using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Essentions.IO;
using NUnit.Framework;
using FileAccess = Essentions.IO.FileAccess;
using FileMode = Essentions.IO.FileMode;
using FileShare = Essentions.IO.FileShare;
using Path = Essentions.IO.Path;

namespace Essentions.Tests.IO
{
    /// <summary>
    /// This is not a test fixture, but an example of a standard minimalistic implementation of all needed interfaces.
    /// </summary>
    [TestFixture]
    public class StandardImplementationExampleTests
    {
        internal sealed class File : IFile
        {
            private readonly FileInfo _file;

            public FilePath Path { get; }

            Path IFileSystemInfo.Path => Path;

            public bool Exists => _file.Exists;

            public bool Hidden => (_file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

            public long Length => _file.Length;

            /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
            /// <exception cref="UnauthorizedAccessException">Access to <paramref name="path" /> is denied. </exception>
            public File(FilePath path)
            {
                Debug.Assert(path != null);
                Path = path;
                _file = new FileInfo(path.FullPath);
            }

            /// <exception cref="ArgumentNullException"><paramref name="destination"/> is <see langword="null"/></exception>
            public void Copy(FilePath destination, bool overwrite)
            {
                if (destination == null) {
                    throw new ArgumentNullException(nameof(destination));
                }
                _file.CopyTo(destination.FullPath, overwrite);
            }

            public void Move(FilePath destination)
            {
                if (destination == null) {
                    throw new ArgumentNullException(nameof(destination));
                }
                _file.MoveTo(destination.FullPath);
            }

            public void Delete()
            {
                _file.Delete();
            }

            public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
            {
                return _file.Open((System.IO.FileMode)fileMode,
                                  (System.IO.FileAccess)fileAccess,
                                  (System.IO.FileShare)fileShare);
            }
        }

        internal sealed class Directory : IDirectory
        {
            private readonly DirectoryInfo _directory;

            public DirectoryPath Path { get; }

            Path IFileSystemInfo.Path => Path;

            public bool Exists => _directory.Exists;

            public bool Hidden => (_directory.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

            /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
            public Directory(DirectoryPath path)
            {
                Debug.Assert(path != null);
                Path = path;
                _directory = new DirectoryInfo(Path.FullPath);
            }

            /// <exception cref="IOException">The directory cannot be created. </exception>
            public void Create()
            {
                _directory.Create();
            }

            /// <exception cref="UnauthorizedAccessException">The directory contains a read-only file.</exception>
            /// <exception cref="DirectoryNotFoundException">The directory described by this
            ///  <see cref="T:System.IO.DirectoryInfo" /> object does not exist or could not be found.</exception>
            /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
            public void Delete(bool recursive)
            {
                var fileSearchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                foreach (var info in _directory.GetFileSystemInfos("*", fileSearchOption)) {
                    info.Attributes &= ~FileAttributes.ReadOnly;
                }

                _directory.Delete(recursive);
            }

            public IEnumerable<IDirectory> GetDirectories(string filter, SearchScope scope)
            {
                var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
                return _directory.GetDirectories(filter, option)
                                 .Select(directory => new Directory(directory.FullName));
            }

            public IEnumerable<IFile> GetFiles(string filter, SearchScope scope)
            {
                var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
                return _directory.GetFiles(filter, option)
                                 .Select(file => new File(file.FullName));
            }
        }

        /// <summary>
        /// A physical file system implementation.
        /// </summary>
        public sealed class FileSystem : IFileSystem
        {
            /// <summary>
            /// Gets a <see cref="IFile" /> instance representing the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>A <see cref="IFile" /> instance representing the specified path.</returns>
            /// <exception cref="System.Security.SecurityException">The caller does not have the required permission. </exception>
            /// <exception cref="UnauthorizedAccessException">Access to <paramref name="path" /> is denied. </exception>
            public IFile GetFile(FilePath path)
            {
                Check.NotNull(path);

                return new File(path);
            }

            /// <summary>
            /// Gets a <see cref="IDirectory" /> instance representing the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>A <see cref="IDirectory" /> instance representing the specified path.</returns>
            /// <exception cref="System.Security.SecurityException">The caller does not have the required permission. </exception>
            public IDirectory GetDirectory(DirectoryPath path)
            {
                Check.NotNull(path);

                return new Directory(path);
            }
        }

        public sealed class FileSystemEnvironment : IFileSystemEnvironment
        {
            /// <summary>Initializes a new instance of the <see cref="FileSystemEnvironment"/> class.</summary>
            public FileSystemEnvironment()
            {
                Globber = new Globber(this);
                FS = new FileSystem();
                WorkingDirectory = new DirectoryPath(System.IO.Directory.GetCurrentDirectory());
            }

            /// <summary>Gets the application root path.</summary>
            /// <value>The application root path.</value>
            public DirectoryPath ApplicationRoot => new DirectoryPath(
                System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            /// <summary>Gets the file system.</summary>
            public IFileSystem FS { get; set; }

            /// <summary>Gets the globber.</summary>
            public IGlobber Globber { get; set; }

            /// <summary>Gets or sets the working directory.</summary>
            /// <value>The working directory.</value>
            public DirectoryPath WorkingDirectory
            {
                get { return System.IO.Directory.GetCurrentDirectory(); }
                set { SetWorkingDirectory(value); }
            }

            /// <exception cref="NotSupportedException">The special path <paramref name="path"/> is not supported.
            /// </exception>
            public DirectoryPath GetSpecialPath(SpecialPath path)
            {
                switch (path) {
                case SpecialPath.ApplicationData:
                    return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                case SpecialPath.CommonApplicationData:
                    return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
                case SpecialPath.LocalApplicationData:
                    return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                case SpecialPath.ProgramFiles:
                    return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
                case SpecialPath.ProgramFilesX86:
                    return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
                case SpecialPath.Windows:
                    return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
                case SpecialPath.LocalTemp:
                    return new DirectoryPath(System.IO.Path.GetTempPath());
                }

                throw new NotSupportedException($"The special path '{path}' is not supported.");
            }

            /// <summary>Gets whether or not the current operative system is 64 bit.</summary>
            /// <returns>
            /// Whether or not the current operative system is 64 bit.
            /// </returns>
            public bool Is64BitOS()
            {
                return Environment.Is64BitProcess;
            }

            /// <summary>Determines whether the current machine is running Unix.</summary>
            /// <returns>Whether or not the current machine is running Unix.</returns>
            public bool IsUnix()
            {
                var platform = (int)Environment.OSVersion.Platform;
                return (platform == 4) || (platform == 6) || (platform == 128);
            }

            private static void SetWorkingDirectory(DirectoryPath path)
            {
                if (path.IsRelative) {
                    throw new InvalidOperationException("Working directory can not be set to a relative path.");
                }

                System.IO.Directory.SetCurrentDirectory(path.FullPath);
            }
        }
    }
}