using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dios.Models
{
    public sealed class ErrorReport
    {
        [Key]
        public int Id { get; set; }

        public DateTime? Seen { get; set; }

        [ForeignKey("Flat")]
        public int FlatId { get; set; } 

        public string Description { get; set; }

        public string Subject { get; set; }

        public DateTime Submitted { get; set; }

        public Status CurrentStatus { get; set; }

        public Priority CurrentPriority { get; set; }
    }
}
