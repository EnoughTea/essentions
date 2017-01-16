using System;
using System.Collections.Generic;
using System.Linq;
using Essentions.IO;
using NUnit.Framework;

namespace Essentions.Tests.IO
{
    public static class AssertEx
    {
        private static readonly PathComparer _comparer = new PathComparer(false);

        public static void ContainsFilePath(IEnumerable<Path> paths, FilePath expected)
        {
            ContainsPath(paths, expected);
        }

        public static void ContainsDirectoryPath(IEnumerable<Path> paths, DirectoryPath expected)
        {
            ContainsPath(paths, expected);
        }

        public static void ContainsPath<T>(IEnumerable<Path> paths, T expected)
            where T : Path
        {
            // Find the path.
            var path = paths.FirstOrDefault(x => _comparer.Equals(x, expected));

            // Assert
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Is.TypeOf<T>());
        }

        public static void IsArgumentNullException(Exception result, string paramName)
        {
            Assert.That(result, Is.TypeOf<ArgumentNullException>());
            Assert.That(((ArgumentNullException)result).ParamName, Is.EqualTo(paramName));
        }
    }
}