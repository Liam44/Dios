using System.Collections.Generic;

namespace Dios.Helpers
{
    public interface IZipFile
    {
        ZipResult CreateZip(string fileName, string path, List<string> files);
    }
}
