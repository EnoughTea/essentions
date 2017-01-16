using System;
using System.Collections.Generic;
using System.Linq;

namespace Essentions.IO
{
    /// <summary>
    /// Contains functionality related to directory operations.
    /// </summary>
    public static class FileSystemEnvironmentDirectoryExtensions
    {
        /// <summary>
        /// Gets a directory path from string.
        /// </summary>
        /// <example>
        /// <code>
        /// // Get the temp directory.
        /// var root = Directory("./");
        /// var temp = root + Directory("temp");
        /// // Clean the directory.
        /// CleanDirectory(temp);
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="path">The path.</param>
        /// <returns>A directory path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or
        ///  <paramref name="path"/> is <see langword="null"/></exception>
        public static ConvertableDirectoryPath Directory(this IFileSystemEnvironment env, string path)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            return new ConvertableDirectoryPath(new DirectoryPath(path));
        }

        /// <summary>
        /// Deletes the specified directories.
        /// </summary>
        /// <example>
        /// <code>
        /// var directoriesToDelete = new DirectoryPath[]{
        ///     Directory("be"),
        ///     Directory("gone")
        /// };
        /// DeleteDirectories(directoriesToDelete, recursive:true);
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="directories">The directory paths.</param>
        /// <param name="recursive">Will perform a recursive delete if set to <c>true</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directories"/> is <see langword="null"/></exception>
        /// <exception cref="System.IO.IOException">The directory does not exist. -or-
        ///  Cannot delete directory without recursion since it's not empty.
        /// </exception>
        public static void DeleteDirectories(this IFileSystemEnvironment env,
                                             IEnumerable<DirectoryPath> directories, bool recursive = false)
        {
            if (directories == null) {
                throw new ArgumentNullException(nameof(directories));
            }

            foreach (var directory in directories) {
                DeleteDirectory(env, directory, recursive);
            }
        }

        /// <summary>
        /// Deletes the specified directories.
        /// </summary>
        /// <example>
        /// <code>
        /// var directoriesToDelete = new []{
        ///     "be",
        ///     "gone"
        /// };
        /// DeleteDirectories(directoriesToDelete, recursive:true);
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="directories">The directory paths.</param>
        /// <param name="recursive">Will perform a recursive delete if set to <c>true</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directories"/> is <see langword="null"/></exception>
        /// <exception cref="System.IO.IOException">The directory does not exist. -or-
        ///  Cannot delete directory without recursion since it's not empty.
        /// </exception>
        public static void DeleteDirectories(this IFileSystemEnvironment env, IEnumerable<string> directories,
                                             bool recursive = false)
        {
            if (directories == null) {
                throw new ArgumentNullException(nameof(directories));
            }

            var paths = directories.Select(p => new DirectoryPath(p));
            foreach (var directory in paths) {
                DeleteDirectory(env, directory, recursive);
            }
        }

        /// <summary>
        /// Deletes the specified directory.
        /// </summary>
        /// <example>
        /// <code>
        /// DeleteDirectory("./be/gone", recursive:true);
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="path">The directory path.</param>
        /// <param name="recursive">Will perform a recursive delete if set to <c>true</c>.</param>
        /// <exception cref="System.IO.IOException">The directory <paramref name="path.FullPath"/> does not exist. -or-
        ///  Cannot delete directory <paramref name="path.FullPath"/> without recursion since it's not empty.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="path "/>is
        ///  <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot delete directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void DeleteDirectory(this IFileSystemEnvironment env, DirectoryPath path,
                                           bool recursive = false)
        {
            DirectoryDeleter.Delete(env, path, recursive);
        }

        /// <summary>
        /// Cleans the directories matching the specified pattern.
        /// Cleaning the directory will remove all it's content but not the directory itself.
        /// </summary>
        /// <example>
        /// <code>
        /// CleanDirectories("./src/**/bin/debug");
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <exception cref="InvalidOperationException">Cannot clean directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void CleanDirectories(this IFileSystemEnvironment env, string pattern)
        {
            var directories = env.GetDirectories(pattern);
            if (directories.Count == 0) {
                return;
            }

            CleanDirectories(env, directories);
        }

        /// <summary>
        /// Cleans the directories matching the specified pattern.
        /// Cleaning the directory will remove all it's content but not the directory itself.
        /// </summary>
        /// <example>
        /// <code>
        /// Func&lt;IFileSystemInfo, bool&gt; exclude_node_modules =
        /// fileSystemInfo=>!fileSystemInfo.Path.FullPath.EndsWith(
        ///                 "node_modules",
        ///                 StringComparison.OrdinalIgnoreCase);
        /// CleanDirectories("./src/**/bin/debug", exclude_node_modules);
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="predicate">The predicate used to filter directories based on file system information.</param>
        /// <exception cref="InvalidOperationException">Cannot clean directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void CleanDirectories(this IFileSystemEnvironment env, string pattern,
                                            Func<IFileSystemInfo, bool> predicate)
        {
            var directories = env.GetDirectories(pattern, predicate);
            if (directories.Count == 0) {
                return;
            }

            CleanDirectories(env, directories);
        }

        /// <summary>
        /// Cleans the specified directories.
        /// Cleaning a directory will remove all it's content but not the directory itself.
        /// </summary>
        /// <example>
        /// <code>
        /// var directoriesToClean = GetDirectories("./src/**/bin/");
        /// CleanDirectories(directoriesToClean);
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="directories">The directory paths.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directories"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="InvalidOperationException">Cannot clean directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void CleanDirectories(this IFileSystemEnvironment env, IEnumerable<DirectoryPath> directories)
        {
            if (directories == null) {
                throw new ArgumentNullException(nameof(directories));
            }
            foreach (var directory in directories) {
                CleanDirectory(env, directory);
            }
        }

        /// <summary>
        /// Cleans the specified directories.
        /// Cleaning a directory will remove all it's content but not the directory itself.
        /// </summary>
        /// <example>
        /// <code>
        /// var directoriesToClean = new []{
        ///     "./src/Cake/obj",
        ///     "./src/Cake.Common/obj"
        /// };
        /// CleanDirectories(directoriesToClean);
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="directories">The directory paths.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directories"/> is <see langword="null"/></exception>
        public static void CleanDirectories(this IFileSystemEnvironment env, IEnumerable<string> directories)
        {
            if (directories == null) {
                throw new ArgumentNullException(nameof(directories));
            }
            var paths = directories.Select(p => new DirectoryPath(p));
            foreach (var directory in paths) {
                CleanDirectory(env, directory);
            }
        }

        /// <summary>
        /// Cleans the specified directory.
        /// </summary>
        /// <example>
        /// <code>
        /// CleanDirectory("./src/Cake.Common/obj");
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="path">The directory path.</param>
        /// <exception cref="ArgumentNullException">env or path is
        ///  <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot clean directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void CleanDirectory(this IFileSystemEnvironment env, DirectoryPath path)
        {
            DirectoryCleaner.Clean(env, path);
        }

        /// <summary>
        /// Cleans the specified directory.
        /// </summary>
        /// <example>
        /// <code>
        /// CleanDirectory("./src/Cake.Common/obj", fileSystemInfo=>!fileSystemInfo.Hidden);
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="path">The directory path.</param>
        /// <param name="predicate">Predicate used to determine which files/directories should get deleted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="path"/>
        ///  or <paramref name="predicate"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot clean directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void CleanDirectory(this IFileSystemEnvironment env, DirectoryPath path,
                                          Func<IFileSystemInfo, bool> predicate)
        {
            DirectoryCleaner.Clean(env, path, predicate);
        }

        /// <summary>
        /// Creates the specified directory.
        /// </summary>
        /// <example>
        /// <code>
        /// CreateDirectory("publish");
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="path">The directory path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="path"/> is
        ///  <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot create directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void CreateDirectory(this IFileSystemEnvironment env, DirectoryPath path)
        {
            DirectoryCreator.Create(env, path);
        }

        /// <summary>
        /// Copies the contents of a directory to the specified location.
        /// </summary>
        /// <example>
        /// <code>
        /// CopyDirectory("source_path", "destination_path");
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="source">The source directory path.</param>
        /// <param name="destination">The destination directory path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="source"/>
        ///  or <paramref name="destination"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Source directory does not exist or could not be found.</exception>
        public static void CopyDirectory(this IFileSystemEnvironment env, DirectoryPath source,
                                         DirectoryPath destination)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null) {
                throw new ArgumentNullException(nameof(destination));
            }

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot check if directory exists when {nameof(env)}.{nameof(env.FS)} is null");
            }

            if (source.IsRelative) {
                source = source.MakeAbsolute(env);
            }

            // Get the subdirectories for the specified directory.
            var sourceDir = env.FS.GetDirectory(source);
            if (!sourceDir.Exists) {
                throw new InvalidOperationException("Source directory does not exist or could not be found: "
                                                    + source.FullPath);
            }

            var dirs = sourceDir.GetDirectories("*", SearchScope.Current);

            var destinationDir = env.FS.GetDirectory(destination);
            if (!destinationDir.Exists) {
                destinationDir.Create();
            }

            // Get the files in the directory and copy them to the new location.
            var files = sourceDir.GetFiles("*", SearchScope.Current);
            foreach (var file in files) {
                var temppath = destinationDir.Path.CombineWithFilePath(file.Path.GetFilename());
                file.Copy(temppath, true);
            }

            // Copy all of the subdirectories
            foreach (var subdir in dirs) {
                var temppath = destination.Combine(subdir.Path.GetDirectoryName());
                CopyDirectory(env, subdir.Path, temppath);
            }
        }

        /// <summary>
        /// Determines whether the given path refers to an existing directory.
        /// </summary>
        /// <example>
        /// <code>
        /// var dir = "publish";
        /// if (!DirectoryExists(dir))
        /// {
        ///     CreateDirectory(dir);
        /// }
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="path">The <see cref="DirectoryPath"/> to check.</param>
        /// <returns><c>true</c> if <paramref name="path"/> refers to an existing directory;
        /// <c>false</c> if the directory does not exist or an error occurs when trying to
        /// determine if the specified path exists.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="path"/> is
        ///  <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot check if directory exists when
        ///  <paramref name="env.FS"/> is null.</exception>
        public static bool DirectoryExists(this IFileSystemEnvironment env, DirectoryPath path)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot check if directory exists when {nameof(env)}.{nameof(env.FS)} is null");
            }

            return env.FS.GetDirectory(path).Exists;
        }

        /// <summary>
        /// Makes the path absolute (if relative) using the current working directory.
        /// </summary>
        /// <example>
        /// <code>
        /// var path = MakeAbsolute(Directory("./resources"));
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="path">The path.</param>
        /// <returns>An absolute directory path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="path"/> is
        ///  <see langword="null"/></exception>
        public static DirectoryPath MakeAbsolute(this IFileSystemEnvironment env, DirectoryPath path)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            return path.MakeAbsolute(env);
        }
    }
}