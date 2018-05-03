using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dios.Models
{
    public class Flat
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string Floor { get; set; }

        [ForeignKey("Address")]
        public int AddressID { get; set; }
        public Address Address { get; set; }

        public ICollection<UserFlat> UserFlats { get; set; } = new List<UserFlat>();
    }
}
