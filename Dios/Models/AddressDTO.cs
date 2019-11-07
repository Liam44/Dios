using System.Collections.Generic;

namespace Dios.Models
{
    public sealed class AddressDTO
    {
        public int ID { get; set; } = -1;

        public string Street { get; set; } = string.Empty;

        public string Number { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string Town { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public ICollection<FlatDTO> Flats { get; set; } = new List<FlatDTO>();

        public ICollection<UserDTO> Hosts { get; set; } = new List<UserDTO>();

        public override string ToString()
        {
            return $"{Street}, {Number} - {ZipCode} {Town} - {Country}".Trim().Replace("  ", " ");
        }

        public AddressDTO()
        {

        }

        public AddressDTO(Address address)
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
        }
    }
}