using System.ComponentModel.DataAnnotations;

namespace Dios.ViewModels.AccountViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nuvarande lösenord")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "Nytt lösenord")]
        [StringLength(100, ErrorMessage = "{0} måste minst vara {2} och högst {1} långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta nytt lösenord")]
        [Compare("NewPassword", ErrorMessage = "Lösenorden stämmer inte överens.")]
        public string ConfirmPassword { get; set; }
    }
}
