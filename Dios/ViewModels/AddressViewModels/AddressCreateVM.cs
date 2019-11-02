using Dios.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dios.ViewModels
{
    public sealed class AddressCreateVM
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Gata")]
        public string Street { get; set; }

        [Required]
        [Display(Name = "Nummer")]
        public string Number { get; set; }

        [Required]
        [Display(Name = "Postkod")]
        public string ZipCode { get; set; }

        [Required]
        [Display(Name = "Ort")]
        public string Town { get; set; }

        [Required]
        [Display(Name = "Land")]
        public string Country { get; set; }

        [Display(Name = "Hyresansvarig")]
        public List<UserDetailsVM> Hosts { get; set; }
        public List<UserDetailsVM> AvailableHosts { get; set; }

        public AddressCreateVM()
        {

        }

        public AddressCreateVM(AddressDTO address, List<UserDetailsVM> hosts = null, List<UserDetailsVM> availableHosts = null)
        {
            if (address != null)
            {
                ID = address.ID;
                Street = address.Street;
                Number = address.Number;
                ZipCode = address.ZipCode;
                Town = address.Town;
                Country = address.Country;
            }

            Hosts = hosts;
            AvailableHosts = availableHosts;
        }
    }
}
