using Dios.Models;
using System.Collections.Generic;

namespace Dios.Repositories
{
    public interface IAddressesRepository
    {
        IEnumerable<AddressDTO> Addresses();
        AddressDTO Address(int? addressId);
        int Add(AddressDTO address);
        int Edit(AddressDTO address);
        int Delete(int addressId);
    }
}
