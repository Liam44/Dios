using Dios.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Dios.Helpers
{
    public static class ZipFile
    {
        public static ZipResult CreateZip(string fileName, string path, List<string> files)
        {
            string zipFilePath = Path.Combine(path, fileName);
            using (FileStream zipToCreate = new FileStream(zipFilePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    foreach (string file in files)
                    {
                        // Create a new entry in the zip archive
                        archive.CreateEntryFromFile(file, new FileInfo(file).Name);
                    }
                }
            }

            MemoryStream mem = new MemoryStream();
            using (var stream = new FileStream(zipFilePath, FileMode.Open))
            {
                stream.CopyToAsync(mem).Wait();
            }

            mem.Position = 0;

            return new ZipResult
            {
                FileName = fileName,
                MemoryStream = mem,
                ContentType = "application/zip"
            };
        }
    }

    public class ZipResult
    {
        public string FileName { get; set; }
        public MemoryStream MemoryStream { get; set; }
        public string ContentType { get; set; }
    }
}
