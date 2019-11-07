using System.Collections.Generic;

namespace Dios.Models
{
    public sealed class FlatDTO
    {
        public int ID { get; set; } = -1;

        public string Number { get; set; } = string.Empty;

        public int Floor { get; set; } = -1;

        public string EntryDoorCode { get; set; } = string.Empty;

        public int AddressID { get; set; } = -1;
        public AddressDTO Address { get; set; } = new AddressDTO();

        public ICollection<ParameterDTO> Parameters { get; set; } = new List<ParameterDTO>();

        public override string ToString()
        {
            return $"{Number} ({Floor})".Trim();
        }

        public FlatDTO()
        {

        }

        public FlatDTO(Flat flat)
        {
            if (flat != null)
            {
                ID = flat.ID;
                AddressID = flat.AddressID;
                Number = flat.Number;
                Floor = flat.Floor;
                EntryDoorCode = flat.EntryDoorCode;
            }
        }
    }
}