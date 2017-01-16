using System;
using System.Linq;
using Essentions.IO;
using NUnit.Framework;

namespace Essentions.Tests.IO
{
    public sealed class FilePathCollectionTests
    {
        public sealed class TheConstructor
        {
            [Test]
            public void Should_Throw_If_Comparer_Is_Null()
            {
                // Given, When
                var result = Assert.Catch(() => new FilePathCollection(Enumerable.Empty<FilePath>(), null));

                // Then
                AssertEx.IsArgumentNullException(result, "comparer");
            }
        }

        public sealed class TheCountProperty
        {
            [Test]
            public void Should_Return_The_Number_Of_Paths_In_The_Collection()
            {
                // Given
                var collection = new FilePathCollection(
                    new[] { new FilePath("A.txt"), new FilePath("B.txt") },
                    new PathComparer(false));

                // When, Then
                Assert.AreEqual(2, collection.Count);
            }
        }

        public sealed class TheAddMethod
        {
            public sealed class WithSinglePath
            {
                [Test]
                public void Should_Add_Path_If_Not_Already_Present()
                {
                    // Given
                    var collection = new FilePathCollection(new PathComparer(false));
                    collection.Add(new FilePath("B.txt"));

                    // When
                    collection.Add(new FilePath("A.txt"));

                    // Then
                    Assert.AreEqual(2, collection.Count);
                }

                [Test]
                [TestCase(true, 2)]
                [TestCase(false, 1)]
                public void Should_Respect_File_System_Case_Sensitivity_When_Adding_Path(bool caseSensitive, int expectedCount)
                {
                    // Given
                    var collection = new FilePathCollection(new PathComparer(caseSensitive));
                    collection.Add(new FilePath("A.TXT"));

                    // When
                    collection.Add(new FilePath("a.txt"));

                    // Then
                    Assert.AreEqual(expectedCount, collection.Count);
                }
            }

            public sealed class WithMultiplePaths
            {
                [Test]
                public void Should_Add_Paths_That_Are_Not_Present()
                {
                    // Given
                    var collection = new FilePathCollection(new FilePath[] { "A.TXT", "B.TXT" }, new PathComparer(false));

                    // When
                    collection.Add(new FilePath[] { "A.TXT", "B.TXT", "C.TXT" });

                    // Then
                    Assert.AreEqual(3, collection.Count);
                }

                [Test]
                [TestCase(true, 5)]
                [TestCase(false, 3)]
                public void Should_Respect_File_System_Case_Sensitivity_When_Adding_Paths(bool caseSensitive, int expectedCount)
                {
                    // Given
                    var collection = new FilePathCollection(new FilePath[] { "A.TXT", "B.TXT" }, new PathComparer(caseSensitive));

                    // When
                    collection.Add(new FilePath[] { "a.txt", "b.txt", "c.txt" });

                    // Then
                    Assert.AreEqual(expectedCount, collection.Count);
                }
            }
        }

        public sealed class TheRemoveMethod
        {
            public sealed class WithSinglePath
            {
                [Test]
                [TestCase(true, 1)]
                [TestCase(false, 0)]
                public void Should_Respect_File_System_Case_Sensitivity_When_Removing_Path(bool caseSensitive, int expectedCount)
                {
                    // Given
                    var collection = new FilePathCollection(new PathComparer(caseSensitive));
                    collection.Add(new FilePath("A.TXT"));

                    // When
                    collection.Remove(new FilePath("a.txt"));

                    // Then
                    Assert.AreEqual(expectedCount, collection.Count);
                }
            }

            public sealed class WithMultiplePaths
            {
                [Test]
                [TestCase(true, 2)]
                [TestCase(false, 0)]
                public void Should_Respect_File_System_Case_Sensitivity_When_Removing_Paths(bool caseSensitive, int expectedCount)
                {
                    // Given
                    var collection = new FilePathCollection(new FilePath[] { "A.TXT", "B.TXT" }, new PathComparer(caseSensitive));

                    // When
                    collection.Remove(new FilePath[] { "a.txt", "b.txt", "c.txt" });

                    // Then
                    Assert.AreEqual(expectedCount, collection.Count);
                }
            }
        }

        public sealed class ThePlusOperator
        {
            public sealed class WithSinglePath
            {
                [Test]
                public void Should_Respect_File_System_Case_Sensitivity_When_Adding_Path()
                {
                    // Given
                    var collection = new FilePathCollection(new PathComparer(false));
                    collection.Add("B.txt");

                    // When
                    var result = collection + new FilePath("A.txt");

                    // Then
                    Assert.AreEqual(2, result.Count);
                }

                [Test]
                public void Should_Return_New_Collection()
                {
                    // Given
                    var collection = new FilePathCollection(new PathComparer(false));

                    // When
                    var result = collection + new FilePath("A.txt");

                    // Then
                    Assert.False(ReferenceEquals(result, collection));
                }
            }

            public sealed class WithMultiplePaths
            {
                [Test]
                public void Should_Respect_File_System_Case_Sensitivity_When_Adding_Paths()
                {
                    // Given
                    var comparer = new PathComparer(false);
                    var collection = new FilePathCollection(comparer);
                    var second = new FilePathCollection(new FilePath[] { "A.txt", "B.txt" }, comparer);

                    // When
                    var result = collection + second;

                    // Then
                    Assert.AreEqual(2, result.Count);
                }

                [Test]
                public void Should_Return_New_Collection()
                {
                    // Given
                    var comparer = new PathComparer(false);
                    var collection = new FilePathCollection(comparer);
                    var second = new FilePathCollection(new FilePath[] { "A.txt", "B.txt" }, comparer);

                    // When
                    var result = collection + second;

                    // Then
                    Assert.False(ReferenceEquals(result, collection));
                }
            }
        }

        public sealed class TheMinusOperator
        {
            public sealed class WithSinglePath
            {
                [Test]
                [TestCase(true, 2)]
                [TestCase(false, 1)]
                public void Should_Respect_File_System_Case_Sensitivity_When_Removing_Paths(bool caseSensitive, int expectedCount)
                {
                    // Given
                    var comparer = new PathComparer(caseSensitive);
                    var collection = new FilePathCollection(comparer);
                    collection.Add("A.txt");
                    collection.Add("B.txt");

                    // When
                    var result = collection - new FilePath("a.txt");

                    // Then
                    Assert.AreEqual(expectedCount, result.Count);
                }

                [Test]
                public void Should_Return_New_Collection()
                {
                    // Given
                    var collection = new FilePathCollection(new PathComparer(false));
                    collection.Add("A.txt");
                    collection.Add("B.txt");

                    // When
                    var result = collection - new FilePath("A.txt");

                    // Then
                    Assert.False(ReferenceEquals(result, collection));
                }
            }

            public sealed class WithMultiplePaths
            {
                [Test]
                [TestCase(true, 3)]
                [TestCase(false, 1)]
                public void Should_Respect_File_System_Case_Sensitivity_When_Removing_Paths(bool caseSensitive, int expectedCount)
                {
                    // Given
                    var collection = new FilePathCollection(new PathComparer(caseSensitive));
                    collection.Add("A.txt");
                    collection.Add("B.txt");
                    collection.Add("C.txt");

                    // When
                    var result = collection - new[] { new FilePath("b.txt"), new FilePath("c.txt") };

                    // Then
                    Assert.AreEqual(expectedCount, result.Count);
                }

                [Test]
                public void Should_Return_New_Collection()
                {
                    // Given
                    var collection = new FilePathCollection(new PathComparer(false));
                    collection.Add("A.txt");
                    collection.Add("B.txt");
                    collection.Add("C.txt");

                    // When
                    var result = collection - new[] { new FilePath("B.txt"), new FilePath("C.txt") };

                    // Then
                    Assert.False(ReferenceEquals(result, collection));
                }
            }
        }
    }

    public sealed class PathCollapserTests
    {
        public sealed class TheCollapseMethod
        {
            [Test]
            public void Should_Throw_If_Path_Is_Null()
            {
                // Given, When
                var result = Assert.Catch(() => PathCollapser.Collapse(null));

                // Then
                Assert.That(result, Is.TypeOf<ArgumentNullException>());
                Assert.That(((ArgumentNullException)result).ParamName, Is.EqualTo("path"));
            }

            [Test]
            public void Should_Collapse_Relative_Path()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("hello/temp/test/../../world"));

                // Then
                Assert.AreEqual("hello/world", path);
            }

            [Test]
            public void Should_Collapse_Path_With_Separated_Ellipsis()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("hello/temp/../temp2/../world"));

                // Then
                Assert.AreEqual("hello/world", path);
            }

            [Test]
            public void Should_Collapse_Path_With_Windows_Root()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("c:/hello/temp/test/../../world"));

                // Then
                Assert.AreEqual("c:/hello/world", path);
            }

            [Test]
            public void Should_Collapse_Path_With_Non_Windows_Root()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("/hello/temp/test/../../world"));

                // Then
                Assert.AreEqual("/hello/world", path);
            }

            [Test]
            public void Should_Stop_Collapsing_When_Windows_Root_Is_Reached()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("c:/../../../../../../temp"));

                // Then
                Assert.AreEqual("c:/temp", path);
            }

            [Test]
            public void Should_Stop_Collapsing_When_Root_Is_Reached()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("/hello/../../../../../../temp"));

                // Then
                Assert.AreEqual("/temp", path);
            }

            [Test]
            [TestCase(".")]
            [TestCase("./")]
            [TestCase("/.")]
            public void Should_Collapse_Single_Dot_To_Single_Dot(string uncollapsedPath)
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath(uncollapsedPath));

                // Then
                Assert.AreEqual(".", path);
            }

            [Test]
            public void Should_Collapse_Single_Dot_With_Ellipsis()
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath("./.."));

                // Then
                Assert.AreEqual(".", path);
            }

            [Test]
            [TestCase("./a", "a")]
            [TestCase("a/./b", "a/b")]
            [TestCase("/a/./b", "/a/b")]
            [TestCase("a/b/.", "a/b")]
            [TestCase("/a/b/.", "/a/b")]
            [TestCase("/./a/b", "/a/b")]
            public void Should_Collapse_Single_Dot(string uncollapsedPath, string collapsedPath)
            {
                // Given, When
                var path = PathCollapser.Collapse(new DirectoryPath(uncollapsedPath));

                // Then
                Assert.AreEqual(collapsedPath, path);
            }
        }
    }
}