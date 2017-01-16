using System;
using Essentions.IO;
using NSubstitute;
using NUnit.Framework;

namespace Essentions.Tests.IO
{
    public sealed class DirectoryPathTests
    {
        public sealed class TheGetDirectoryNameMethod
        {
            [Test]
            [TestCase("C:/Data", "Data")]
            [TestCase("C:/Data/Work", "Work")]
            [TestCase("C:/Data/Work/file.txt", "file.txt")]
            public void Should_Return_Directory_Name(string directoryPath, string name)
            {
                // Given
                var path = new DirectoryPath(directoryPath);

                // When
                var result = path.GetDirectoryName();

                // Then
                Assert.AreEqual(name, result);
            }
        }

        public sealed class TheGetFilePathMethod
        {
            [Test]
            public void Should_Throw_If_Path_Is_Null()
            {
                // Given
                var path = new DirectoryPath("assets");

                // When
                var result = Assert.Catch(() => path.GetFilePath(null));

                // Then
                AssertEx.IsArgumentNullException(result, "path");
            }

            [Test]
#if !UNIX
            [TestCase("c:/assets/shaders/", "simple.frag", "c:/assets/shaders/simple.frag")]
            [TestCase("c:/", "simple.frag", "c:/simple.frag")]
            [TestCase("c:/assets/shaders/", "test/simple.frag", "c:/assets/shaders/simple.frag")]
            [TestCase("c:/", "test/simple.frag", "c:/simple.frag")]
#endif
            [TestCase("assets/shaders", "simple.frag", "assets/shaders/simple.frag")]
            [TestCase("assets/shaders/", "simple.frag", "assets/shaders/simple.frag")]
            [TestCase("/assets/shaders/", "simple.frag", "/assets/shaders/simple.frag")]
            [TestCase("assets/shaders", "test/simple.frag", "assets/shaders/simple.frag")]
            [TestCase("assets/shaders/", "test/simple.frag", "assets/shaders/simple.frag")]
            [TestCase("/assets/shaders/", "test/simple.frag", "/assets/shaders/simple.frag")]
            public void Should_Combine_Paths(string first, string second, string expected)
            {
                // Given
                var path = new DirectoryPath(first);

                // When
                var result = path.GetFilePath(new FilePath(second));

                // Then
                Assert.AreEqual(expected, result.FullPath);
            }
        }

        public sealed class TheCombineWithFilePathMethod
        {
            [Test]
            public void Should_Throw_If_Path_Is_Null()
            {
                // Given
                var path = new DirectoryPath("assets");

                // When
                var result = Assert.Catch(() => path.CombineWithFilePath(null));

                // Then
                AssertEx.IsArgumentNullException(result, "path");
            }

            [Test]
#if !UNIX
            [TestCase("c:/assets/shaders/", "simple.frag", "c:/assets/shaders/simple.frag")]
            [TestCase("c:/", "simple.frag", "c:/simple.frag")]
            [TestCase("c:/assets/shaders/", "test/simple.frag", "c:/assets/shaders/test/simple.frag")]
            [TestCase("c:/", "test/simple.frag", "c:/test/simple.frag")]
#endif
            [TestCase("assets/shaders", "simple.frag", "assets/shaders/simple.frag")]
            [TestCase("assets/shaders/", "simple.frag", "assets/shaders/simple.frag")]
            [TestCase("/assets/shaders/", "simple.frag", "/assets/shaders/simple.frag")]
            [TestCase("assets/shaders", "test/simple.frag", "assets/shaders/test/simple.frag")]
            [TestCase("assets/shaders/", "test/simple.frag", "assets/shaders/test/simple.frag")]
            [TestCase("/assets/shaders/", "test/simple.frag", "/assets/shaders/test/simple.frag")]
            public void Should_Combine_Paths(string first, string second, string expected)
            {
                // Given
                var path = new DirectoryPath(first);

                // When
                var result = path.CombineWithFilePath(new FilePath(second));

                // Then
                Assert.AreEqual(expected, result.FullPath);
            }

            [Test]
            public void Can_Not_Combine_Directory_Path_With_Absolute_File_Path()
            {
                // Given
                var path = new DirectoryPath("assets");

                // When
                var result = Assert.Catch(() => path.CombineWithFilePath(new FilePath("/other/asset.txt")));

                // Then
                Assert.That(result, Is.TypeOf<InvalidOperationException>());
                Assert.AreEqual("Cannot combine a directory path with an absolute file path.", result.Message);
            }
        }

        public sealed class TheCombineWithDirectoryPathMethod
        {
            [Test]
#if !UNIX
            [TestCase("c:/assets/shaders/", "simple", "c:/assets/shaders/simple")]
            [TestCase("c:/", "simple", "c:/simple")]
#endif
            [TestCase("assets/shaders", "simple", "assets/shaders/simple")]
            [TestCase("assets/shaders/", "simple", "assets/shaders/simple")]
            [TestCase("/assets/shaders/", "simple", "/assets/shaders/simple")]
            public void Should_Combine_Paths(string first, string second, string expected)
            {
                // Given
                var path = new DirectoryPath(first);

                // When
                var result = path.Combine(new DirectoryPath(second));

                // Then
                Assert.AreEqual(expected, result.FullPath);
            }

            [Test]
            public void Should_Throw_If_Path_Is_Null()
            {
                // Given
                var path = new DirectoryPath("assets");

                // When
                var result = Assert.Catch(() => path.Combine(null));

                // Then
                AssertEx.IsArgumentNullException(result, "path");
            }

            [Test]
            public void Can_Not_Combine_Directory_Path_With_Absolute_Directory_Path()
            {
                // Given
                var path = new DirectoryPath("assets");

                // When
                var result = Assert.Catch(() => path.Combine(new DirectoryPath("/other/assets")));

                // Then
                Assert.That(result, Is.TypeOf<InvalidOperationException>());
                Assert.AreEqual("Cannot combine a directory path with an absolute directory path.", result.Message);
            }
        }

        public sealed class TheMakeAbsoluteMethod
        {
            public sealed class ThatTakesAnEnvironment
            {
                [Test]
                public void Should_Throw_If_Provided_Environment_Is_Null()
                {
                    // Given
                    var path = new DirectoryPath("assets");

                    // When
                    var result = Assert.Catch(
                        () => path.MakeAbsolute((IFileSystemEnvironment)null));

                    // Then
                    AssertEx.IsArgumentNullException(result, "environment");
                }

                [Test]
                public void Should_Create_New_Absolute_Path_When_Path_Is_Relative()
                {
                    // Given
                    var environment = Substitute.For<IFileSystemEnvironment>();
                    environment.WorkingDirectory.Returns("/Working");
                    var path = new DirectoryPath("assets");

                    // When
                    var result = path.MakeAbsolute(environment);

                    // Then
                    Assert.AreEqual("/Working/assets", result.FullPath);
                }

                [Test]
                public void Should_Create_New_Absolute_Path_Identical_To_The_Path()
                {
                    // Given
                    var environment = Substitute.For<IFileSystemEnvironment>();
                    var path = new DirectoryPath("/assets");

                    // When
                    var result = path.MakeAbsolute(environment);

                    // Then
                    Assert.AreEqual("/assets", result.FullPath);
                }
            }

            public sealed class ThatTakesAnotherDirectoryPath
            {
                [Test]
                public void Should_Throw_If_Provided_Path_Is_Null()
                {
                    // Given
                    var path = new DirectoryPath("assets");

                    // When
                    var result = Assert.Catch(
                        () => path.MakeAbsolute((DirectoryPath)null));

                    // Then
                    AssertEx.IsArgumentNullException(result, "path");
                }

                [Test]
                public void Should_Throw_If_Provided_Path_Is_Relative()
                {
                    // Given
                    var path = new DirectoryPath("assets");

                    // When
                    var result = Assert.Catch(() => path.MakeAbsolute("Working"));

                    // Then
                    Assert.That(result, Is.TypeOf<InvalidOperationException>());
                    Assert.AreEqual("The provided path cannot be relative.", result.Message);
                }

                [Test]
                public void Should_Create_New_Absolute_Path_When_Path_Is_Relative()
                {
                    // Given
                    var path = new DirectoryPath("assets");

                    // When
                    var result = path.MakeAbsolute("/absolute");

                    // Then
                    Assert.AreEqual("/absolute/assets", result.FullPath);
                }

                [Test]
                public void Should_Create_New_Absolute_Path_Identical_To_The_Path()
                {
                    // Given
                    var path = new DirectoryPath("/assets");

                    // When
                    var result = path.MakeAbsolute("/absolute");

                    // Then
                    Assert.AreEqual("/assets", result.FullPath);
                }
            }
        }

        public sealed class TheGetRelativePathMethod
        {
            public sealed class WithDirectoryPath
            {
                public sealed class InWindowsFormat
                {
                    [Test]
                    [TestCase("C:/A/B/C", "C:/A/B/C", ".")]
                    [TestCase("C:/", "C:/", ".")]
                    [TestCase("C:/A/B/C", "C:/A/D/E", "../../D/E")]
                    [TestCase("C:/A/B/C", "C:/", "../../..")]
                    [TestCase("C:/A/B/C/D/E/F", "C:/A/B/C", "../../..")]
                    [TestCase("C:/A/B/C", "C:/A/B/C/D/E/F", "D/E/F")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new DirectoryPath(from);

                        // When
                        var relativePath = path.GetRelativePath(new DirectoryPath(to));

                        // Then
                        Assert.AreEqual(expected, relativePath.FullPath);
                    }

                    [Test]
                    [TestCase("C:/A/B/C", "D:/A/B/C")]
                    [TestCase("C:/A/B", "D:/E/")]
                    [TestCase("C:/", "B:/")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new DirectoryPath(from);

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new DirectoryPath(to)));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Paths must share a common prefix.", result.Message);
                    }

                    [Test]
                    public void Should_Throw_If_Target_DirectoryPath_Is_Null()
                    {
                        // Given
                        var path = new DirectoryPath("C:/A/B/C");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath((DirectoryPath)null));

                        // Then
                        AssertEx.IsArgumentNullException(result, "to");
                    }

                    [Test]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new DirectoryPath("A");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new DirectoryPath("C:/D/E/F")));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Source path must be an absolute path.", result.Message);
                    }

                    [Test]
                    public void Should_Throw_If_Target_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new DirectoryPath("C:/A/B/C");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new DirectoryPath("D")));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Target path must be an absolute path.", result.Message);
                    }
                }

                public sealed class InUnixFormat
                {
                    [Test]
                    [TestCase("/C/A/B/C", "/C/A/B/C", ".")]
                    [TestCase("/C/", "/C/", ".")]
                    [TestCase("/C/A/B/C", "/C/A/D/E", "../../D/E")]
                    [TestCase("/C/A/B/C", "/C/", "../../..")]
                    [TestCase("/C/A/B/C/D/E/F", "/C/A/B/C", "../../..")]
                    [TestCase("/C/A/B/C", "/C/A/B/C/D/E/F", "D/E/F")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new DirectoryPath(from);

                        // When
                        var relativePath = path.GetRelativePath(new DirectoryPath(to));

                        // Then
                        Assert.AreEqual(expected, relativePath.FullPath);
                    }

                    [Test]
                    [TestCase("/C/A/B/C", "/D/A/B/C")]
                    [TestCase("/C/A/B", "/D/E/")]
                    [TestCase("/C/", "/B/")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new DirectoryPath(from);

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new DirectoryPath(to)));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Paths must share a common prefix.", result.Message);
                    }

                    [Test]
                    public void Should_Throw_If_Target_DirectoryPath_Is_Null()
                    {
                        // Given
                        var path = new DirectoryPath("/C/A/B/C");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath((DirectoryPath)null));

                        // Then
                        AssertEx.IsArgumentNullException(result, "to");
                    }

                    [Test]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new DirectoryPath("A");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new DirectoryPath("/C/D/E/F")));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Source path must be an absolute path.", result.Message);
                    }

                    [Test]
                    public void Should_Throw_If_Target_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new DirectoryPath("/C/A/B/C");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new DirectoryPath("D")));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Target path must be an absolute path.", result.Message);
                    }
                }
            }

            public sealed class WithFilePath
            {
                public sealed class InWindowsFormat
                {
                    [Test]
                    [TestCase("C:/A/B/C", "C:/A/B/C/hello.txt", "hello.txt")]
                    [TestCase("C:/", "C:/hello.txt", "hello.txt")]
                    [TestCase("C:/A/B/C", "C:/A/D/E/hello.txt", "../../D/E/hello.txt")]
                    [TestCase("C:/A/B/C", "C:/hello.txt", "../../../hello.txt")]
                    [TestCase("C:/A/B/C/D/E/F", "C:/A/B/C/hello.txt", "../../../hello.txt")]
                    [TestCase("C:/A/B/C", "C:/A/B/C/D/E/F/hello.txt", "D/E/F/hello.txt")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new DirectoryPath(from);

                        // When
                        var relativePath = path.GetRelativePath(new FilePath(to));

                        // Then
                        Assert.AreEqual(expected, relativePath.FullPath);
                    }

                    [Test]
                    [TestCase("C:/A/B/C", "D:/A/B/C/hello.txt")]
                    [TestCase("C:/A/B", "D:/E/hello.txt")]
                    [TestCase("C:/", "B:/hello.txt")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new DirectoryPath(from);

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new FilePath(to)));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Paths must share a common prefix.", result.Message);
                    }

                    [Test]
                    public void Should_Throw_If_Target_FilePath_Is_Null()
                    {
                        // Given
                        var path = new DirectoryPath("C:/A/B/C");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath((FilePath)null));

                        // Then
                        AssertEx.IsArgumentNullException(result, "to");
                    }

                    [Test]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new DirectoryPath("A");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new FilePath("C:/D/E/F/hello.txt")));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Source path must be an absolute path.", result.Message);
                    }

                    [Test]
                    public void Should_Throw_If_Target_FilePath_Is_Relative()
                    {
                        // Given
                        var path = new DirectoryPath("C:/A/B/C");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new FilePath("D/hello.txt")));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Target path must be an absolute path.", result.Message);
                    }
                }

                public sealed class InUnixFormat
                {
                    [Test]
                    [TestCase("/C/A/B/C", "/C/A/B/C/hello.txt", "hello.txt")]
                    [TestCase("/C/", "/C/hello.txt", "hello.txt")]
                    [TestCase("/C/A/B/C", "/C/A/D/E/hello.txt", "../../D/E/hello.txt")]
                    [TestCase("/C/A/B/C", "/C/hello.txt", "../../../hello.txt")]
                    [TestCase("/C/A/B/C/D/E/F", "/C/A/B/C/hello.txt", "../../../hello.txt")]
                    [TestCase("/C/A/B/C", "/C/A/B/C/D/E/F/hello.txt", "D/E/F/hello.txt")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new DirectoryPath(from);

                        // When
                        var relativePath = path.GetRelativePath(new FilePath(to));

                        // Then
                        Assert.AreEqual(expected, relativePath.FullPath);
                    }

                    [Test]
                    [TestCase("/C/A/B/C", "/D/A/B/C/hello.txt")]
                    [TestCase("/C/A/B", "/D/E/hello.txt")]
                    [TestCase("/C/", "/B/hello.txt")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new DirectoryPath(from);

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new FilePath(to)));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Paths must share a common prefix.", result.Message);
                    }

                    [Test]
                    public void Should_Throw_If_Target_FilePath_Is_Null()
                    {
                        // Given
                        var path = new DirectoryPath("/C/A/B/C");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath((FilePath)null));

                        // Then
                        AssertEx.IsArgumentNullException(result, "to");
                    }

                    [Test]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new DirectoryPath("A");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new FilePath("/C/D/E/F/hello.txt")));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Source path must be an absolute path.", result.Message);
                    }

                    [Test]
                    public void Should_Throw_If_Target_FilePath_Is_Relative()
                    {
                        // Given
                        var path = new DirectoryPath("/C/A/B/C");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath(new FilePath("D/hello.txt")));

                        // Then
                        Assert.That(result, Is.TypeOf<InvalidOperationException>());
                        Assert.AreEqual("Target path must be an absolute path.", result.Message);
                    }
                }
            }
        }
    }
}