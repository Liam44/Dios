using Dios.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dios.ViewModels
{
    public class UserDetailsVM
    {
        public string Id { get; set; }

        [Display(Name = "Personnummer")]
        public string PersonalNumber { get; set; }

        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Telefonnummer 2")]
        public string PhoneNumber2 { get; set; }

        public bool IsPhoneNumber2Visible { get; set; }

        public Dictionary<string, List<FlatDetailsVM>> Flats { get; set; } = new Dictionary<string, List<FlatDetailsVM>>();
        public Dictionary<int, List<AddressDetailsVM>> Addresses { get; set; } = new Dictionary<int, List<AddressDetailsVM>>();
        public Dictionary<int, List<ParameterDetailsVM>> Parameters { get; set; } = new Dictionary<int, List<ParameterDetailsVM>>();

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public UserDetailsVM(UserDTO user, bool isPhoneNumber2Visible, Dictionary<string, List<FlatDetailsVM>> flats = null)
        {
            IsPhoneNumber2Visible = isPhoneNumber2Visible;

            if (user != null)
            {
                Id = user.Id;
                PersonalNumber = user.PersonalNumber;
                FirstName = user.FirstName;
                LastName = user.LastName;
                Email = user.Email;
                PhoneNumber = user.PhoneNumber;
                PhoneNumber2 = user.PhoneNumber2;

                Flats = flats;

                if (user.Parameters != null)
                {
                    Parameters = user.Parameters
                                     .GroupBy(p => p.Flat == null ? 0 : p.Flat.Floor)
                                     .OrderBy(pg => pg.Key)
                                     .ToDictionary(pg => pg.Key,
                                                   pg => pg.Select(p => new ParameterDetailsVM(p))
                                                           .OrderBy(p => p.Flat == null ? string.Empty : p.Flat.Number)
                                                           .ToList());

                }

                if (user.Addresses != null)
                {
                    Addresses = user.Addresses
                                    .OrderBy(a => a.Country)
                                    .ThenBy(a => a.Town)
                                    .ThenBy(a => a.Street)
                                    .ThenBy(a => a.Number)
                                    .GroupBy(a => a.ID)
                                    .ToDictionary(ag => ag.Key,
                                                  ag => ag.Select(a => new AddressDetailsVM(a)).ToList());
                }
            }
        }
    }
}
