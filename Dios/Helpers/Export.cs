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

        private static IUsersRepository _usersRepository { get; set; }
        private static AddressDTO _address { get; set; }
        private static string _path { get; set; }

        public static ZipResult ExportUsers(IUsersRepository usersRepository, AddressDTO address, string path)
        {
            _usersRepository = usersRepository;
            _address = address;
            _path = path;

            // Cleans the old version of generated files
            if (Directory.Exists(path))
            {
                try
                {
                    foreach (string fName in Directory.GetFiles(path))
                    {
                        File.Delete(fName);
                    }
                }
                catch (IOException)
                {
                    throw;
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }

            // Let's start with the A4 "all users" list
            string allUsers = ExportAllUsers();

            if (string.IsNullOrEmpty(allUsers))
            {
                throw new ExportUsersException();
            }

            // Then the 'A5' version
            string usersA5 = ExportUsersA5();

            if (string.IsNullOrEmpty(usersA5))
            {
                throw new ExportUsersException();
            }

            // And finally the list with entrydoor codes
            string entryDoorCodes = ExportUsersEntryDoorCodes();

            if (string.IsNullOrEmpty(entryDoorCodes))
            {
                throw new ExportUsersException();
            }

            return ZipFile.CreateZip(DocumentName(address, ExportFormat.undefined, ".zip"),
                                     path,
                                     new List<string> { allUsers, usersA5, entryDoorCodes });
        }

        private static string DocumentName(AddressDTO address, ExportFormat exportFormat, string extention = ".docx")
        {
            if (address == null)
            {
                return string.Empty;
            }

            string fileName = address.Street + address.Number;

            switch (exportFormat)
            {
                case ExportFormat.a5:
                    fileName += "A5";
                    break;
                case ExportFormat.entryDoorCodes:
                    fileName += "Portkodstavla";
                    break;
                default:
                    break;
            }

            fileName += extention;

            return fileName;
        }

        private static string ExportAllUsers()
        {
            string fileName = DocumentName(_address, ExportFormat.all);
            string filePath = Path.Combine(_path, fileName);

            // Create Document
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document, true))
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
                                                               .GroupBy(f => f.Floor)
                                                               .OrderByDescending(fg => fg.Key)
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
                        if (tc1 == null)
                        {
                            tc1 = new TableCellRightRedLine();
                        }

                        // First column - Floor
                        tr = new TableRow();
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
                        Run r = new Run();
                        RunProperties rp = new RunProperties
                        {
                            FontSize = new FontSize { Val = SMALLFONTSIZE_ALLUSERS },
                            RunFonts = new RunFonts { HighAnsi = FONT_ALLUSERS, ComplexScript = FONT_ALLUSERS }
                        };
                        r.Append(rp);
                        r.Append(new Text("L. " + flat.Number));
                        p.Append(r);
                        tc2.Append(p);
                        tr.Append(tc2);

                        // Third column - Users
                        Dictionary<string, List<string>> users = flat.Parameters
                                                                     .Select(param =>
                                                                     {
                                                                         param.User = _usersRepository.User(param.UserId);
                                                                         return param;
                                                                     })
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
                        r = new Run();
                        rp = new RunProperties
                        {
                            FontSize = new FontSize { Val = MEDIUMFONTSIZE_ALLUSERS },
                            RunFonts = new RunFonts { HighAnsi = FONT_ALLUSERS, ComplexScript = FONT_ALLUSERS }
                        };
                        r.Append(rp);
                        r.Append(new Text(GetUsersNames(users)));
                        p.Append(r);
                        tc3.Append(p);
                        tr.Append(tc3);

                        // Add a new row
                        table.Append(tr);

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
                PageMargin pageMargin = new PageMargin()
                {
                    Top = 1008,
                    Right = 1008U,
                    Bottom = 1008,
                    Left = 1008U,
                    Header = 720U,
                    Footer = 720U,
                    Gutter = 0U
                };
                sectionProps.Append(pageMargin);
                docBody.Append(sectionProps);

                mainPart.Document.Body = docBody;
            }

            return filePath;
        }

        private static string ExportUsersEntryDoorCodes()
        {
            string fileName = DocumentName(_address, ExportFormat.entryDoorCodes);
            string filePath = Path.Combine(_path, fileName);

            // Create Document
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document, true))
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
                Run run = new Run();
                RunProperties runProperties = new RunProperties
                {
                    FontSize = new FontSize { Val = SMALLFONTSIZE_ENTRYDOORCODES },
                    RunFonts = new RunFonts { HighAnsi = FONT_ENTRYDOORCODES, ComplexScript = FONT_ENTRYDOORCODES }
                };
                Text t = new Text
                {
                    Text = string.Format("{0} {1} portkodstavla", _address.Street, _address.Number)
                };
                runProperties.Append(t);
                run.Append(runProperties);
                paragraph.Append(run);
                docBody.Append(paragraph);

                Table table = new Table();

                table.Append(new TableProperties { TableBorders = new TableBorders() });

                // Empty row
                TableRow tr = new TableRow();
                TableCell tc1 = new TableCellRightRedLine();
                TableCell tc2 = new TableCell(new Paragraph(new Run(new Text())));

                tr.Append(tc1);
                tr.Append(tc2);

                table.Append(tr);

                foreach (FlatDTO flat in _address.Flats.OrderBy(f => f.EntryDoorCode))
                {
                    tr = new TableRow();

                    // First column - Floor
                    tc1 = new TableCellRightRedLine(flat.EntryDoorCode, ExportFormat.entryDoorCodes);
                    tr.Append(tc1);

                    // Second column - Users
                    Dictionary<string, List<string>> users = flat.Parameters
                                                                 .Select(param =>
                                                                 {
                                                                     param.User = _usersRepository.User(param.UserId);
                                                                     return param;
                                                                 })
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
                    run = new Run();
                    runProperties = new RunProperties
                    {
                        FontSize = new FontSize { Val = MEDIUMFONTSIZE_ENTRYDOORCODES },
                        RunFonts = new RunFonts { HighAnsi = FONT_ENTRYDOORCODES, ComplexScript = FONT_ENTRYDOORCODES }
                    };
                    run.Append(runProperties);
                    run.Append(new Text(GetUsersNames(users)));
                    paragraph.Append(run);
                    tc2.Append(paragraph);
                    tr.Append(tc2);

                    // Add a new row
                    table.Append(tr);
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
                PageMargin pageMargin = new PageMargin()
                {
                    Top = 1008,
                    Right = 1008U,
                    Bottom = 1008,
                    Left = 1008U,
                    Header = 720U,
                    Footer = 720U,
                    Gutter = 0U
                };
                sectionProps.Append(pageMargin);
                docBody.Append(sectionProps);

                mainPart.Document.Body = docBody;
            }

            return filePath;
        }

        private static string ExportUsersA5()
        {
            string fileName = DocumentName(_address, ExportFormat.a5);
            string filePath = Path.Combine(_path, fileName);

            // Create Document
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document, true))
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

                Dictionary<int, List<FlatDTO>> flats = _address.Flats
                                                               .GroupBy(f => f.Floor)
                                                               .OrderByDescending(fg => fg.Key)
                                                               .ToDictionary(fg => fg.Key,
                                                                             fg => fg.Select(f => f)
                                                                                     .ToList());
                foreach (int floor in flats.Keys)
                {
                    tc1 = new TableCellRightRedLine("Vån " + floor.ToString(), ExportFormat.a5);

                    foreach (FlatDTO flat in flats[floor].Where(f => f.Parameters.Count > 0))
                    {
                        Dictionary<string, List<string>> users = flat.Parameters
                                                                     .Select(param =>
                                                                     {
                                                                         param.User = _usersRepository.User(param.UserId);
                                                                         return param;
                                                                     })
                                                                     .GroupBy(param => param.User.LastName)
                                                                     .OrderBy(param => param.Key)
                                                                     .ToDictionary(pg => pg.Key,
                                                                                   pg => pg.Select(param => param.User.FirstName)
                                                                                           .OrderBy(fn => fn)
                                                                                           .ToList());

                        tr = new TableRow();
                        // First column - Floor
                        if (tc1 == null)
                        {
                            tc1 = new TableCellRightRedLine(string.Empty, ExportFormat.a5);
                        }

                        tr.Append(tc1);

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
                        Run r = new Run();
                        RunProperties rp = new RunProperties
                        {
                            FontSize = new FontSize { Val = MEDIUMFONTSIZE_A5 },
                            RunFonts = new RunFonts { HighAnsi = FONT_A5, ComplexScript = FONT_A5 }
                        };
                        r.Append(rp);
                        r.Append(new Text(GetUsersNames(users)));
                        p.Append(r);
                        tc2.Append(p);

                        // Second column - Users
                        tr.Append(tc2);

                        table.Append(tr);

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

                PageMargin pageMargin = new PageMargin()
                {
                    Top = 1008,
                    Right = 1008U,
                    Bottom = 1008,
                    Left = 1008U,
                    Header = 720U,
                    Footer = 720U,
                    Gutter = 0U
                };
                sectionProps.Append(pageMargin);
                docBody.Append(sectionProps);

                mainPart.Document.Body = docBody;
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

        #endregion
    }

    /// <summary>
    /// Class allowing the Export static class to be mocked while testing
    /// </summary>
    public class ExportWrapper : IExport
    {
        public ZipResult ExportUsers(IUsersRepository usersRepository, AddressDTO address, string path)
        {
            return ExportUsers(usersRepository, address, path);
        }
    }
}
