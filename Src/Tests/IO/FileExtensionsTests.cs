using System.Linq;
using System.Text;
using Essentions.IO;
using NSubstitute;
using NUnit.Framework;

namespace Essentions.Tests.IO
{
    public sealed class FileExtensionsTests
    {
        public sealed class TheOpenMethod
        {
            public sealed class WithFileMode
            {
                [Test]
                public void Should_Throw_If_File_Is_Null()
                {
                    // Given, When
                    var result = Assert.Catch(() => FileExtensions.Open(null, FileMode.Create));

                    // Then
                    AssertEx.IsArgumentNullException(result, "file");
                }

                [Test]
                [TestCase(FileMode.Append, FileAccess.Write)]
                [TestCase(FileMode.Create, FileAccess.ReadWrite)]
                [TestCase(FileMode.CreateNew, FileAccess.ReadWrite)]
                [TestCase(FileMode.Open, FileAccess.ReadWrite)]
                [TestCase(FileMode.OpenOrCreate, FileAccess.ReadWrite)]
                [TestCase(FileMode.Truncate, FileAccess.ReadWrite)]
                public void Should_Open_With_Specified_File_Mode_And_Infer_File_Access(FileMode mode, FileAccess access)
                {
                    // Given
                    var file = Substitute.For<IFile>();

                    // When
                    file.Open(mode);

                    // Then
                    file.Received(1).Open(mode, access, FileShare.None);
                }
            }

            public sealed class WithFileModeAndFileAccess
            {
                [Test]
                public void Should_Throw_If_File_Is_Null()
                {
                    // Given, When
                    var result = Assert.Catch(() => FileExtensions.Open(null, FileMode.Create, FileAccess.Write));

                    // Then
                    AssertEx.IsArgumentNullException(result, "file");
                }

                [Test]
                [TestCase(FileMode.Append, FileAccess.Write)]
                [TestCase(FileMode.Create, FileAccess.ReadWrite)]
                [TestCase(FileMode.CreateNew, FileAccess.ReadWrite)]
                [TestCase(FileMode.Open, FileAccess.ReadWrite)]
                [TestCase(FileMode.OpenOrCreate, FileAccess.ReadWrite)]
                [TestCase(FileMode.Truncate, FileAccess.ReadWrite)]
                public void Should_Open_With_Specified_File_Mode_And_Infer_File_Access(FileMode mode, FileAccess access)
                {
                    // Given
                    var file = Substitute.For<IFile>();

                    // When
                    file.Open(mode, access);

                    // Then
                    file.Received(1).Open(mode, access, FileShare.None);
                }
            }
        }

        public sealed class TheOpenReadMethod
        {
            [Test]
            public void Should_Throw_If_File_Is_Null()
            {
                // Given, When
                var result = Assert.Catch(() => FileExtensions.OpenRead(null));

                // Then
                AssertEx.IsArgumentNullException(result, "file");
            }

            [Test]
            public void Should_Open_Stream_With_Expected_FileMode_And_FileAccess()
            {
                // Given
                var file = Substitute.For<IFile>();

                // When
                file.OpenRead();

                // Then
                file.Received(1).Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }

        public sealed class TheOpenWriteMethod
        {
            [Test]
            public void Should_Throw_If_File_Is_Null()
            {
                // Given, When
                var result = Assert.Catch(() => FileExtensions.OpenWrite(null));

                // Then
                AssertEx.IsArgumentNullException(result, "file");
            }

            [Test]
            public void Should_Open_Stream_With_Expected_FileMode_And_FileAccess()
            {
                // Given
                var file = Substitute.For<IFile>();

                // When
                file.OpenWrite();

                // Then
                file.Received(1).Open(FileMode.Create, FileAccess.Write, FileShare.None);
            }
        }

        public sealed class TheReadLinesMethod
        {
            [Test]
            public void Should_Throw_If_File_Is_Null()
            {
                // Given, When
                var result = Assert.Catch(() => FileExtensions.ReadLines(null, Encoding.UTF8));

                // Then
                AssertEx.IsArgumentNullException(result, "file");
            }

            [Test]
            public void Should_Return_Empty_List_If_File_Contains_No_Lines()
            {
                // Given
                var environment = FakeEnvironment.CreateUnixEnvironment();
                var fileSystem = new FakeFileSystem(environment);
                var file = fileSystem.CreateFile("text.txt");

                // When
                var result = file.ReadLines(Encoding.UTF8).ToList();

                // Then
                Assert.AreEqual(0, result.Count);
            }

            [Test]
            public void Should_Read_File_With_Single_Line_Correctly()
            {
                // Given
                var environment = FakeEnvironment.CreateUnixEnvironment();
                var fileSystem = new FakeFileSystem(environment);
                var file = fileSystem.CreateFile("text.txt").SetText("Hello World");

                // When
                var result = file.ReadLines(Encoding.UTF8).ToList();

                // Then
                Assert.AreEqual(1, result.Count);
            }

            [Test]
            public void Should_Read_File_With_Multiple_Lines_Correctly()
            {
                // Given
                var environment = FakeEnvironment.CreateUnixEnvironment();
                var fileSystem = new FakeFileSystem(environment);
                var content = new StringBuilder();
                content.AppendLine("1");
                content.AppendLine("2");
                content.AppendLine("3");
                var file = fileSystem.CreateFile("text.txt").SetText(content.ToString());

                // When
                var result = file.ReadLines(Encoding.UTF8).ToList();

                // Then
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("1", result[0]);
                Assert.AreEqual("2", result[1]);
                Assert.AreEqual("3", result[2]);
            }
        }
    }
}