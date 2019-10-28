using Dios.Data;
using Dios.Models;
using Dios.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace DiosTest.Repositories
{
    public class ErrorReportsRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _dbContext;
        private readonly ErrorReportsRepository _repository;

        public ErrorReportsRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            _dbContext = new ApplicationDbContext(_options);
            _repository = new ErrorReportsRepository(_dbContext);
        }

        #region ErrorReports

        [Fact]
        public void ErrorReports_NoEntry()
        {
            // Arrange
            string hostId = "someHostId";

            // Act
            var result = _repository.ErrorReports(hostId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ErrorReports_OneEntry_NoMatch()
        {
            // Arrange
            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            int flatId = 1;
            int floor = 1;
            string flatNumber = "1";
            string entryDoorCode = "entryDoorCode";

            string hostId = "someHostId";

            int errorId = 1;
            DateTime? seen = null;
            string subject = "subject";
            string description = "description";
            DateTime submitted = DateTime.Now;
            Status status = Status.waiting;
            Priority prioriy = Priority.low;

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = "someOtherHostId"
            };

            Address address = new Address
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            ErrorReport errorReport = new ErrorReport
            {
                Id = errorId,
                Seen = seen,
                Subject = subject,
                Description = description,
                Submitted = submitted,
                CurrentStatus = status,
                CurrentPriority = prioriy,
                FlatId = flatId
            };

            _dbContext.Add(address);
            _dbContext.Add(addressHost);
            _dbContext.Add(flat);
            _dbContext.Add(errorReport);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.ErrorReports(hostId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ErrorReports_OneEntry_OneMatch()
        {
            // Arrange
            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            int flatId = 1;
            int floor = 1;
            string flatNumber = "1";
            string entryDoorCode = "entryDoorCode";

            string hostId = "someHostId";

            int errorId = 1;
            DateTime? seen = null;
            string subject = "subject";
            string description = "description";
            DateTime submitted = DateTime.Now;
            Status status = Status.waiting;
            Priority priority = Priority.low;

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId
            };

            Address address = new Address
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            ErrorReport errorReport = new ErrorReport
            {
                Id = errorId,
                Seen = seen,
                Subject = subject,
                Description = description,
                Submitted = submitted,
                CurrentStatus = status,
                CurrentPriority = priority,
                FlatId = flatId
            };

            _dbContext.Add(address);
            _dbContext.Add(addressHost);
            _dbContext.Add(flat);
            _dbContext.Add(errorReport);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.ErrorReports(hostId);

            // Assert
            Assert.Single(result);
            Assert.Equal(seen, result.First().Seen);
            Assert.Equal(subject, result.First().Subject);
            Assert.Equal(description, result.First().Description);
            Assert.Equal(submitted, result.First().Submitted);
            Assert.Equal(status, result.First().CurrentStatus);
            Assert.Equal(priority, result.First().CurrentPriority);
        }

        [Fact]
        public void ErrorReports_TwoEntries_OneMatch()
        {
            // Arrange
            int addressId1 = 1;
            string street1 = "street1";
            string addressNumber1 = "addressNumber1";
            string zipCode1 = "zipCode1";
            string town1 = "town1";
            string country1 = "country1";

            int addressId2 = 2;
            string street2 = "street2";
            string addressNumber2 = "addressNumber2";
            string zipCode2 = "zipCode2";
            string town2 = "town2";
            string country2 = "country2";

            int flatId1 = 1;
            int floor1 = 1;
            string flatNumber1 = "1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string flatNumber2 = "2";
            string entryDoorCode2 = "entryDoorCode2";

            string hostId1 = "someHostId1";
            string hostId2 = "someHostId2";

            int errorId1 = 1;
            DateTime? seen1 = null;
            string subject1 = "subject1";
            string description1 = "description1";
            DateTime submitted1 = DateTime.Now;
            DateTime responded1 = DateTime.Now.AddMinutes(30);
            Status status1 = Status.waiting;
            Priority priority1 = Priority.low;

            int errorId2 = 2;
            DateTime seen2 = DateTime.Now.AddMinutes(30);
            string subject2 = "subject2";
            string description2 = "description2";
            DateTime submitted2 = DateTime.Now.AddMinutes(10);
            Status status2 = Status.finished;
            Priority priority2 = Priority.high;

            AddressHost addressHost1 = new AddressHost
            {
                AddressID = addressId1,
                UserId = hostId1
            };

            AddressHost addressHost2 = new AddressHost
            {
                AddressID = addressId2,
                UserId = hostId2
            };

            Address address1 = new Address
            {
                ID = addressId1,
                Street = street1,
                Number = addressNumber1,
                ZipCode = zipCode1,
                Town = town1,
                Country = country1
            };

            Address address2 = new Address
            {
                ID = addressId2,
                Street = street2,
                Number = addressNumber2,
                ZipCode = zipCode2,
                Town = town2,
                Country = country2
            };

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId1
            };

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId2
            };

            ErrorReport errorReport1 = new ErrorReport
            {
                Id = errorId1,
                Seen = seen1,
                Subject = subject1,
                Description = description1,
                Submitted = submitted1,
                CurrentStatus = status1,
                CurrentPriority = priority1,
                FlatId = flatId1
            };

            ErrorReport errorReport2 = new ErrorReport
            {
                Id = errorId2,
                Seen = seen2,
                Subject = subject2,
                Description = description2,
                Submitted = submitted2,
                CurrentStatus = status2,
                CurrentPriority = priority2,
                FlatId = flatId2
            };

            _dbContext.Add(addressHost1);
            _dbContext.Add(addressHost2);
            _dbContext.Add(address1);
            _dbContext.Add(address2);
            _dbContext.Add(flat1);
            _dbContext.Add(flat2);
            _dbContext.Add(errorReport1);
            _dbContext.Add(errorReport2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.ErrorReports(hostId1);

            // Assert
            Assert.Single(result);
            Assert.Equal(errorId1, result.First().Id);
            Assert.Equal(seen1, result.First().Seen);
            Assert.Equal(subject1, result.First().Subject);
            Assert.Equal(description1, result.First().Description);
            Assert.Equal(submitted1, result.First().Submitted);
            Assert.Equal(status1, result.First().CurrentStatus);
            Assert.Equal(priority1, result.First().CurrentPriority);
            Assert.Equal(flatId1, result.First().FlatId);
        }

        [Fact]
        public void ErrorReports_TwoEntries_TwoMatches()
        {
            // Arrange
            int addressId1 = 1;
            string street1 = "street1";
            string addressNumber1 = "addressNumber1";
            string zipCode1 = "zipCode1";
            string town1 = "town1";
            string country1 = "country1";

            int addressId2 = 2;
            string street2 = "street2";
            string addressNumber2 = "addressNumber2";
            string zipCode2 = "zipCode2";
            string town2 = "town2";
            string country2 = "country2";

            int flatId1 = 1;
            int floor1 = 1;
            string flatNumber1 = "1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string flatNumber2 = "2";
            string entryDoorCode2 = "entryDoorCode2";

            string hostId = "someHostId";

            int errorId1 = 1;
            DateTime? seen1 = null;
            string subject1 = "subject1";
            string description1 = "description1";
            DateTime submitted1 = DateTime.Now;
            Status status1 = Status.waiting;
            Priority priority1 = Priority.low;

            int errorId2 = 2;
            DateTime seen2 = DateTime.Now.AddMinutes(40);
            string subject2 = "subject2";
            string description2 = "description2";
            DateTime submitted2 = DateTime.Now.AddMinutes(10);
            Status status2 = Status.finished;
            Priority priority2 = Priority.high;

            AddressHost addressHost1 = new AddressHost
            {
                AddressID = addressId1,
                UserId = hostId
            };

            AddressHost addressHost2 = new AddressHost
            {
                AddressID = addressId2,
                UserId = hostId
            };

            Address address1 = new Address
            {
                ID = addressId1,
                Street = street1,
                Number = addressNumber1,
                ZipCode = zipCode1,
                Town = town1,
                Country = country1
            };

            Address address2 = new Address
            {
                ID = addressId2,
                Street = street2,
                Number = addressNumber2,
                ZipCode = zipCode2,
                Town = town2,
                Country = country2
            };

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId1
            };

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId2
            };

            ErrorReport errorReport1 = new ErrorReport
            {
                Id = errorId1,
                Seen = seen1,
                Subject = subject1,
                Description = description1,
                Submitted = submitted1,
                CurrentStatus = status1,
                CurrentPriority = priority1,
                FlatId = flatId1
            };

            ErrorReport errorReport2 = new ErrorReport
            {
                Id = errorId2,
                Seen = seen2,
                Subject = subject2,
                Description = description2,
                Submitted = submitted2,
                CurrentStatus = status2,
                CurrentPriority = priority2,
                FlatId = flatId2
            };

            _dbContext.Add(addressHost1);
            _dbContext.Add(addressHost2);
            _dbContext.Add(address1);
            _dbContext.Add(address2);
            _dbContext.Add(flat1);
            _dbContext.Add(flat2);
            _dbContext.Add(errorReport1);
            _dbContext.Add(errorReport2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.ErrorReports(hostId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(errorId1, result.First().Id);
            Assert.Equal(seen1, result.First().Seen);
            Assert.Equal(subject1, result.First().Subject);
            Assert.Equal(description1, result.First().Description);
            Assert.Equal(submitted1, result.First().Submitted);
            Assert.Equal(status1, result.First().CurrentStatus);
            Assert.Equal(priority1, result.First().CurrentPriority);
            Assert.Equal(flatId1, result.First().FlatId);

            Assert.Equal(errorId2, result.Last().Id);
            Assert.Equal(seen2, result.Last().Seen);
            Assert.Equal(subject2, result.Last().Subject);
            Assert.Equal(description2, result.Last().Description);
            Assert.Equal(submitted2, result.Last().Submitted);
            Assert.Equal(status2, result.Last().CurrentStatus);
            Assert.Equal(priority2, result.Last().CurrentPriority);
            Assert.Equal(flatId2, result.Last().FlatId);
        }

        #endregion

        #region Add

        [Fact]
        public void Add()
        {
            // Arrange
            DateTime? seen = null;
            int flatId = 1;
            string description = "someDescription";
            string subject = "someSubject";
            DateTime submitted = DateTime.Now;
            DateTime responded = DateTime.Now.AddMinutes(30);
            Status currentStatus = Status.waiting;
            Priority currentPriority = Priority.high;

            ErrorReportDTO errorReportDTO = new ErrorReportDTO
            {
                Seen = seen,
                FlatId = flatId,
                Description = description,
                Subject = subject,
                Submitted = submitted,
                CurrentStatus = currentStatus,
                CurrentPriority = currentPriority
            };

            // Act
            var result = _repository.Add(errorReportDTO);

            // Assert
            Assert.Equal(1, result);
        }

        #endregion

        #region Edit

        [Fact]
        public void Edit_Null()
        {
            // Arrange
            ErrorReportDTO errorReportDTO = null;

            // Act
            var result = _repository.Edit(errorReportDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_IdNotFound()
        {
            // Arrange
            int errorId = 1;
            DateTime? seen = null;
            int flatId = 1;
            string description = "someDescription";
            string subject = "someSubject";
            DateTime submitted = DateTime.Now;
            Status currentStatus = Status.waiting;
            Priority currentPriority = Priority.high;

            ErrorReport errorReport = new ErrorReport
            {
                Id = errorId,
                Seen = seen,
                FlatId = flatId,
                Description = description,
                Subject = subject,
                Submitted = submitted,
                CurrentStatus = currentStatus,
                CurrentPriority = currentPriority
            };

            _dbContext.Add(errorReport);
            _dbContext.SaveChanges();

            DateTime newSeen = DateTime.Now.AddMilliseconds(40);
            string newDescription = "someOtherDescription";
            string newSubject = "someOtherSubject";
            DateTime newSubmitted = DateTime.Now.AddMinutes(10);
            Status newCurrentStatus = Status.irrelevant;
            Priority newCurrentPriority = Priority.low;

            ErrorReportDTO errorReportDTO = new ErrorReportDTO
            {
                Id = 2,
                Seen = newSeen,
                FlatId = flatId,
                Description = newDescription,
                Subject = newSubject,
                Submitted = newSubmitted,
                CurrentStatus = newCurrentStatus,
                CurrentPriority = newCurrentPriority
            };

            // Act
            var result = _repository.Edit(errorReportDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_IdFound()
        {
            // Arrange
            int errorId = 1;
            DateTime? seen = null;
            int flatId = 1;
            string description = "someDescription";
            string subject = "someSubject";
            DateTime submitted = DateTime.Now;
            Status currentStatus = Status.waiting;
            Priority currentPriority = Priority.high;

            ErrorReport errorReport = new ErrorReport
            {
                Id = errorId,
                Seen = seen,
                FlatId = flatId,
                Description = description,
                Subject = subject,
                Submitted = submitted,
                CurrentStatus = currentStatus,
                CurrentPriority = currentPriority
            };

            _dbContext.Add(errorReport);
            _dbContext.SaveChanges();

            DateTime? newSeen = DateTime.Now.AddMinutes(40);
            int newFlatId = 2;
            string newDescription = "someOtherDescription";
            string newSubject = "someOtherSubject";
            DateTime newSubmitted = DateTime.Now.AddMinutes(10);
            Status newCurrentStatus = Status.irrelevant;
            Priority newCurrentPriority = Priority.low;

            ErrorReportDTO errorReportDTO = new ErrorReportDTO
            {
                Id = errorId,
                Seen = newSeen,
                FlatId = newFlatId,
                Description = newDescription,
                Subject = newSubject,
                Submitted = newSubmitted,
                CurrentStatus = newCurrentStatus,
                CurrentPriority = newCurrentPriority
            };

            // Act
            var result = _repository.Edit(errorReportDTO);
            ErrorReport editedErrorReport = _repository.GetErrorReport(errorId);

            // Assert
            Assert.Equal(1, result);

            Assert.Equal(newSeen, editedErrorReport.Seen);
            Assert.Equal(flatId, editedErrorReport.FlatId);
            Assert.Equal(description, editedErrorReport.Description);
            Assert.Equal(subject, editedErrorReport.Subject);
            Assert.Equal(submitted, editedErrorReport.Submitted);
            Assert.Equal(newCurrentStatus, editedErrorReport.CurrentStatus);
            Assert.Equal(newCurrentPriority, editedErrorReport.CurrentPriority);
        }

        #endregion

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
