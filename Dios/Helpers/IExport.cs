using Dios.Models;
using Dios.Repositories;

namespace Dios.Helpers
{
    public interface IExport
    {
        ZipResult ExportUsers(IZipFile zipFile, IUsersRepository usersRepository, AddressDTO address, string path);
    }
}
