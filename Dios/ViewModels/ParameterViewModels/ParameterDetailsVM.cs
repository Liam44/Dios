using Dios.Models;
using System.ComponentModel.DataAnnotations;

namespace Dios.ViewModels
{
    public sealed class ParameterDetailsVM
    {
        public UserDetailsVM User { get; set; }
        public FlatDetailsVM Flat { get; set; }

        [Display(Name = "Synligt e-post?")]
        public bool IsEmailVisible { get; set; }

        [Display(Name = "Synligt telefonnummer?")]
        public bool IsPhoneNumberVisible { get; set; }

        [Display(Name = "Kontaktbar?")]
        public bool CanBeContacted { get; set; }

        public ParameterDetailsVM(ParameterDTO parameter)
        {
            if (parameter != null)
            {
                IsEmailVisible = parameter.IsEmailVisible;
                IsPhoneNumberVisible = parameter.IsPhoneNumberVisible;
                CanBeContacted = parameter.CanBeContacted;

                if (parameter.Flat != null)
                {
                    Flat = new FlatDetailsVM(parameter.Flat);
                }

                if (parameter.User != null)
                {
                    User = new UserDetailsVM(parameter.User, false);
                }
            }
        }
    }
}
