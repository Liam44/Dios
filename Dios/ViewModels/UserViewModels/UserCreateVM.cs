using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dios.ViewModels.UsersViewModels
{
    public sealed class UserCreateVM
    {
        public string Id { get; set; }

        [Required]
        [CustomValidation(typeof(UserCreateVM), nameof(PersonNummer), ErrorMessage = "Ogiltigt personnummer.")]
        [Display(Name = "Personnummer")]
        public string PersonalNumber { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        public bool CanChangeName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Phone]
        [Display(Name = "Telefonnummer 2")]
        public string PhoneNumber2 { get; set; }

        public bool IsPhoneNumber2Visible { get; set; }

        public bool CanChangePassword { get; set; }

        public string StatusMessage { get; set; }

        // Properties used only when creating a new Host
        // allowing the user to automatically create the matching User
        public bool CreateMatchingUser { get; set; } = false;

        public string MatchingUsersEmailAddress { get; set; } = string.Empty;

        #region Custom validation rules

        public static ValidationResult PersonNummer(string x)
        {
            if (string.IsNullOrEmpty(x))
            {
                return ValidationResult.Success;
            }

            Regex regex = new Regex("^(19|20)?[0-9]{2}((0[1-9]{1})|(1[0-2]))((0[1-9]{1})|((1|2)[0-9]|(3[0-1])))[-]?[0-9]{4}$");

            if (regex.Matches(x).Count() == 0)
            {
                return new ValidationResult(null);
            }

            return ValidationResult.Success;
        }

        #endregion
    }
}
