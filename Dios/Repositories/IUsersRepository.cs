using Dios.Models;
using System.Collections.Generic;

namespace Dios.Repositories
{
    public interface IUsersRepository
    {
        IEnumerable<UserDTO> Users();
        IEnumerable<UserDTO> UsersAtAddress(int addressId);
        IEnumerable<UserDTO> Hosts();
        IEnumerable<UserDTO> HostsAtAddress(int addressId);
        UserDTO User(string id);
        UserDTO UserByPersonalNumber(string personalNumber, string role);
        User GetUser(string userId);
        int Edit(UserDTO user);
        int Delete(string userId);
        string GenerateRegistrationCode();
        string GeneratePassword();
        UserDTO UserByRegistrationCode(string code);
    }
}
