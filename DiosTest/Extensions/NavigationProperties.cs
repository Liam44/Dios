using Dios.Extensions;
using Dios.Models;
using System.Collections.Generic;
using Xunit;

namespace DiosTest.Extensions
{
    public class NavigationProperties
    {
        #region FlatDTO Include

        [Fact]
        public void FlatDTOInclude_NoParameters()
        {
            // Arrange
            FlatDTO flat = new FlatDTO
            {
                Parameters = new List<ParameterDTO>(),
                Address = new AddressDTO()
            };

            // Act
            flat.Include();

            // Assert
            Assert.NotNull(flat.Parameters);
            Assert.Empty(flat.Parameters);
            Assert.NotNull(flat.Address);
        }

        [Fact]
        public void FlatDTOInclude_Null()
        {
            // Arrange
            List<ParameterDTO> parameters = null;
            AddressDTO address = null;

            FlatDTO flat = new FlatDTO
            {
                Parameters = new List<ParameterDTO>(),
                Address = new AddressDTO()
            };

            // Act
            flat.Include(parameters, address);

            // Assert
            Assert.NotNull(flat.Parameters);
            Assert.Empty(flat.Parameters);
            Assert.NotNull(flat.Address);
        }

        [Fact]
        public void FlatDTOInclude_NotNull()
        {
            // Arrange
            List<ParameterDTO> parameters = new List<ParameterDTO> { new ParameterDTO() };

            int addressId = 1;
            string street = "street";
            string number = "number";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            FlatDTO flat = new FlatDTO
            {
                Parameters = null,
                Address = null
            };

            // Act
            flat.Include(parameters, address);

            // Assert
            Assert.NotNull(flat.Parameters);
            Assert.Equal(parameters, flat.Parameters);

            Assert.NotNull(flat.Address);
            Assert.Equal(address, flat.Address);
        }

        #endregion

        #region AddressDTO Include

        [Fact]
        public void AddressDTOInclude_NoParameters()
        {
            // Arrange
            List<FlatDTO> flats = null;

            AddressDTO address = new AddressDTO
            {
                Flats = new List<FlatDTO>(),
                Hosts = new List<UserDTO>()
            };

            // Act
            address.Include(flats);

            // Assert
            Assert.Null(address.Flats);
            Assert.Null(address.Hosts);
        }

        [Fact]
        public void AddressDTOInclude_Null()
        {
            // Arrange
            List<FlatDTO> flats = null;
            List<UserDTO> hosts = null;

            AddressDTO address = new AddressDTO
            {
                Flats = new List<FlatDTO>(),
                Hosts = new List<UserDTO>()
            };

            // Act
            address.Include(flats, hosts);

            // Assert
            Assert.Null(address.Flats);
            Assert.Null(address.Hosts);
        }

        [Fact]
        public void AddressDTOInclude_NotNull()
        {
            // Arrange
            List<FlatDTO> flats = new List<FlatDTO> { new FlatDTO() };
            List<UserDTO> hosts = new List<UserDTO> { new UserDTO() };

            AddressDTO address = new AddressDTO
            {
                Flats = new List<FlatDTO>(),
                Hosts = new List<UserDTO>()
            };

            // Act
            address.Include(flats, hosts);

            // Assert
            Assert.NotNull(address.Flats);
            Assert.Equal(flats, address.Flats);
            Assert.NotNull(address.Hosts);
            Assert.Equal(hosts, address.Hosts);
        }

        #endregion

        #region UserDTO Include

        [Fact]
        public void UserDTOInclude_NoParameter()
        {
            // Arrange
            UserDTO user = new UserDTO
            {
                Parameters = new List<ParameterDTO>(),
                Addresses = new List<AddressDTO>()
            };

            // Act
            user.Include();

            // Assert
            Assert.Null(user.Parameters);
            Assert.Null(user.Addresses);
        }

        [Fact]
        public void UserDTOInclude_Null()
        {
            // Arrange
            List<ParameterDTO> parameters = null;
            List<AddressDTO> addresses = null;

            UserDTO user = new UserDTO
            {
                Parameters = new List<ParameterDTO>(),
                Addresses = new List<AddressDTO>()
            };

            // Act
            user.Include(parameters, addresses);

            // Assert
            Assert.Null(user.Parameters);
            Assert.Null(user.Addresses);
        }

        [Fact]
        public void UserDTOInclude_NotNull()
        {
            // Arrange
            List<ParameterDTO> parameters = new List<ParameterDTO> { new ParameterDTO() };
            List<AddressDTO> addresses = new List<AddressDTO> { new AddressDTO() };

            UserDTO user = new UserDTO
            {
                Parameters = null,
                Addresses = null
            };

            // Act
            user.Include(parameters, addresses);

            // Assert
            Assert.NotNull(user.Parameters);
            Assert.Equal(parameters, user.Parameters);
            Assert.NotNull(user.Addresses);
            Assert.Equal(addresses, user.Addresses);
        }

        #endregion
    }
}
