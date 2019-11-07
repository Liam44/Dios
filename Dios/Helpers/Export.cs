using Dios.Exceptions;
using Dios.Models;
using Dios.Repositories;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dios.Helpers
{
    public enum ExportFormat
    {
        undefined,
        all,
        a5,
        entryDoorCodes
    }

    public static class Export
    {
        private const string FONT_ALLUSERS = "Arimo;arial";
        private const string BIGFONTSIZE_ALLUSERS = "56";
        private const string MEDIUMFONTSIZE_ALLUSERS = "52";
        private const string SMALLFONTSIZE_ALLUSERS = "20";

        private const string FONT_A5 = "Arimo;arial";
        private const string BIGFONTSIZE_A5 = "56";
        private const string MEDIUMFONTSIZE_A5 = "52";

        private const string FONT_ENTRYDOORCODES = "Arimo;arial";
        private const string BIGFONTSIZE_ENTRYDOORCODES = "56";
        private const string MEDIUMFONTSIZE_ENTRYDOORCODES = "52";
        private const string SMALLFONTSIZE_ENTRYDOORCODES = "40";

        private const int MARGIN_TOP = 1008;
        private const uint MARGIN_RIGHT = 1008;
        private const int MARGIN_BOTTOM = 1008;
        private const uint MARGIN_LEFT = 1008;
        private const uint MARGIN_HEADER = 720;
        private const uint MARGIN_FOOTER = 720;
        private const uint MARGIN_GUTTER = 0;

        private static IUsersRepository _usersRepository { get; set; }
        private static AddressDTO _address { get; set; }
        private static string _path { get; set; }

        /// <summary>
        /// Export the list of people living at a given address, according to 3 standard formats
        /// </summary>
        /// <param name="zipFile">Object implementing the IZipFile interface, allowing to physically create the ZIP file</param>
        /// <param name="usersRepository">Object implementing the IUsersRepository allowing to retrieve information about users in the database</param>
        /// <param name="address">Address where the people to be listed are living</param>
        /// <param name="path">Path of the folder the temporary files should be saved</param>
        /// <returns>Returns a ZIP file containing 3 .DOCX files respectively named "<address>.docx",
        /// "<address> - Portkodstavla.docx" and "<address> - a5.docx".</returns>
        public static ZipResult ExportUsers(IZipFile zipFile,
                                            IUsersRepository usersRepository,
                                            AddressDTO address,
                                            string path)
        {
            if (zipFile == null)
            {
                return new ZipResult();
            }

            if (usersRepository == null)
            {
                return new ZipResult();
            }

            if (address == null)
            {
                return new ZipResult();
            }

            if (string.IsNullOrEmpty(address.Street) || string.IsNullOrEmpty(address.Number))
            {
                return new ZipResult();
            }

            if (string.IsNullOrEmpty(path))
            {
                return new ZipResult();
            }

            _usersRepository = usersRepository;
            _address = address;
            _path = path;

            // Cleans the old version of generated files
            if (Directory.Exists(_path))
            {
                try
                {
                    ClearDirectory();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(_path);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            // Let's start with the A4 "all users" list
            string allUsers = ExportAllUsers();

            if (string.IsNullOrEmpty(allUsers))
            {
                ClearDirectory();
                throw new ExportUsersException();
            }

            // Then the 'A5' version
            string usersA5 = ExportUsersA5();

            if (string.IsNullOrEmpty(usersA5))
            {
                ClearDirectory();
                throw new ExportUsersException();
            }

            // And finally the list with entrydoor codes
            string entryDoorCodes = ExportUsersEntryDoorCodes();

            if (string.IsNullOrEmpty(entryDoorCodes))
            {
                ClearDirectory();
                throw new ExportUsersException();
            }

            return zipFile.CreateZip(DocumentName(ExportFormat.undefined, ".zip"),
                                     _path,
                                     new List<string> { allUsers, usersA5, entryDoorCodes });
        }

        private static void ClearDirectory()
        {
            try
            {
                foreach (string fName in Directory.GetFiles(_path))
                {
                    File.Delete(fName);
                }
            }
            catch (IOException)
            {
                throw;
            }
        }

        private static string DocumentName(ExportFormat exportFormat, string extention = ".docx")
        {
            string fileName = $"{_address.Street} {_address.Number}";

            switch (exportFormat)
            {
                case ExportFormat.a5:
                    fileName += " - A5";
                    break;
                case ExportFormat.entryDoorCodes:
                    fileName += " - Portkodstavla";
                    break;
                default:
                    break;
            }

            fileName += extention;

            return fileName;
        }

        private static string ExportAllUsers()
        {
            string filePath = Path.Combine(_path, DocumentName(ExportFormat.all));

            // Create Document
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath,
                                                                                       WordprocessingDocumentType.Document,
                                                                                       true))
            {
                // Add a main document part
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                // Create the document structure and add some text
                mainPart.Document = new Document();
                Body docBody = new Body();

                Table table = new Table();

                table.Append(new TableProperties { TableBorders = new TableBorders() });

                // Empty row
                TableRow tr = new TableRow();
                TableCell tc1 = new TableCellRightRedLine();
                TableCell tc2 = new TableCell(new Paragraph(new Run(new Text())));
                TableCell tc3 = new TableCell(new Paragraph(new Run(new Text())));

                tr.Append(tc1);
                tr.Append(tc2);
                tr.Append(tc3);

                table.Append(tr);

                Dictionary<int, List<FlatDTO>> flats = _address.Flats?
                                                               .Where(f => f != null)
                                                               .Select(f => f)
                                                               .GroupBy(f => f.Floor)
                                                               .OrderBy(fg => fg.Key)
                                                               .ToDictionary(fg => fg.Key,
                                                                             fg => fg.Select(f => f)
                                                                                     .ToList());

                if (flats == null)
                {
                    return string.Empty;
                }

                foreach (int floor in flats.Keys)
                {
                    tc1 = new TableCellRightRedLine("Vån " + floor.ToString(), ExportFormat.all);

                    foreach (FlatDTO flat in flats[floor])
                    {
                        // Add a new row
                        tr = new TableRow();
                        table.Append(tr);

                        // First column - Floor
                        if (tc1 == null)
                        {
                            tc1 = new TableCellRightRedLine();
                        }
                        tr.Append(tc1);

                        // Second column - Flat number
                        tc2 = new TableCell
                        {
                            TableCellProperties = new TableCellProperties
                            {
                                TableCellVerticalAlignment = new TableCellVerticalAlignment
                                {
                                    Val = TableVerticalAlignmentValues.Center
                                }
                            }
                        };
                        tr.Append(tc2);

                        Paragraph p = new Paragraph
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                TextAlignment = new TextAlignment
                                {
                                    Val = VerticalTextAlignmentValues.Center
                                },
                                Indentation = new Indentation
                                {
                                    Left = "170",
                                    Right = "170",
                                    Hanging = "0"
                                },
                                SpacingBetweenLines = new SpacingBetweenLines
                                {
                                    Before = "150",
                                    After = "0"
                                }
                            }
                        };
                        tc2.Append(p);

                        RunProperties rp = new RunProperties
                        {
                            FontSize = new FontSize { Val = SMALLFONTSIZE_ALLUSERS },
                            RunFonts = new RunFonts { HighAnsi = FONT_ALLUSERS, ComplexScript = FONT_ALLUSERS }
                        };

                        Run r = new Run();
                        r.Append(rp);
                        r.Append(new Text("L. " + flat.Number));

                        p.Append(r);

                        // Third column - Users
                        Dictionary<string, List<string>> users = flat.Parameters?
                                                                     .Where(param => param != null)
                                                                     .Select(param =>
                                                                     {
                                                                         param.User = _usersRepository.User(param.UserId);
                                                                         return param;
                                                                     })
                                                                     .Where(param => param.User != null)
                                                                     .Select(param => param)
                                                                     .GroupBy(param => param.User.LastName)
                                                                     .OrderBy(param => param.Key)
                                                                     .ToDictionary(pg => pg.Key,
                                                                                   pg => pg.Select(param => param.User.FirstName)
                                                                                           .OrderBy(fn => fn)
                                                                                           .ToList());

                        tc3 = new TableCell
                        {
                            TableCellProperties = new TableCellProperties
                            {
                                TableCellVerticalAlignment = new TableCellVerticalAlignment
                                {
                                    Val = TableVerticalAlignmentValues.Bottom
                                }
                            }
                        };
                        tr.Append(tc3);

                        p = new Paragraph
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                TextAlignment = new TextAlignment
                                {
                                    Val = VerticalTextAlignmentValues.Center
                                }
                            }
                        };
                        tc3.Append(p);

                        rp = new RunProperties
                        {
                            FontSize = new FontSize { Val = MEDIUMFONTSIZE_ALLUSERS },
                            RunFonts = new RunFonts { HighAnsi = FONT_ALLUSERS, ComplexScript = FONT_ALLUSERS }
                        };

                        r = new Run();
                        r.Append(rp);
                        p.Append(r);

                        if (users != null)
                        {
                            r.Append(new Text(GetUsersNames(users)));
                        }

                        tc1 = null;
                    }

                    // Empty row between all floors
                    tr = new TableRow();
                    tc1 = new TableCellRightRedLine();
                    tc2 = new TableCell(new Paragraph(new Run(new Text())));
                    tc3 = new TableCell(new Paragraph(new Run(new Text())));

                    tr.Append(tc1);
                    tr.Append(tc2);
                    tr.Append(tc3);

                    table.Append(tr);
                }

                // Add your table to docx body
                docBody.Append(table);

                // Set the margins in the page
                SectionProperties sectionProps = new SectionProperties();
                sectionProps.Append(new StandardPageMargin());
                docBody.Append(sectionProps);

                mainPart.Document.Body = docBody;

                wordDocument.Save();
            }

            return filePath;
        }

        private static string ExportUsersEntryDoorCodes()
        {
            string filePath = Path.Combine(_path, DocumentName(ExportFormat.entryDoorCodes));

            // Create Document
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath,
                                                                                       WordprocessingDocumentType.Document,
                                                                                       true))
            {
                // Add a main document part
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                // Create the document structure and add some text
                mainPart.Document = new Document();
                Body docBody = new Body();

                Paragraph paragraph = new Paragraph
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        Justification = new Justification { Val = JustificationValues.Right }
                    }
                };
                docBody.Append(paragraph);

                Run run = new Run();
                paragraph.Append(run);

                RunProperties runProperties = new RunProperties
                {
                    FontSize = new FontSize { Val = SMALLFONTSIZE_ENTRYDOORCODES },
                    RunFonts = new RunFonts { HighAnsi = FONT_ENTRYDOORCODES, ComplexScript = FONT_ENTRYDOORCODES }
                };
                run.Append(runProperties);

                Text t = new Text
                {
                    Text = $"{_address.Street} {_address.Number} portkodstavla"
                };
                runProperties.Append(t);

                Table table = new Table();
                table.Append(new TableProperties { TableBorders = new TableBorders() });

                // Empty row
                TableRow tr = new TableRow();
                table.Append(tr);

                TableCell tc1 = new TableCellRightRedLine();
                tr.Append(tc1);

                TableCell tc2 = new TableCell(new Paragraph(new Run(new Text())));
                tr.Append(tc2);

                foreach (FlatDTO flat in _address.Flats?
                                                 .Where(f => f != null)
                                                 .Select(f => f)
                                                 .OrderBy(f => f.EntryDoorCode))
                {
                    // Add a new row
                    tr = new TableRow();
                    table.Append(tr);

                    // First column - Floor
                    tc1 = new TableCellRightRedLine(flat.EntryDoorCode, ExportFormat.entryDoorCodes);
                    tr.Append(tc1);

                    // Second column - Users
                    Dictionary<string, List<string>> users = flat.Parameters?
                                                                 .Where(param => param != null)
                                                                 .Select(param =>
                                                                 {
                                                                     param.User = _usersRepository.User(param.UserId);
                                                                     return param;
                                                                 })
                                                                 .Where(param => param.User != null)
                                                                 .Select(param => param)
                                                                 .GroupBy(param => param.User.LastName)
                                                                 .OrderBy(param => param.Key)
                                                                 .ToDictionary(pg => pg.Key,
                                                                               pg => pg.Select(param => param.User.FirstName)
                                                                                       .OrderBy(fn => fn)
                                                                                       .ToList());

                    tc2 = new TableCell
                    {
                        TableCellProperties = new TableCellProperties
                        {
                            TableCellVerticalAlignment = new TableCellVerticalAlignment
                            {
                                Val = TableVerticalAlignmentValues.Bottom
                            }
                        }
                    };
                    tr.Append(tc2);

                    paragraph = new Paragraph
                    {
                        ParagraphProperties = new ParagraphProperties
                        {
                            TextAlignment = new TextAlignment
                            {
                                Val = VerticalTextAlignmentValues.Center
                            },
                            Indentation = new Indentation
                            {
                                Left = "170",
                                Hanging = "0"
                            }
                        }
                    };
                    tc2.Append(paragraph);

                    runProperties = new RunProperties
                    {
                        FontSize = new FontSize { Val = MEDIUMFONTSIZE_ENTRYDOORCODES },
                        RunFonts = new RunFonts { HighAnsi = FONT_ENTRYDOORCODES, ComplexScript = FONT_ENTRYDOORCODES }
                    };

                    run = new Run();
                    run.Append(runProperties);
                    paragraph.Append(run);

                    if (users != null)
                    {
                        run.Append(new Text(GetUsersNames(users)));
                    }
                }

                // Empty row in the end of the table
                tr = new TableRow();
                tc1 = new TableCellRightRedLine();
                tc2 = new TableCell(new Paragraph(new Run(new Text())));

                tr.Append(tc1);
                tr.Append(tc2);

                table.Append(tr);

                // Add your table to docx body
                docBody.Append(table);

                // Set the margins in the page
                SectionProperties sectionProps = new SectionProperties();
                sectionProps.Append(new StandardPageMargin());
                docBody.Append(sectionProps);

                mainPart.Document.Body = docBody;

                wordDocument.Save();
            }

            return filePath;
        }

        private static string ExportUsersA5()
        {
            string filePath = Path.Combine(_path, DocumentName(ExportFormat.a5));

            // Create Document
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath,
                                                                                       WordprocessingDocumentType.Document,
                                                                                       true))
            {
                // Add a main document part
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                // Create the document structure and add some text
                mainPart.Document = new Document();
                Body docBody = new Body();

                Table table = new Table();

                table.Append(new TableProperties { TableBorders = new TableBorders() });

                // Empty row
                TableRow tr = new TableRow();
                TableCell tc1 = new TableCellRightRedLine();
                TableCell tc2 = new TableCell(new Paragraph(new Run(new Text())));

                tr.Append(tc1);
                tr.Append(tc2);

                table.Append(tr);

                Dictionary<int, List<FlatDTO>> flats = _address.Flats?
                                                               .Where(f => f != null)
                                                               .Select(f => f)
                                                               .GroupBy(f => f.Floor)
                                                               .OrderBy(fg => fg.Key)
                                                               .ToDictionary(fg => fg.Key,
                                                                             fg => fg.Select(f => f)
                                                                                     .ToList());
                if (flats == null)
                {
                    return string.Empty;
                }

                foreach (int floor in flats.Keys)
                {
                    tc1 = new TableCellRightRedLine("Vån " + floor.ToString(), ExportFormat.a5);

                    foreach (FlatDTO flat in flats[floor].Where(f => f.Parameters?.Count > 0))
                    {
                        Dictionary<string, List<string>> users = flat.Parameters
                                                                     .Where(param => param != null)
                                                                     .Select(param =>
                                                                     {
                                                                         param.User = _usersRepository.User(param.UserId);
                                                                         return param;
                                                                     })
                                                                     .Where(param => param.User != null)
                                                                     .Select(param => param)
                                                                     .GroupBy(param => param.User.LastName)
                                                                     .OrderBy(param => param.Key)
                                                                     .ToDictionary(pg => pg.Key,
                                                                                   pg => pg.Select(param => param.User.FirstName)
                                                                                           .OrderBy(fn => fn)
                                                                                           .ToList());

                        // Add a new row
                        tr = new TableRow();
                        table.Append(tr);

                        // First column - Floor
                        if (tc1 == null)
                        {
                            tc1 = new TableCellRightRedLine(string.Empty, ExportFormat.a5);
                        }
                        tr.Append(tc1);

                        // Second column - Users
                        tc2 = new TableCell
                        {
                            TableCellProperties = new TableCellProperties
                            {
                                TableCellVerticalAlignment = new TableCellVerticalAlignment
                                {
                                    Val = TableVerticalAlignmentValues.Bottom
                                }
                            }
                        };
                        tr.Append(tc2);

                        Paragraph p = new Paragraph
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                TextAlignment = new TextAlignment
                                {
                                    Val = VerticalTextAlignmentValues.Center
                                },
                                Indentation = new Indentation
                                {
                                    Left = "170",
                                    Hanging = "0"
                                }
                            }
                        };
                        tc2.Append(p);

                        RunProperties rp = new RunProperties
                        {
                            FontSize = new FontSize { Val = MEDIUMFONTSIZE_A5 },
                            RunFonts = new RunFonts { HighAnsi = FONT_A5, ComplexScript = FONT_A5 }
                        };

                        Run r = new Run();
                        r.Append(rp);
                        p.Append(r);

                        if (users != null)
                        {
                            r.Append(new Text(GetUsersNames(users)));
                        }

                        tc1 = null;
                    }

                    if (tc1 != null)
                    {
                        tr = new TableRow();
                        tr.Append(tc1);
                        tr.Append(new TableCell(new Paragraph(new Run(new RunProperties(new Text())))));

                        table.Append(tr);
                    }
                    else if (tc2 == null)
                    {
                        tc2 = new TableCell(new Paragraph(new Run(new Text())));
                        tr.Append(tc2);

                        table.Append(tr);
                    }

                    // Empty row between all floors
                    tr = new TableRow();
                    tc1 = new TableCellRightRedLine();
                    tc2 = new TableCell(new Paragraph(new Run(new Text())));

                    tr.Append(tc1);
                    tr.Append(tc2);

                    table.Append(tr);
                }

                // Add your table to docx body
                docBody.Append(table);

                // Set the margins in the page
                SectionProperties sectionProps = new SectionProperties();
                PageSize pageSize = new PageSize
                {
                    Orient = PageOrientationValues.Landscape,
                    Height = 11906U,
                    Width = 16838
                };
                sectionProps.Append(pageSize);

                sectionProps.Append(new StandardPageMargin());
                docBody.Append(sectionProps);

                mainPart.Document.Body = docBody;

                wordDocument.Save();
            }

            return filePath;
        }

        private static string GetUsersNames(Dictionary<string, List<string>> users)
        {
            string text = string.Empty;

            foreach (string lastName in users.Keys)
            {
                foreach (string firstName in users[lastName])
                {
                    text += string.Join("-", firstName.Split("-").Select(n => n.Substring(0, 1)).ToList()) + " & ";
                }

                // Gets rid of the last '&'
                text = string.Join(" & ", text.Split(" & ", StringSplitOptions.RemoveEmptyEntries));

                text += " " + lastName + "/";
            }

            // Gets rid of the last '/'
            return string.Join("/", text.Split("/", StringSplitOptions.RemoveEmptyEntries));
        }

        #region Private classes

        private class TableCellRightRedLine : TableCell
        {
            public TableCellRightRedLine(params OpenXmlElement[] children) : base(children)
            {
                SetTableCellProperties();

                if (children.Count() == 0)
                {
                    Append(new Paragraph(new Run(new RunProperties(new Text(string.Empty)))));
                }

            }

            public TableCellRightRedLine(string text, ExportFormat exportFormat) : base()
            {
                SetTableCellProperties();
                SetParagraphProperties(text, exportFormat);
            }

            private void SetTableCellProperties()
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth
                    {
                        Width = "0",
                        Type = TableWidthUnitValues.Auto
                    },
                    TableCellMargin = new TableCellMargin
                    {
                        LeftMargin = new LeftMargin
                        {
                            Width = "20",
                            Type = TableWidthUnitValues.Dxa
                        },
                        BottomMargin = new BottomMargin
                        {
                            Width = "0",
                            Type = TableWidthUnitValues.Auto
                        }
                    },
                    TableCellBorders = new TableCellBorders
                    {
                        RightBorder = new RightBorder
                        {
                            Color = "FF0000",
                            Size = 40,
                            Space = 0,
                            Val = BorderValues.Single
                        }
                    }
                };
            }

            private void SetParagraphProperties(string text, ExportFormat exportFormat)
            {
                Paragraph p = null;
                Run r = null;
                RunProperties rp = null;

                switch (exportFormat)
                {
                    case ExportFormat.all:
                        p = new Paragraph
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                TextAlignment = new TextAlignment
                                {
                                    Val = VerticalTextAlignmentValues.Center
                                },
                                SpacingBetweenLines = new SpacingBetweenLines
                                {
                                    Before = "150",
                                    After = "0"
                                },
                                Indentation = new Indentation
                                {
                                    Left = "0",
                                    Right = "283",
                                    Hanging = "0"
                                }
                            }
                        };
                        r = new Run();
                        rp = new RunProperties
                        {
                            FontSize = new FontSize { Val = BIGFONTSIZE_ALLUSERS },
                            FontSizeComplexScript = new FontSizeComplexScript { Val = BIGFONTSIZE_ALLUSERS },
                            RunFonts = new RunFonts { HighAnsi = FONT_ALLUSERS, ComplexScript = FONT_ALLUSERS }
                        };
                        break;
                    case ExportFormat.a5:
                        p = new Paragraph
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                TextAlignment = new TextAlignment
                                {
                                    Val = VerticalTextAlignmentValues.Center
                                },
                                SpacingBetweenLines = new SpacingBetweenLines
                                {
                                    Before = "150",
                                    After = "0"
                                },
                                Indentation = new Indentation
                                {
                                    Left = "0",
                                    Right = "283",
                                    Hanging = "0"
                                }
                            }
                        };
                        r = new Run();
                        rp = new RunProperties
                        {
                            FontSize = new FontSize { Val = BIGFONTSIZE_A5 },
                            FontSizeComplexScript = new FontSizeComplexScript { Val = BIGFONTSIZE_A5 },
                            RunFonts = new RunFonts { HighAnsi = FONT_A5, ComplexScript = FONT_A5 }
                        };
                        break;
                    case ExportFormat.entryDoorCodes:
                        p = new Paragraph
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                TextAlignment = new TextAlignment
                                {
                                    Val = VerticalTextAlignmentValues.Center
                                },
                                SpacingBetweenLines = new SpacingBetweenLines
                                {
                                    Before = "150",
                                    After = "0"
                                },
                                Indentation = new Indentation
                                {
                                    Left = "0",
                                    Right = "283",
                                    Hanging = "0"
                                }
                            }
                        };
                        r = new Run();
                        rp = new RunProperties
                        {
                            FontSize = new FontSize { Val = BIGFONTSIZE_ENTRYDOORCODES },
                            FontSizeComplexScript = new FontSizeComplexScript { Val = BIGFONTSIZE_ENTRYDOORCODES },
                            RunFonts = new RunFonts { HighAnsi = FONT_ENTRYDOORCODES, ComplexScript = FONT_ENTRYDOORCODES }
                        };

                        break;
                }

                r.Append(rp);
                r.Append(new Text(text));
                p.Append(r);

                Append(p);
            }
        }

        private class StandardPageMargin : PageMargin
        {
            public StandardPageMargin(int top = MARGIN_TOP,
                                      uint right = MARGIN_RIGHT,
                                      int bottom = MARGIN_BOTTOM,
                                      uint left = MARGIN_LEFT,
                                      uint header = MARGIN_HEADER,
                                      uint footer = MARGIN_FOOTER,
                                      uint gutter = MARGIN_GUTTER)
            {
                Top = top;
                Right = right;
                Bottom = bottom;
                Left = left;
                Header = header;
                Footer = footer;
                Gutter = gutter;
            }
        }

        #endregion
    }

    /// <summary>
    /// Class allowing the Export static class to be mocked while testing
    /// </summary>
    public sealed class ExportWrapper : IExport
    {
        public ZipResult ExportUsers(IZipFile zipFile, IUsersRepository usersRepository, AddressDTO address, string path)
        {
            return Export.ExportUsers(zipFile, usersRepository, address, path);
        }
    }
}
