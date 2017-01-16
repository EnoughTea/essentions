using System;
using System.Collections.Generic;
using System.IO;

namespace Essentions.IO
{
    internal static class FileMover
    {
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/> or
        ///  <paramref name="targetDirectoryPath"/>is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The target directory do not exist..</exception>
        public static void MoveFileToDirectory(IFileSystemEnvironment env, FilePath filePath, DirectoryPath targetDirectoryPath)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (filePath == null) {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (targetDirectoryPath == null) {
                throw new ArgumentNullException(nameof(targetDirectoryPath));
            }

            MoveFile(env, filePath, targetDirectoryPath.GetFilePath(filePath));
        }

        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="pattern"/> or
        ///  is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The target directory do not exist.</exception>
        public static void MoveFiles(IFileSystemEnvironment env, string pattern, DirectoryPath targetDirectoryPath)
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

            MoveFiles(env, files, targetDirectoryPath);
        }

        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePaths"/> or
        ///  <paramref name="targetDirectoryPath"/>is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">The directory <paramref name="targetDirectoryPath.FullPath"/>
        ///  do not exist.</exception>
        /// <exception cref="FileNotFoundException">The target directory do not exist.</exception>
        public static void MoveFiles(this IFileSystemEnvironment env, IEnumerable<FilePath> filePaths,
            DirectoryPath targetDirectoryPath)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (filePaths == null) {
                throw new ArgumentNullException(nameof(filePaths));
            }
            if (targetDirectoryPath == null) {
                throw new ArgumentNullException(nameof(targetDirectoryPath));
            }

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot move files when {nameof(env)}.{nameof(env.FS)} is null");
            }

            targetDirectoryPath = targetDirectoryPath.MakeAbsolute(env);

            // Make sure the target directory exist.
            if (!env.FS.Exists(targetDirectoryPath)) {
                throw new InvalidOperationException($"The directory '{targetDirectoryPath.FullPath}' do not exist.");
            }

            // Iterate all files and copy them.
            foreach (var filePath in filePaths) {
                MoveFile(env, filePath, targetDirectoryPath.GetFilePath(filePath));
            }
        }

        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/> or
        ///  <paramref name="targetFilePath"/>is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">The file <paramref name="filePath.FullPath"/> do not exist.
        /// </exception>
        /// <exception cref="FileNotFoundException">The target directory do not exist..</exception>
        public static void MoveFile(IFileSystemEnvironment env, FilePath filePath, FilePath targetFilePath)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }
            if (filePath == null) {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (targetFilePath == null) {
                throw new ArgumentNullException(nameof(targetFilePath));
            }

            filePath = filePath.MakeAbsolute(env);
            targetFilePath = targetFilePath.MakeAbsolute(env);

            // Make sure the target directory exist.
            var targetDirectoryPath = targetFilePath.GetDirectory().MakeAbsolute(env);
            if (!env.FS.Exists(targetDirectoryPath)) {
                throw new InvalidOperationException($"The directory '{targetDirectoryPath.FullPath}' do not exist.");
            }

            // Get the file and verify it exist.
            var file = env.FS.GetFile(filePath);
            if (!file.Exists) {
                throw new FileNotFoundException($"The file '{filePath.FullPath}' do not exist.", filePath.FullPath);
            }

            // Move the file.
            file.Move(targetFilePath.MakeAbsolute(env));
        }
    }
}