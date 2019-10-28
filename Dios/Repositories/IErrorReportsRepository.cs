using Dios.Models;
using System.Collections.Generic;

namespace Dios.Repositories
{
    public interface IErrorReportsRepository
    {
        IEnumerable<ErrorReportDTO> ErrorReports(string hostId);
        IEnumerable<ErrorReportDTO> UnreadErrorReports(string hostId);
        IEnumerable<ErrorReportDTO> ErrorReports(int flatId);
        int Add(ErrorReportDTO errorReport);
        int Edit(ErrorReportDTO errorReport);
    }
}
