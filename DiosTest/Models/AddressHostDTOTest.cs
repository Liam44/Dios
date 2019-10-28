using Dios.Models;
using Xunit;

namespace DiosTest.Models
{
    public class AddressHostDTOTest
    {
        [Fact]
        public void AddressHostDTO_Null()
        {
            // Arrange
            AddressDTO address = null;
            UserDTO host = null;

            // Act
            AddressHostDTO addressHostDTO = new AddressHostDTO(address, host);

            // Assert
            Assert.Null(addressHostDTO.Address);
            Assert.Null(addressHostDTO.Host);
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

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string email = "email";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";
            string normalizedEmail = "normalizedEmail";

            UserDTO host = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2,
                NormalizedEmail = normalizedEmail
            };

            // Act
            AddressHostDTO addressHostDTO = new AddressHostDTO(address, host);

            // Assert
            Assert.NotNull(addressHostDTO.Address);
            Assert.Equal(address, addressHostDTO.Address);

            Assert.Equal(addressId, addressHostDTO.Address.ID);
            Assert.Equal(street, addressHostDTO.Address.Street);
            Assert.Equal(number, addressHostDTO.Address.Number);
            Assert.Equal(zipCode, addressHostDTO.Address.ZipCode);
            Assert.Equal(town, addressHostDTO.Address.Town);
            Assert.Equal(country, addressHostDTO.Address.Country);
            Assert.Empty(addressHostDTO.Address.Flats);
            Assert.Empty(addressHostDTO.Address.Hosts);

            Assert.NotNull(addressHostDTO.Host);
            Assert.Equal(host, addressHostDTO.Host);

            Assert.Equal(userId, addressHostDTO.Host.Id);
            Assert.Equal(personalNumber, addressHostDTO.Host.PersonalNumber);
            Assert.Equal(firstName, addressHostDTO.Host.FirstName);
            Assert.Equal(lastName, addressHostDTO.Host.LastName);
            Assert.Equal(email, addressHostDTO.Host.Email);
            Assert.Equal(phoneNumber, addressHostDTO.Host.PhoneNumber);
            Assert.Equal(phoneNumber2, addressHostDTO.Host.PhoneNumber2);
            Assert.Equal(normalizedEmail, addressHostDTO.Host.NormalizedEmail);
            Assert.Empty(addressHostDTO.Host.Addresses);
            Assert.Empty(addressHostDTO.Host.Parameters);
        }
    }
}
