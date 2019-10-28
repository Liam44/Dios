using Dios.Data;
using Dios.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dios.Repositories
{
    public class ErrorReportsRepository : IErrorReportsRepository
    {
        private readonly ApplicationDbContext _context;

        public ErrorReportsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ErrorReport GetErrorReport(int id)
        {
            return _context.ErrorReports.FirstOrDefault(e => e.Id == id);
        }

        public int Add(ErrorReportDTO errorReport)
        {
            ErrorReport errorReportTmp = new ErrorReport
            {
                Seen = errorReport.Seen,
                FlatId = errorReport.FlatId,
                Description = errorReport.Description,
                Subject = errorReport.Subject,
                Submitted = errorReport.Submitted,
                CurrentStatus = errorReport.CurrentStatus,
                CurrentPriority = errorReport.CurrentPriority
            };

            _context.ErrorReports.Add(errorReportTmp);

            return _context.SaveChanges();
        }

        public int Edit(ErrorReportDTO errorReport)
        {
            if (errorReport == null)
            {
                return 0;
            }

            ErrorReport errorReportTmp = GetErrorReport(errorReport.Id);

            if (errorReportTmp == null)
            {
                return 0;
            }

            errorReportTmp.Seen = errorReport.Seen;
            errorReportTmp.CurrentPriority = errorReport.CurrentPriority;
            errorReportTmp.CurrentStatus = errorReport.CurrentStatus;

            errorReportTmp.CurrentPriority = errorReport.CurrentPriority;
            errorReportTmp.CurrentStatus = errorReport.CurrentStatus;

            _context.Update(errorReportTmp);

            return _context.SaveChanges();
        }

        public IEnumerable<ErrorReportDTO> ErrorReports(string hostId)
        {
            return _context.AddressHosts
                           .Where(ah => string.Compare(ah.UserId, hostId, false) == 0)
                           .Join(_context.Addresses,
                                 ah => ah.AddressID,
                                 a => a.ID,
                                 (ah, a) => a.ID)
                           .Join(_context.Flats,
                                 aId => aId,
                                 f => f.AddressID,
                                 (aId, f) => f.ID)
                           .Join(_context.ErrorReports,
                                 fId => fId,
                                 e => e.FlatId,
                                 (fId, e) => new ErrorReportDTO(e))
                           .ToList();
        }

        public IEnumerable<ErrorReportDTO> UnreadErrorReports(string hostId)
        {
            return ErrorReports(hostId).Where(e => e.Seen == null);
        }

        public IEnumerable<ErrorReportDTO> ErrorReports(int flatId)
        {
            return _context.ErrorReports
                           .Where(e => e.FlatId == flatId)
                           .Select(e => new ErrorReportDTO(e))
                           .ToList();
        }
    }
}
