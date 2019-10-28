using System.Collections.Generic;

namespace Dios.Models
{
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string PersonalNumber { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PhoneNumber2 { get; set; } = string.Empty;
        public string RegistrationCode { get; set; } = string.Empty;

        public ICollection<AddressDTO> Addresses { get; set; } = new List<AddressDTO>();
        public ICollection<ParameterDTO> Parameters { get; set; } = new List<ParameterDTO>();
        public string NormalizedEmail { get; set; } = string.Empty;

        public UserDTO()
        {

        }

        public UserDTO(User user)
        {
            if (user != null)
            {
                Id = user.Id;
                PersonalNumber = user.PersonalNumber;
                LastName = user.LastName;
                FirstName = user.FirstName;
                Email = user.Email;
                PhoneNumber = user.PhoneNumber;
                PhoneNumber2 = user.PhoneNumber2;
                RegistrationCode = user.RegistrationCode;
            }
        }

        public override string ToString()
        {
            return (FirstName + " " + LastName).Trim();
        }
    }
}