using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dios.Models
{
    public sealed class ErrorReportDTO
    {
        public int Id { get; set; } = -1;

        [Display(Name = "Läst")]
        public DateTime? Seen { get; set; } = null;

        public int FlatId { get; set; } = -1;
        public FlatDTO Flat { get; set; } = new FlatDTO();

        [Display(Name = "Beskrivning")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Ämne")]
        public string Subject { get; set; } = string.Empty;

        [Display(Name = "Skickat")]
        public DateTime Submitted { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        public Status CurrentStatus { get; set; } = Status.waiting;

        [Display(Name = "Prioritet")]
        public Priority CurrentPriority { get; set; } = Priority.undefined;

        [Display(Name = "Kommentär")]
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();

        public ErrorReportDTO()
        {
        }

        public ErrorReportDTO(ErrorReport errorReport)
        {
            if (errorReport != null)
            {
                Id = errorReport.Id;
                Seen = errorReport.Seen;
                FlatId = errorReport.FlatId;
                Description = errorReport.Description;
                Subject = errorReport.Subject;
                Submitted = errorReport.Submitted;
                CurrentStatus = errorReport.CurrentStatus;
                CurrentPriority = errorReport.CurrentPriority;
            }
        }
    }
}
