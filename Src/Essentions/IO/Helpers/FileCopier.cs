using System;
using System.Collections.Generic;
using System.IO;

namespace Essentions.IO
{
    internal static class FileCopier
    {
        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/> or
        ///  <paramref name="targetDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">The target directory does not exist.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        public static void CopyFileToDirectory(IFileSystemEnvironment env,
                                               FilePath filePath,
                                               DirectoryPath targetDirectoryPath)
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

            CopyFile(env, filePath, targetDirectoryPath.GetFilePath(filePath));
        }

        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePath"/> or
        ///  <paramref name="targetFilePath"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">The target directory does not exist.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        public static void CopyFile(IFileSystemEnvironment env, FilePath filePath, FilePath targetFilePath)
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

            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot copy file when {nameof(env)}.{nameof(env.FS)} is null");
            }

            var targetDirectoryPath = targetFilePath.GetDirectory().MakeAbsolute(env);

            // Make sure the target directory exist.
            if (!env.FS.Exists(targetDirectoryPath)) {
                throw new InvalidOperationException($"The directory '{targetDirectoryPath.FullPath}' does not exist.");
            }

            CopyFileCore(env, filePath, targetFilePath);
        }

        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="pattern"/> or
        ///  is <see langword="null"/></exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        /// <exception cref="InvalidOperationException">The target directory does not exist.</exception>
        public static void CopyFiles(IFileSystemEnvironment env, string pattern, DirectoryPath targetDirectoryPath)
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

            CopyFiles(env, files, targetDirectoryPath);
        }

        /// <exception cref="ArgumentNullException"><paramref name="env"/> or <paramref name="filePaths"/> or
        ///  <paramref name="targetDirectoryPath"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">The target directory does not exist.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        public static void CopyFiles(IFileSystemEnvironment env, IEnumerable<FilePath> filePaths,
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
                    $"Cannot copy files when {nameof(env)}.{nameof(env.FS)} is null");
            }

            var absTargetDirectoryPath = targetDirectoryPath.MakeAbsolute(env);

            // Make sure the target directory exist.
            if (!env.FS.Exists(absTargetDirectoryPath)) {
                throw new InvalidOperationException($"The directory '{absTargetDirectoryPath.FullPath}' does not exist.");
            }

            // Iterate all files and copy them.
            foreach (var filePath in filePaths) {
                CopyFileCore(env, filePath, absTargetDirectoryPath.GetFilePath(filePath));
            }
        }

        /// <exception cref="FileNotFoundException">The file does not exist.</exception>
        private static void CopyFileCore(IFileSystemEnvironment env, FilePath filePath, FilePath targetFilePath)
        {
            if (env.FS == null) {
                throw new InvalidOperationException(
                    $"Cannot copy files when {nameof(env)}.{nameof(env.FS)} is null");
            }

            var absoluteFilePath = filePath.MakeAbsolute(env);

            // Get the file.
            if (!env.FS.Exists(absoluteFilePath)) {
                throw new FileNotFoundException($"The file '{absoluteFilePath.FullPath}' does not exist.",
                                                absoluteFilePath.FullPath);
            }

            // Copy the file.
            var absoluteTargetPath = targetFilePath.MakeAbsolute(env);
            var file = env.FS.GetFile(absoluteFilePath);
            file.Copy(absoluteTargetPath, true);
        }
    }
}