using System;
using System.Collections.Generic;
using System.IO;

namespace Essentions.IO
{
    internal static class FileDeleter
    {
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="pattern"/>
        ///  is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="InvalidOperationException">Cannot delete files when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void DeleteFiles(IFileSystemEnvironment env, string pattern)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (pattern == null) {
                throw new ArgumentNullException(nameof(pattern));
            }

            var files = env.GetFiles(pattern);
            if (files.Count == 0) {
                return;
            }

            DeleteFiles(env, files);
        }

        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePaths"/>
        ///  is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="InvalidOperationException">Cannot delete files when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void DeleteFiles(IFileSystemEnvironment env, IEnumerable<FilePath> filePaths)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (filePaths == null) {
                throw new ArgumentNullException(nameof(filePaths));
            }

            foreach (var filePath in filePaths) {
                DeleteFile(env, filePath);
            }
        }

        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/>
        ///  is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file <paramref name="filePath"/> does not exist.</exception>
        /// <exception cref="InvalidOperationException">Cannot delete files when <paramref name="env.FS"/> is null.
        /// </exception>
        public static void DeleteFile(IFileSystemEnvironment env, FilePath filePath)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (filePath == null) {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot delete files when {nameof(env)}.{nameof(env.FS)} is null");
            }

            filePath = filePath.MakeAbsolute(env);

            var file = env.FS.GetFile(filePath);
            if (!file.Exists) {
                throw new FileNotFoundException($"The file '{filePath.FullPath}' does not exist.", filePath.FullPath);
            }

            file.Delete();
        }
    }
}