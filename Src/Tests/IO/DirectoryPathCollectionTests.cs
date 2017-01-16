using System.Linq;
using Essentions.IO;
using NUnit.Framework;

namespace Essentions.Tests.IO
{
    public sealed class DirectoryPathCollectionTests
    {
        public sealed class TheConstructor
        {
            [Test]
            public void Should_Throw_If_Comparer_Is_Null()
            {
                // Given, When
                var result = Assert.Catch(() => new DirectoryPathCollection(Enumerable.Empty<DirectoryPath>(), null));

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
                var collection = new DirectoryPathCollection(
                    new[] { new DirectoryPath("A.txt"), new DirectoryPath("B.txt") },
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
                    var collection = new DirectoryPathCollection(new PathComparer(false));
                    collection.Add(new DirectoryPath("B"));

                    // When
                    collection.Add(new DirectoryPath("A"));

                    // Then
                    Assert.AreEqual(2, collection.Count);
                }

                [Test]
                [TestCase(true, 2)]
                [TestCase(false, 1)]
                public void Should_Respect_File_System_Case_Sensitivity_When_Adding_Path(bool caseSensitive, int expectedCount)
                {
                    // Given
                    var collection = new DirectoryPathCollection(new PathComparer(caseSensitive));
                    collection.Add(new DirectoryPath("A"));

                    // When
                    collection.Add(new DirectoryPath("a"));

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
                    var collection = new DirectoryPathCollection(new DirectoryPath[] { "A", "B" }, new PathComparer(false));

                    // When
                    collection.Add(new DirectoryPath[] { "A", "B", "C" });

                    // Then
                    Assert.AreEqual(3, collection.Count);
                }

                [Test]
                [TestCase(true, 5)]
                [TestCase(false, 3)]
                public void Should_Respect_File_System_Case_Sensitivity_When_Adding_Paths(bool caseSensitive, int expectedCount)
                {
                    // Given
                    var collection = new DirectoryPathCollection(new DirectoryPath[] { "A", "B" }, new PathComparer(caseSensitive));

                    // When
                    collection.Add(new DirectoryPath[] { "a", "b", "c" });

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
                    var collection = new DirectoryPathCollection(new PathComparer(caseSensitive));
                    collection.Add(new DirectoryPath("A"));

                    // When
                    collection.Remove(new DirectoryPath("a"));

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
                    var collection = new DirectoryPathCollection(new DirectoryPath[] { "A", "B" }, new PathComparer(caseSensitive));

                    // When
                    collection.Remove(new DirectoryPath[] { "a", "b", "c" });

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
                    var collection = new DirectoryPathCollection(new PathComparer(false));
                    collection.Add("B");

                    // When
                    var result = collection + new DirectoryPath("A");

                    // Then
                    Assert.AreEqual(2, result.Count);
                }

                [Test]
                public void Should_Return_New_Collection()
                {
                    // Given
                    var collection = new DirectoryPathCollection(new PathComparer(false));

                    // When
                    var result = collection + new DirectoryPath("A");

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
                    var collection = new DirectoryPathCollection(comparer);
                    var second = new DirectoryPathCollection(new DirectoryPath[] { "A", "B" }, comparer);

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
                    var collection = new DirectoryPathCollection(comparer);
                    var second = new DirectoryPathCollection(new DirectoryPath[] { "A", "B" }, comparer);

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
                    var collection = new DirectoryPathCollection(comparer);
                    collection.Add("A");
                    collection.Add("B");

                    // When
                    var result = collection - new DirectoryPath("a");

                    // Then
                    Assert.AreEqual(expectedCount, result.Count);
                }

                [Test]
                public void Should_Return_New_Collection()
                {
                    // Given
                    var collection = new DirectoryPathCollection(new PathComparer(false));
                    collection.Add("A");
                    collection.Add("B");

                    // When
                    var result = collection - new DirectoryPath("A");

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
                    var collection = new DirectoryPathCollection(new PathComparer(caseSensitive));
                    collection.Add("A");
                    collection.Add("B");
                    collection.Add("C");

                    // When
                    var result = collection - new[] { new DirectoryPath("b"), new DirectoryPath("c") };

                    // Then
                    Assert.AreEqual(expectedCount, result.Count);
                }

                [Test]
                public void Should_Return_New_Collection()
                {
                    // Given
                    var collection = new DirectoryPathCollection(new PathComparer(false));
                    collection.Add("A");
                    collection.Add("B");
                    collection.Add("C");

                    // When
                    var result = collection - new[] { new DirectoryPath("B"), new DirectoryPath("C") };

                    // Then
                    Assert.False(ReferenceEquals(result, collection));
                }
            }
        }
    }
}