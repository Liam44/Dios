using System.ComponentModel.DataAnnotations;

namespace Dios.ViewModels
{
    public sealed class ParameterEditVM
    {
        public string UserId { get; set; }
        public UserDetailsVM User { get; set; }

        public int FlatID { get; set; }
        public FlatDetailsVM Flat { get; set; }

        [Display(Name = "Synligt e-post?")]
        public bool IsEmailVisible { get; set; }

        [Display(Name = "Synligt telefonnummer?")]
        public bool IsPhoneNumberVisible { get; set; }

        [Display(Name = "Kontaktbar?")]
        public bool CanBeContacted { get; set; }
    }
}
