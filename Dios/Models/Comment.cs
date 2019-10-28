using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dios.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [ForeignKey(nameof(ErrorReport))]
        public int ErrorId { get; set; }

        public string Text { get; set; }

        public DateTime TimeOfComment { get; set; }

        [ForeignKey(nameof(User))]
        [Required]
        public string HostId { get; set; }
    }
}
