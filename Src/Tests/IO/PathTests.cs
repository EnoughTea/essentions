using System;
using Essentions.IO;
using NUnit.Framework;

namespace Essentions.Tests.IO
{
    [TestFixture]
    public class PathTests
    {
        #region Private Test Classes

        private sealed class TestingPath : Path
        {
            public TestingPath(string path)
                : base(path)
            {
            }
        }

        #endregion

        [Test]
        public void Should_Throw_If_Path_Is_Null()
        {
            // Given, When
            var result = Assert.Catch(() => new TestingPath(null));

            // Then
            Assert.That(result, Is.TypeOf<ArgumentNullException>());
            Assert.AreEqual("path", ((ArgumentNullException)result).ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("\t ")]
        public void Should_Throw_If_Path_Is_Empty(string fullPath)
        {
            // Given, When
            var result = Assert.Catch(() => new TestingPath(fullPath));

            // Then
            Assert.That(result, Is.TypeOf<ArgumentException>());
            Assert.AreEqual("path", ((ArgumentException)result).ParamName);
            Assert.AreEqual($"Path cannot be empty.{Environment.NewLine}Parameter name: path", result.Message);
        }

        [Test]
        public void Current_Directory_Returns_Empty_Path()
        {
            // Given, When
            var path = new TestingPath("./");

            // Then
            Assert.AreEqual(string.Empty, path.FullPath);
        }

        [Test]
        public void Will_Normalize_Path_Separators()
        {
            // Given, When
            var path = new TestingPath("shaders\\basic");

            // Then
            Assert.AreEqual("shaders/basic", path.FullPath);
        }

        [Test]
        public void Will_Trim_WhiteSpace_From_Path()
        {
            // Given, When
            var path = new TestingPath(" shaders/basic ");

            // Then
            Assert.AreEqual("shaders/basic", path.FullPath);
        }

        [Test]
        public void Will_Not_Remove_WhiteSpace_Within_Path()
        {
            // Given, When
            var path = new TestingPath("my awesome shaders/basic");

            // Then
            Assert.AreEqual("my awesome shaders/basic", path.FullPath);
        }

        [Test]
        public void Should_Throw_If_Path_Contains_Illegal_Characters()
        {
            // Given
            var result = Assert.Catch(() => new TestingPath("hello/**/world.txt"));

            // Then
            Assert.That(result, Is.TypeOf<ArgumentException>());
            Assert.AreEqual("path", ((ArgumentException)result).ParamName);
            Assert.AreEqual(string.Format("Illegal characters in directory path (*).{0}Parameter name: path", Environment.NewLine), result.Message);
        }

        [Test]
        [TestCase("/Hello/World/", "/Hello/World")]
        [TestCase("\\Hello\\World\\", "/Hello/World")]
        [TestCase("file.txt/", "file.txt")]
        [TestCase("file.txt\\", "file.txt")]
        [TestCase("Temp/file.txt/", "Temp/file.txt")]
        [TestCase("Temp\\file.txt\\", "Temp/file.txt")]
        public void Should_Remove_Trailing_Slashes(string value, string expected)
        {
            // Given, When
            var path = new TestingPath(value);

            // Then
            Assert.AreEqual(expected, path.FullPath);
        }

        [Test]
        [TestCase("Hello/World")]
        [TestCase("./Hello/World/")]
        public void Should_Return_Segments_Of_Path(string pathName)
        {
            // Given
            var path = new TestingPath(pathName);

            // When, Then
            Assert.AreEqual(2, path.Segments.Length);
            Assert.AreEqual("Hello", path.Segments[0]);
            Assert.AreEqual("World", path.Segments[1]);
        }

        [Test]
        [TestCase("/Hello/World")]
        [TestCase("/Hello/World/")]
        public void Should_Return_Segments_Of_Path_And_Leave_Absolute_Directory_Separator_Intact(string pathName)
        {
            // Given
            var path = new TestingPath(pathName);

            // When, Then
            Assert.AreEqual(2, path.Segments.Length);
            Assert.AreEqual("/Hello", path.Segments[0]);
            Assert.AreEqual("World", path.Segments[1]);
        }

        [Test]
        public void Should_Return_Full_Path()
        {
            // Given, When
            const string expected = "shaders/basic";
            var path = new TestingPath(expected);

            // Then
            Assert.AreEqual(expected, path.FullPath);
        }

        [Test]
        [TestCase("assets/shaders", true)]
        [TestCase("assets/shaders/basic.frag", true)]
        [TestCase("/assets/shaders", false)]
        [TestCase("/assets/shaders/basic.frag", false)]
        public void Should_Return_Whether_Or_Not_A_Path_Is_Relative(string fullPath, bool expected)
        {
            // Given, When
            var path = new TestingPath(fullPath);

            // Then
            Assert.AreEqual(expected, path.IsRelative);
        }

        [Test]
        [TestCase("c:/assets/shaders", false)]
        [TestCase("c:/assets/shaders/basic.frag", false)]
        [TestCase("c:/", false)]
        [TestCase("c:", false)]
        public void Should_Return_Whether_Or_Not_A_Path_Is_Relative_On_Windows(string fullPath, bool expected)
        {
            // Given, When
            var path = new TestingPath(fullPath);

            // Then
            Assert.AreEqual(expected, path.IsRelative);
        }

        [Test]
        public void Should_Return_The_Full_Path()
        {
            var path = new TestingPath("temp/hello");
            Assert.That(path.ToString(), Is.EqualTo("temp/hello"));
        }
    }
}
