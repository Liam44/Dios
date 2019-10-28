using Dios.Models;
using System.Collections.Generic;

namespace Dios.ViewModels.ErrorReports
{
    public class ErrorReportsVM
    {
        public int FlatId { get; set; }
        public string Flat { get; set; }
        public List<ErrorReportDTO> ErrorReports { get; set; }
    }
}
