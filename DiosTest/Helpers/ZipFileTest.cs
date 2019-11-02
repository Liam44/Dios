using Dios.Helpers;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xunit;
using ZipFile = System.IO.Compression.ZipFile;
using ZipFileDios = Dios.Helpers.ZipFile;

namespace DiosTest.Helpers
{
    public sealed class ZipFileTest
    {
        private readonly Mock<IHostingEnvironment> _environment = new Mock<IHostingEnvironment>();
        private readonly string _localPath = Directory.GetCurrentDirectory();
        private const string _tests = "tests";

        [Fact]
        public void CreateZip_PathNull()
        {
            // Arrange
            string path = null;
            string fileName = null;
            List<string> files = null;

            // Act
            ZipResult zipResult = ZipFileDios.CreateZip(path, fileName, files);

            // Assert
            Assert.NotNull(zipResult);
            Assert.Empty(zipResult.FileName);
            Assert.Equal(0, zipResult.MemoryStream.Length);
            Assert.Equal("application/zip", zipResult.ContentType);
        }

        [Fact]
        public void CreateZip_PathEmpty()
        {
            // Arrange
            string path = string.Empty;
            string fileName = null;
            List<string> files = null;

            // Act
            ZipResult zipResult = ZipFileDios.CreateZip(path, fileName, files);

            // Assert
            Assert.NotNull(zipResult);
            Assert.Empty(zipResult.FileName);
            Assert.Equal(0, zipResult.MemoryStream.Length);
            Assert.Equal("application/zip", zipResult.ContentType);
        }

        [Fact]
        public void CreateZip_PathNotEmpty_FileNameNull()
        {
            // Arrange
            string path = "somePath";
            string fileName = null;
            List<string> files = null;

            // Act
            ZipResult zipResult = ZipFileDios.CreateZip(path, fileName, files);

            // Assert
            Assert.NotNull(zipResult);
            Assert.Empty(zipResult.FileName);
            Assert.Equal(0, zipResult.MemoryStream.Length);
            Assert.Equal("application/zip", zipResult.ContentType);
        }

        [Fact]
        public void CreateZip_PathNotEmpty_FileNameEmpty()
        {
            // Arrange
            string path = "somePath";
            string fileName = string.Empty;
            List<string> files = null;

            // Act
            ZipResult zipResult = ZipFileDios.CreateZip(path, fileName, files);

            // Assert
            Assert.NotNull(zipResult);
            Assert.Empty(zipResult.FileName);
            Assert.Equal(0, zipResult.MemoryStream.Length);
            Assert.Equal("application/zip", zipResult.ContentType);
        }

        [Fact]
        public void CreateZip_PathInvalid_FileNameNotEmpty()
        {
            // Arrange
            string path = "somePath";
            string fileName = "someFileName";
            List<string> files = null;

            // Act
            DirectoryNotFoundException exception = Assert.Throws<DirectoryNotFoundException>(() =>
                                                   ZipFileDios.CreateZip(path, fileName, files));

            // Assert
            Assert.NotNull(exception);
            Assert.Contains(path, exception.Message);
        }

        [Fact]
        public void CreateZip_PathValid_FileNameNotEmpty_FilesNull()
        {
            // Arrange
            string path = Path.Combine(_tests, nameof(CreateZip_PathValid_FileNameNotEmpty_FilesNull));
            string fileName = "someFileName";
            string expectedFileName = fileName + ".zip";
            List<string> files = null;

            // Creates a temporary directory
            Directory.CreateDirectory(path);

            _environment.Setup(e => e.WebRootPath)
                        .Returns(path);

            // Act
            ZipResult result = ZipFileDios.CreateZip(path, fileName, files);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFileName, result.FileName);
            Assert.NotEqual(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            using (ZipArchive archive = ZipFile.Open(Path.Combine(path, result.FileName),
                                                     ZipArchiveMode.Read))
            {
                Assert.Empty(archive.Entries);
            }

            // Cleans up the directory
            Directory.Delete(path, true);
        }

        [Fact]
        public void CreateZip_PathValid_FileNameNotEmpty_FilesEmpty()
        {
            // Arrange
            string path = Path.Combine(_tests, nameof(CreateZip_PathValid_FileNameNotEmpty_FilesEmpty));
            string fileName = "someFileName";
            string expectedFileName = fileName + ".zip";
            List<string> files = new List<string>();

            // Creates a temporary directory
            Directory.CreateDirectory(path);

            _environment.Setup(e => e.WebRootPath)
                        .Returns(path);

            // Act
            ZipResult result = ZipFileDios.CreateZip(path, fileName, files);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFileName, result.FileName);
            Assert.NotEqual(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            using (ZipArchive archive = ZipFile.Open(Path.Combine(path, result.FileName),
                                                     ZipArchiveMode.Read))
            {
                Assert.Empty(archive.Entries);
            }

            // Cleans up the directory
            Directory.Delete(path, true);
        }

        [Fact]
        public void CreateZip_PathValid_FileNameNotEmpty_FilesNotEmpty_InvalidFileNames()
        {
            // Arrange
            string path = Path.Combine(_tests, nameof(CreateZip_PathValid_FileNameNotEmpty_FilesNotEmpty_InvalidFileNames));
            string fileName = "someFileName";
            string invalidFileName = "someInvalidFileName";
            List<string> files = new List<string> { invalidFileName };

            // Creates a temporary directory
            Directory.CreateDirectory(path);

            _environment.Setup(e => e.WebRootPath)
                        .Returns(path);

            // Act
            FileNotFoundException exception = Assert.Throws<FileNotFoundException>(() =>
                                              ZipFileDios.CreateZip(path, fileName, files));

            // Assert
            Assert.NotNull(exception);
            Assert.Contains(invalidFileName, exception.Message);

            // Cleans up the directory
            Directory.Delete(path, true);
        }

        [Fact]
        public void CreateZip_PathValid_FileNameNotEmpty_FilesNotEmpty_ValidFileNames()
        {
            // Arrange
            string path = Path.Combine(_tests, nameof(CreateZip_PathValid_FileNameNotEmpty_FilesNotEmpty_ValidFileNames));
            string fileName = "someFileName";
            string expectedFileName = fileName + ".zip";
            string fileToBeZipped = "someFileToBeZipped.txt";
            string content = "someContent";
            List<string> files = new List<string> { Path.Combine(path, fileToBeZipped) };

            // Creates a temporary directory
            Directory.CreateDirectory(path);
            // And a new text file
            using (StreamWriter stream = new StreamWriter(Path.Combine(path, fileToBeZipped), false))
            {
                stream.Write(content);

                stream.Flush();
                stream.Close();
            }

            _environment.Setup(e => e.WebRootPath)
                        .Returns(path);

            // Act
            ZipResult result = ZipFileDios.CreateZip(path, fileName, files);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFileName, result.FileName);
            Assert.NotEqual(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            using (ZipArchive archive = ZipFile.Open(Path.Combine(path, result.FileName), 
                                                     ZipArchiveMode.Read))
            {
                Assert.Single(archive.Entries);
                Assert.Equal(fileToBeZipped, archive.Entries.First().FullName);
                Assert.Equal(content.Length, archive.Entries.First().Length);
            }

            // Cleans up the directory
            Directory.Delete(path, true);
        }
    }
}
