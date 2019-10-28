using System.ComponentModel.DataAnnotations;

namespace Dios.ViewModels.ErrorReports
{
    public class ErrorReportCreateVM
    {
        public int FlatId { get; set; }
        public string Flat { get; set; }

        [Required]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Ämne")]
        public string Subject { get; set; }
    }
}
