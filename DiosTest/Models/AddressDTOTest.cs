using Dios.Models;
using Xunit;

namespace DiosTest.Models
{
    public sealed class AddressDTOTest
    {
        [Fact]
        public void AddressDTO_NoParameter()
        {
            // Arrange

            // Act
            AddressDTO addressDTO = new AddressDTO();

            // Assert
            Assert.Equal(-1, addressDTO.ID);
            Assert.Empty(addressDTO.Street);
            Assert.Empty(addressDTO.Number);
            Assert.Empty(addressDTO.ZipCode);
            Assert.Empty(addressDTO.Town);
            Assert.Empty(addressDTO.Country);
            Assert.Empty(addressDTO.Flats);
            Assert.Empty(addressDTO.Hosts);
        }

        [Fact]
        public void AddressDTO_Null()
        {
            // Arrange
            Address address = null;

            // Act
            AddressDTO addressDTO = new AddressDTO(address);

            // Assert
            Assert.Equal(-1, addressDTO.ID);
            Assert.Empty(addressDTO.Street);
            Assert.Empty(addressDTO.Number);
            Assert.Empty(addressDTO.ZipCode);
            Assert.Empty(addressDTO.Town);
            Assert.Empty(addressDTO.Country);
            Assert.Empty(addressDTO.Flats);
            Assert.Empty(addressDTO.Hosts);
        }

        [Fact]
        public void AddressDTO_NotNull()
        {
            // Arrange
            int addressId = 1;
            string street = "street";
            string number = "number";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            Address address = new Address
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            // Act
            AddressDTO addressDTO = new AddressDTO(address);

            // Assert
            Assert.Equal(addressId, addressDTO.ID);
            Assert.Equal(street, addressDTO.Street);
            Assert.Equal(number, addressDTO.Number);
            Assert.Equal(zipCode, addressDTO.ZipCode);
            Assert.Equal(town, addressDTO.Town);
            Assert.Equal(country, addressDTO.Country);
            Assert.Empty(addressDTO.Flats);
            Assert.Empty(addressDTO.Hosts);
        }

        [Fact]
        public void ToString_NoParameter()
        {
            // Assert
            AddressDTO addressDTO = new AddressDTO();

            // Act
            var result = addressDTO.ToString();

            // Assert
            Assert.Equal(", -  -", result);
        }

        [Fact]
        public void ToString_Null()
        {
            // Assert
            Address address = null;
            AddressDTO addressDTO = new AddressDTO(address);

            // Act
            var result = addressDTO.ToString();

            // Assert
            Assert.Equal(", -  -", result);
        }

        [Fact]
        public void ToString_NotNull()
        {
            // Assert
            int addressId = 1;
            string street = "street";
            string number = "number";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            Address address = new Address
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            AddressDTO addressDTO = new AddressDTO(address);

            string toString = $"{street}, {number} - {zipCode} {town} - {country}";

            // Act
            var result = addressDTO.ToString();

            // Assert
            Assert.Equal(toString, result);
        }
    }
}
