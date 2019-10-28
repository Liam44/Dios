using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dios.Models
{
    public class Address
    {
        public int ID { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        public string Country { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1} - {2} {3} - {4}", Street, Number, ZipCode, Town, Country);
        }
    }
}
