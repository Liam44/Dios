using Dios.Models;
using System.Collections.Generic;

namespace Dios.Extensions
{
    static public class NavigationProperties
    {
        public static void Include(this FlatDTO flat, List<ParameterDTO> parameters = null, AddressDTO address = null)
        {
            if (address == null)
            {
                flat.Address = new AddressDTO();
            }
            else
            {
                flat.Address = address;
            }

            if (parameters == null)
            {
                flat.Parameters = new List<ParameterDTO>();
            }
            else
            {
                flat.Parameters = parameters;
            }
        }

        public static void Include(this AddressDTO address, List<FlatDTO> flats, List<UserDTO> hosts = null)
        {
            address.Flats = flats;
            address.Hosts = hosts;
        }

        public static void Include(this UserDTO user, List<ParameterDTO> parameters = null, List<AddressDTO> addresses = null)
        {
            user.Parameters = parameters;
            user.Addresses = addresses;
        }
    }
}
