using Dios.Data;
using Dios.Models;
using Dios.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace DiosTest.Repositories
{
    public class AddressesRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _dbContext;
        private readonly AddressesRepository _repository;

        public AddressesRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            _dbContext = new ApplicationDbContext(_options);
            _repository = new AddressesRepository(_dbContext);
        }

        #region Address

        [Fact]
        public void Address_NoEntry()
        {
            // Arrange
            int addressId = -1;

            // Act
            var result = _repository.Address(addressId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Address_OneEntry_NoMatch()
        {
            // Arrange
            int addressId = 0;

            string street = "street";
            string number = "1";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            Address address = new Address
            {
                ID = 1,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _dbContext.Add(address);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Address(addressId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Address_OneEntry_OneMatch()
        {
            // Arrange
            int addressId = 1;

            string street = "street";
            string number = "1";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            Address address = new Address
            {
                ID = 1,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _dbContext.Add(address);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Address(addressId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(street, result.Street);
            Assert.Equal(number, result.Number);
            Assert.Equal(zipCode, result.ZipCode);
            Assert.Equal(town, result.Town);
            Assert.Equal(country, result.Country);
        }

        #endregion

        #region Addresses

        [Fact]
        public void Addresses_NoEntry()
        {
            // Arrange

            // Act
            var result = _repository.Addresses();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Addresses_OneEntry()
        {
            // Arrange
            int addressId = -1;
            string street = "street";
            string number = "1";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            Address address = new Address
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _dbContext.Add(address);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Addresses();

            // Assert
            Assert.Single(result);
            Assert.Equal(addressId, result.First().ID);
            Assert.Equal(street, result.First().Street);
            Assert.Equal(number, result.First().Number);
            Assert.Equal(zipCode, result.First().ZipCode);
            Assert.Equal(town, result.First().Town);
            Assert.Equal(country, result.First().Country);
        }

        [Fact]
        public void Addresses_TwoEntries()
        {
            // Arrange
            int addressId1 = 1;
            string street1 = "street1";
            string number1 = "1";
            string zipCode1 = "zipCode1";
            string town1 = "town1";
            string country1 = "country1";

            int addressId2 = 2;
            string street2 = "street2";
            string number2 = "2";
            string zipCode2 = "zipCode2";
            string town2 = "town2";
            string country2 = "country2";

            Address address1 = new Address
            {
                ID = addressId1,
                Street = street1,
                Number = number1,
                ZipCode = zipCode1,
                Town = town1,
                Country = country1
            };

            Address address2 = new Address
            {
                ID = addressId2,
                Street = street2,
                Number = number2,
                ZipCode = zipCode2,
                Town = town2,
                Country = country2
            };

            _dbContext.Add(address1);
            _dbContext.Add(address2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Addresses();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(addressId1, result.First().ID);
            Assert.Equal(street1, result.First().Street);
            Assert.Equal(number1, result.First().Number);
            Assert.Equal(zipCode1, result.First().ZipCode);
            Assert.Equal(town1, result.First().Town);
            Assert.Equal(country1, result.First().Country);
            Assert.Equal(addressId2, result.Last().ID);
            Assert.Equal(street2, result.Last().Street);
            Assert.Equal(number2, result.Last().Number);
            Assert.Equal(zipCode2, result.Last().ZipCode);
            Assert.Equal(town2, result.Last().Town);
            Assert.Equal(country2, result.Last().Country);
        }

        #endregion

        #region Add

        [Fact]
        public void Add()
        {
            // Arrange
            string street = "street";
            string number = "1";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO addressDTO = new AddressDTO
            {
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            // Act
            var result = _repository.Add(addressDTO);

            // Assert
            Assert.Equal(1, result);
        }

        #endregion

        #region Edit

        [Fact]
        public void Edit_Null()
        {
            // Arrange
            AddressDTO addressDTO = null;

            // Act
            var result = _repository.Edit(addressDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_IdNotFound()
        {
            // Arrange
            int addressId = 1;
            string street = "street";
            string number = "1";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            Address flat = new Address
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            int newAddressId = addressId + 1;
            string newStreet = "newStreet";
            string newNumber = "newNumber";
            string newZipCode = "newZipCode";
            string newTown = "newTown";
            string newCountry = "newCountry";

            AddressDTO flatDTO = new AddressDTO
            {
                ID = newAddressId,
                Street = newStreet,
                Number = newNumber,
                ZipCode = newZipCode,
                Town = newTown,
                Country = newCountry
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
            int addressId = 1;
            string street = "street";
            string number = "1";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            Address address = new Address
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _dbContext.Add(address);
            _dbContext.SaveChanges();

            string newStreet = "newStreet";
            string newNumber = "newNumber";
            string newZipCode = "newZipCode";
            string newTown = "newTown";
            string newCountry = "newCountry";

            AddressDTO addressDTO = new AddressDTO
            {
                ID = addressId,
                Street = newStreet,
                Number = newNumber,
                ZipCode = newZipCode,
                Town = newTown,
                Country = newCountry
            };

            // Act
            var result = _repository.Edit(addressDTO);
            AddressDTO editedAddressDTO = _repository.Address(addressId);

            // Assert
            Assert.Equal(1, result);
            Assert.Equal(newStreet, editedAddressDTO.Street);
            Assert.Equal(newNumber, editedAddressDTO.Number);
            Assert.Equal(newZipCode, editedAddressDTO.ZipCode);
            Assert.Equal(newTown, editedAddressDTO.Town);
            Assert.Equal(newCountry, editedAddressDTO.Country);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_IdNotFound()
        {
            // Arrange
            int addressId = 0;
            string street = "street";
            string number = "1";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            Address address = new Address
            {
                ID = 1,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _dbContext.Add(address);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(addressId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Delete_IdFound()
        {
            // Arrange
            int addressId = 1;
            string street = "street";
            string number = "1";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            Address address = new Address
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _dbContext.Add(address);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(addressId);
            AddressDTO deletedAddressDTO = _repository.Address(addressId);

            // Assert
            Assert.Equal(1, result);
            Assert.Null(deletedAddressDTO);
        }

        #endregion

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
