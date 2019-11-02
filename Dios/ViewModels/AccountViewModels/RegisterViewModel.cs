using System.ComponentModel.DataAnnotations;

namespace Dios.ViewModels.AccountViewModels
{
    public sealed class RegisterViewModel
    {
        public string RegistrationCode { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }
    }
}
