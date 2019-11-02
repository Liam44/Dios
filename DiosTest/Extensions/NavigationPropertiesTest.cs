using Dios.Exceptions;
using Dios.Extensions;
using Dios.Models;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DiosTest.Extensions
{
    public sealed class NavigationPropertiesTest
    {
        #region NavigationPropertiesWrapper

        #region FlatDTO Include

        [Fact]
        public void FlatDTOInclude_NoParameters()
        {
            // Arrange
            NavigationPropertiesWrapper navigationPropertiesWrapper = new NavigationPropertiesWrapper();

            FlatDTO flat = new FlatDTO
            {
                Parameters = null,
                Address = null
            };

            // Act
            navigationPropertiesWrapper.Include(flat);

            // Assert
            Assert.NotNull(flat.Parameters);
            Assert.Empty(flat.Parameters);
            Assert.NotNull(flat.Address);
        }

        [Fact]
        public void FlatDTOInclude_Null()
        {
            // Arrange
            NavigationPropertiesWrapper navigationPropertiesWrapper = new NavigationPropertiesWrapper();

            List<ParameterDTO> parameters = null;
            AddressDTO address = null;

            FlatDTO flat = new FlatDTO
            {
                Parameters = null,
                Address = null
            };

            // Act
            navigationPropertiesWrapper.Include(flat, parameters, address);

            // Assert
            Assert.NotNull(flat.Parameters);
            Assert.Empty(flat.Parameters);
            Assert.NotNull(flat.Address);
        }

        [Fact]
        public void FlatDTOInclude_NotNull()
        {
            // Arrange
            NavigationPropertiesWrapper navigationPropertiesWrapper = new NavigationPropertiesWrapper();

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
            navigationPropertiesWrapper.Include(flat, parameters, address);

            // Assert
            Assert.NotNull(flat.Parameters);
            Assert.Equal(parameters, flat.Parameters);

            Assert.NotNull(flat.Address);
            Assert.Equal(address, flat.Address);
        }

        #endregion

        #region AddressDTO Include

        [Fact]
        public void AddressDTOInclude_FlatsNull_HostsUndefined()
        {
            // Arrange
            NavigationPropertiesWrapper navigationPropertiesWrapper = new NavigationPropertiesWrapper();

            List<FlatDTO> flats = null;

            AddressDTO address = new AddressDTO
            {
                Flats = new List<FlatDTO>(),
                Hosts = new List<UserDTO>()
            };

            // Act
            navigationPropertiesWrapper.Include(address, flats);

            // Assert
            Assert.Null(address.Flats);
            Assert.Null(address.Hosts);
        }

        [Fact]
        public void AddressDTOInclude_Null()
        {
            // Arrange
            NavigationPropertiesWrapper navigationPropertiesWrapper = new NavigationPropertiesWrapper();

            List<FlatDTO> flats = null;
            List<UserDTO> hosts = null;

            AddressDTO address = new AddressDTO
            {
                Flats = new List<FlatDTO>(),
                Hosts = new List<UserDTO>()
            };

            // Act
            navigationPropertiesWrapper.Include(address, flats, hosts);

            // Assert
            Assert.Null(address.Flats);
            Assert.Null(address.Hosts);
        }

        [Fact]
        public void AddressDTOInclude_NotNull()
        {
            // Arrange
            NavigationPropertiesWrapper navigationPropertiesWrapper = new NavigationPropertiesWrapper();

            List<FlatDTO> flats = new List<FlatDTO> { new FlatDTO() };
            List<UserDTO> hosts = new List<UserDTO> { new UserDTO() };

            AddressDTO address = new AddressDTO
            {
                Flats = null,
                Hosts = null
            };

            // Act
            navigationPropertiesWrapper.Include(address, flats, hosts);

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
            NavigationPropertiesWrapper navigationPropertiesWrapper = new NavigationPropertiesWrapper();

            UserDTO user = new UserDTO
            {
                Parameters = new List<ParameterDTO>(),
                Addresses = new List<AddressDTO>()
            };

            // Act
            navigationPropertiesWrapper.Include(user);

            // Assert
            Assert.Null(user.Parameters);
            Assert.Null(user.Addresses);
        }

        [Fact]
        public void UserDTOInclude_Null()
        {
            // Arrange
            NavigationPropertiesWrapper navigationPropertiesWrapper = new NavigationPropertiesWrapper();

            List<ParameterDTO> parameters = null;
            List<AddressDTO> addresses = null;

            UserDTO user = new UserDTO
            {
                Parameters = new List<ParameterDTO>(),
                Addresses = new List<AddressDTO>()
            };

            // Act
            navigationPropertiesWrapper.Include(user, parameters, addresses);

            // Assert
            Assert.Null(user.Parameters);
            Assert.Null(user.Addresses);
        }

        [Fact]
        public void UserDTOInclude_NotNull()
        {
            // Arrange
            NavigationPropertiesWrapper navigationPropertiesWrapper = new NavigationPropertiesWrapper();

            List<ParameterDTO> parameters = new List<ParameterDTO> { new ParameterDTO() };
            List<AddressDTO> addresses = new List<AddressDTO> { new AddressDTO() };

            UserDTO user = new UserDTO
            {
                Parameters = null,
                Addresses = null
            };

            // Act
            navigationPropertiesWrapper.Include(user, parameters, addresses);

            // Assert
            Assert.NotNull(user.Parameters);
            Assert.Equal(parameters, user.Parameters);
            Assert.NotNull(user.Addresses);
            Assert.Equal(addresses, user.Addresses);
        }

        #endregion

        #endregion

        #region NavigationProperties

        #region FlatDTO Include

        [Fact]
        public void FlatDTOInclude_NavigationPropertiesWrapperNull()
        {
            // Arrange
            INavigationPropertiesWrapper navigationPropertiesWrapper = null;
            NavigationProperties.NavigationPropertiesWrapper = navigationPropertiesWrapper;

            FlatDTO flat = new FlatDTO
            {
                Parameters = null,
                Address = null
            };

            // Act
            NavigationPropertiesWrapperUndefinedException exception = Assert.Throws<NavigationPropertiesWrapperUndefinedException>(() =>
                                                                      flat.Include());

            // Assert
            Assert.Equal("NavigationPropertiesWrapper is null in NavigationProperties.", exception.Message);
        }

        [Fact]
        public void FlatDTOInclude_NavigationPropertiesWrapperNotNull()
        {
            // Arrange
            Mock<INavigationPropertiesWrapper> navigationPropertiesWrapper = new Mock<INavigationPropertiesWrapper>();
            NavigationProperties.NavigationPropertiesWrapper = navigationPropertiesWrapper.Object;

            FlatDTO flat = new FlatDTO
            {
                Parameters = null,
                Address = null
            };

            List<ParameterDTO> parameters = null;
            AddressDTO address = null;

            navigationPropertiesWrapper.Setup(n => n.Include(flat,
                                                             It.IsAny<List<ParameterDTO>>(),
                                                             It.IsAny<AddressDTO>()))
                                       .Callback(() =>
                                       {
                                           flat.Parameters = new List<ParameterDTO>();
                                           flat.Address = new AddressDTO();
                                       });

            // Act
            flat.Include(parameters, address);

            // Assert
            Assert.NotNull(flat.Parameters);
            Assert.Empty(flat.Parameters);
            Assert.NotNull(flat.Address);
        }

        #endregion

        #region AddressDTO Include

        [Fact]
        public void AddressDTOInclude_NavigationPropertiesWrapperNull()
        {
            // Arrange
            INavigationPropertiesWrapper navigationPropertiesWrapper = null;
            NavigationProperties.NavigationPropertiesWrapper = navigationPropertiesWrapper;

            AddressDTO address = new AddressDTO
            {
                Flats = null,
                Hosts = null
            };

            List<FlatDTO> flats = new List<FlatDTO>();

            // Act
            NavigationPropertiesWrapperUndefinedException exception = Assert.Throws<NavigationPropertiesWrapperUndefinedException>(() =>
                                                                      address.Include(flats));

            // Assert
            Assert.Equal("NavigationPropertiesWrapper is null in NavigationProperties.", exception.Message);
        }

        [Fact]
        public void AddressDTOInclude_NavigationPropertiesWrapperNotNull()
        {
            // Arrange
            Mock<INavigationPropertiesWrapper> navigationPropertiesWrapper = new Mock<INavigationPropertiesWrapper>();
            NavigationProperties.NavigationPropertiesWrapper = navigationPropertiesWrapper.Object;

            AddressDTO address = new AddressDTO
            {
                Flats = null,
                Hosts = null
            };

            List<FlatDTO> flats = null;
            List<UserDTO> hosts = null;

            navigationPropertiesWrapper.Setup(n => n.Include(address,
                                                             It.IsAny<List<FlatDTO>>(),
                                                             It.IsAny<List<UserDTO>>()))
                                       .Callback(() =>
                                       {
                                           address.Flats = new List<FlatDTO>();
                                           address.Hosts = new List<UserDTO>();
                                       });

            // Act
            address.Include(flats, hosts);

            // Assert
            Assert.NotNull(address.Flats);
            Assert.Empty(address.Flats);
            Assert.NotNull(address.Hosts);
            Assert.Empty(address.Hosts);

            navigationPropertiesWrapper.Verify(n => n.Include(address,
                                                              flats,
                                                              hosts), Times.Once);

        }

        #endregion

        #endregion
    }
}
