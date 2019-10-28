using Dios.Models;
using System.ComponentModel.DataAnnotations;

namespace Dios.ViewModels
{
    public class UserIndexVM
    {
        public string Id { get; set; }

        [Display(Name = "Personnummer")]
        public string PersonalNumber { get; set; }

        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "Synligt namn?")]
        public bool IsNameVisible { get; set; }

        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Telefonnummer 2")]
        public string PhoneNumber2 { get; set; }

        public UserIndexVM(UserDTO user)
        {
            if (user != null)
            {
                Id = user.Id;
                PersonalNumber = user.PersonalNumber;
                FirstName = user.FirstName;
                LastName = user.LastName;
                Email = user.Email;
                PhoneNumber = user.PhoneNumber;
                PhoneNumber2 = user.PhoneNumber2;
            }
        }
    }
}
