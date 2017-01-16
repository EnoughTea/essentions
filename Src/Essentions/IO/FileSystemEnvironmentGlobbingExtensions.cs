using System;
using System.Linq;

namespace Essentions.IO
{
    /// <summary>
    /// Contains functionality related to file system globbing.
    /// </summary>
    public static class FileSystemEnvironmentGlobbingExtensions
    {
        /// <summary>
        /// Gets all files matching the specified pattern.
        /// </summary>
        /// <example>
        /// <code>
        /// var files = GetFiles("./**/Cake.*.dll");
        /// foreach(var file in files)
        /// {
        ///     Information("File: {0}", file);
        /// }
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="pattern">The glob pattern to match.</param>
        /// <returns>A <see cref="FilePathCollection" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot get files by pattern when
        ///  <paramref name="env.Globber"/> is null.</exception>
        public static FilePathCollection GetFiles(this IFileSystemEnvironment env, string pattern)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            if (env.Globber == null) {
                throw new InvalidOperationException(
                    $"Cannot get files by pattern when {nameof(env)}.{nameof(env.Globber)} is null.");
            }

            return new FilePathCollection(env.Globber.Match(pattern).OfType<FilePath>(),
                                          new PathComparer(env.IsUnix()));
        }

        /// <summary>
        /// Gets all files matching the specified pattern.
        /// </summary>
        /// <example>
        /// <code>
        /// Func&lt;IFileSystemInfo, bool&gt; exclude_node_modules =
        /// fileSystemInfo=>!fileSystemInfo.Path.FullPath.EndsWith(
        ///                 "node_modules",
        ///                 StringComparison.OrdinalIgnoreCase);
        /// var files = GetFiles("./**/Cake.*.dll", exclude_node_modules);
        /// foreach(var file in files)
        /// {
        ///     Information("File: {0}", file);
        /// }
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="pattern">The glob pattern to match.</param>
        /// <param name="predicate">The predicate used to filter files based on file system information.</param>
        /// <returns>A <see cref="FilePathCollection" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot get files by pattern when
        ///  <paramref name="env.Globber"/> is null.</exception>
        public static FilePathCollection GetFiles(this IFileSystemEnvironment env, string pattern, Func<IFileSystemInfo, bool> predicate)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            if (env.Globber == null) {
                throw new InvalidOperationException(
                    $"Cannot get files by pattern when {nameof(env)}.{nameof(env.Globber)} is null.");
            }

            return new FilePathCollection(env.Globber.Match(pattern, predicate).OfType<FilePath>(),
                                          new PathComparer(env.IsUnix()));
        }

        /// <summary>
        /// Gets all directory matching the specified pattern.
        /// </summary>
        /// <example>
        /// <code>
        /// var directories = GetDirectories("./src/**/obj/*");
        /// foreach(var directory in directories)
        /// {
        ///     Information("Directory: {0}", directory);
        /// }
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="pattern">The glob pattern to match.</param>
        /// <returns>A <see cref="DirectoryPathCollection" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot get files by pattern when
        ///  <paramref name="env.Globber"/> is null.</exception>
        public static DirectoryPathCollection GetDirectories(this IFileSystemEnvironment env, string pattern)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            if (env.Globber == null) {
                throw new InvalidOperationException(
                    $"Cannot get files by pattern when {nameof(env)}.{nameof(env.Globber)} is null.");
            }

            return new DirectoryPathCollection(env.Globber.Match(pattern).OfType<DirectoryPath>(),
                                               new PathComparer(env.IsUnix()));
        }

        /// <summary>
        /// Gets all directory matching the specified pattern.
        /// </summary>
        /// <example>
        /// <code>
        /// Func&lt;IFileSystemInfo, bool&gt; exclude_node_modules =
        /// fileSystemInfo=>!fileSystemInfo.Path.FullPath.EndsWith(
        ///                 "node_modules",
        ///                 StringComparison.OrdinalIgnoreCase);
        /// var directories = GetDirectories("./src/**/obj/*", exclude_node_modules);
        /// foreach(var directory in directories)
        /// {
        ///     Information("Directory: {0}", directory);
        /// }
        /// </code>
        /// </example>
        /// <param name="env">The context.</param>
        /// <param name="pattern">The glob pattern to match.</param>
        /// <param name="predicate">The predicate used to filter directories based on file system information.</param>
        /// <returns>A <see cref="DirectoryPathCollection" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot get files by pattern when
        ///  <paramref name="env.Globber"/> is null.</exception>
        public static DirectoryPathCollection GetDirectories(this IFileSystemEnvironment env, string pattern, Func<IFileSystemInfo, bool> predicate)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            if (env.Globber == null) {
                throw new InvalidOperationException(
                    $"Cannot get files by pattern when {nameof(env)}.{nameof(env.Globber)} is null.");
            }

            return new DirectoryPathCollection(env.Globber.Match(pattern, predicate).OfType<DirectoryPath>(),
                                               new PathComparer(env.IsUnix()));
        }
    }
}