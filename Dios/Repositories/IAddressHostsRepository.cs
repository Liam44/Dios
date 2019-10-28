using Dios.Models;
using System.Collections.Generic;

namespace Dios.Repositories
{
    public interface IAddressHostsRepository
    {
        AddressHostDTO AddressHost(int addressId, string hostId);
        IEnumerable<AddressDTO> Addresses(string hostId);
        IEnumerable<UserDTO> Hosts(int addressId);
        IEnumerable<string> HostIds(int addressId);
        int Add(AddressHostDTO addressHost);
        int AddHosts(int addressId, List<string> hostsIds);
        int Delete(int addressId, string hostId);
    }
}
