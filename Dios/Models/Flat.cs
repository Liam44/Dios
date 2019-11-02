using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dios.Models
{
    public sealed class Flat
    {
        public int ID { get; set; }

        [Required]
        public string Number { get; set; }

        public int Floor { get; set; }

        [Required]
        public string EntryDoorCode { get; set; }

        [ForeignKey(nameof(Address))]
        public int AddressID { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Number, Floor);
        }
    }
}
