using System;

namespace Essentions.IO
{
    internal static class DirectoryCreator
    {
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="path"/> is
        ///  <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Cannot create directory when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void Create(IFileSystemEnvironment env, DirectoryPath path)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot create directory when {nameof(env)}.{nameof(env.FS)} is null");
            }

            if (path.IsRelative) {
                path = path.MakeAbsolute(env);
            }

            var directory = env.FS.GetDirectory(path);
            if (!directory.Exists) {
                directory.Create();
            }
        }
    }
}