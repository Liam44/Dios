using Dios.Data;
using Dios.Models;
using Dios.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace DiosTest.Repositories
{
    public sealed class UsersRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _dbContext;
        private readonly UsersRepository _repository;

        public UsersRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            _dbContext = new ApplicationDbContext(_options);
            _repository = new UsersRepository(_dbContext);
        }

        #region Users

        [Fact]
        public void Users_NoEntry()
        {
            // Arrange

            // Act
            var result = _repository.Users();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Users_NoUsers()
        {
            // Arrange
            string roleName = "Host";
            var roleStore = new RoleStore<IdentityRole>(_dbContext);
            var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

            roleManager.CreateAsync(new IdentityRole(roleName)).Wait();

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string password = "somePassword1!";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            var userStore = new UserStore<User>(_dbContext);
            var userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);
            userManager.CreateAsync(user, password).Wait();
            userManager.AddToRoleAsync(user, roleName).Wait();

            // Act
            var result = _repository.Users();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Users_OneEntry()
        {
            // Arrange
            string roleName = "User";
            var roleStore = new RoleStore<IdentityRole>(_dbContext);
            var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

            roleManager.CreateAsync(new IdentityRole(roleName)).Wait();

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string password = "somePassword1!";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            var userStore = new UserStore<User>(_dbContext);
            var userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);
            userManager.CreateAsync(user, password).Wait();
            userManager.AddToRoleAsync(user, roleName).Wait();

            // Act
            var result = _repository.Users();

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result.First().Id);
            Assert.Equal(personalNumber, result.First().PersonalNumber);
            Assert.Equal(firstName, result.First().FirstName);
            Assert.Equal(lastName, result.First().LastName);
            Assert.Equal(emailAddress, result.First().Email);
            Assert.Equal(phoneNumber, result.First().PhoneNumber);
        }

        #endregion

        #region UsersAtAddress

        [Fact]
        public void UsersAtAddress_NoEntry()
        {
            // Arrange
            int addressId = -1;

            // Act
            var result = _repository.UsersAtAddress(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UsersAtAddress_OneEntry_NoMatch()
        {
            // Arrange
            int addressId = 1;

            int flatId = 1;
            int floor = 1;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = 2
            };

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            bool isEmailVisible = true;
            bool isPhoneNumberVisible = true;
            bool canBeContacted = true;

            Parameter parameter = new Parameter
            {
                UserId = userId,
                FlatID = flatId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(flat);
            _dbContext.Add(user);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.UsersAtAddress(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UsersAtAddress_OneEntry_OneMatch()
        {
            // Arrange
            int addressId = 1;

            int flatId = 1;
            int floor = 1;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            bool isEmailVisible = true;
            bool isPhoneNumberVisible = true;
            bool canBeContacted = true;

            Parameter parameter = new Parameter
            {
                UserId = userId,
                FlatID = flatId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(flat);
            _dbContext.Add(user);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.UsersAtAddress(addressId);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result.First().Id);
            Assert.Equal(personalNumber, result.First().PersonalNumber);
            Assert.Equal(firstName, result.First().FirstName);
            Assert.Equal(lastName, result.First().LastName);
            Assert.Equal(emailAddress, result.First().Email);
            Assert.Equal(phoneNumber, result.First().PhoneNumber);
        }

        [Fact]
        public void UsersAtAddress_TwoEntries_OneMatch()
        {
            // Arrange
            int addressId1 = 1;

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "number1";
            string entryDoorCode1 = "entryDoorCode1";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId1
            };

            int addressId2 = 2;
            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "number2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = number2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId2
            };

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user2 = new User
            {
                Id = userId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2
            };

            bool isEmailVisible1 = true;
            bool isPhoneNumberVisible1 = true;
            bool canBeContacted1 = true;

            Parameter parameter1 = new Parameter
            {
                UserId = userId1,
                FlatID = flatId1,
                IsEmailVisible = isEmailVisible1,
                IsPhoneNumberVisible = isPhoneNumberVisible1,
                CanBeContacted = canBeContacted1
            };

            bool isEmailVisible2 = false;
            bool isPhoneNumberVisible2 = false;
            bool canBeContacted2 = false;

            Parameter parameter2 = new Parameter
            {
                UserId = userId2,
                FlatID = flatId2,
                IsEmailVisible = isEmailVisible2,
                IsPhoneNumberVisible = isPhoneNumberVisible2,
                CanBeContacted = canBeContacted2
            };

            _dbContext.Add(flat1);
            _dbContext.Add(flat2);
            _dbContext.Add(user1);
            _dbContext.Add(user2);
            _dbContext.Add(parameter1);
            _dbContext.Add(parameter2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.UsersAtAddress(addressId1);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId1, result.First().Id);
            Assert.Equal(personalNumber1, result.First().PersonalNumber);
            Assert.Equal(firstName1, result.First().FirstName);
            Assert.Equal(lastName1, result.First().LastName);
            Assert.Equal(emailAddress1, result.First().Email);
            Assert.Equal(phoneNumber1, result.First().PhoneNumber);
        }

        [Fact]
        public void UsersAtAddress_TwoEntries_TwoMatches()
        {
            // Arrange
            int addressId = 1;

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "number1";
            string entryDoorCode1 = "entryDoorCode1";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId
            };

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "number2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = number2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId
            };

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user2 = new User
            {
                Id = userId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2
            };

            bool isEmailVisible1 = true;
            bool isPhoneNumberVisible1 = true;
            bool canBeContacted1 = true;

            Parameter parameter1 = new Parameter
            {
                UserId = userId1,
                FlatID = flatId1,
                IsEmailVisible = isEmailVisible1,
                IsPhoneNumberVisible = isPhoneNumberVisible1,
                CanBeContacted = canBeContacted1
            };

            bool isEmailVisible2 = false;
            bool isPhoneNumberVisible2 = false;
            bool canBeContacted2 = false;

            Parameter parameter2 = new Parameter
            {
                UserId = userId2,
                FlatID = flatId2,
                IsEmailVisible = isEmailVisible2,
                IsPhoneNumberVisible = isPhoneNumberVisible2,
                CanBeContacted = canBeContacted2
            };

            _dbContext.Add(flat1);
            _dbContext.Add(flat2);
            _dbContext.Add(user1);
            _dbContext.Add(user2);
            _dbContext.Add(parameter1);
            _dbContext.Add(parameter2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.UsersAtAddress(addressId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(userId1, result.First().Id);
            Assert.Equal(personalNumber1, result.First().PersonalNumber);
            Assert.Equal(firstName1, result.First().FirstName);
            Assert.Equal(lastName1, result.First().LastName);
            Assert.Equal(emailAddress1, result.First().Email);
            Assert.Equal(phoneNumber1, result.First().PhoneNumber);

            Assert.Equal(userId2, result.Last().Id);
            Assert.Equal(personalNumber2, result.Last().PersonalNumber);
            Assert.Equal(firstName2, result.Last().FirstName);
            Assert.Equal(lastName2, result.Last().LastName);
            Assert.Equal(emailAddress2, result.Last().Email);
            Assert.Equal(phoneNumber2, result.Last().PhoneNumber);
        }

        #endregion

        #region User

        [Fact]
        public void User_NoEntry()
        {
            // Arrange
            string userId = string.Empty;

            // Act
            var result = _repository.User(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void User_OneEntry_NoMatch()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.User("someOtherUserId");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void User_OneEntry_OneMatch()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.User(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(personalNumber, result.PersonalNumber);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
            Assert.Equal(emailAddress, result.Email);
            Assert.Equal(phoneNumber, result.PhoneNumber);
        }

        #endregion

        #region GetUser

        [Fact]
        public void GetUser_NoEntry()
        {
            // Arrange
            string userId = string.Empty;

            // Act
            var result = _repository.GetUser(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUser_OneEntry_NoMatch()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.GetUser("someOtherUserId");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUser_OneEntry_OneMatch()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.GetUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(personalNumber, result.PersonalNumber);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
            Assert.Equal(emailAddress, result.Email);
            Assert.Equal(phoneNumber, result.PhoneNumber);
        }

        #endregion

        #region UserByRegistrationCode

        [Fact]
        public void UserByRegistrationCode_NoEntry()
        {
            // Arrange
            string registrationCode = string.Empty;

            // Act
            var result = _repository.UserByRegistrationCode(registrationCode);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UserByRegistrationCode_OneEntry_NoMatch()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string registrationCode = "someRegistrationCode";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                RegistrationCode = registrationCode
            };

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.UserByRegistrationCode("someOtherRegistrationCode");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UserByRegistrationCode_OneEntry_OneMatch()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string registrationCode = "someRegistrationCode";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                RegistrationCode = registrationCode
            };

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.UserByRegistrationCode(registrationCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(personalNumber, result.PersonalNumber);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
            Assert.Equal(emailAddress, result.Email);
            Assert.Equal(phoneNumber, result.PhoneNumber);
        }

        #endregion

        #region Hosts

        [Fact]
        public void Hosts_NoEntry()
        {
            // Arrange

            // Act
            var result = _repository.Hosts();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Hosts_NoHosts()
        {
            // Arrange
            string roleName = "User";
            var roleStore = new RoleStore<IdentityRole>(_dbContext);
            var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

            roleManager.CreateAsync(new IdentityRole(roleName)).Wait();

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string password = "somePassword1!";

            User host = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            var userStore = new UserStore<User>(_dbContext);
            var userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);
            userManager.CreateAsync(host, password).Wait();
            userManager.AddToRoleAsync(host, roleName).Wait();

            // Act
            var result = _repository.Hosts();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Hosts_OneEntry()
        {
            // Arrange
            string roleName = "Host";
            var roleStore = new RoleStore<IdentityRole>(_dbContext);
            var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

            roleManager.CreateAsync(new IdentityRole(roleName)).Wait();

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string password = "somePassword1!";

            User host = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            var userStore = new UserStore<User>(_dbContext);
            var userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);
            userManager.CreateAsync(host, password).Wait();
            userManager.AddToRoleAsync(host, roleName).Wait();

            // Act
            var result = _repository.Hosts();

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result.First().Id);
            Assert.Equal(personalNumber, result.First().PersonalNumber);
            Assert.Equal(firstName, result.First().FirstName);
            Assert.Equal(lastName, result.First().LastName);
            Assert.Equal(emailAddress, result.First().Email);
            Assert.Equal(phoneNumber, result.First().PhoneNumber);
        }

        #endregion

        #region HostsAtAddress

        [Fact]
        public void HostsAtAddress_NoEntry()
        {
            // Arrange
            int addressId = -1;

            // Act
            var result = _repository.HostsAtAddress(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void HostsAtAddress_OneEntry_NoMatch()
        {
            // Arrange
            int addressId = 1;

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            AddressHost addressHost = new AddressHost
            {
                UserId = userId,
                AddressID = 2
            };

            _dbContext.Add(user);
            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.HostsAtAddress(addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void HostsAtAddress_OneEntry_OneMatch()
        {
            // Arrange
            int addressId = 1;

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            AddressHost addressHost = new AddressHost
            {
                UserId = userId,
                AddressID = addressId
            };

            _dbContext.Add(user);
            _dbContext.Add(addressHost);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.HostsAtAddress(addressId);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result.First().Id);
            Assert.Equal(personalNumber, result.First().PersonalNumber);
            Assert.Equal(firstName, result.First().FirstName);
            Assert.Equal(lastName, result.First().LastName);
            Assert.Equal(emailAddress, result.First().Email);
            Assert.Equal(phoneNumber, result.First().PhoneNumber);
        }

        [Fact]
        public void HostsAtAddress_TwoEntries_OneMatch()
        {
            // Arrange
            int addressId1 = 1;
            int addressId2 = 2;

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user2 = new User
            {
                Id = userId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2
            };

            AddressHost addressHost1 = new AddressHost
            {
                UserId = userId1,
                AddressID = addressId1
            };

            AddressHost addressHost2 = new AddressHost
            {
                UserId = userId2,
                AddressID = addressId2
            };

            _dbContext.Add(user1);
            _dbContext.Add(user2);
            _dbContext.Add(addressHost1);
            _dbContext.Add(addressHost2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.HostsAtAddress(addressId1);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId1, result.First().Id);
            Assert.Equal(personalNumber1, result.First().PersonalNumber);
            Assert.Equal(firstName1, result.First().FirstName);
            Assert.Equal(lastName1, result.First().LastName);
            Assert.Equal(emailAddress1, result.First().Email);
            Assert.Equal(phoneNumber1, result.First().PhoneNumber);
        }

        [Fact]
        public void HostsAtAddress_TwoEntries_TwoMatches()
        {
            // Arrange
            int addressId = 1;

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user2 = new User
            {
                Id = userId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2
            };

            AddressHost addressHost1 = new AddressHost
            {
                UserId = userId1,
                AddressID = addressId
            };

            AddressHost addressHost2 = new AddressHost
            {
                UserId = userId2,
                AddressID = addressId
            };

            _dbContext.Add(user1);
            _dbContext.Add(user2);
            _dbContext.Add(addressHost1);
            _dbContext.Add(addressHost2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.HostsAtAddress(addressId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(userId1, result.First().Id);
            Assert.Equal(personalNumber1, result.First().PersonalNumber);
            Assert.Equal(firstName1, result.First().FirstName);
            Assert.Equal(lastName1, result.First().LastName);
            Assert.Equal(emailAddress1, result.First().Email);
            Assert.Equal(phoneNumber1, result.First().PhoneNumber);

            Assert.Equal(userId2, result.Last().Id);
            Assert.Equal(personalNumber2, result.Last().PersonalNumber);
            Assert.Equal(firstName2, result.Last().FirstName);
            Assert.Equal(lastName2, result.Last().LastName);
            Assert.Equal(emailAddress2, result.Last().Email);
            Assert.Equal(phoneNumber2, result.Last().PhoneNumber);
        }

        #endregion

        #region Edit

        [Fact]
        public void Edit_Null()
        {
            // Arrange
            UserDTO userToBeEdited = null;

            // Act
            var result = _repository.Edit(userToBeEdited);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_IdNotFound()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            UserDTO userToBeEdited = new UserDTO
            {
                Id = "someOtherUserId"
            };

            // Act
            var result = _repository.Edit(userToBeEdited);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_IdFound()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                NormalizedEmail = emailAddress.ToUpper(),
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            string newPersonalNumber = "newPersonalNumber";
            string newFirstName = "newFirstName";
            string newLastName = "newLastName";
            string newEmailAddress = "newEmailAddress";
            string newNormalizedEmailAddress = newEmailAddress.ToUpper();
            string newPhoneNumber = "newPhoneNumber";
            string newPhoneNumber2 = "newPhoneNumber2";

            UserDTO userToBeEdited = new UserDTO
            {
                Id = userId,
                PersonalNumber = newPersonalNumber,
                FirstName = newFirstName,
                LastName = newLastName,
                Email = newEmailAddress,
                NormalizedEmail = newNormalizedEmailAddress,
                PhoneNumber = newPhoneNumber,
                PhoneNumber2 = newPhoneNumber2
            };

            // Act
            var result = _repository.Edit(userToBeEdited);
            User editedUser = _repository.GetUser(userId);

            // Assert
            Assert.Equal(1, result);

            Assert.Equal(newPersonalNumber, editedUser.PersonalNumber);
            Assert.Equal(newFirstName, editedUser.FirstName);
            Assert.Equal(newLastName, editedUser.LastName);
            Assert.Equal(newEmailAddress, editedUser.Email);
            Assert.Equal(newNormalizedEmailAddress, editedUser.NormalizedEmail);
            Assert.Equal(newPhoneNumber, editedUser.PhoneNumber);
            Assert.Equal(newPhoneNumber2, editedUser.PhoneNumber2);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_IdNotFound()
        {
            // Arrange
            string userId = string.Empty;

            // Act
            var result = _repository.Delete(userId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Delete_IdFound()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                NormalizedEmail = emailAddress.ToUpper(),
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(userId);
            User deletedUser = _repository.GetUser(userId);

            // Assert
            Assert.Equal(1, result);
            Assert.Null(deletedUser);
        }

        #endregion

        #region GenerateRegistrationCode

        [Fact]
        public void GenerateRegistrationCode()
        {
            // Arrange
            Regex regex = new Regex(@"^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]{20})$");

            // Act
            var result = _repository.GenerateRegistrationCode();

            // Assert
            Assert.True(regex.Matches(result).Count > 0);
        }

        #endregion

        #region GeneratePassword

        [Fact]
        public void GeneratePassword()
        {
            // Arrange
            Regex regex = new Regex(@"^(?=.*[0-9])(?=.*[a-zA-Z])(?=.*[!:\/;\-_&%\?])([a-zA-Z0-9!:\/;\-_&%\?]{15})$");

            // Act
            var result = _repository.GeneratePassword();

            // Assert
            Assert.True(regex.Matches(result).Count > 0);
        }

        #endregion

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
