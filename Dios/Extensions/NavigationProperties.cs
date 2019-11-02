using Dios.Exceptions;
using Dios.Models;
using System.Collections.Generic;

namespace Dios.Extensions
{
    public interface INavigationPropertiesWrapper
    {
        void Include(FlatDTO flat, List<ParameterDTO> parameters = null, AddressDTO address = null);

        void Include(AddressDTO address, List<FlatDTO> flats, List<UserDTO> hosts = null);

        void Include(UserDTO user, List<ParameterDTO> parameters = null, List<AddressDTO> addresses = null);
    }

    public class NavigationPropertiesWrapper : INavigationPropertiesWrapper
    {
        public void Include(FlatDTO flat, List<ParameterDTO> parameters = null, AddressDTO address = null)
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

        public void Include(AddressDTO address, List<FlatDTO> flats, List<UserDTO> hosts = null)
        {
            address.Flats = flats;
            address.Hosts = hosts;
        }

        public void Include(UserDTO user, List<ParameterDTO> parameters = null, List<AddressDTO> addresses = null)
        {
            user.Parameters = parameters;
            user.Addresses = addresses;
        }
    }

    public static class NavigationProperties
    {
        public static INavigationPropertiesWrapper NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

        public static void Include(this FlatDTO flat, List<ParameterDTO> parameters = null, AddressDTO address = null)
        {
            if (NavigationPropertiesWrapper == null)
            {
                throw new NavigationPropertiesWrapperUndefinedException();
            }

            NavigationPropertiesWrapper.Include(flat, parameters, address);
        }

        public static void Include(this AddressDTO address, List<FlatDTO> flats, List<UserDTO> hosts = null)
        {
            if (NavigationPropertiesWrapper == null)
            {
                throw new NavigationPropertiesWrapperUndefinedException();
            }

            NavigationPropertiesWrapper.Include(address, flats, hosts);
        }

        public static void Include(this UserDTO user, List<ParameterDTO> parameters = null, List<AddressDTO> addresses = null)
        {
            if (NavigationPropertiesWrapper == null)
            {
                throw new NavigationPropertiesWrapperUndefinedException();
            }

            NavigationPropertiesWrapper.Include(user, parameters, addresses);
        }
    }
}
