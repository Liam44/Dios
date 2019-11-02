using Dios.Models;
using Xunit;

namespace DiosTest.Models
{
    public sealed class ParameterDTOTest
    {
        [Fact]
        public void ParameterDTO_NoParameter()
        {
            // Arrange

            // Act
            ParameterDTO parameterDTO = new ParameterDTO();

            // Assert
            Assert.Empty(parameterDTO.UserId);

            Assert.NotNull(parameterDTO.User);
            Assert.Empty(parameterDTO.User.Id);
            Assert.Empty(parameterDTO.User.PersonalNumber);
            Assert.Empty(parameterDTO.User.FirstName);
            Assert.Empty(parameterDTO.User.LastName);
            Assert.Empty(parameterDTO.User.Email);
            Assert.Empty(parameterDTO.User.PhoneNumber);
            Assert.Empty(parameterDTO.User.PhoneNumber2);
            Assert.Empty(parameterDTO.User.NormalizedEmail);
            Assert.Empty(parameterDTO.User.Addresses);
            Assert.Empty(parameterDTO.User.Parameters);

            Assert.NotNull(parameterDTO.Flat);
            Assert.Equal(-1, parameterDTO.Flat.ID);
            Assert.Equal(-1, parameterDTO.Flat.Floor);
            Assert.Empty(parameterDTO.Flat.Number);
            Assert.Empty(parameterDTO.Flat.EntryDoorCode);
            Assert.Equal(-1, parameterDTO.Flat.AddressID);
            Assert.NotNull(parameterDTO.Flat.Address);
            Assert.Equal(-1, parameterDTO.Flat.Address.ID);
            Assert.Empty(parameterDTO.Flat.Address.Street);
            Assert.Empty(parameterDTO.Flat.Address.Number);
            Assert.Empty(parameterDTO.Flat.Address.ZipCode);
            Assert.Empty(parameterDTO.Flat.Address.Town);
            Assert.Empty(parameterDTO.Flat.Address.Country);
            Assert.Empty(parameterDTO.Flat.Address.Flats);
            Assert.Empty(parameterDTO.Flat.Address.Hosts);
            Assert.NotNull(parameterDTO.Flat.Parameters);
            Assert.Empty(parameterDTO.Flat.Parameters);

            Assert.True(parameterDTO.IsPhoneNumberVisible);
            Assert.True(parameterDTO.IsEmailVisible);
            Assert.True(parameterDTO.CanBeContacted);
        }

        [Fact]
        public void ParameterDTO_Null()
        {
            // Arrange
            Flat flat = null;
            User user = null;
            Parameter parameter = null;

            // Act
            ParameterDTO parameterDTO = new ParameterDTO(flat, user, parameter);

            // Assert
            Assert.Empty(parameterDTO.UserId);

            Assert.NotNull(parameterDTO.User);
            Assert.Empty(parameterDTO.User.Id);
            Assert.Empty(parameterDTO.User.PersonalNumber);
            Assert.Empty(parameterDTO.User.FirstName);
            Assert.Empty(parameterDTO.User.LastName);
            Assert.Empty(parameterDTO.User.Email);
            Assert.Empty(parameterDTO.User.PhoneNumber);
            Assert.Empty(parameterDTO.User.PhoneNumber2);
            Assert.Empty(parameterDTO.User.NormalizedEmail);
            Assert.Empty(parameterDTO.User.Addresses);
            Assert.Empty(parameterDTO.User.Parameters);

            Assert.NotNull(parameterDTO.Flat);
            Assert.Equal(-1, parameterDTO.Flat.ID);
            Assert.Equal(-1, parameterDTO.Flat.Floor);
            Assert.Empty(parameterDTO.Flat.Number);
            Assert.Empty(parameterDTO.Flat.EntryDoorCode);
            Assert.Equal(-1, parameterDTO.Flat.AddressID);
            Assert.NotNull(parameterDTO.Flat.Address);
            Assert.Equal(-1, parameterDTO.Flat.Address.ID);
            Assert.Empty(parameterDTO.Flat.Address.Street);
            Assert.Empty(parameterDTO.Flat.Address.Number);
            Assert.Empty(parameterDTO.Flat.Address.ZipCode);
            Assert.Empty(parameterDTO.Flat.Address.Town);
            Assert.Empty(parameterDTO.Flat.Address.Country);
            Assert.Empty(parameterDTO.Flat.Address.Flats);
            Assert.Empty(parameterDTO.Flat.Address.Hosts);
            Assert.NotNull(parameterDTO.Flat.Parameters);
            Assert.Empty(parameterDTO.Flat.Parameters);

            Assert.True(parameterDTO.IsPhoneNumberVisible);
            Assert.True(parameterDTO.IsEmailVisible);
            Assert.True(parameterDTO.CanBeContacted);
        }

        [Fact]
        public void ParameterDTO_NotNull()
        {
            // Arrange
            int flatId = 1;
            int floor = 1;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            int addressId = 2;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string email = "email";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";
            string normalizedEmail = "normalizedEmail";

            User user = new User
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

            bool isPhoneNumberVisible = false;
            bool isEmailVisible = false;
            bool canBeContacted = false;

            Parameter parameter = new Parameter
            {
                UserId = userId,
                FlatID = flatId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            // Act
            ParameterDTO parameterDTO = new ParameterDTO(flat, user, parameter);

            // Assert
            Assert.Equal(userId, parameterDTO.UserId);

            Assert.NotNull(parameterDTO.User);
            Assert.Equal(userId, parameterDTO.User.Id);
            Assert.Equal(personalNumber, parameterDTO.User.PersonalNumber);
            Assert.Equal(firstName, parameterDTO.User.FirstName);
            Assert.Equal(lastName, parameterDTO.User.LastName);
            Assert.Equal(email, parameterDTO.User.Email);
            Assert.Equal(phoneNumber, parameterDTO.User.PhoneNumber);
            Assert.Equal(phoneNumber2, parameterDTO.User.PhoneNumber2);
            Assert.Empty(parameterDTO.User.NormalizedEmail);
            Assert.Empty(parameterDTO.User.Addresses);
            Assert.Empty(parameterDTO.User.Parameters);

            Assert.NotNull(parameterDTO.Flat);
            Assert.Equal(flatId, parameterDTO.Flat.ID);
            Assert.Equal(floor, parameterDTO.Flat.Floor);
            Assert.Equal(number, parameterDTO.Flat.Number);
            Assert.Equal(entryDoorCode, parameterDTO.Flat.EntryDoorCode);
            Assert.Equal(addressId, parameterDTO.Flat.AddressID);
            Assert.NotNull(parameterDTO.Flat.Address);
            Assert.Equal(-1, parameterDTO.Flat.Address.ID);
            Assert.Empty(parameterDTO.Flat.Address.Street);
            Assert.Empty(parameterDTO.Flat.Address.Number);
            Assert.Empty(parameterDTO.Flat.Address.ZipCode);
            Assert.Empty(parameterDTO.Flat.Address.Town);
            Assert.Empty(parameterDTO.Flat.Address.Country);
            Assert.Empty(parameterDTO.Flat.Address.Flats);
            Assert.Empty(parameterDTO.Flat.Address.Hosts);
            Assert.NotNull(parameterDTO.Flat.Parameters);
            Assert.Empty(parameterDTO.Flat.Parameters);

            Assert.Equal(isPhoneNumberVisible, parameterDTO.IsPhoneNumberVisible);
            Assert.Equal(isEmailVisible, parameterDTO.IsEmailVisible);
            Assert.Equal(canBeContacted, parameterDTO.CanBeContacted);
        }
    }
}
