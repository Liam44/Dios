using Dios.Exceptions;
using Dios.Helpers;
using Dios.Models;
using Dios.Repositories;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace DiosTest.Helpers
{
    public sealed class ExportTest
    {
        [Fact]
        public void Export_ZipFileNull()
        {
            // Arrange
            IZipFile zipFile = null;
            IUsersRepository usersRepository = null;
            AddressDTO address = null;
            string path = null;

            // Act
            ZipResult result = Export.ExportUsers(zipFile, usersRepository, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.FileName);
            Assert.Equal(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);
        }

        [Fact]
        public void Export_ZipFileNotNull_UsersRepositoryNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            IUsersRepository usersRepository = null;
            AddressDTO address = null;
            string path = null;

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.FileName);
            Assert.Equal(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            zipFile.Verify(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
                                Times.Never);
        }

        [Fact]
        public void Export_ZipFileNotNull_UsersRepositoryNotNull_AddressNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            AddressDTO address = null;
            string path = null;

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.FileName);
            Assert.Equal(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
                                Times.Never);
        }

        [Fact]
        public void Export_ZipFileNotNull_UsersRepositoryNotNull_AddressNotNull_AddressStreetNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            AddressDTO address = new AddressDTO
            {
                Street = null,
                Number = null
            };
            string path = null;

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.FileName);
            Assert.Equal(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
                                Times.Never);
        }

        [Fact]
        public void Export_ZipFileNotNull_UsersRepositoryNotNull_AddressNotNull_AddressStreetEmpty()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            AddressDTO address = new AddressDTO
            {
                Street = string.Empty,
                Number = null
            };
            string path = null;

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.FileName);
            Assert.Equal(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
                                Times.Never);
        }

        [Fact]
        public void Export_ZipFileNotNull_UsersRepositoryNotNull_AddressNotNull_AddressStreetNotEmpty_AddressNumberNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            AddressDTO address = new AddressDTO
            {
                Street = "someStreet",
                Number = null
            };
            string path = null;

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.FileName);
            Assert.Equal(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
                                Times.Never);
        }

        [Fact]
        public void Export_ZipFileNotNull_UsersRepositoryNotNull_AddressNotNull_AddressStreetNotEmpty_AddressNumberEmpty()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            AddressDTO address = new AddressDTO
            {
                Street = "someStreet",
                Number = string.Empty
            };
            string path = null;

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.FileName);
            Assert.Equal(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
                                Times.Never);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path null
        public void Export_ZFNotNull_URNotNull_ANotNull_ASNotEmpty_ANNotEmpty_PathNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            AddressDTO address = new AddressDTO
            {
                Street = "someStreet",
                Number = "someNumber"
            };
            string path = null;

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.FileName);
            Assert.Equal(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
                                Times.Never);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path empty
        public void Export_ZFNotNull_URNotNull_ANotNull_ASNotEmpty_ANNotEmpty_PathEmpty()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            AddressDTO address = new AddressDTO
            {
                Street = "someStreet",
                Number = "someNumber"
            };
            string path = string.Empty;

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.FileName);
            Assert.Equal(0, result.MemoryStream.Length);
            Assert.Equal("application/zip", result.ContentType);

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
                                Times.Never);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats null
        public void Export_ParamsOK_AddressFlatsNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            string street = "someStreet";
            string number = "someNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = null
            };
            string path = nameof(Export_ParamsOK_AddressFlatsNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";

            // Act
            ExportUsersException exception = Assert.Throws<ExportUsersException>(() =>
                                             Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path));

            // Assert
            Assert.NotNull(exception);
            Assert.False(File.Exists(file1Name));
            Assert.False(File.Exists(file2Name));
            Assert.False(File.Exists(file3Name));

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
                                Times.Never);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats empty
        /// Creation failed
        public void Export_ParamsOK_AddressFlatsEmpty_CreationFailed()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            string street = "someStreet";
            string number = "someNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO>()
            };
            string path = nameof(Export_ParamsOK_AddressFlatsEmpty_CreationFailed);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = null;

            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.Null(result);
            Assert.True(File.Exists(file1Name));
            Assert.True(File.Exists(file2Name));
            Assert.True(File.Exists(file3Name));

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats empty
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsEmpty_CreationSucceeded()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();
            string street = "someStreet";
            string number = "someNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO>()
            };
            string path = nameof(Export_ParamsOK_AddressFlatsEmpty_CreationSucceeded);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);
            Assert.True(File.Exists(file1Name));
            Assert.True(File.Exists(file2Name));
            Assert.True(File.Exists(file3Name));

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() null
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            FlatDTO flat = null;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one (null) flat defined at the given address,
                // the table should only contain one row, which is actually being used as a top margin
                var rows = table.Descendants<TableRow>();
                Assert.Single(rows);

                // It should contain three columns and be empty
                TableRow row = rows.First();
                IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                Assert.Equal(3, cells.Count());
                foreach (TableCell cell in cells)
                {
                    Assert.Empty(cell.InnerText);
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one (null) flat defined at the given address
                // the table should only contain one rows, which is actually being used as a top margin
                var rows = table.Descendants<TableRow>();
                Assert.Single(rows);

                // It should contain two columns and be empty
                TableRow row = rows.First();
                IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                Assert.Equal(2, cells.Count());
                foreach (TableCell cell in cells)
                {
                    Assert.Empty(cell.InnerText);
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one (null) flat defined at the given address,
                // the table should only contain two rows, the first row actually being used as a top margin
                // and the last one as bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(2, rows.Count());

                // Each row should contain two columns and be empty
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    foreach (TableCell cell in cells)
                    {
                        Assert.Empty(cell.InnerText);
                    }
                }
            }

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters null
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            List<ParameterDTO> parameters = null;
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address - but parameters are null -,
                // the table should only contain three rows, the first row actually being used as a top margin
                // and the second one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address - but parameters are null -,
                // the table should only contain three rows, the first row actually being used as a top margin
                // and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address - but parameters are null -,
                // the table should only contain three rows, the first row actually being used as a top margin
                // and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters empty
        /// Creation succeeded
        public void Export_ParametersOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersEmpty()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            List<ParameterDTO> parameters = new List<ParameterDTO>();
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParametersOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersEmpty);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain
                // three rows, the first row actually being used as a top margin and the last one
                // as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain
                // three rows, the first row actually being used as a top margin and the last one
                // as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address - but parameters are null -,
                // the table should only contain three rows, the first row actually being used as a top margin
                // and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains one entry
        /// Address.Flats.First().Parameters.First() null
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            ParameterDTO parameter = null;
            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter };

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the flat parameters are null, should only be displayed the floor and the number of the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the flat parameters are null, should only be displayed the floor of the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the flat parameters are null, should only be displayed the entry code to the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains one entry
        /// Address.Flats.First().Parameters.First() not null
        /// Address.Flats.First().Parameters.First().UserId null
        /// Address.Flats.First().Parameters.First().User null
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNotNull_UserIdNull_UserNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            string userId = null;
            UserDTO user = null;

            ParameterDTO parameter = new ParameterDTO { UserId = userId };
            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter };

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            usersRepository.Setup(u => u.User(It.IsAny<string>()))
                           .Returns(user);
            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is null, should only be displayed
                    // the floor and the number of the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Since the user from the flat parameters is null, should only be displayed
                    // the floor of the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is null, should only be displayed
                    // the entry code to the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Exactly(3));
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains one entry
        /// Address.Flats.First().Parameters.First() not null
        /// Address.Flats.First().Parameters.First().UserId empty
        /// Address.Flats.First().Parameters.First().User null
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNotNull_UserIdEmpty_UserNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            string userId = string.Empty;
            UserDTO user = null;

            ParameterDTO parameter = new ParameterDTO { UserId = userId };
            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter };

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            usersRepository.Setup(u => u.User(It.IsAny<string>()))
                           .Returns(user);
            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is null, should only be displayed
                    // the floor and the number of the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Since the user from the flat parameters is null, should only be displayed
                    // the floor of the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is null, should only be displayed
                    // the entry code to the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Exactly(3));
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains one entry
        /// Address.Flats.First().Parameters.First() not null
        /// Address.Flats.First().Parameters.First().UserId not empty
        /// Address.Flats.First().Parameters.First().User null
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNotNull_UserIdNotEmpty_UserNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            string userId = "someUserId";
            UserDTO user = null;

            ParameterDTO parameter = new ParameterDTO { UserId = userId };
            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter };

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            usersRepository.Setup(u => u.User(It.IsAny<string>()))
                           .Returns(user);
            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is null, should only be displayed
                    // the floor and the number of the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Since the user from the flat parameters is null, should only be displayed
                    // the floor of the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is null, should only be displayed
                    // the entry code to the flat
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Empty(cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Exactly(3));
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains one entry
        /// Address.Flats.First().Parameters.First() not null
        /// Address.Flats.First().Parameters.First().UserId not empty - no dashes
        /// Address.Flats.First().Parameters.First().User not null
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNotNull_UserIdNotEmptyNoDashes_UserNotNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            string userId = "someUserId";
            string firstName = "SomeFirstName";
            string lastName = "SomeLastName";

            string initials = "S SomeLastName";

            UserDTO user = new UserDTO
            {
                FirstName = firstName,
                LastName = lastName
            };

            ParameterDTO parameter = new ParameterDTO { UserId = userId };
            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter };

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            usersRepository.Setup(u => u.User(It.IsAny<string>()))
                           .Returns(user);
            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor and the number of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the entry code to the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(userId), Times.Exactly(3));
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains one entry
        /// Address.Flats.First().Parameters.First() not null
        /// Address.Flats.First().Parameters.First().UserId not empty - lots of dashes
        /// Address.Flats.First().Parameters.First().User not null
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNotNull_UserIdNotEmptyLotsOfDashes_UserNotNull()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            string userId = "someUserId";
            string firstName = "SomeFirstName-With-Plenty-Of-Dashes";
            string lastName = "SomeLastName";

            string initials = "S-W-P-O-D SomeLastName";

            UserDTO user = new UserDTO
            {
                FirstName = firstName,
                LastName = lastName
            };

            ParameterDTO parameter = new ParameterDTO { UserId = userId };
            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter };

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            usersRepository.Setup(u => u.User(It.IsAny<string>()))
                           .Returns(user);
            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor and the number of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the entry code to the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(userId), Times.Exactly(3));
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains two entries
        /// Address.Flats.First().Parameters.First() not null
        /// Address.Flats.First().Parameters.First().UserId not empty
        /// Address.Flats.First().Parameters.First().User not null
        /// Address.Flats.First().Parameters.Last() not null
        /// Address.Flats.First().Parameters.Last().UserId not empty
        /// Address.Flats.First().Parameters.Last().User not null
        /// User1 and User2 have same lastname
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersTwoEntries_ParametersNotNull_UserId1NotEmpty_User1NotNull_UserId2NotEmpty_User2NotNull_SameLastName()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            string userId1 = "someUserId1";
            string firstName1 = "SomeFirstName1";
            string lastName1 = "SomeLastName";

            UserDTO user1 = new UserDTO
            {
                FirstName = firstName1,
                LastName = lastName1
            };

            string userId2 = "someUserId2";
            string firstName2 = "AnotherFirstName";
            string lastName2 = lastName1;

            UserDTO user2 = new UserDTO
            {
                FirstName = firstName2,
                LastName = lastName2
            };

            string initials = $"A & S {lastName1}";

            ParameterDTO parameter1 = new ParameterDTO { UserId = userId1 };
            ParameterDTO parameter2 = new ParameterDTO { UserId = userId2 };
            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter1, parameter2 };

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            usersRepository.Setup(u => u.User(It.IsAny<string>()))
                           .Returns<string>(u =>
                           {
                               if (string.Compare(u, userId1) == 0)
                                   return user1;
                               else
                                   return user2;
                           });
            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor and the number of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the entry code to the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(userId1), Times.Exactly(3));
            usersRepository.Verify(u => u.User(userId2), Times.Exactly(3));
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains one entry
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains two entries
        /// Address.Flats.First().Parameters.First() not null
        /// Address.Flats.First().Parameters.First().UserId not empty
        /// Address.Flats.First().Parameters.First().User not null
        /// Address.Flats.First().Parameters.Last() not null
        /// Address.Flats.First().Parameters.Last().UserId not empty
        /// Address.Flats.First().Parameters.Last().User not null
        /// User1 and User2 have a different lastname
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersTwoEntries_ParametersNotNull_UserId1NotEmpty_User1NotNull_UserId2NotEmpty_User2NotNull_DifferentLastName()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            string userId1 = "someUserId1";
            string firstName1 = "SomeFirstName1";
            string lastName1 = "SomeLastName";

            UserDTO user1 = new UserDTO
            {
                FirstName = firstName1,
                LastName = lastName1
            };

            string userId2 = "someUserId2";
            string firstName2 = "AnotherFirstName";
            string lastName2 = "SomeOtherLastName";

            UserDTO user2 = new UserDTO
            {
                FirstName = firstName2,
                LastName = lastName2
            };

            string initials = $"S {lastName1}/A {lastName2}";

            ParameterDTO parameter1 = new ParameterDTO { UserId = userId1 };
            ParameterDTO parameter2 = new ParameterDTO { UserId = userId2 };
            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter1, parameter2 };

            int floor = 1;
            string flatNumber = "someFlatNumber";
            string entryDoorCode = "someEntryDoorCode";
            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                Parameters = parameters
            };

            string våning = "Vån " + floor;
            string lagenhet = "L. " + flatNumber;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            usersRepository.Setup(u => u.User(It.IsAny<string>()))
                           .Returns<string>(u =>
                           {
                               if (string.Compare(u, userId1) == 0)
                                   return user1;
                               else
                                   return user2;
                           });
            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor and the number of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet, cells.ElementAt(1).InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there is only one flat defined at the given address, the table should only contain three rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(3, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the entry code to the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode, cells.First().InnerText);
                        Assert.Equal(initials, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(userId1), Times.Exactly(3));
            usersRepository.Verify(u => u.User(userId2), Times.Exactly(3));
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains two entries
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains one entry
        /// Address.Flats.First().Parameters.First() not null
        /// Address.Flats.First().Parameters.First().UserId not empty
        /// Address.Flats.First().Parameters.First().User not null
        /// Address.Flats.Last() not null
        /// Address.Flats.Last().Parameters contains one entry
        /// Address.Flats.Last().Parameters.First() not null
        /// Address.Flats.Last().Parameters.First().UserId not empty
        /// Address.Flats.Last().Parameters.First().User not null
        /// Flat1 and Flat2 are on the same floor
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsTwoEntries_Flat1NotNull_Flat2NotNull_SameFloors()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            string userId1 = "someUserId1";
            string firstName1 = "SomeFirstName";
            string lastName1 = "SomeLastName";

            string initials1 = "S SomeLastName";

            UserDTO user1 = new UserDTO
            {
                FirstName = firstName1,
                LastName = lastName1
            };

            ParameterDTO parameter1 = new ParameterDTO { UserId = userId1 };
            List<ParameterDTO> parameters1 = new List<ParameterDTO> { parameter1 };

            string userId2 = "someUserId2";
            string firstName2 = "AnotherFirstName";
            string lastName2 = "AnotherLastName";

            string initials2 = "A AnotherLastName";

            UserDTO user2 = new UserDTO
            {
                FirstName = firstName2,
                LastName = lastName2
            };

            ParameterDTO parameter2 = new ParameterDTO { UserId = userId2 };
            List<ParameterDTO> parameters2 = new List<ParameterDTO> { parameter2 };

            int floor = 1;
            string flatNumber1 = "someFlatNumber1";
            string entryDoorCode1 = "someEntryDoorCode1";
            FlatDTO flat1 = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                Parameters = parameters1
            };

            string flatNumber2 = "someFlatNumber2";
            string entryDoorCode2 = "someEntryDoorCode2";
            FlatDTO flat2 = new FlatDTO
            {
                Floor = floor,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                Parameters = parameters2
            };

            string våning = "Vån " + floor;
            string lagenhet1 = "L. " + flatNumber1;
            string lagenhet2 = "L. " + flatNumber2;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat1, flat2 }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            usersRepository.Setup(u => u.User(It.IsAny<string>()))
                           .Returns<string>(u =>
                           {
                               if (string.Compare(u, userId1) == 0)
                               {
                                   return user1;
                               }
                               else
                               {
                                   return user2;
                               }
                           });
            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there are two flats on the same floor, the table should contain four rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(4, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor and the number of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(lagenhet1, cells.ElementAt(1).InnerText);
                        Assert.Equal(initials1, cells.Last().InnerText);
                    }
                    // And the third row
                    // Since the user from the flat parameters is not null and the flats are on the same floor,
                    // should only be displayed the number of the flat and the user's initials
                    else if (rowNumber == 2)
                    {
                        Assert.Empty(cells.First().InnerText);
                        Assert.Equal(lagenhet2, cells.ElementAt(1).InnerText);
                        Assert.Equal(initials2, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there are two flats on the same floor, the table should contain four rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(4, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second line
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning, cells.First().InnerText);
                        Assert.Equal(initials1, cells.Last().InnerText);
                    }
                    // And the third
                    // Since the user from the flat parameters is not null and the flats are on the same floor,
                    // should only be displayed the user's initials
                    else if (rowNumber == 2)
                    {
                        Assert.Empty(cells.First().InnerText);
                        Assert.Equal(initials2, cells.Last().InnerText);
                    }
                    else

                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there are two flats on the same floor, the table should contain four rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(4, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the entry code to the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode1, cells.First().InnerText);
                        Assert.Equal(initials1, cells.Last().InnerText);
                    }
                    // And the third row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the entry code to the flat and the user's initials
                    else if (rowNumber == 2)
                    {
                        Assert.Equal(entryDoorCode2, cells.First().InnerText);
                        Assert.Equal(initials2, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(userId1), Times.Exactly(3));
            usersRepository.Verify(u => u.User(userId2), Times.Exactly(3));
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }

        [Fact]
        /// ZipFile not null
        /// UsersRepository not null
        /// Address not null
        /// Address.Number not null
        /// Address.Street not null
        /// Path not empty
        /// Address.Flats contains two entries
        /// Address.Flats.First() not null
        /// Address.Flats.First().Parameters contains one entry
        /// Address.Flats.First().Parameters.First() not null
        /// Address.Flats.First().Parameters.First().UserId not empty
        /// Address.Flats.First().Parameters.First().User not null
        /// Address.Flats.Last() not null
        /// Address.Flats.Last().Parameters contains one entry
        /// Address.Flats.Last().Parameters.First() not null
        /// Address.Flats.Last().Parameters.First().UserId not empty
        /// Address.Flats.Last().Parameters.First().User not null
        /// Flat1 and Flat2 are on different floors
        /// Creation succeeded
        public void Export_ParamsOK_AddressFlatsTwoEntries_Flat1NotNull_Flat2NotNull_DifferentFloors()
        {
            // Arrange
            Mock<IZipFile> zipFile = new Mock<IZipFile>();
            Mock<IUsersRepository> usersRepository = new Mock<IUsersRepository>();

            string userId1 = "someUserId1";
            string firstName1 = "SomeFirstName";
            string lastName1 = "SomeLastName";

            string initials1 = "S SomeLastName";

            UserDTO user1 = new UserDTO
            {
                FirstName = firstName1,
                LastName = lastName1
            };

            ParameterDTO parameter1 = new ParameterDTO { UserId = userId1 };
            List<ParameterDTO> parameters1 = new List<ParameterDTO> { parameter1 };

            string userId2 = "someUserId2";
            string firstName2 = "AnotherFirstName";
            string lastName2 = "AnotherLastName";

            string initials2 = "A AnotherLastName";

            UserDTO user2 = new UserDTO
            {
                FirstName = firstName2,
                LastName = lastName2
            };

            ParameterDTO parameter2 = new ParameterDTO { UserId = userId2 };
            List<ParameterDTO> parameters2 = new List<ParameterDTO> { parameter2 };

            int floor1 = 2;
            string flatNumber1 = "someFlatNumber1";
            string entryDoorCode1 = "someOtherEntryDoorCode";
            FlatDTO flat1 = new FlatDTO
            {
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                Parameters = parameters1
            };

            int floor2 = 1;
            string flatNumber2 = "someFlatNumber2";
            string entryDoorCode2 = "someFirstEntryDoorCode";
            FlatDTO flat2 = new FlatDTO
            {
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                Parameters = parameters2
            };

            string våning1 = "Vån " + floor1;
            string lagenhet1 = "L. " + flatNumber1;

            string våning2 = "Vån " + floor2;
            string lagenhet2 = "L. " + flatNumber2;

            string street = "someStreet";
            string number = "someAddressNumber";
            AddressDTO address = new AddressDTO
            {
                Street = street,
                Number = number,
                Flats = new List<FlatDTO> { flat1, flat2 }
            };

            string portkodstavla = $"{street} {number} portkodstavla";

            string path = nameof(Export_ParamsOK_AddressFlatsOneEntry_FlatNotNull_FlatParametersOneEntry_ParametersNull);

            string file1Name = $@"{path}\{street} {number}.docx";
            string file2Name = $@"{path}\{street} {number} - A5.docx";
            string file3Name = $@"{path}\{street} {number} - Portkodstavla.docx";
            string zipFileName = $"{street} {number}.zip";

            ZipResult zipResult = new ZipResult();

            usersRepository.Setup(u => u.User(It.IsAny<string>()))
                           .Returns<string>(u =>
                           {
                               if (string.Compare(u, userId1) == 0)
                               {
                                   return user1;
                               }
                               else
                               {
                                   return user2;
                               }
                           });
            zipFile.Setup(z => z.CreateZip(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .Returns(zipResult);

            // Act
            ZipResult result = Export.ExportUsers(zipFile.Object, usersRepository.Object, address, path);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zipResult, result);

            // - First file
            Assert.True(File.Exists(file1Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file1Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there are two flats on different floors, the table should contain five rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(5, rows.Count());

                // Each row should contain three columns
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(3, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor and the number of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning2, cells.First().InnerText);
                        Assert.Equal(lagenhet2, cells.ElementAt(1).InnerText);
                        Assert.Equal(initials2, cells.Last().InnerText);
                    }
                    // And the fourth row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor and the number of the flat and the user's initials
                    else if (rowNumber == 3)
                    {
                        Assert.Equal(våning1, cells.First().InnerText);
                        Assert.Equal(lagenhet1, cells.ElementAt(1).InnerText);
                        Assert.Equal(initials1, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Second file - A5 format
            Assert.True(File.Exists(file2Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file2Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there are two flats on different floors, the table should contain five rows,
                // the first row actually being used as a top margin and the last one as a separator between the floors
                var rows = table.Descendants<TableRow>();
                Assert.Equal(5, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second line
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor of the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(våning2, cells.First().InnerText);
                        Assert.Equal(initials2, cells.Last().InnerText);
                    }
                    // And the fourth row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the floor of the flat and the user's initials
                    else if (rowNumber == 3)
                    {
                        Assert.Equal(våning1, cells.First().InnerText);
                        Assert.Equal(initials1, cells.Last().InnerText);
                    }
                    else

                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            // - Third file - with door codes
            Assert.True(File.Exists(file3Name));
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(file3Name, false))
            {
                var tables = wordDocument.MainDocumentPart.RootElement.Descendants<Table>();

                // The document starts with a legend
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();
                Assert.Equal(portkodstavla, paragraphs.First().InnerText);

                // There should only be one table in the entire document
                Assert.Single(tables);
                Table table = tables.First();

                // Since there are two flats on different floors, the table should contain four rows,
                // the first row actually being used as a top margin and the last one as a bottom margin
                var rows = table.Descendants<TableRow>();
                Assert.Equal(4, rows.Count());

                // Each row should contain two columns and be empty
                int rowNumber = 0;
                foreach (TableRow row in rows)
                {
                    IEnumerable<TableCell> cells = row.Descendants<TableCell>();
                    Assert.Equal(2, cells.Count());

                    // Except for the second row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the entry code to the flat and the user's initials
                    if (rowNumber == 1)
                    {
                        Assert.Equal(entryDoorCode2, cells.First().InnerText);
                        Assert.Equal(initials2, cells.Last().InnerText);
                    }
                    // And the third row
                    // Since the user from the flat parameters is not null, should be displayed
                    // the entry code to the flat and the user's initials
                    else if (rowNumber == 2)
                    {
                        Assert.Equal(entryDoorCode1, cells.First().InnerText);
                        Assert.Equal(initials1, cells.Last().InnerText);
                    }
                    else
                    {
                        foreach (TableCell cell in cells)
                        {
                            Assert.Empty(cell.InnerText);
                        }
                    }

                    rowNumber += 1;
                }
            }

            usersRepository.Verify(u => u.User(userId1), Times.Exactly(3));
            usersRepository.Verify(u => u.User(userId2), Times.Exactly(3));
            zipFile.Verify(z => z.CreateZip(zipFileName, path, new List<string> { file1Name, file2Name, file3Name }),
                                Times.Once);

            // Cleans up the mess
            Directory.Delete(path, true);
        }
    }
}
