using System.ComponentModel.DataAnnotations;

namespace Dios.Models
{
    public sealed class Address
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
            return $"{Street}, {Number} - {ZipCode} {Town} - {Country}";
        }
    }
}
