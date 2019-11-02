using Dios.Data;
using Dios.Models;
using Dios.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DiosTest.Repositories
{
    public sealed class AddressHostsRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _dbContext;
        private readonly AddressHostsRepository _repository;

        public AddressHostsRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            _dbContext = new ApplicationDbContext(_options);
            _repository = new AddressHostsRepository(_dbContext);
        }

        #region AddressHost

        [Fact]
        public void AddressHost_NoEntry()
        {
            // Arrange
            int addressId = -1;
            string hostId = string.Empty;

            // Act
            var result = _repository.AddressHost(addressId, hostId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddressHost_OneEntry_NoMatch_1()
        {
            // Arrange
            int addressId = 1;
            string hostId = string.Empty;

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = "someHostId"
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.AddressHost(addressId, hostId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddressHost_OneEntry_NoMatch_2()
        {
            // Arrange
            int addressId = 0;
            string hostId = "someHostId";

            AddressHost addressHost = new AddressHost
            {
                AddressID = 1,
                UserId = hostId
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.AddressHost(addressId, hostId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddressHost_OneEntry_OneMatch()
        {
            // Arrange
            int addressId = 1;
            string street = "street";
            string number = "number";
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

            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User host = new User
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId
            };

            _dbContext.Add(address);
            _dbContext.Add(host);
            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.AddressHost(addressId, hostId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(street, result.Address.Street);
            Assert.Equal(number, result.Address.Number);
            Assert.Equal(zipCode, result.Address.ZipCode);
            Assert.Equal(town, result.Address.Town);
            Assert.Equal(country, result.Address.Country);
            Assert.Equal(personalNumber, result.Host.PersonalNumber);
            Assert.Equal(firstName, result.Host.FirstName);
            Assert.Equal(lastName, result.Host.LastName);
            Assert.Equal(emailAddress, result.Host.Email);
            Assert.Equal(phoneNumber, result.Host.PhoneNumber);
        }

        #endregion

        #region AddressHosts

        [Fact]
        public void AddressHosts_NoEntry()
        {
            // Arrange
            string hostId = string.Empty;

            // Act
            var result = _repository.AddressHosts(hostId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddressHosts_OneEntry_NoMatch()
        {
            // Arrange
            string hostId = "someHostId";
            int addressId = 1;

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = "someOtherHostId"
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.AddressHosts(hostId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddressHosts_OneEntry_OneMatch()
        {
            // Arrange
            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string email = "emailAddress";
            string phoneNumber = "phoneNumber";

            User host = new User
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            int addressId = 1;
            string street = "street";
            string number = "number";
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

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId
            };

            _dbContext.Add(host);
            _dbContext.Add(address);
            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.AddressHosts(hostId);

            // Assert
            Assert.Single(result);
            Assert.Equal(street, result.First().Address.Street);
            Assert.Equal(number, result.First().Address.Number);
            Assert.Equal(zipCode, result.First().Address.ZipCode);
            Assert.Equal(town, result.First().Address.Town);
            Assert.Equal(country, result.First().Address.Country);
            Assert.Equal(personalNumber, result.First().Host.PersonalNumber);
            Assert.Equal(firstName, result.First().Host.FirstName);
            Assert.Equal(lastName, result.First().Host.LastName);
            Assert.Equal(email, result.First().Host.Email);
            Assert.Equal(phoneNumber, result.First().Host.PhoneNumber);
        }

        [Fact]
        public void AddressHosts_TwoEntries_TwoMatches()
        {
            // Arrange
            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string email = "emailAddress";
            string phoneNumber = "phoneNumber";

            User host = new User
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            int addressId1 = 1;
            string street1 = "street1";
            string number1 = "1";
            string zipCode1 = "zipCode1";
            string town1 = "town1";
            string country1 = "country1";

            Address address1 = new Address
            {
                ID = addressId1,
                Street = street1,
                Number = number1,
                ZipCode = zipCode1,
                Town = town1,
                Country = country1
            };

            int addressId2 = 2;
            string street2 = "street2";
            string number2 = "2";
            string zipCode2 = "zipCode2";
            string town2 = "town2";
            string country2 = "country2";

            Address address2 = new Address
            {
                ID = addressId2,
                Street = street2,
                Number = number2,
                ZipCode = zipCode2,
                Town = town2,
                Country = country2
            };

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

            _dbContext.Add(address1);
            _dbContext.Add(address2);
            _dbContext.Add(host);
            _dbContext.Add(addressHost1);
            _dbContext.Add(addressHost2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.AddressHosts(hostId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(street1, result.First().Address.Street);
            Assert.Equal(number1, result.First().Address.Number);
            Assert.Equal(zipCode1, result.First().Address.ZipCode);
            Assert.Equal(town1, result.First().Address.Town);
            Assert.Equal(country1, result.First().Address.Country);
            Assert.Equal(personalNumber, result.First().Host.PersonalNumber);
            Assert.Equal(firstName, result.First().Host.FirstName);
            Assert.Equal(lastName, result.First().Host.LastName);
            Assert.Equal(email, result.First().Host.Email);
            Assert.Equal(phoneNumber, result.First().Host.PhoneNumber);

            Assert.Equal(street2, result.Last().Address.Street);
            Assert.Equal(number2, result.Last().Address.Number);
            Assert.Equal(zipCode2, result.Last().Address.ZipCode);
            Assert.Equal(town2, result.Last().Address.Town);
            Assert.Equal(country2, result.Last().Address.Country);
            Assert.Equal(personalNumber, result.Last().Host.PersonalNumber);
            Assert.Equal(firstName, result.Last().Host.FirstName);
            Assert.Equal(lastName, result.Last().Host.LastName);
            Assert.Equal(email, result.Last().Host.Email);
            Assert.Equal(phoneNumber, result.Last().Host.PhoneNumber);
        }

        #endregion

        #region Addresses

        [Fact]
        public void Addresses_NoEntry()
        {
            // Arrange
            string hostId = string.Empty;

            // Act
            var result = _repository.Addresses(hostId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Addresses_OneEntry_NoMatch()
        {
            // Arrange
            string hostId = "someHostId";
            int addressId = 1;

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = "someOtherHostId"
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Addresses(hostId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Addresses_OneEntry_OneMatch()
        {
            // Arrange
            string hostId = "someHostId";

            int addressId = 1;
            string street = "street";
            string number = "number";
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

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId
            };

            _dbContext.Add(address);
            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Addresses(hostId);

            // Assert
            Assert.Single(result);
            Assert.Equal(street, result.First().Street);
            Assert.Equal(number, result.First().Number);
            Assert.Equal(zipCode, result.First().ZipCode);
            Assert.Equal(town, result.First().Town);
            Assert.Equal(country, result.First().Country);
        }

        [Fact]
        public void Addresses_TwoEntries_TwoMatches()
        {
            // Arrange
            string hostId = "someHostId";

            int addressId1 = 1;
            string street1 = "street1";
            string number1 = "1";
            string zipCode1 = "zipCode1";
            string town1 = "town1";
            string country1 = "country1";

            Address address1 = new Address
            {
                ID = addressId1,
                Street = street1,
                Number = number1,
                ZipCode = zipCode1,
                Town = town1,
                Country = country1
            };

            int addressId2 = 2;
            string street2 = "street2";
            string number2 = "2";
            string zipCode2 = "zipCode2";
            string town2 = "town2";
            string country2 = "country2";

            Address address2 = new Address
            {
                ID = addressId2,
                Street = street2,
                Number = number2,
                ZipCode = zipCode2,
                Town = town2,
                Country = country2
            };

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

            _dbContext.Add(address1);
            _dbContext.Add(address2);
            _dbContext.Add(addressHost1);
            _dbContext.Add(addressHost2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Addresses(hostId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(street1, result.First().Street);
            Assert.Equal(number1, result.First().Number);
            Assert.Equal(zipCode1, result.First().ZipCode);
            Assert.Equal(town1, result.First().Town);
            Assert.Equal(country1, result.First().Country);

            Assert.Equal(street2, result.Last().Street);
            Assert.Equal(number2, result.Last().Number);
            Assert.Equal(zipCode2, result.Last().ZipCode);
            Assert.Equal(town2, result.Last().Town);
            Assert.Equal(country2, result.Last().Country);
        }

        #endregion

        #region Hosts

        [Fact]
        public void Hosts_NoEntry()
        {
            // Arrange
            int addressId = -1;

            // Act
            var result = _repository.Hosts(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Hosts_OneEntry_NoMatch()
        {
            // Arrange
            string hostId = "someHostId";
            int addressId = 0;

            AddressHost addressHost = new AddressHost
            {
                AddressID = 1,
                UserId = hostId
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Hosts(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Hosts_OneEntry_OneMatch()
        {
            // Arrange
            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string email = "emailAddress";
            string phoneNumber = "phoneNumber";

            User host = new User
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            int addressId = 1;

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId
            };

            _dbContext.Add(host);
            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Hosts(addressId);

            // Assert
            Assert.Single(result);
            Assert.Equal(personalNumber, result.First().PersonalNumber);
            Assert.Equal(firstName, result.First().FirstName);
            Assert.Equal(lastName, result.First().LastName);
            Assert.Equal(email, result.First().Email);
            Assert.Equal(phoneNumber, result.First().PhoneNumber);
        }

        [Fact]
        public void Hosts_TwoEntries_TwoMatches()
        {
            // Arrange
            string hostId1 = "someHostId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string email1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            User host1 = new User
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = email1,
                PhoneNumber = phoneNumber1
            };

            string hostId2 = "someHostId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string email2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User host2 = new User
            {
                Id = hostId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = email2,
                PhoneNumber = phoneNumber2
            };

            int addressId = 1;

            AddressHost addressHost1 = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId1
            };

            AddressHost addressHost2 = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId2
            };

            _dbContext.Add(host1);
            _dbContext.Add(host2);
            _dbContext.Add(addressHost1);
            _dbContext.Add(addressHost2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Hosts(addressId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(personalNumber1, result.First().PersonalNumber);
            Assert.Equal(firstName1, result.First().FirstName);
            Assert.Equal(lastName1, result.First().LastName);
            Assert.Equal(email1, result.First().Email);
            Assert.Equal(phoneNumber1, result.First().PhoneNumber);

            Assert.Equal(personalNumber2, result.Last().PersonalNumber);
            Assert.Equal(firstName2, result.Last().FirstName);
            Assert.Equal(lastName2, result.Last().LastName);
            Assert.Equal(email2, result.Last().Email);
            Assert.Equal(phoneNumber2, result.Last().PhoneNumber);
        }

        #endregion

        #region HostIds

        [Fact]
        public void HostIds_NoEntry()
        {
            // Arrange
            int addressId = -1;

            // Act
            var result = _repository.HostIds(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void HostIds_OneEntry_NoMatch()
        {
            // Arrange
            string hostId = "someHostId";
            int addressId = 0;

            AddressHost addressHost = new AddressHost
            {
                AddressID = 1,
                UserId = hostId
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.HostIds(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void HostIds_OneEntry_OneMatch()
        {
            // Arrange
            string hostId = "someHostId";
            int addressId = 1;

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.HostIds(addressId);

            // Assert
            Assert.Single(result);
            Assert.Equal(hostId, result.First());
        }

        [Fact]
        public void HostIds_TwoEntries_TwoMatches()
        {
            // Arrange
            string hostId1 = "someHostId1";
            string hostId2 = "someHostId2";
            int addressId = 1;

            AddressHost addressHost1 = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId1
            };

            AddressHost addressHost2 = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId2
            };

            _dbContext.Add(addressHost1);
            _dbContext.Add(addressHost2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.HostIds(addressId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(hostId1, result.First());
            Assert.Equal(hostId2, result.Last());
        }

        #endregion

        #region Add

        [Fact]
        public void Add()
        {
            // Arrange
            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string email = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO host = new UserDTO
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            int addressId = 1;
            string street = "street1";
            string number = "1";
            string zipCode = "zipCode1";
            string town = "town1";
            string country = "country1";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            AddressHostDTO addressHostDTO = new AddressHostDTO(address, host);

            // Act
            var result = _repository.Add(addressHostDTO);

            // Assert
            Assert.Equal(1, result);
        }

        #endregion

        #region AddHosts

        [Fact]
        public void AddHosts_HostIdsNull()
        {
            // Arrange
            List<string> hostIds = null;
            int addressId = -1;

            // Act
            var result = _repository.AddHosts(addressId, hostIds);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void AddHosts_HostIdsEmpty()
        {
            // Arrange
            List<string> hostIds = new List<string>();
            int addressId = -1;

            // Act
            var result = _repository.AddHosts(addressId, hostIds);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void AddHosts_HostIdsOneElement()
        {
            // Arrange
            List<string> hostIds = new List<string> { "someHostId" };
            int addressId = -1;

            // Act
            var result = _repository.AddHosts(addressId, hostIds);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void AddHosts_HostIdsTwoElements()
        {
            // Arrange
            List<string> hostIds = new List<string> { "someHostId1", "someHostId2" };
            int addressId = -1;

            // Act
            var result = _repository.AddHosts(addressId, hostIds);

            // Assert
            Assert.Equal(2, result);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_IdNotFound_1()
        {
            // Arrange
            int addressId = 0;
            string hostId = "someHostId";

            AddressHost addressHost = new AddressHost
            {
                AddressID = 1,
                UserId = hostId
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(addressId, hostId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Delete_IdNotFound_2()
        {
            // Arrange
            int addressId = 1;
            string hostId = "someHostId";

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = "someOtherHostId"
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(addressId, hostId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Delete_IdFound()
        {
            // Arrange
            int addressId = 1;
            string hostId = "someHostId";

            AddressHost addressHost = new AddressHost
            {
                AddressID = addressId,
                UserId = hostId
            };

            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(addressId, hostId);
            AddressHostDTO deletedAddressHostDTO = _repository.AddressHost(addressId, hostId);

            // Assert
            Assert.Equal(1, result);
            Assert.Null(deletedAddressHostDTO);
        }

        #endregion

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
