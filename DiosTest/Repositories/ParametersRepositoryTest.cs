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
    public class ParametersRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _dbContext;
        private readonly ParametersRepository _repository;

        public ParametersRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            _dbContext = new ApplicationDbContext(_options);
            _repository = new ParametersRepository(_dbContext);
        }

        #region Parameters - UserId/AddressId

        [Fact]
        public void ParametersUserIdAddressId_NoEntry()
        {
            // Arrange
            int addressId = -1;
            string userId = string.Empty;

            // Act
            var result = _repository.Parameters(userId, addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ParametersUserIdAddressId_OneEntry_NoMatch_1()
        {
            // Arrange
            int addressId = 1;
            string userId = "someUserId";

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

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId
            };

            _dbContext.Add(flat);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Parameters(userId, addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ParametersUserIdAddressId_OneEntry_NoMatch_2()
        {
            // Arrange
            int addressId = 1;
            string userId = "someUserId";

            int flatId = 1;
            int floor = 1;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            bool isEmailVisible = true;
            bool isPhoneNumberVisible = true;
            bool canBeContacted = true;

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
                UserId = "someOtherUserId",
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(flat);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Parameters(userId, addressId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ParametersUserIdAddressId_OneEntry_OneMatch()
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
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(flat);
            _dbContext.Add(user);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Parameters(userId, addressId);

            // Assert
            Assert.Single(result);

            Assert.Equal(isEmailVisible, result.First().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible, result.First().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted, result.First().CanBeContacted);

            Assert.Equal(floor, result.First().Flat.Floor);
            Assert.Equal(number, result.First().Flat.Number);
            Assert.Equal(entryDoorCode, result.First().Flat.EntryDoorCode);

            Assert.Equal(personalNumber, result.First().User.PersonalNumber);
            Assert.Equal(firstName, result.First().User.FirstName);
            Assert.Equal(lastName, result.First().User.LastName);
            Assert.Equal(emailAddress, result.First().User.Email);
            Assert.Equal(phoneNumber, result.First().User.PhoneNumber);
        }

        [Fact]
        public void ParametersUserIdAddressId_TwoEntries_OneMatch()
        {
            // Arrange
            int addressId = 1;

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "number1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "number2";
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

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

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

            bool isEmailVisible2 = false;
            bool isPhoneNumberVisible2 = false;
            bool canBeContacted2 = false;

            Parameter parameter1 = new Parameter
            {
                FlatID = flatId1,
                UserId = userId1,
                IsEmailVisible = isEmailVisible1,
                IsPhoneNumberVisible = isPhoneNumberVisible1,
                CanBeContacted = canBeContacted1
            };

            Parameter parameter2 = new Parameter
            {
                FlatID = flatId2,
                UserId = userId2,
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
            var result = _repository.Parameters(userId1, addressId);

            // Assert
            Assert.Single(result);

            Assert.Equal(isEmailVisible1, result.First().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible1, result.First().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted1, result.First().CanBeContacted);

            Assert.Equal(floor1, result.First().Flat.Floor);
            Assert.Equal(number1, result.First().Flat.Number);
            Assert.Equal(entryDoorCode1, result.First().Flat.EntryDoorCode);

            Assert.Equal(personalNumber1, result.First().User.PersonalNumber);
            Assert.Equal(firstName1, result.First().User.FirstName);
            Assert.Equal(lastName1, result.First().User.LastName);
            Assert.Equal(emailAddress1, result.First().User.Email);
            Assert.Equal(phoneNumber1, result.First().User.PhoneNumber);
        }

        [Fact]
        public void ParametersUserIdAddressId_TwoEntries_TwoMatches()
        {
            // Arrange
            int addressId = 1;

            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "number1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "number2";
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

            bool isEmailVisible1 = true;
            bool isPhoneNumberVisible1 = true;
            bool canBeContacted1 = true;

            bool isEmailVisible2 = false;
            bool isPhoneNumberVisible2 = false;
            bool canBeContacted2 = false;

            Parameter parameter1 = new Parameter
            {
                FlatID = flatId1,
                UserId = userId,
                IsEmailVisible = isEmailVisible1,
                IsPhoneNumberVisible = isPhoneNumberVisible1,
                CanBeContacted = canBeContacted1
            };

            Parameter parameter2 = new Parameter
            {
                FlatID = flatId2,
                UserId = userId,
                IsEmailVisible = isEmailVisible2,
                IsPhoneNumberVisible = isPhoneNumberVisible2,
                CanBeContacted = canBeContacted2
            };

            _dbContext.Add(flat1);
            _dbContext.Add(flat2);
            _dbContext.Add(user);
            _dbContext.Add(parameter1);
            _dbContext.Add(parameter2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Parameters(userId, addressId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(isEmailVisible1, result.First().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible1, result.First().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted1, result.First().CanBeContacted);

            Assert.Equal(floor1, result.First().Flat.Floor);
            Assert.Equal(number1, result.First().Flat.Number);
            Assert.Equal(entryDoorCode1, result.First().Flat.EntryDoorCode);

            Assert.Equal(personalNumber, result.First().User.PersonalNumber);
            Assert.Equal(firstName, result.First().User.FirstName);
            Assert.Equal(lastName, result.First().User.LastName);
            Assert.Equal(emailAddress, result.First().User.Email);
            Assert.Equal(phoneNumber, result.First().User.PhoneNumber);

            Assert.Equal(isEmailVisible2, result.Last().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible2, result.Last().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted2, result.Last().CanBeContacted);

            Assert.Equal(floor2, result.Last().Flat.Floor);
            Assert.Equal(number2, result.Last().Flat.Number);
            Assert.Equal(entryDoorCode2, result.Last().Flat.EntryDoorCode);

            Assert.Equal(personalNumber, result.Last().User.PersonalNumber);
            Assert.Equal(firstName, result.Last().User.FirstName);
            Assert.Equal(lastName, result.Last().User.LastName);
            Assert.Equal(emailAddress, result.Last().User.Email);
            Assert.Equal(phoneNumber, result.Last().User.PhoneNumber);
        }

        #endregion

        #region Parameters - FlatId

        [Fact]
        public void ParametersFlatId_NoEntry()
        {
            // Arrange
            int flatId = -1;

            // Act
            var result = _repository.Parameters(flatId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ParametersFlatId_OneEntry_NoMatch_1()
        {
            // Arrange
            string userId = "someUserId";

            int flatId = 1;
            int floor = 1;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId + 1,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = 1
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
            var result = _repository.Parameters(flatId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ParametersFlatId_OneEntry_NoMatch_2()
        {
            // Arrange
            string userId = "someUserId";

            int flatId = 1;
            int floor = 1;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            bool isEmailVisible = true;
            bool isPhoneNumberVisible = true;
            bool canBeContacted = true;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = 1
            };

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(flat);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Parameters(flatId + 1);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ParametersFlatId_OneEntry_OneMatch()
        {
            // Arrange
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
                AddressID = 1
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
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(flat);
            _dbContext.Add(user);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Parameters(flatId);

            // Assert
            Assert.Single(result);

            Assert.Equal(isEmailVisible, result.First().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible, result.First().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted, result.First().CanBeContacted);

            Assert.Equal(floor, result.First().Flat.Floor);
            Assert.Equal(number, result.First().Flat.Number);
            Assert.Equal(entryDoorCode, result.First().Flat.EntryDoorCode);

            Assert.Equal(personalNumber, result.First().User.PersonalNumber);
            Assert.Equal(firstName, result.First().User.FirstName);
            Assert.Equal(lastName, result.First().User.LastName);
            Assert.Equal(emailAddress, result.First().User.Email);
            Assert.Equal(phoneNumber, result.First().User.PhoneNumber);
        }

        [Fact]
        public void ParametersFlatId_TwoEntries_OneMatch()
        {
            // Arrange
            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "number1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "number2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = 1
            };

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = number2,
                EntryDoorCode = entryDoorCode2,
                AddressID = 1
            };

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

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

            bool isEmailVisible2 = false;
            bool isPhoneNumberVisible2 = false;
            bool canBeContacted2 = false;

            Parameter parameter1 = new Parameter
            {
                FlatID = flatId1,
                UserId = userId1,
                IsEmailVisible = isEmailVisible1,
                IsPhoneNumberVisible = isPhoneNumberVisible1,
                CanBeContacted = canBeContacted1
            };

            Parameter parameter2 = new Parameter
            {
                FlatID = flatId2,
                UserId = userId2,
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
            var result = _repository.Parameters(flatId1);

            // Assert
            Assert.Single(result);

            Assert.Equal(isEmailVisible1, result.First().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible1, result.First().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted1, result.First().CanBeContacted);

            Assert.Equal(floor1, result.First().Flat.Floor);
            Assert.Equal(number1, result.First().Flat.Number);
            Assert.Equal(entryDoorCode1, result.First().Flat.EntryDoorCode);

            Assert.Equal(personalNumber1, result.First().User.PersonalNumber);
            Assert.Equal(firstName1, result.First().User.FirstName);
            Assert.Equal(lastName1, result.First().User.LastName);
            Assert.Equal(emailAddress1, result.First().User.Email);
            Assert.Equal(phoneNumber1, result.First().User.PhoneNumber);
        }

        [Fact]
        public void ParametersFlatId_TwoEntries_TwoMatches()
        {
            // Arrange
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
                AddressID = 1
            };

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

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

            bool isEmailVisible2 = false;
            bool isPhoneNumberVisible2 = false;
            bool canBeContacted2 = false;

            Parameter parameter1 = new Parameter
            {
                FlatID = flatId,
                UserId = userId1,
                IsEmailVisible = isEmailVisible1,
                IsPhoneNumberVisible = isPhoneNumberVisible1,
                CanBeContacted = canBeContacted1
            };

            Parameter parameter2 = new Parameter
            {
                FlatID = flatId,
                UserId = userId2,
                IsEmailVisible = isEmailVisible2,
                IsPhoneNumberVisible = isPhoneNumberVisible2,
                CanBeContacted = canBeContacted2
            };

            _dbContext.Add(flat);
            _dbContext.Add(user1);
            _dbContext.Add(user2);
            _dbContext.Add(parameter1);
            _dbContext.Add(parameter2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Parameters(flatId);

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(isEmailVisible1, result.First().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible1, result.First().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted1, result.First().CanBeContacted);

            Assert.Equal(floor, result.First().Flat.Floor);
            Assert.Equal(number, result.First().Flat.Number);
            Assert.Equal(entryDoorCode, result.First().Flat.EntryDoorCode);

            Assert.Equal(personalNumber1, result.First().User.PersonalNumber);
            Assert.Equal(firstName1, result.First().User.FirstName);
            Assert.Equal(lastName1, result.First().User.LastName);
            Assert.Equal(emailAddress1, result.First().User.Email);
            Assert.Equal(phoneNumber1, result.First().User.PhoneNumber);

            Assert.Equal(isEmailVisible2, result.Last().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible2, result.Last().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted2, result.Last().CanBeContacted);

            Assert.Equal(floor, result.Last().Flat.Floor);
            Assert.Equal(number, result.Last().Flat.Number);
            Assert.Equal(entryDoorCode, result.Last().Flat.EntryDoorCode);

            Assert.Equal(personalNumber2, result.Last().User.PersonalNumber);
            Assert.Equal(firstName2, result.Last().User.FirstName);
            Assert.Equal(lastName2, result.Last().User.LastName);
            Assert.Equal(emailAddress2, result.Last().User.Email);
            Assert.Equal(phoneNumber2, result.Last().User.PhoneNumber);
        }

        #endregion

        #region UserIds

        [Fact]
        public void UserIds_NoEntry()
        {
            // Arrange
            int flatId = -1;

            // Act
            var result = _repository.UserIds(flatId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UserIds_OneEntry_NoMatch()
        {
            // Arrange
            string userId = "someUserId";

            int flatId = 1;
            int floor = 1;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            bool isEmailVisible = true;
            bool isPhoneNumberVisible = true;
            bool canBeContacted = true;

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = 1
            };

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(flat);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.UserIds(flatId + 1);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UserIds_OneEntry_OneMatch()
        {
            // Arrange
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
                AddressID = 1
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
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(flat);
            _dbContext.Add(user);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.UserIds(flatId);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result.First());
        }

        [Fact]
        public void UserIds_TwoEntries_OneMatch()
        {
            // Arrange
            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "number1";
            string entryDoorCode1 = "entryDoorCode1";

            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "number2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = 1
            };

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = number2,
                EntryDoorCode = entryDoorCode2,
                AddressID = 1
            };

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

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

            bool isEmailVisible2 = false;
            bool isPhoneNumberVisible2 = false;
            bool canBeContacted2 = false;

            Parameter parameter1 = new Parameter
            {
                FlatID = flatId1,
                UserId = userId1,
                IsEmailVisible = isEmailVisible1,
                IsPhoneNumberVisible = isPhoneNumberVisible1,
                CanBeContacted = canBeContacted1
            };

            Parameter parameter2 = new Parameter
            {
                FlatID = flatId2,
                UserId = userId2,
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
            var result = _repository.UserIds(flatId1);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId1, result.First());
        }

        [Fact]
        public void UserIds_TwoEntries_TwoMatches()
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

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

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

            bool isEmailVisible2 = false;
            bool isPhoneNumberVisible2 = false;
            bool canBeContacted2 = false;

            Parameter parameter1 = new Parameter
            {
                FlatID = flatId,
                UserId = userId1,
                IsEmailVisible = isEmailVisible1,
                IsPhoneNumberVisible = isPhoneNumberVisible1,
                CanBeContacted = canBeContacted1
            };

            Parameter parameter2 = new Parameter
            {
                FlatID = flatId,
                UserId = userId2,
                IsEmailVisible = isEmailVisible2,
                IsPhoneNumberVisible = isPhoneNumberVisible2,
                CanBeContacted = canBeContacted2
            };

            _dbContext.Add(flat);
            _dbContext.Add(user1);
            _dbContext.Add(user2);
            _dbContext.Add(parameter1);
            _dbContext.Add(parameter2);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.UserIds(flatId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(userId1, result.First());
            Assert.Equal(userId2, result.Last());
        }

        #endregion

        #region Add

        [Fact]
        public void Add_AlreadyExists()
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

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string email = "emailAddress";
            string phoneNumber = "phoneNumber";

            User user = new User
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            bool isEmailVisible = true;
            bool isPhoneNumberVisible = true;
            bool canBeContacted = true;

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            ParameterDTO parameterDTO = new ParameterDTO(flat, user, parameter);

            _dbContext.Add(user);
            _dbContext.Add(flat);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Add(parameterDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Add_DoesntExist()
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
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            ParameterDTO parameterDTO = new ParameterDTO(flat, user, parameter);

            _dbContext.Add(user);
            _dbContext.Add(flat);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Add(parameterDTO);
            ParameterDTO addedParameterDTO = _repository.Get(userId, flatId);

            // Assert
            Assert.Equal(1, result);

            Assert.Equal(isEmailVisible, addedParameterDTO.IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible, addedParameterDTO.IsPhoneNumberVisible);
            Assert.Equal(canBeContacted, addedParameterDTO.CanBeContacted);

            Assert.Equal(floor, addedParameterDTO.Flat.Floor);
            Assert.Equal(number, addedParameterDTO.Flat.Number);
            Assert.Equal(entryDoorCode, addedParameterDTO.Flat.EntryDoorCode);

            Assert.Equal(personalNumber, addedParameterDTO.User.PersonalNumber);
            Assert.Equal(firstName, addedParameterDTO.User.FirstName);
            Assert.Equal(lastName, addedParameterDTO.User.LastName);
            Assert.Equal(emailAddress, addedParameterDTO.User.Email);
            Assert.Equal(phoneNumber, addedParameterDTO.User.PhoneNumber);
        }

        #endregion

        #region AddUsers

        [Fact]
        public void AddUsers_UserIdsNull()
        {
            // Arrange
            List<string> userIds = null;
            int flatId = -1;

            // Act
            var result = _repository.AddUsers(flatId, userIds);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void AddUsers_UserIdsEmpty()
        {
            // Arrange
            List<string> userIds = new List<string>();
            int addressId = -1;

            // Act
            var result = _repository.AddUsers(addressId, userIds);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void AddUsers_UserIdsOneElement()
        {
            // Arrange
            List<string> userIds = new List<string> { "someUserId" };
            int addressId = -1;

            // Act
            var result = _repository.AddUsers(addressId, userIds);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void AddUsers_UserIdsTwoElements()
        {
            // Arrange
            List<string> userIds = new List<string> { "someUserId1", "someUserId2" };
            int addressId = -1;

            // Act
            var result = _repository.AddUsers(addressId, userIds);

            // Assert
            Assert.Equal(2, result);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_FlatIdNotFound()
        {
            // Arrange
            int flatId = 0;
            string userId = "someUserId";

            Parameter parameter = new Parameter
            {
                FlatID = 1,
                UserId = userId
            };

            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(userId, flatId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Delete_UserIdNotFound()
        {
            // Arrange
            int flatId = 1;
            string userId = "someUserId";

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = "someOtherUserId"
            };

            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(userId, flatId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Delete_IdFound()
        {
            // Arrange
            int flatId = 1;
            string userId = "someUserId";

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId
            };

            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Delete(userId, flatId);
            ParameterDTO deletedParameterDTO = _repository.Get(userId, flatId);

            // Assert
            Assert.Equal(1, result);
            Assert.Null(deletedParameterDTO);
        }

        #endregion

        #region Edit

        [Fact]
        public void Edit_Null()
        {
            // Arrange
            ParameterDTO parameterDTO = null;

            // Act
            var result = _repository.Edit(parameterDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_FlatIdNotFound()
        {
            // Arrange
            string userId = "someUserId";
            int flatId = 1;
            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            bool newIsEmailVisible = true;
            bool newIsPhoneNumberVisible = true;
            bool newCanBeContacted = true;

            ParameterDTO parameterDTO = new ParameterDTO
            {
                User = new UserDTO { Id = userId },
                Flat = new FlatDTO { ID = flatId + 1 },
                IsEmailVisible = newIsEmailVisible,
                IsPhoneNumberVisible = newIsPhoneNumberVisible,
                CanBeContacted = newCanBeContacted
            };

            // Act
            var result = _repository.Edit(parameterDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_UserIdNotFound()
        {
            // Arrange
            string userId = "someUserId";
            int flatId = 1;
            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            bool newIsEmailVisible = true;
            bool newIsPhoneNumberVisible = true;
            bool newCanBeContacted = true;

            ParameterDTO parameterDTO = new ParameterDTO
            {
                User = new UserDTO { Id = "someOtherUserId" },
                Flat = new FlatDTO { ID = flatId },
                IsEmailVisible = newIsEmailVisible,
                IsPhoneNumberVisible = newIsPhoneNumberVisible,
                CanBeContacted = newCanBeContacted
            };

            // Act
            var result = _repository.Edit(parameterDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_Unchanged()
        {
            // Arrange
            string userId = "someUserId";
            int flatId = 1;
            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            bool newIsEmailVisible = isEmailVisible;
            bool newIsPhoneNumberVisible = isPhoneNumberVisible;
            bool newCanBeContacted = canBeContacted;

            ParameterDTO parameterDTO = new ParameterDTO
            {
                User = new UserDTO { Id = "someOtherUserId" },
                Flat = new FlatDTO { ID = flatId + 1 },
                IsEmailVisible = newIsEmailVisible,
                IsPhoneNumberVisible = newIsPhoneNumberVisible,
                CanBeContacted = newCanBeContacted
            };

            // Act
            var result = _repository.Edit(parameterDTO);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Edit_Changed()
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

            int flatId = 1;
            int floor = 1;
            string number = "number";
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

            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(user);
            _dbContext.Add(flat);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            bool newIsEmailVisible = true;
            bool newIsPhoneNumberVisible = true;
            bool newCanBeContacted = true;

            ParameterDTO parameterDTO = new ParameterDTO
            {
                User = new UserDTO { Id = userId },
                Flat = new FlatDTO { ID = flatId },
                IsEmailVisible = newIsEmailVisible,
                IsPhoneNumberVisible = newIsPhoneNumberVisible,
                CanBeContacted = newCanBeContacted
            };

            // Act
            var result = _repository.Edit(parameterDTO);
            ParameterDTO editedParameterDTO = _repository.Get(userId, flatId);

            // Assert
            Assert.Equal(1, result);
            Assert.Equal(newIsEmailVisible, editedParameterDTO.IsEmailVisible);
            Assert.Equal(newIsPhoneNumberVisible, editedParameterDTO.IsPhoneNumberVisible);
            Assert.Equal(newCanBeContacted, editedParameterDTO.CanBeContacted);
        }

        #endregion

        #region Get

        [Fact]
        public void Get_FlatIdNotFound()
        {
            // Arrange
            string userId = "someUserId";
            int flatId = 1;
            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Get(userId, flatId + 1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Get_UserIdNotFound()
        {
            // Arrange
            string userId = "someUserId";
            int flatId = 1;
            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Get("someOtherUserId", flatId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Get_Found()
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

            int flatId = 1;
            int floor = 1;
            string number = "number";
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

            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;

            Parameter parameter = new Parameter
            {
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(user);
            _dbContext.Add(flat);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Get(userId, flatId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(isEmailVisible, result.IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible, result.IsPhoneNumberVisible);
            Assert.Equal(canBeContacted, result.CanBeContacted);
        }

        #endregion

        #region AllParameters

        [Fact]
        public void AllParameters_NoEntry()
        {
            // Arrange

            // Act
            var result = _repository.AllParameters();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AllParameters_OneEntry()
        {
            // Arrange
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
                AddressID = 1
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
                FlatID = flatId,
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _dbContext.Add(flat);
            _dbContext.Add(user);
            _dbContext.Add(parameter);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.AllParameters();

            // Assert
            Assert.Single(result);

            Assert.Equal(isEmailVisible, result.First().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible, result.First().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted, result.First().CanBeContacted);

            Assert.Equal(floor, result.First().Flat.Floor);
            Assert.Equal(number, result.First().Flat.Number);
            Assert.Equal(entryDoorCode, result.First().Flat.EntryDoorCode);

            Assert.Equal(personalNumber, result.First().User.PersonalNumber);
            Assert.Equal(firstName, result.First().User.FirstName);
            Assert.Equal(lastName, result.First().User.LastName);
            Assert.Equal(emailAddress, result.First().User.Email);
            Assert.Equal(phoneNumber, result.First().User.PhoneNumber);
        }

        [Fact]
        public void AllParameters_TwoEntries()
        {
            // Arrange
            int addressId1 = 1;
            int flatId1 = 1;
            int floor1 = 1;
            string number1 = "number1";
            string entryDoorCode1 = "entryDoorCode1";

            int addressId2 = 2;
            int flatId2 = 2;
            int floor2 = 2;
            string number2 = "number2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = number1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId1
            };

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

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1
            };

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

            bool isEmailVisible2 = false;
            bool isPhoneNumberVisible2 = false;
            bool canBeContacted2 = false;

            Parameter parameter1 = new Parameter
            {
                FlatID = flatId1,
                UserId = userId1,
                IsEmailVisible = isEmailVisible1,
                IsPhoneNumberVisible = isPhoneNumberVisible1,
                CanBeContacted = canBeContacted1
            };

            Parameter parameter2 = new Parameter
            {
                FlatID = flatId2,
                UserId = userId2,
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
            var result = _repository.AllParameters();

            // Assert
            Assert.Equal(2, result.Count());

            Assert.Equal(isEmailVisible1, result.First().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible1, result.First().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted1, result.First().CanBeContacted);

            Assert.Equal(flatId1, result.First().Flat.ID);
            Assert.Equal(floor1, result.First().Flat.Floor);
            Assert.Equal(number1, result.First().Flat.Number);
            Assert.Equal(entryDoorCode1, result.First().Flat.EntryDoorCode);
            Assert.Equal(addressId1, result.First().Flat.AddressID);

            Assert.Equal(userId1, result.First().User.Id);
            Assert.Equal(personalNumber1, result.First().User.PersonalNumber);
            Assert.Equal(firstName1, result.First().User.FirstName);
            Assert.Equal(lastName1, result.First().User.LastName);
            Assert.Equal(emailAddress1, result.First().User.Email);
            Assert.Equal(phoneNumber1, result.First().User.PhoneNumber);

            Assert.Equal(isEmailVisible2, result.Last().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible2, result.Last().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted2, result.Last().CanBeContacted);

            Assert.Equal(flatId2, result.Last().Flat.ID);
            Assert.Equal(floor2, result.Last().Flat.Floor);
            Assert.Equal(number2, result.Last().Flat.Number);
            Assert.Equal(entryDoorCode2, result.Last().Flat.EntryDoorCode);
            Assert.Equal(addressId2, result.Last().Flat.AddressID);

            Assert.Equal(userId2, result.Last().User.Id);
            Assert.Equal(personalNumber2, result.Last().User.PersonalNumber);
            Assert.Equal(firstName2, result.Last().User.FirstName);
            Assert.Equal(lastName2, result.Last().User.LastName);
            Assert.Equal(emailAddress2, result.Last().User.Email);
            Assert.Equal(phoneNumber2, result.Last().User.PhoneNumber);
        }

        #endregion

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
