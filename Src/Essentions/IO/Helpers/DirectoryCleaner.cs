using System;

namespace Essentions.IO
{
    internal static class DirectoryCleaner
    {
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="path"/> is
        ///  <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot clean directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void Clean(IFileSystemEnvironment env, DirectoryPath path)
        {
            Clean(env, path, null);
        }

        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="path"/> is
        ///  <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot clean directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void Clean(IFileSystemEnvironment env, DirectoryPath path, Func<IFileSystemInfo, bool> predicate)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot clean directory when {nameof(env)}.{nameof(env.FS)} is null");
            }

            if (path.IsRelative) {
                path = path.MakeAbsolute(env);
            }

            // Get the root directory.
            var root = env.FS.GetDirectory(path);
            if (!root.Exists) {
                root.Create();
                return;
            }

            predicate = predicate ?? (info => true);
            CleanDirectory(root, predicate, 0);
        }

        private static bool CleanDirectory(IDirectory root, Func<IFileSystemInfo, bool> predicate, int level)
        {
            var shouldDeleteRoot = predicate(root);

            // Delete all child directories.
            var directories = root.GetDirectories("*", SearchScope.Current);
            foreach (var directory in directories) {
                if (!CleanDirectory(directory, predicate, level + 1)) {
                    // Since the child directory reported it shouldn't be
                    // removed, we should not remove the current directory either.
                    shouldDeleteRoot = false;
                }
            }

            // Delete all files in the directory.
            var files = root.GetFiles("*", SearchScope.Current);
            foreach (var file in files) {
                if (predicate(file)) {
                    file.Delete();
                } else {
                    shouldDeleteRoot = false;
                }
            }

            // Should we delete current directory?
            // Make sure it's not the initial directory.
            if (shouldDeleteRoot && level > 0) {
                root.Delete(false);
                return true;
            }

            // We did not delete this directory.
            return false;
        }
    }
}