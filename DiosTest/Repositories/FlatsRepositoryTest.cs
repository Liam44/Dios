using Dios.Data;
using Dios.Models;
using Dios.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace DiosTest.Repositories
{
    public sealed class FlatsRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _dbContext;
        private readonly FlatsRepository _repository;

        public FlatsRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            _dbContext = new ApplicationDbContext(_options);
            _repository = new FlatsRepository(_dbContext);
        }

        #region Flats - AddressId

        [Fact]
        public void FlatsAddressId_NoEntry()
        {
            // Arrange
            int addressId = -1;

            // Act
            var result = _repository.Flats(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FlatsAddressId_OneEntry_NoMatch()
        {
            // Arrange
            int addressId = -1;

            int flatId = 1;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = 0
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FlatsAddressId_OneEntry_OneMatch()
        {
            // Arrange
            int addressId = -1;

            int flatId = 1;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(addressId);

            // Assert
            Assert.Single(result);
            Assert.Equal(flatId, result.First().ID);
            Assert.Equal(floor, result.First().Floor);
            Assert.Equal(number, result.First().Number);
            Assert.Equal(entryDoorCode, result.First().EntryDoorCode);
        }

        [Fact]
        public void FlatsAddressId_TwoEntries_NoMatch()
        {
            // Arrange
            int addressId = -1;

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = 0
            };

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = number2,
                EntryDoorCode = entryDoorCode2,
                AddressID = 1
            };

            _dbContext.Add(flat1);
            _dbContext.Add(flat2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FlatsAddressId_TwoEntries_OneMatch()
        {
            // Arrange
            int addressId = 0;

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId
            };

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = number2,
                EntryDoorCode = entryDoorCode2,
                AddressID = 1
            };

            _dbContext.Add(flat1);
            _dbContext.Add(flat2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(addressId);

            // Assert
            Assert.Single(result);
            Assert.Equal(flatId1, result.First().ID);
            Assert.Equal(floor1, result.First().Floor);
            Assert.Equal(number1, result.First().Number);
            Assert.Equal(entryDoorCode1, result.First().EntryDoorCode);
        }

        [Fact]
        public void FlatsAddressId_TwoEntries_TwoMatches()
        {
            // Arrange
            int addressId = 0;

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId
            };

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = number2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId
            };

            _dbContext.Add(flat1);
            _dbContext.Add(flat2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(addressId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(flatId1, result.First().ID);
            Assert.Equal(floor1, result.First().Floor);
            Assert.Equal(number1, result.First().Number);
            Assert.Equal(entryDoorCode1, result.First().EntryDoorCode);
            Assert.Equal(flatId2, result.Last().ID);
            Assert.Equal(floor2, result.Last().Floor);
            Assert.Equal(number2, result.Last().Number);
            Assert.Equal(entryDoorCode2, result.Last().EntryDoorCode);
        }

        #endregion

        #region AmountAvailableFlats

        [Fact]
        public void AmountAvailableFlats_NoEntry()
        {
            // Arrange
            int addressId = -1;
            int amountAvailableFlats = 0;

            // Act
            int result = _repository.AmountAvailableFlats(addressId);

            // Assert
            Assert.Equal(amountAvailableFlats, result);
        }

        [Fact]
        public void AmountAvailableFlats_OneEntry_NoMatch_1()
        {
            // Arrange
            int addressId = 1;
            int amountAvailableFlats = 0;

            int flatId = 1;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = 0
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            int result = _repository.AmountAvailableFlats(addressId);

            // Assert
            Assert.Equal(amountAvailableFlats, result);
        }

        [Fact]
        public void AmountAvailableFlats_OneEntry_NoMatch_2()
        {
            // Arrange
            int addressId = 1;
            int amountAvailableFlats = 0;

            int flatId = 1;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = string.Empty
            };

            _dbContext.Add(flat);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            int result = _repository.AmountAvailableFlats(addressId);

            // Assert
            Assert.Equal(amountAvailableFlats, result);
        }

        [Fact]
        public void AmountAvailableFlats_OneEntry_OneMatch()
        {
            // Arrange
            int addressId = 1;
            int amountAvailableFlats = 1;

            int flatId = 1;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            int result = _repository.AmountAvailableFlats(addressId);

            // Assert
            Assert.Equal(amountAvailableFlats, result);
        }

        [Fact]
        public void AmountAvailableFlats_TwoEntries_TwoMatches()
        {
            // Arrange
            int addressId = 1;
            int amountAvailableFlats = 2;

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId
            };

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = number2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId
            };

            _dbContext.Add(flat1);
            _dbContext.Add(flat2);
            _dbContext.SaveChanges();

            // Act
            int result = _repository.AmountAvailableFlats(addressId);

            // Assert
            Assert.Equal(amountAvailableFlats, result);
        }

        #endregion

        #region Flats - UserId

        [Fact]
        public void FlatsUserId_NoEntry()
        {
            // Arrange
            string userId = string.Empty;

            // Act
            var result = _repository.Flats(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FlatsUserId_OneEntry_NoMatch()
        {
            // Arrange
            string userId = "SomeUserId";

            int flatId = 1;

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = "someUserId"
            };

            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FlatsUserId_OneEntry_OneMatch()
        {
            // Arrange
            string userId = "SomeUserId";

            int flatId = 1;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId
            };

            _dbContext.Add(flat);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal(flatId, result.First().ID);
            Assert.Equal(floor, result.First().Floor);
            Assert.Equal(number, result.First().Number);
            Assert.Equal(entryDoorCode, result.First().EntryDoorCode);
            Assert.Equal(addressId, result.First().AddressID);
        }

        [Fact]
        public void FlatsUserId_TwoEntries_NoMatch()
        {
            // Arrange
            string userId = "SomeUserId";

            int flatId1 = 1;
            Parameter parameter1 = new Parameter
            {
                FlatID = flatId1,
                UserId = "someUserId"
            };

            int flatId2 = 2;
            Parameter parameter2 = new Parameter
            {
                FlatID = flatId2,
                UserId = "SomeOtherUserId"
            };

            _dbContext.Add(parameter1);
            _dbContext.Add(parameter2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FlatsUserId_TwoEntries_OneMatch()
        {
            // Arrange
            string userId = "SomeUserId";

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "1";
            string entryDoorCode1 = "entryDoorCode1";
            int addressId1 = 0;

            int flatId2 = 2;

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId1
            };

            Parameter parameter1 = new Parameter
            {
                FlatID = flatId1,
                UserId = userId
            };

            Parameter parameter2 = new Parameter
            {
                FlatID = flatId2,
                UserId = "someUserId"
            };

            _dbContext.Add(flat1);
            _dbContext.Add(parameter1);
            _dbContext.Add(parameter2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal(flatId1, result.First().ID);
            Assert.Equal(floor1, result.First().Floor);
            Assert.Equal(number1, result.First().Number);
            Assert.Equal(entryDoorCode1, result.First().EntryDoorCode);
            Assert.Equal(addressId1, result.First().AddressID);
        }

        [Fact]
        public void FlatsUserId_TwoEntries_TwoMatches()
        {
            // Arrange
            string userId = "someUserId";

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "1";
            string entryDoorCode1 = "entryDoorCode1";
            int addressId1 = 1;

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "2";
            string entryDoorCode2 = "entryDoorCode2";
            int addressId2 = 2;

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId1
            };

            Parameter parameter1 = new Parameter
            {
                FlatID = flatId1,
                UserId = userId
            };

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = number2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId2
            };

            Parameter parameter2 = new Parameter
            {
                FlatID = flatId2,
                UserId = userId
            };

            _dbContext.Add(flat1);
            _dbContext.Add(parameter1);
            _dbContext.Add(flat2);
            _dbContext.Add(parameter2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flats(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(flatId1, result.First().ID);
            Assert.Equal(floor1, result.First().Floor);
            Assert.Equal(number1, result.First().Number);
            Assert.Equal(entryDoorCode1, result.First().EntryDoorCode);
            Assert.Equal(addressId1, result.First().AddressID);
            Assert.Equal(flatId2, result.Last().ID);
            Assert.Equal(floor2, result.Last().Floor);
            Assert.Equal(number2, result.Last().Number);
            Assert.Equal(entryDoorCode2, result.Last().EntryDoorCode);
            Assert.Equal(addressId2, result.Last().AddressID);
        }

        #endregion

        #region Flat

        [Fact]
        public void Flat_NoEntry()
        {
            // Arrange
            int flatId = -1;

            // Act
            var result = _repository.Flat(flatId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Flat_OneEntry_NoMatch()
        {
            // Arrange
            int flatId = 0;

            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            Flat flat = new Flat
            {
                ID = 1,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flat(flatId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Flat_OneEntry_OneMatch()
        {
            // Arrange
            int flatId = 1;

            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Flat(flatId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(floor, result.Floor);
            Assert.Equal(number, result.Number);
            Assert.Equal(entryDoorCode, result.EntryDoorCode);
            Assert.Equal(addressId, result.AddressID);
        }

        #endregion

        #region Exists

        [Fact]
        public void Exists_False()
        {
            // Arrange
            int flatId = 0;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor + 1,
                Number = "2",
                EntryDoorCode = entryDoorCode,
                AddressID = addressId + 1
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Exists(addressId, floor, number);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Exists_True()
        {
            // Arrange
            int flatId = 0;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Exists(addressId, floor, number);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Add

        [Fact]
        public void Add()
        {
            // Arrange
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            FlatDTO flat = new FlatDTO
            {
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            // Act
            var result = _repository.Add(flat);

            // Assert
            Assert.Equal(1, result);
        }

        #endregion

        #region Edit

        [Fact]
        public void Edit_Null()
        {
            // Arrange
            FlatDTO flatDTO = null;

            // Act
            var result = _repository.Edit(flatDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_IdNotFound()
        {
            // Arrange
            int flatId = 1;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            int newFloor = floor + 1;
            string newNumber = "newNumber";
            string newEntryDoorCode = "newEntryDoorCode";
            int newAddressId = addressId + 1;

            FlatDTO flatDTO = new FlatDTO
            {
                ID = 0,
                Floor = newFloor,
                Number = newNumber,
                EntryDoorCode = newEntryDoorCode,
                AddressID = newAddressId
            };

            // Act
            var result = _repository.Edit(flatDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_IdFound()
        {
            // Arrange
            int flatId = 1;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            int newFloor = floor + 1;
            string newNumber = "newNumber";
            string newEntryDoorCode = "newEntryDoorCode";
            int newAddressId = addressId + 1;

            FlatDTO flatDTO = new FlatDTO
            {
                ID = flatId,
                Floor = newFloor,
                Number = newNumber,
                EntryDoorCode = newEntryDoorCode,
                AddressID = newAddressId
            };

            // Act
            var result = _repository.Edit(flatDTO);
            FlatDTO editedFlatDTO = _repository.Flat(flatId);

            // Assert
            Assert.Equal(1, result);
            Assert.Equal(newFloor, editedFlatDTO.Floor);
            Assert.Equal(newNumber, editedFlatDTO.Number);
            Assert.Equal(newEntryDoorCode, editedFlatDTO.EntryDoorCode);
            Assert.Equal(addressId, editedFlatDTO.AddressID);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_IdNotFound()
        {
            // Arrange
            int flatId = 0;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            Flat flat = new Flat
            {
                ID = 1,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(flatId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Delete_IdFound()
        {
            // Arrange
            int flatId = 1;
            int floor = 1;
            string number = "1";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor + 1,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(flatId);
            FlatDTO deletedFlatDTO = _repository.Flat(flatId);

            // Assert
            Assert.Equal(1, result);
            Assert.Null(deletedFlatDTO);
        }

        #endregion

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
