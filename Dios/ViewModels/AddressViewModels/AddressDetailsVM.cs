using Dios.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dios.ViewModels
{
    public class AddressDetailsVM
    {
        public int ID { get; set; }

        [Display(Name = "Gatan")]
        public string Street { get; set; }

        [Display(Name = "Nummer")]
        public string Number { get; set; }

        [Display(Name = "Postkod")]
        public string ZipCode { get; set; }

        [Display(Name = "Ort")]
        public string Town { get; set; }

        [Display(Name = "Land")]
        public string Country { get; set; }

        [Display(Name = "Antal lägenheter")]
        public int AmountFlats { get; set; }

        [Display(Name = "Antal lediga lägenheter")]
        public int AmountAvailableFlats { get; set; }

        [Display(Name = "Antal boende")]
        public int AmountUsers { get; set; }

        [Display(Name = "Antal hyresansvarigar")]
        public int AmountHosts { get; set; }

        public bool CanDataBeDeleted { get; set; }

        [Display(Name = "Lägenheter")]
        public Dictionary<int, List<FlatDetailsVM>> Flats { get; set; } = new Dictionary<int, List<FlatDetailsVM>>();

        public List<UserDetailsVM> Hosts { get; set; } = new List<UserDetailsVM>();

        public override string ToString()
        {
            return string.Format("{0}, {1} - {2} {3} - {4}", Street, Number, ZipCode, Town, Country);
        }

        public AddressDetailsVM(AddressDTO address = null, int amountAvailableFlats = 0, bool canDataBeDeleted = true)
        {
            CanDataBeDeleted = canDataBeDeleted;

            if (address != null)
            {
                ID = address.ID;
                Street = address.Street;
                Number = address.Number;
                ZipCode = address.ZipCode;
                Town = address.Town;
                Country = address.Country;

                AmountFlats = address.Flats == null ? 0 : address.Flats.Count();
                AmountAvailableFlats = amountAvailableFlats < 0 ? 0 : amountAvailableFlats;
                AmountUsers = address.Flats == null ? 0 : address.Flats.Sum(f => f.Parameters == null ? 0 : f.Parameters.Count);
                AmountHosts = address.Hosts == null ? 0 : address.Hosts.Count();

                Flats = address.Flats?
                               .GroupBy(f => f.Floor)
                               .ToDictionary(fg => fg.Key, fg => fg.Select(f => new FlatDetailsVM(f))
                                                                   .OrderBy(f => f.Number)
                                                                   .ToList());

                Hosts = address.Hosts?.Select(h => new UserDetailsVM(h, true)).ToList();
            }
        }
    }
}
