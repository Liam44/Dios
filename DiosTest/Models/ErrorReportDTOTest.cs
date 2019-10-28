using Dios.Models;
using System;
using Xunit;

namespace DiosTest.Models
{
    public class ErrorReportDTOTest
    {
        [Fact]
        public void ErrorReportDTO_NoParameter()
        {
            // Arrange

            // Act
            ErrorReportDTO errorReportDTO = new ErrorReportDTO();

            // Assert
            Assert.Equal(-1, errorReportDTO.Id);
            Assert.Null(errorReportDTO.Seen);
            Assert.Empty(errorReportDTO.Description);
            Assert.Empty(errorReportDTO.Subject);
            Assert.NotEmpty(errorReportDTO.Submitted.ToString());
            Assert.Equal(Status.waiting, errorReportDTO.CurrentStatus);
            Assert.Equal(Priority.undefined, errorReportDTO.CurrentPriority);
            Assert.Equal(-1, errorReportDTO.FlatId);

            Assert.NotNull(errorReportDTO.Flat);
            Assert.Equal(-1, errorReportDTO.Flat.ID);
            Assert.Equal(-1, errorReportDTO.Flat.Floor);
            Assert.Empty(errorReportDTO.Flat.Number);
            Assert.Empty(errorReportDTO.Flat.EntryDoorCode);
            Assert.Equal(-1, errorReportDTO.Flat.AddressID);
            Assert.NotNull(errorReportDTO.Flat.Parameters);
            Assert.Empty(errorReportDTO.Flat.Parameters);

            Assert.Empty(errorReportDTO.Comments);
        }

        [Fact]
        public void ErrorReportDTO_Null()
        {
            // Arrange
            ErrorReport errorReport = null;

            // Act
            ErrorReportDTO errorReportDTO = new ErrorReportDTO(errorReport);

            // Assert
            Assert.Equal(-1, errorReportDTO.Id);
            Assert.Null(errorReportDTO.Seen);
            Assert.Empty(errorReportDTO.Description);
            Assert.Empty(errorReportDTO.Subject);
            Assert.NotEmpty(errorReportDTO.Submitted.ToString());
            Assert.Equal(Status.waiting, errorReportDTO.CurrentStatus);
            Assert.Equal(Priority.undefined, errorReportDTO.CurrentPriority);
            Assert.Equal(-1, errorReportDTO.FlatId);

            Assert.NotNull(errorReportDTO.Flat);
            Assert.Equal(-1, errorReportDTO.Flat.ID);
            Assert.Equal(-1, errorReportDTO.Flat.Floor);
            Assert.Empty(errorReportDTO.Flat.Number);
            Assert.Empty(errorReportDTO.Flat.EntryDoorCode);
            Assert.Equal(-1, errorReportDTO.Flat.AddressID);
            Assert.NotNull(errorReportDTO.Flat.Parameters);
            Assert.Empty(errorReportDTO.Flat.Parameters);

            Assert.Empty(errorReportDTO.Comments);
        }

        [Fact]
        public void ErrorReportDTO_NotNull()
        {
            // Arrange
            int errorReportId = 1;
            DateTime seen = DateTime.Now.AddMinutes(30);
            int flatId = 2;
            string subject = "subject";
            string description = "description";
            DateTime submitted = DateTime.Now;
            Status status = Status.irrelevant;
            Priority priority = Priority.high;

            ErrorReport errorReport = new ErrorReport
            {
                Id = errorReportId,
                Seen = seen,
                FlatId = flatId,
                Subject = subject,
                Description = description,
                Submitted = submitted,
                CurrentStatus = status,
                CurrentPriority = priority
            };

            // Act
            ErrorReportDTO errorReportDTO = new ErrorReportDTO(errorReport);

            // Assert
            Assert.Equal(errorReportId, errorReportDTO.Id);
            Assert.Equal(seen, errorReportDTO.Seen);
            Assert.Equal(subject, errorReportDTO.Subject);
            Assert.Equal(description, errorReportDTO.Description);
            Assert.Equal(submitted, errorReportDTO.Submitted);
            Assert.Equal(status, errorReportDTO.CurrentStatus);
            Assert.Equal(priority, errorReportDTO.CurrentPriority);
            Assert.Equal(flatId, errorReportDTO.FlatId);

            Assert.NotNull(errorReportDTO.Flat);
            Assert.Equal(-1, errorReportDTO.Flat.ID);
            Assert.Equal(-1, errorReportDTO.Flat.Floor);
            Assert.Empty(errorReportDTO.Flat.Number);
            Assert.Empty(errorReportDTO.Flat.EntryDoorCode);
            Assert.Equal(-1, errorReportDTO.Flat.AddressID);
            Assert.NotNull(errorReportDTO.Flat.Parameters);
            Assert.Empty(errorReportDTO.Flat.Parameters);

            Assert.Empty(errorReportDTO.Comments);
        }
    }
}
