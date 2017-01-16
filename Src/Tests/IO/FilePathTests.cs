using System;
using Essentions.IO;
using NSubstitute;
using NUnit.Framework;

namespace Essentions.Tests.IO
{
    public sealed class FilePathTests
    {
        public sealed class TheHasExtensionProperty
        {
            [Test]
            [TestCase("assets/shaders/basic.txt", true)]
            [TestCase("assets/shaders/basic", false)]
            [TestCase("assets/shaders/basic/", false)]
            public void Can_See_If_A_Path_Has_An_Extension(string fullPath, bool expected)
            {
                // Given, When
                var path = new FilePath(fullPath);

                // Then
                Assert.AreEqual(expected, path.HasExtension);
            }
        }

        public sealed class TheGetExtensionMethod
        {
            [Test]
            [TestCase("assets/shaders/basic.frag", ".frag")]
            [TestCase("assets/shaders/basic.frag/test.vert", ".vert")]
            [TestCase("assets/shaders/basic", null)]
            [TestCase("assets/shaders/basic.frag/test", null)]
            public void Can_Get_Extension(string fullPath, string expected)
            {
                // Given, When
                var result = new FilePath(fullPath);
                var extension = result.GetExtension();

                // Then
                Assert.AreEqual(expected, extension);
            }
        }

        public sealed class TheGetDirectoryMethod
        {
            [Test]
            public void Can_Get_Directory_For_File_Path()
            {
                // Given, When
                var path = new FilePath("temp/hello.txt");
                var directory = path.GetDirectory();

                // Then
                Assert.AreEqual("temp", directory.FullPath);
            }

            [Test]
            public void Can_Get_Directory_For_File_Path_In_Root()
            {
                // Given, When
                var path = new FilePath("hello.txt");
                var directory = path.GetDirectory();

                // Then
                Assert.AreEqual(string.Empty, directory.FullPath);
            }
        }

        public sealed class TheChangeExtensionMethod
        {
            [Test]
            public void Can_Change_Extension_Of_Path()
            {
                // Given
                var path = new FilePath("temp/hello.txt");

                // When
                path = path.ChangeExtension(".dat");

                // Then
                Assert.AreEqual("temp/hello.dat", path.ToString());
            }
        }

        public sealed class TheAppendExtensionMethod
        {
            [Test]
            public void Should_Throw_If_Extension_Is_Null()
            {
                // Given
                var path = new FilePath("temp/hello.txt");

                // When
                var result = Assert.Catch(() => path.AppendExtension(null));

                // Then
                AssertEx.IsArgumentNullException(result, "extension");
            }

            [Test]
            [TestCase("dat", "temp/hello.txt.dat")]
            [TestCase(".dat", "temp/hello.txt.dat")]
            public void Can_Append_Extension_To_Path(string extension, string expected)
            {
                // Given
                var path = new FilePath("temp/hello.txt");

                // When
                path = path.AppendExtension(extension);

                // Then
                Assert.AreEqual(expected, path.ToString());
            }
        }

        public sealed class TheGetFilenameMethod
        {
            [Test]
            public void Can_Get_Filename_From_Path()
            {
                // Given
                var path = new FilePath("/input/test.txt");

                // When
                var result = path.GetFilename();

                // Then
                Assert.AreEqual("test.txt", result.FullPath);
            }
        }

        public sealed class TheGetFilenameWithoutExtensionMethod
        {
            [Test]
            [TestCase("/input/test.txt", "test")]
            [TestCase("/input/test", "test")]
            public void Should_Return_Filename_Without_Extension_From_Path(string fullPath, string expected)
            {
                // Given
                var path = new FilePath(fullPath);

                // When
                var result = path.GetFilenameWithoutExtension();

                // Then
                Assert.AreEqual(expected, result.FullPath);
            }
        }

        public sealed class TheMakeAbsoluteMethod
        {
            public sealed class WithEnvironment
            {
                [Test]
                public void Should_Throw_If_Environment_Is_Null()
                {
                    // Given
                    var path = new FilePath("temp/hello.txt");

                    // When
                    var result = Assert.Catch(() => path.MakeAbsolute((IFileSystemEnvironment)null));

                    // Then
                    AssertEx.IsArgumentNullException(result, "environment");
                }

                [Test]
                public void Should_Return_A_Absolute_File_Path_If_File_Path_Is_Relative()
                {
                    // Given
                    var path = new FilePath("./test.txt");
                    var environment = Substitute.For<IFileSystemEnvironment>();
                    environment.WorkingDirectory.Returns(new DirectoryPath("/absolute"));

                    // When
                    var result = path.MakeAbsolute(environment);

                    // Then
                    Assert.AreEqual("/absolute/test.txt", result.FullPath);
                }

                [Test]
                public void Should_Return_Same_File_Path_If_File_Path_Is_Absolute()
                {
                    // Given
                    var path = new FilePath("/test.txt");
                    var environment = Substitute.For<IFileSystemEnvironment>();
                    environment.WorkingDirectory.Returns(new DirectoryPath("/absolute"));

                    // When
                    var result = path.MakeAbsolute(environment);

                    // Then
                    Assert.AreEqual("/test.txt", result.FullPath);
                }
            }

            public sealed class WithDirectoryPath
            {
                [Test]
                public void Should_Throw_If_Provided_Directory_Is_Null()
                {
                    // Given
                    var path = new FilePath("./test.txt");

                    // When
                    var result = Assert.Catch(() => path.MakeAbsolute((DirectoryPath)null));

                    // Then
                    AssertEx.IsArgumentNullException(result, "path");
                }

                [Test]
                public void Should_Throw_If_Provided_Directory_Is_Relative()
                {
                    // Given
                    var path = new FilePath("./test.txt");
                    var directory = new DirectoryPath("./relative");

                    // When
                    var result = Assert.Catch(() => path.MakeAbsolute(directory));

                    // Then
                    Assert.That(result, Is.TypeOf<InvalidOperationException>());
                    Assert.AreEqual("Cannot make a file path absolute with a relative directory path.", result.Message);
                }

                [Test]
                public void Should_Return_A_Absolute_File_Path_If_File_Path_Is_Relative()
                {
                    // Given
                    var path = new FilePath("./test.txt");
                    var directory = new DirectoryPath("/absolute");

                    // When
                    var result = path.MakeAbsolute(directory);

                    // Then
                    Assert.AreEqual("/absolute/test.txt", result.FullPath);
                }

                [Test]
                public void Should_Return_Same_File_Path_If_File_Path_Is_Absolute()
                {
                    // Given
                    var path = new FilePath("/test.txt");
                    var directory = new DirectoryPath("/absolute");

                    // When
                    var result = path.MakeAbsolute(directory);

                    // Then
                    Assert.AreEqual("/test.txt", result.FullPath);
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
                    [TestCase("C:/A/B/C/hello.txt", "C:/A/B/C", ".")]
                    [TestCase("C:/hello.txt", "C:/", ".")]
                    [TestCase("C:/A/B/C/hello.txt", "C:/A/D/E", "../../D/E")]
                    [TestCase("C:/A/B/C/hello.txt", "C:/", "../../..")]
                    [TestCase("C:/A/B/C/D/E/F/hello.txt", "C:/A/B/C", "../../..")]
                    [TestCase("C:/A/B/C/hello.txt", "C:/A/B/C/D/E/F", "D/E/F")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var relativePath = path.GetRelativePath(new DirectoryPath(to));

                        // Then
                        Assert.AreEqual(expected, relativePath.FullPath);
                    }

                    [Test]
                    [TestCase("C:/A/B/C/hello.txt", "D:/A/B/C")]
                    [TestCase("C:/A/B/hello.txt", "D:/E/")]
                    [TestCase("C:/hello.txt", "B:/")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new FilePath(from);

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
                        var path = new FilePath("C:/A/B/C/hello.txt");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath((DirectoryPath)null));

                        // Then
                        AssertEx.IsArgumentNullException(result, "to");
                    }

                    [Test]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new FilePath("A/hello.txt");

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
                        var path = new FilePath("C:/A/B/C/hello.txt");

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
                    [TestCase("/C/A/B/C/hello.txt", "/C/A/B/C", ".")]
                    [TestCase("/C/hello.txt", "/C/", ".")]
                    [TestCase("/C/A/B/C/hello.txt", "/C/A/D/E", "../../D/E")]
                    [TestCase("/C/A/B/C/hello.txt", "/C/", "../../..")]
                    [TestCase("/C/A/B/C/D/E/F/hello.txt", "/C/A/B/C", "../../..")]
                    [TestCase("/C/A/B/C/hello.txt", "/C/A/B/C/D/E/F", "D/E/F")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var relativePath = path.GetRelativePath(new DirectoryPath(to));

                        // Then
                        Assert.AreEqual(expected, relativePath.FullPath);
                    }

                    [Test]
                    [TestCase("/C/A/B/C/hello.txt", "/D/A/B/C")]
                    [TestCase("/C/A/B/hello.txt", "/D/E/")]
                    [TestCase("/C/hello.txt", "/B/")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new FilePath(from);

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
                        var path = new FilePath("/C/A/B/C/hello.txt");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath((DirectoryPath)null));

                        // Then
                        AssertEx.IsArgumentNullException(result, "to");
                    }

                    [Test]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new FilePath("A/hello.txt");

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
                        var path = new FilePath("/C/A/B/C/hello.txt");

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
                    [TestCase("C:/A/B/C/hello.txt", "C:/A/B/C/hello.txt", "hello.txt")]
                    [TestCase("C:/hello.txt", "C:/hello.txt", "hello.txt")]
                    [TestCase("C:/hello.txt", "C:/world.txt", "world.txt")]
                    [TestCase("C:/A/B/C/hello.txt", "C:/A/D/E/hello.txt", "../../D/E/hello.txt")]
                    [TestCase("C:/A/B/C/hello.txt", "C:/hello.txt", "../../../hello.txt")]
                    [TestCase("C:/A/B/C/D/E/F/hello.txt", "C:/A/B/C/hello.txt", "../../../hello.txt")]
                    [TestCase("C:/A/B/C/hello.txt", "C:/A/B/C/D/E/F/hello.txt", "D/E/F/hello.txt")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var relativePath = path.GetRelativePath(new FilePath(to));

                        // Then
                        Assert.AreEqual(expected, relativePath.FullPath);
                    }

                    [Test]
                    [TestCase("C:/A/B/C/hello.txt", "D:/A/B/C/hello.txt")]
                    [TestCase("C:/A/B/hello.txt", "D:/E/hello.txt")]
                    [TestCase("C:/hello.txt", "B:/hello.txt")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new FilePath(from);

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
                        var path = new FilePath("C:/A/B/C/hello.txt");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath((FilePath)null));

                        // Then
                        AssertEx.IsArgumentNullException(result, "to");
                    }

                    [Test]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new FilePath("A/hello.txt");

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
                        var path = new FilePath("C:/A/B/C/hello.txt");

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
                    [TestCase("/C/A/B/C/hello.txt", "/C/A/B/C/hello.txt", "hello.txt")]
                    [TestCase("/C/hello.txt", "/C/hello.txt", "hello.txt")]
                    [TestCase("/C/hello.txt", "/C/world.txt", "world.txt")]
                    [TestCase("/C/A/B/C/hello.txt", "/C/A/D/E/hello.txt", "../../D/E/hello.txt")]
                    [TestCase("/C/A/B/C/hello.txt", "/C/hello.txt", "../../../hello.txt")]
                    [TestCase("/C/A/B/C/D/E/F/hello.txt", "/C/A/B/C/hello.txt", "../../../hello.txt")]
                    [TestCase("/C/A/B/C/hello.txt", "/C/A/B/C/D/E/F/hello.txt", "D/E/F/hello.txt")]
                    public void Should_Returns_Relative_Path_Between_Paths(string from, string to, string expected)
                    {
                        // Given
                        var path = new FilePath(from);

                        // When
                        var relativePath = path.GetRelativePath(new FilePath(to));

                        // Then
                        Assert.AreEqual(expected, relativePath.FullPath);
                    }

                    [Test]
                    [TestCase("/C/A/B/C/hello.txt", "/D/A/B/C/hello.txt")]
                    [TestCase("/C/A/B/hello.txt", "/D/E/hello.txt")]
                    [TestCase("/C/hello.txt", "/B/hello.txt")]
                    public void Should_Throw_If_No_Relative_Path_Can_Be_Found(string from, string to)
                    {
                        // Given
                        var path = new FilePath(from);

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
                        var path = new FilePath("/C/A/B/C/hello.txt");

                        // When
                        var result = Assert.Catch(() => path.GetRelativePath((FilePath)null));

                        // Then
                        AssertEx.IsArgumentNullException(result, "to");
                    }

                    [Test]
                    public void Should_Throw_If_Source_DirectoryPath_Is_Relative()
                    {
                        // Given
                        var path = new FilePath("A/hello.txt");

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
                        var path = new FilePath("/C/A/B/C/hello.txt");

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