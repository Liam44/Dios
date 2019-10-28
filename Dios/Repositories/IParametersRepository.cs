using Dios.Models;
using System.Collections.Generic;

namespace Dios.Repositories
{
    public interface IParametersRepository
    {
        int Add(ParameterDTO parameter);
        int AddUsers(int flatId, List<string> userIds);
        ParameterDTO Get(string uid, int fid);
        int Edit(ParameterDTO parameter);
        List<ParameterDTO> AllParameters();
        int Delete(string userId, int flatId);
        IEnumerable<ParameterDTO> Parameters(string userId, int? addressId = null);
        IEnumerable<ParameterDTO> Parameters(int flatId);
        IEnumerable<string> UserIds(int flatId);
    }
}
