using System;
using System.IO;
using System.Linq;

namespace Essentions.IO
{
    internal static class DirectoryDeleter
    {
        /// <exception cref="IOException">The directory <paramref name="path.FullPath"/> does not exist. -or-
        ///  Cannot delete directory <paramref name="path.FullPath"/> without recursion since it's not empty.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot delete directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void Delete(IFileSystemEnvironment env, DirectoryPath path, bool recursive)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot delete directory when {nameof(env)}.{nameof(env.FS)} is null");
            }

            if (path.IsRelative) {
                path = path.MakeAbsolute(env);
            }

            var directory = env.FS.GetDirectory(path);
            if (!directory.Exists) {
                throw new IOException($"The directory '{path.FullPath}' does not exist.");
            }

            var hasDirectories = directory.GetDirectories("*", SearchScope.Current).Any();
            var hasFiles = directory.GetFiles("*", SearchScope.Current).Any();
            if (!recursive && (hasDirectories || hasFiles)) {
                throw new IOException($"Cannot delete directory '{path.FullPath}' " +
                                      "without recursion since it's not empty.");
            }

            directory.Delete(recursive);
        }
    }
}