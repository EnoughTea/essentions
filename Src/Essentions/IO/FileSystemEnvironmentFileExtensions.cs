using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Essentions.IO
{
    /// <summary>
    /// Contains functionality related to file operations.
    /// </summary>
    public static class FileSystemEnvironmentFileExtensions
    {
        /// <summary>
        /// Gets a file path from string.
        /// </summary>
        /// <example>
        /// <code>
        /// // Get the temp file.
        /// var root = Directory("./");
        /// var temp = root + File("temp");
        /// // Delete the file.
        /// CleanDirectory(temp);
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="path">The path.</param>
        /// <returns>A file path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="path"/>
        ///  is <see langword="null"/></exception>
        public static ConvertableFilePath File(this IFileSystemEnvironment env, string path)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }
            return new ConvertableFilePath(new FilePath(path));
        }

        /// <summary>
        /// Copies an existing file to a new location.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="targetDirectoryPath">The target directory path.</param>
        /// <example>
        /// <code>
        /// CopyFileToDirectory("test.txt", "./targetdir");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/>
        ///  or <paramref name="targetDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="InvalidOperationException">The target directory does not exist.</exception>
        public static void CopyFileToDirectory(this IFileSystemEnvironment env,
                                               FilePath filePath,
                                               DirectoryPath targetDirectoryPath)
        {
            FileCopier.CopyFileToDirectory(env, filePath, targetDirectoryPath);
        }

        /// <summary>
        /// Copies an existing file to a new file, providing the option to specify a new file name.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="targetFilePath">The target file path.</param>
        /// <example>
        /// <code>
        /// CopyFile("test.tmp", "test.txt");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/>
        ///  or <paramref name="targetFilePath"/> is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="InvalidOperationException">The target directory does not exist.</exception>
        public static void CopyFile(this IFileSystemEnvironment env, FilePath filePath, FilePath targetFilePath)
        {
            FileCopier.CopyFile(env, filePath, targetFilePath);
        }

        /// <summary>
        /// Copies all files matching the provided pattern to a new location.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="targetDirectoryPath">The target directory path.</param>
        /// <example>
        /// <code>
        /// CopyFiles("Cake.*", "./publish");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="pattern"/>
        ///  or <paramref name="targetDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="InvalidOperationException">The target directory does not exist.</exception>
        public static void CopyFiles(this IFileSystemEnvironment env, string pattern,
                                     DirectoryPath targetDirectoryPath)
        {
            FileCopier.CopyFiles(env, pattern, targetDirectoryPath);
        }

        /// <summary>
        /// Copies existing files to a new location.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePaths">The file paths.</param>
        /// <param name="targetDirectoryPath">The target directory path.</param>
        /// <example>
        /// <code>
        /// var files = GetFiles("./**/Cake.*");
        /// CopyFiles(files, "destination");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePaths"/>
        ///  or <paramref name="targetDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="InvalidOperationException">The target directory does not exist.</exception>
        public static void CopyFiles(this IFileSystemEnvironment env, IEnumerable<FilePath> filePaths,
                                     DirectoryPath targetDirectoryPath)
        {
            FileCopier.CopyFiles(env, filePaths, targetDirectoryPath);
        }

        /// <summary>
        /// Copies existing files to a new location.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePaths">The file paths.</param>
        /// <param name="targetDirectoryPath">The target directory path.</param>
        /// <example>
        /// <code>
        /// CreateDirectory("destination");
        /// var files = new [] {
        ///     "Cake.exe",
        ///     "Cake.pdb"
        /// };
        /// CopyFiles(files, "destination");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePaths"/>
        ///  or <paramref name="targetDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">The target directory does not exist.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        public static void CopyFiles(this IFileSystemEnvironment env, IEnumerable<string> filePaths,
                                     DirectoryPath targetDirectoryPath)
        {
            if (filePaths == null) {
                throw new ArgumentNullException(nameof(filePaths));
            }

            var paths = filePaths.Select(p => new FilePath(p));
            FileCopier.CopyFiles(env, paths, targetDirectoryPath);
        }

        /// <summary>
        /// Moves an existing file to a new location.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="targetDirectoryPath">The target directory path.</param>
        /// <example>
        /// <code>
        /// MoveFileToDirectory("test.txt", "./targetdir");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/>
        ///  or <paramref name="targetDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The target directory do not exist..</exception>
        public static void MoveFileToDirectory(this IFileSystemEnvironment env, FilePath filePath,
                                               DirectoryPath targetDirectoryPath)
        {
            FileMover.MoveFileToDirectory(env, filePath, targetDirectoryPath);
        }

        /// <summary>
        /// Moves existing files matching the specified pattern to a new location.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="targetDirectoryPath">The target directory path.</param>
        /// <example>
        /// <code>
        /// MoveFiles("./publish/Cake.*", "./destination");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="pattern"/>
        ///  or <paramref name="targetDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The target directory do not exist.</exception>
        public static void MoveFiles(this IFileSystemEnvironment env, string pattern,
                                     DirectoryPath targetDirectoryPath)
        {
            FileMover.MoveFiles(env, pattern, targetDirectoryPath);
        }

        /// <summary>
        /// Moves existing files to a new location.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePaths">The file paths.</param>
        /// <param name="targetDirectoryPath">The target directory path.</param>
        /// <example>
        /// <code>
        /// var files = GetFiles("./publish/Cake.*");
        /// MoveFiles(files, "destination");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePaths"/>
        ///  or <paramref name="targetDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The target directory do not exist.</exception>
        /// <exception cref="InvalidOperationException">The directory <paramref name="targetDirectoryPath"/>.FullPath
        ///  do not exist.</exception>
        public static void MoveFiles(this IFileSystemEnvironment env, IEnumerable<FilePath> filePaths,
                                     DirectoryPath targetDirectoryPath)
        {
            FileMover.MoveFiles(env, filePaths, targetDirectoryPath);
        }

        /// <summary>
        /// Moves an existing file to a new location, providing the option to specify a new file name.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="targetFilePath">The target file path.</param>
        /// <example>
        /// <code>
        /// MoveFile("test.tmp", "test.txt");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/>
        ///  or <paramref name="targetFilePath"/> is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The target directory do not exist..</exception>
        /// <exception cref="InvalidOperationException">The file <paramref name="filePath.FullPath"/> do not exist.
        /// </exception>
        public static void MoveFile(this IFileSystemEnvironment env, FilePath filePath, FilePath targetFilePath)
        {
            FileMover.MoveFile(env, filePath, targetFilePath);
        }

        /// <summary>
        /// Deletes the specified files.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="pattern">The pattern.</param>
        /// <example>
        /// <code>
        /// DeleteFiles("./publish/Cake.*");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="pattern"/>
        ///  is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="InvalidOperationException">Cannot delete files when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void DeleteFiles(this IFileSystemEnvironment env, string pattern)
        {
            FileDeleter.DeleteFiles(env, pattern);
        }

        /// <summary>
        /// Deletes the specified files.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePaths">The file paths.</param>
        /// <example>
        /// <code>
        /// var files = GetFiles("./destination/Cake.*");
        /// DeleteFiles(files);
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePaths"/>
        ///  is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="InvalidOperationException">Cannot delete files when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void DeleteFiles(this IFileSystemEnvironment env, IEnumerable<FilePath> filePaths)
        {
            FileDeleter.DeleteFiles(env, filePaths);
        }

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePath">The file path.</param>
        /// <example>
        /// <code>
        /// DeleteFile("deleteme.txt");
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/>
        ///  is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file <paramref name="filePath"/> does not exist.</exception>
        /// <exception cref="InvalidOperationException">Cannot delete files when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void DeleteFile(this IFileSystemEnvironment env, FilePath filePath)
        {
            FileDeleter.DeleteFile(env, filePath);
        }

        /// <summary>
        /// Determines whether the given path refers to an existing file.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePath">The <see cref="FilePath"/> to check.</param>
        /// <returns><c>true</c> if <paramref name="filePath"/> refers to an existing file;
        /// <c>false</c> if the file does not exist or an error occurs when trying to
        /// determine if the specified file exists.</returns>
        /// <example>
        /// <code>
        /// if (FileExists("findme.txt"))
        /// {
        ///     Information("File exists!");
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/>
        ///  is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot check file for existance when
        ///  <paramref name="env.FS"/> is null.</exception>
        public static bool FileExists(this IFileSystemEnvironment env, FilePath filePath)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            if (filePath == null) {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot check file for existance when {nameof(env)}.{nameof(env.FS)} is null");
            }

            return env.FS.GetFile(filePath.MakeAbsolute(env)).Exists;
        }

        /// <summary>
        /// Makes the path absolute (if relative) using the current working directory.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="path">The path.</param>
        /// <returns>An absolute file path.</returns>
        /// <example>
        /// <code>
        /// var path = MakeAbsolute(File("./resources"));
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> or <paramref name="path"/>
        ///  is <see langword="null"/></exception>
        public static FilePath MakeAbsolute(this IFileSystemEnvironment context, FilePath path)
        {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            return path.MakeAbsolute(context);
        }

        /// <summary>
        /// Gets the size of a file in bytes.
        /// </summary>
        /// <param name="env">The context.</param>
        /// <param name="filePath">The path.</param>
        /// <returns>Size of file in bytes or -1 if file doesn't exist.</returns>
        /// <example>
        /// <code>
        /// Information("File size: {0}", FileSize("./build.cake"));
        /// </code>
        /// </example>
        /// <exception cref="FileNotFoundException">Unable to find the specified file.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/>
        ///  is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot get file size when
        ///  <paramref name="env.FS"/> is null.</exception>
        public static long FileSize(this IFileSystemEnvironment env, FilePath filePath)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            if (filePath == null) {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot get file size when {nameof(env)}.{nameof(env.FS)} is null");
            }

            var file = env.FS.GetFile(filePath.MakeAbsolute(env));

            if (!file.Exists) {
                throw new FileNotFoundException("Unable to find the specified file.", filePath.FullPath);
            }

            return file.Length;
        }
    }
}