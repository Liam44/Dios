using Dios.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dios.ViewModels
{
    public class FlatDetailsVM
    {
        public int ID { get; set; }

        [Display(Name = "Nummer")]
        public string Number { get; set; }

        [Display(Name = "Våning")]
        public int Floor { get; set; }

        [Display(Name = "Portkod")]
        public string EntryDoorCode { get; set; }

        [Display(Name = "Adress")]
        public AddressDetailsVM Address { get; set; } = new AddressDetailsVM();

        [Display(Name = "Inställningar")]
        public List<ParameterDetailsVM> Parameters { get; set; } = new List<ParameterDetailsVM>();

        [Display(Name = "Antal boende")]
        public int AmountUsers { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Number, Floor);
        }

        public FlatDetailsVM(FlatDTO flat, ParameterDTO parameter = null)
        {
            if (flat != null)
            {
                ID = flat.ID;
                Number = flat.Number;
                Floor = flat.Floor;
                EntryDoorCode = flat.EntryDoorCode;

                if (flat.Address != null)
                {
                    Address = new AddressDetailsVM(flat.Address);
                }

                if (parameter != null)
                {
                    Parameters = new List<ParameterDetailsVM> { new ParameterDetailsVM(parameter) };
                }
                else if (flat.Parameters != null)
                {
                    Parameters = flat.Parameters.Select(p => new ParameterDetailsVM(p)).ToList();
                }

                AmountUsers = Parameters.Count();
            }
        }
    }
}
