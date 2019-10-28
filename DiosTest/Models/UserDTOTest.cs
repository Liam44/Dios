using Dios.Models;
using Xunit;

namespace DiosTest.Models
{
    public class UserDTOTest
    {
        [Fact]
        public void UserDTO_NoParameter()
        {
            // Arrange

            // Act
            UserDTO userDTO = new UserDTO();

            // Assert
            Assert.Empty(userDTO.Addresses);
            Assert.Empty(userDTO.Parameters);

            Assert.Empty(userDTO.Id);
            Assert.Empty(userDTO.PersonalNumber);
            Assert.Empty(userDTO.FirstName);
            Assert.Empty(userDTO.LastName);
            Assert.Empty(userDTO.Email);
            Assert.Empty(userDTO.PhoneNumber);
            Assert.Empty(userDTO.PhoneNumber2);
            Assert.Empty(userDTO.NormalizedEmail);
        }

        [Fact]
        public void UserDTO_Null()
        {
            // Arrange
            User user = null;

            // Act
            UserDTO userDTO = new UserDTO(user);

            // Assert
            Assert.Empty(userDTO.Addresses);
            Assert.Empty(userDTO.Parameters);

            Assert.Empty(userDTO.Id);
            Assert.Empty(userDTO.PersonalNumber);
            Assert.Empty(userDTO.FirstName);
            Assert.Empty(userDTO.LastName);
            Assert.Empty(userDTO.Email);
            Assert.Empty(userDTO.PhoneNumber);
            Assert.Empty(userDTO.PhoneNumber2);
            Assert.Empty(userDTO.NormalizedEmail);
        }

        [Fact]
        public void UserDTO_NotNull()
        {
            // Arrange
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

            // Act
            UserDTO userDTO = new UserDTO(user);

            // Assert
            Assert.Empty(userDTO.Addresses);
            Assert.Empty(userDTO.Parameters);

            Assert.Equal(userId, userDTO.Id);
            Assert.Equal(personalNumber, userDTO.PersonalNumber);
            Assert.Equal(firstName, userDTO.FirstName);
            Assert.Equal(lastName, userDTO.LastName);
            Assert.Equal(email, userDTO.Email);
            Assert.Equal(phoneNumber, userDTO.PhoneNumber);
            Assert.Equal(phoneNumber2, userDTO.PhoneNumber2);
            Assert.Empty(userDTO.NormalizedEmail);
        }
    }
}
