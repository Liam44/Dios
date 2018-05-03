using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dios.Models
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public bool IsNameShown { get; set; }

        [Required]
        public bool IsEmailShown { get; set; }

        [Required]
        public bool IsPhoneNumberShown { get; set; }

        [Required]
        public bool CanBeContacted { get; set; }

        public ICollection<UserFlat> UserFlats { get; set; } = new List<UserFlat>();
    }
}
