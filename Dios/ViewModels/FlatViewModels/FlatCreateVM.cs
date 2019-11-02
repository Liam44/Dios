using Dios.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dios.ViewModels
{
    public sealed class FlatCreateVM
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "{0} fältet är obligatoriskt.")]
        [Display(Name = "Nummer")]
        public string Number { get; set; }

        [Required(ErrorMessage = "{0} fältet är obligatoriskt.")]
        [Display(Name = "Våning")]
        [Range(-5, 200, ErrorMessage = "{0} måste vara mellan {1} och {2}.")]
        public int Floor { get; set; }

        [Display(Name = "Portkodstavla")]
        [Required(ErrorMessage = "{0} fältet är obligatoriskt.")]
        public string EntryDoorCode { get; set; }

        public int AddressID { get; set; }
        public AddressDetailsVM Address { get; set; }

        public bool CanDataBeEdited { get; set; }
        public string StatusMessage { get; set; }

        public FlatCreateVM()
        {
            Floor = 1;
        }

        [Display(Name = "Boende")]
        public List<UserDetailsVM> Users { get; set; }
        public List<UserDetailsVM> AvailableUsers { get; set; }

        public FlatCreateVM(AddressDTO address,
                            FlatDTO flat = null,
                            List<UserDetailsVM> users = null,
                            List<UserDetailsVM> availableUsers = null,
                            bool canDataBeEdited = true)
        {
            CanDataBeEdited = canDataBeEdited;

            AddressID = address.ID;
            Address = new AddressDetailsVM(address);

            Users = users;
            AvailableUsers = availableUsers;

            if (flat != null)
            {
                ID = flat.ID;
                Number = flat.Number;
                Floor = flat.Floor;
                EntryDoorCode = flat.EntryDoorCode;
            }
            else
            {
                Floor = 1;
            }
        }
    }
}
