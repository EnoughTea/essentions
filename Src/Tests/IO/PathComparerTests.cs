using System;
using Essentions.IO;
using NUnit.Framework;

namespace Essentions.Tests.IO
{
    public sealed class PathComparerTests
    {
        public sealed class TheEqualsMethod
        {
            [Test]
            [TestCase(true)]
            [TestCase(false)]
            public void Same_Asset_Instances_Is_Considered_Equal(bool isCaseSensitive)
            {
                // Given, When
                var comparer = new PathComparer(isCaseSensitive);
                var path = new FilePath("shaders/basic.vert");

                // Then
                Assert.True(comparer.Equals(path, path));
            }

            [Test]
            [TestCase(true)]
            [TestCase(false)]
            public void Two_Null_Paths_Are_Considered_Equal(bool isCaseSensitive)
            {
                // Given
                var comparer = new PathComparer(isCaseSensitive);

                // When
                var result = comparer.Equals(null, null);

                // Then
                Assert.True(result);
            }

            [Test]
            [TestCase(true)]
            [TestCase(false)]
            public void Paths_Are_Considered_Inequal_If_Any_Is_Null(bool isCaseSensitive)
            {
                // Given
                var comparer = new PathComparer(isCaseSensitive);

                // When
                var result = comparer.Equals(null, new FilePath("test.txt"));

                // Then
                Assert.False(result);
            }


            [Test]
            [TestCase(true)]
            [TestCase(false)]
            public void Same_Paths_Are_Considered_Equal(bool isCaseSensitive)
            {
                // Given, When
                var comparer = new PathComparer(isCaseSensitive);
                var first = new FilePath("shaders/basic.vert");
                var second = new FilePath("shaders/basic.vert");

                // Then
                Assert.True(comparer.Equals(first, second));
                Assert.True(comparer.Equals(second, first));
            }

            [Test]
            [TestCase(true)]
            [TestCase(false)]
            public void Different_Paths_Are_Not_Considered_Equal(bool isCaseSensitive)
            {
                // Given, When
                var comparer = new PathComparer(isCaseSensitive);
                var first = new FilePath("shaders/basic.vert");
                var second = new FilePath("shaders/basic.frag");

                // Then
                Assert.False(comparer.Equals(first, second));
                Assert.False(comparer.Equals(second, first));
            }

            [Test]
            [TestCase(true, false)]
            [TestCase(false, true)]
            public void Same_Paths_But_Different_Casing_Are_Considered_Equal_Depending_On_Case_Sensitivity(bool isCaseSensitive, bool expected)
            {
                // Given, When
                var comparer = new PathComparer(isCaseSensitive);
                var first = new FilePath("shaders/basic.vert");
                var second = new FilePath("SHADERS/BASIC.VERT");

                // Then
                Assert.AreEqual(expected, comparer.Equals(first, second));
                Assert.AreEqual(expected, comparer.Equals(second, first));
            }
        }

        public sealed class TheGetHashCodeMethod
        {
            [Test]
            public void Should_Throw_If_Other_Path_Is_Null()
            {
                // Given
                var comparer = new PathComparer(true);

                // When
                var result = Assert.Catch(() => comparer.GetHashCode(null));

                // Then
                Assert.That(result, Is.TypeOf<ArgumentNullException>());
                Assert.That(((ArgumentNullException)result).ParamName, Is.EqualTo("obj"));
            }

            [Test]
            [TestCase(true)]
            [TestCase(false)]
            public void Same_Paths_Get_Same_Hash_Code(bool isCaseSensitive)
            {
                // Given, When
                var comparer = new PathComparer(isCaseSensitive);
                var first = new FilePath("shaders/basic.vert");
                var second = new FilePath("shaders/basic.vert");

                // Then
                Assert.AreEqual(comparer.GetHashCode(first), comparer.GetHashCode(second));
            }

            [Test]
            [TestCase(true)]
            [TestCase(false)]
            public void Different_Paths_Get_Different_Hash_Codes(bool isCaseSensitive)
            {
                // Given, When
                var comparer = new PathComparer(isCaseSensitive);
                var first = new FilePath("shaders/basic.vert");
                var second = new FilePath("shaders/basic.frag");

                // Then
                Assert.AreNotEqual(comparer.GetHashCode(first), comparer.GetHashCode(second));
            }

            [Test]
            [TestCase(true, false)]
            [TestCase(false, true)]
            public void Same_Paths_But_Different_Casing_Get_Same_Hash_Code_Depending_On_Case_Sensitivity(bool isCaseSensitive, bool expected)
            {
                // Given, When
                var comparer = new PathComparer(isCaseSensitive);
                var first = new FilePath("shaders/basic.vert");
                var second = new FilePath("SHADERS/BASIC.VERT");

                // Then
                Assert.AreEqual(expected, comparer.GetHashCode(first) == comparer.GetHashCode(second));
            }
        }

        public sealed class TheDefaultProperty
        {
            [Test]
            public void Should_Return_Correct_Comparer_Depending_On_Operative_System()
            {
                // Given
                var expected = Machine.IsUnix();

                // When
                var instance = PathComparer.Default;

                // Then
                Assert.AreEqual(expected, instance.IsCaseSensitive);
            }
        }

        public sealed class TheIsCaseSensitiveProperty
        {
            [Test]
            [TestCase(true)]
            [TestCase(false)]
            public void Should_Return_Whether_Or_Not_The_Comparer_Is_Case_Sensitive(bool isCaseSensitive)
            {
                // Given, When
                var comparer = new PathComparer(isCaseSensitive);

                // Then
                Assert.AreEqual(isCaseSensitive, comparer.IsCaseSensitive);
            }
        }
    }
}