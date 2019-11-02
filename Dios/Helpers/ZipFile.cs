using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Dios.Helpers
{
    public static class ZipFile
    {
        public static ZipResult CreateZip(string path, string fileName, List<string> files)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(fileName))
            {
                return new ZipResult();
            }

            if (string.Compare(fileName.Substring(fileName.Length - 4), ".zip", true) != 0)
            {
                fileName += ".zip";
            }

            if (files == null)
            {
                files = new List<string>();
            }

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

            return new ZipResult(fileName, mem);
        }
    }

    public sealed class ZipResult
    {
        public string FileName { get; private set; }
        public MemoryStream MemoryStream { get; private set; }
        public string ContentType
        {
            get
            {
                return "application/zip";
            }
            private set { }
        }

        public ZipResult()
        {
            FileName = string.Empty;
            MemoryStream = new MemoryStream();
        }

        public ZipResult(string fileName, MemoryStream memoryStream)
        {
            FileName = fileName;
            if (memoryStream == null)
            {
                MemoryStream = new MemoryStream();
            }
            else
            {
                MemoryStream = memoryStream;
            }
        }
    }

    /// <summary>
    /// Class allowing the ZipFile static class to be mocked while testing
    /// </summary>
    public sealed class ZipFileWrapper : IZipFile
    {
        public ZipResult CreateZip(string fileName, string path, List<string> files)
        {
            return ZipFile.CreateZip(fileName, path, files);
        }
    }
}
