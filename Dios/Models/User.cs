using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dios.Models
{
    // Add profile data for application users by adding properties to the User class
    public sealed class User : IdentityUser
    {
        [Required]
        public string PersonalNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber2 { get; set; }

        [Required]
        public string RegistrationCode { get; set; }
    }
}
