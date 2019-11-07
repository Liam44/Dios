using Dios.Controllers;
using Dios.Extensions;
using Dios.Helpers;
using Dios.Models;
using Dios.Repositories;
using Dios.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace DiosTest.Controllers
{
    public sealed class AddressesControllerTest
    {
        private readonly Mock<IAddressesRepository> _addressesRepository;
        private readonly Mock<IFlatsRepository> _flatsRepository;
        private readonly Mock<IParametersRepository> _parameterRepository;
        private readonly Mock<IUsersRepository> _usersRepository;
        private readonly Mock<IAddressHostsRepository> _addressHostsRepository;
        private readonly Mock<IHostingEnvironment> _environment;
        private readonly Mock<IExport> _export;
        private readonly Mock<IZipFile> _zipFile;

        private readonly AddressesController _controller;

        private const string _webRootPath = "webRootPath";
        private const string _lists = "lists";

        public AddressesControllerTest()
        {
            _addressesRepository = new Mock<IAddressesRepository>();
            _flatsRepository = new Mock<IFlatsRepository>();
            _parameterRepository = new Mock<IParametersRepository>();
            _usersRepository = new Mock<IUsersRepository>();
            _addressHostsRepository = new Mock<IAddressHostsRepository>();
            _environment = new Mock<IHostingEnvironment>();
            _export = new Mock<IExport>();
            _zipFile = new Mock<IZipFile>();

            _controller = new AddressesController(_addressesRepository.Object,
                                                  _flatsRepository.Object,
                                                  _parameterRepository.Object,
                                                  _usersRepository.Object,
                                                  _addressHostsRepository.Object,
                                                  _environment.Object,
                                                  _export.Object,
                                                  _zipFile.Object);
        }

        #region Index

        [Fact]
        public void Index()
        {
            // Assert

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region Details

        [Fact]
        public void Details_IdNull()
        {
            // Arrange
            int? addressId = null;

            // Act
            var result = _controller.Details(addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Never);
        }

        [Fact]
        public void Details_IdNotFound()
        {
            // Arrange
            int? addressId = 1;

            AddressDTO address = null;

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Details(addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        [Fact]
        public void Details_ValidId_NoFlats_NoHosts_DataCanBeDeleted()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int? addressId = 1;
            string street = "street";
            string number = "number";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = (int)addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country,
                Flats = null,
                Hosts = null
            };

            List<FlatDTO> flats = new List<FlatDTO>();
            List<UserDTO> hosts = new List<UserDTO>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Admin") },
                                                              "Admin"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            bool canDataBeDeleted = true;

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddressDetailsVM>(viewResult.Model);
            Assert.Equal(address.ID, model.ID);
            Assert.Equal(street, model.Street);
            Assert.Equal(number, model.Number);
            Assert.Equal(zipCode, model.ZipCode);
            Assert.Equal(town, model.Town);
            Assert.Equal(country, model.Country);
            Assert.Empty(model.Flats);

            Assert.Equal(0, model.AmountFlats);
            Assert.Equal(0, model.AmountAvailableFlats);

            Assert.Equal(0, model.AmountUsers);

            Assert.Empty(model.Hosts);
            Assert.Equal(0, model.AmountHosts);

            Assert.Equal(canDataBeDeleted, model.CanDataBeDeleted);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _flatsRepository.Verify(f => f.Flats((int)addressId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Hosts((int)addressId), Times.Once);
        }

        [Fact]
        public void Details_ValidId_OneFlat_NoParameter_OneHost_DataCanNotBeDeleted()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int? addressId = 1;
            string street = "street";
            string number = "number";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = (int)addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country,
                Flats = null,
                Hosts = null
            };

            int flatId = 2;
            int floor = 10;
            string flatNumber = "flatNumber";
            string entryDoorCode = "entryDoorCode";

            FlatDTO flat = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode
            };

            List<FlatDTO> flats = new List<FlatDTO> { flat };

            List<ParameterDTO> parameters = new List<ParameterDTO>();

            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserDTO host = new UserDTO
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            List<UserDTO> hosts = new List<UserDTO> { host };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "User") },
                                                              "User"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            bool canDataBeDeleted = false;

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddressDetailsVM>(viewResult.Model);
            Assert.Equal(address.ID, model.ID);
            Assert.Equal(street, model.Street);
            Assert.Equal(number, model.Number);
            Assert.Equal(zipCode, model.ZipCode);
            Assert.Equal(town, model.Town);
            Assert.Equal(country, model.Country);

            Assert.Single(model.Flats);
            Assert.Equal(floor, model.Flats.First().Key);
            Assert.Single(model.Flats[floor]);
            Assert.Equal(flatId, model.Flats[floor].First().ID);
            Assert.Equal(floor, model.Flats[floor].First().Floor);
            Assert.Equal(flatNumber, model.Flats[floor].First().Number);
            Assert.Equal(entryDoorCode, model.Flats[floor].First().EntryDoorCode);
            Assert.Empty(model.Flats[floor].First().Parameters);

            Assert.Equal(1, model.AmountFlats);
            Assert.Equal(0, model.AmountAvailableFlats);

            Assert.Equal(0, model.AmountUsers);

            Assert.Single(model.Hosts);
            Assert.Equal(hostId, model.Hosts.First().Id);
            Assert.Equal(personalNumber, model.Hosts.First().PersonalNumber);
            Assert.Equal(firstName, model.Hosts.First().FirstName);
            Assert.Equal(lastName, model.Hosts.First().LastName);
            Assert.Equal(emailAddress, model.Hosts.First().Email);
            Assert.Equal(phoneNumber, model.Hosts.First().PhoneNumber);
            Assert.Equal(phoneNumber2, model.Hosts.First().PhoneNumber2);

            Assert.Equal(1, model.AmountHosts);
            Assert.Equal(canDataBeDeleted, model.CanDataBeDeleted);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _flatsRepository.Verify(f => f.Flats((int)addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Hosts((int)addressId), Times.Once);
        }

        [Fact]
        public void Details_ValidId_OneFlat_ThreeParameters_ThreeHosts_DataCanNotBeDeleted()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int? addressId = 1;
            string street = "street";
            string number = "number";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = (int)addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country,
                Flats = null,
                Hosts = null
            };

            int flatId = 2;
            int floor = 10;
            string flatNumber = "flatNumber";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode
            };

            FlatDTO flatDTO = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode
            };

            List<FlatDTO> flats = new List<FlatDTO> { flatDTO };

            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "2firstName";
            string lastName1 = "2lastName";
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
            string firstName2 = "1firstName";
            string lastName2 = "1lastName";
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

            string userId3 = "someUserId3";
            string personalNumber3 = "personalNumber3";
            string firstName3 = firstName1;
            string lastName3 = lastName2;
            string emailAddress3 = "emailAddress3";
            string phoneNumber3 = "phoneNumber3";

            User user3 = new User
            {
                Id = userId3,
                PersonalNumber = personalNumber3,
                FirstName = firstName3,
                LastName = lastName3,
                Email = emailAddress3,
                PhoneNumber = phoneNumber3
            };

            ParameterDTO parameter1 = new ParameterDTO(flat, user1);
            ParameterDTO parameter2 = new ParameterDTO(flat, user2);
            ParameterDTO parameter3 = new ParameterDTO(flat, user3);

            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter1, parameter2, parameter3 };

            string hostId1 = "someHostId1";
            string personalNumberHost1 = "personalNumberHost1";
            string firstNameHost1 = "2firstNameHost";
            string lastNameHost1 = "2lastNameHost";
            string emailAddressHost1 = "emailAddressHost1";
            string phoneNumberHost1 = "phoneNumberHost1";
            string phoneNumber2Host1 = "phoneNumber2Host1";

            UserDTO host1 = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumberHost1,
                FirstName = firstNameHost1,
                LastName = lastNameHost1,
                Email = emailAddressHost1,
                PhoneNumber = phoneNumberHost1,
                PhoneNumber2 = phoneNumber2Host1
            };

            string hostId2 = "someHostId2";
            string personalNumberHost2 = "personalNumberHost2";
            string firstNameHost2 = "1firstNameHost";
            string lastNameHost2 = "1lastNameHost";
            string emailAddressHost2 = "emailAddressHost2";
            string phoneNumberHost2 = "phoneNumberHost2";
            string phoneNumber2Host2 = "phoneNumber2Host2";

            UserDTO host2 = new UserDTO
            {
                Id = hostId2,
                PersonalNumber = personalNumberHost2,
                FirstName = firstNameHost2,
                LastName = lastNameHost2,
                Email = emailAddressHost2,
                PhoneNumber = phoneNumberHost2,
                PhoneNumber2 = phoneNumber2Host2
            };

            string hostId3 = "someHostId3";
            string personalNumberHost3 = "personalNumberHost3";
            string firstNameHost3 = firstNameHost1;
            string lastNameHost3 = lastNameHost2;
            string emailAddressHost3 = "emailAddressHost3";
            string phoneNumberHost3 = "phoneNumberHost3";
            string phoneNumber2Host3 = "phoneNumber2Host3";

            UserDTO host3 = new UserDTO
            {
                Id = hostId3,
                PersonalNumber = personalNumberHost3,
                FirstName = firstNameHost3,
                LastName = lastNameHost3,
                Email = emailAddressHost3,
                PhoneNumber = phoneNumberHost3,
                PhoneNumber2 = phoneNumber2Host3
            };

            List<UserDTO> hosts = new List<UserDTO> { host1, host2, host3 };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "User") },
                                                              "User"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            bool canDataBeDeleted = false;

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);

            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddressDetailsVM>(viewResult.Model);
            Assert.Equal(address.ID, model.ID);
            Assert.Equal(street, model.Street);
            Assert.Equal(number, model.Number);
            Assert.Equal(zipCode, model.ZipCode);
            Assert.Equal(town, model.Town);
            Assert.Equal(country, model.Country);

            Assert.Single(model.Flats);
            Assert.Equal(floor, model.Flats.First().Key);
            Assert.Single(model.Flats[floor]);
            Assert.Equal(flatId, model.Flats[floor].First().ID);
            Assert.Equal(floor, model.Flats[floor].First().Floor);
            Assert.Equal(flatNumber, model.Flats[floor].First().Number);
            Assert.Equal(entryDoorCode, model.Flats[floor].First().EntryDoorCode);
            Assert.Equal(3, model.Flats[floor].First().Parameters.Count);
            Assert.Equal(userId2, model.Flats[floor].First().Parameters.First().User.Id);
            Assert.Equal(personalNumber2, model.Flats[floor].First().Parameters.First().User.PersonalNumber);
            Assert.Equal(firstName2, model.Flats[floor].First().Parameters.First().User.FirstName);
            Assert.Equal(lastName2, model.Flats[floor].First().Parameters.First().User.LastName);
            Assert.Equal(emailAddress2, model.Flats[floor].First().Parameters.First().User.Email);
            Assert.Equal(phoneNumber2, model.Flats[floor].First().Parameters.First().User.PhoneNumber);
            Assert.Equal(userId3, model.Flats[floor].First().Parameters[1].User.Id);
            Assert.Equal(personalNumber3, model.Flats[floor].First().Parameters[1].User.PersonalNumber);
            Assert.Equal(firstName3, model.Flats[floor].First().Parameters[1].User.FirstName);
            Assert.Equal(lastName3, model.Flats[floor].First().Parameters[1].User.LastName);
            Assert.Equal(emailAddress3, model.Flats[floor].First().Parameters[1].User.Email);
            Assert.Equal(phoneNumber3, model.Flats[floor].First().Parameters[1].User.PhoneNumber);
            Assert.Equal(userId1, model.Flats[floor].First().Parameters.Last().User.Id);
            Assert.Equal(personalNumber1, model.Flats[floor].First().Parameters.Last().User.PersonalNumber);
            Assert.Equal(firstName1, model.Flats[floor].First().Parameters.Last().User.FirstName);
            Assert.Equal(lastName1, model.Flats[floor].First().Parameters.Last().User.LastName);
            Assert.Equal(emailAddress1, model.Flats[floor].First().Parameters.Last().User.Email);
            Assert.Equal(phoneNumber1, model.Flats[floor].First().Parameters.Last().User.PhoneNumber);

            Assert.Equal(1, model.AmountFlats);
            Assert.Equal(0, model.AmountAvailableFlats);

            Assert.Equal(3, model.AmountUsers);

            Assert.Equal(3, model.Hosts.Count);
            Assert.Equal(hostId2, model.Hosts.First().Id);
            Assert.Equal(personalNumberHost2, model.Hosts.First().PersonalNumber);
            Assert.Equal(firstNameHost2, model.Hosts.First().FirstName);
            Assert.Equal(lastNameHost2, model.Hosts.First().LastName);
            Assert.Equal(emailAddressHost2, model.Hosts.First().Email);
            Assert.Equal(phoneNumberHost2, model.Hosts.First().PhoneNumber);
            Assert.Equal(phoneNumber2Host2, model.Hosts.First().PhoneNumber2);
            Assert.Equal(hostId3, model.Hosts[1].Id);
            Assert.Equal(personalNumberHost3, model.Hosts[1].PersonalNumber);
            Assert.Equal(firstNameHost3, model.Hosts[1].FirstName);
            Assert.Equal(lastNameHost3, model.Hosts[1].LastName);
            Assert.Equal(emailAddressHost3, model.Hosts[1].Email);
            Assert.Equal(phoneNumberHost3, model.Hosts[1].PhoneNumber);
            Assert.Equal(phoneNumber2Host3, model.Hosts[1].PhoneNumber2);
            Assert.Equal(hostId1, model.Hosts.Last().Id);
            Assert.Equal(personalNumberHost1, model.Hosts.Last().PersonalNumber);
            Assert.Equal(firstNameHost1, model.Hosts.Last().FirstName);
            Assert.Equal(lastNameHost1, model.Hosts.Last().LastName);
            Assert.Equal(emailAddressHost1, model.Hosts.Last().Email);
            Assert.Equal(phoneNumberHost1, model.Hosts.Last().PhoneNumber);
            Assert.Equal(phoneNumber2Host1, model.Hosts.Last().PhoneNumber2);

            Assert.Equal(3, model.AmountHosts);
            Assert.Equal(canDataBeDeleted, model.CanDataBeDeleted);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _flatsRepository.Verify(f => f.Flats((int)addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Hosts((int)addressId), Times.Once);
        }

        #endregion

        #region Create

        #region Create - Get

        [Fact]
        public void Create_Get()
        {
            // Assert

            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region Create - Post

        [Fact]
        public void Create_Post_InvalidModelState()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Error");

            // Act
            var result = _controller.Create(new AddressCreateVM());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AddressCreateVM>(viewResult.Model);
        }

        [Fact]
        public void Create_Post_ValidModelState()
        {
            // Arrange
            int addressId = 1;
            string street = " street ";
            string number = " number ";
            string zipCode = " zipCode ";
            string town = " town ";
            string country = " country ";

            AddressCreateVM address = new AddressCreateVM
            {
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _addressesRepository.Setup(a => a.Add(It.IsAny<AddressDTO>()))
                                .Callback((AddressDTO addressDTO) => addressDTO.ID = addressId)
                                .Returns(1);

            // Act
            var result = _controller.Create(address);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(FlatsController.Create), viewResult.ActionName);
            Assert.Equal("Flats", viewResult.ControllerName);
            Assert.Equal(addressId, viewResult.RouteValues.First().Value);

            _addressesRepository.Verify(a => a.Add(It.IsAny<AddressDTO>()), Times.Once);
        }

        #endregion

        #endregion

        #region Edit

        [Fact]
        public void Edit_Get_IdNull()
        {
            // Arrange
            int? addressId = null;

            // Act
            var result = _controller.Edit(addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _addressesRepository.Verify(a => a.Address(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Edit_Get_IdNotFound()
        {
            // Arrange
            int? addressId = 1;

            AddressDTO address = null;

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Edit(addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        [Fact]
        public void Edit_Get_IdFound()
        {
            // Arrange
            int? addressId = 1;
            string street = "street";
            string number = "number";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = (int)addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Edit(addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(addressId, viewResult.Model);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_Get_IdNull()
        {
            // Arrange
            int? addressId = null;

            // Act
            var result = _controller.Delete(addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _addressesRepository.Verify(a => a.Address(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Delete_Get_IdNotFound()
        {
            // Arrange
            int? addressId = 1;

            AddressDTO address = null;

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Delete(addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        [Fact]
        public void Delete_Get_IdFound()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int? addressId = 1;
            string street = "street";
            string number = "number";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = (int)addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Delete(addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddressDetailsVM>(viewResult.Model);
            Assert.Equal(addressId, model.ID);
            Assert.Equal(street, model.Street);
            Assert.Equal(number, model.Number);
            Assert.Equal(zipCode, model.ZipCode);
            Assert.Equal(town, model.Town);
            Assert.Equal(country, model.Country);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        #endregion

        #region DeleteConfirmed

        [Fact]
        public void DeleteConfirmed_IdNotFound()
        {
            // Arrange
            int addressId = 1;

            AddressDTO address = null;

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.DeleteConfirmed(addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _addressesRepository.Verify(a => a.Delete(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void DeleteConfirmed_IdFound()
        {
            // Arrange
            int addressId = 1;

            AddressDTO address = new AddressDTO();

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _addressesRepository.Setup(a => a.Delete(It.IsAny<int>()))
                                .Returns(1);

            // Act
            var result = _controller.DeleteConfirmed(addressId);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), viewResult.ActionName);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _addressesRepository.Verify(a => a.Delete(addressId), Times.Once);
        }

        #endregion

        #region ExportUsers

        [Fact]
        public void ExportUsers_AddressNull()
        {
            // Arrange
            int addressId = 1;
            string webRootPath = _webRootPath + addressId.ToString();

            _environment.Setup(e => e.WebRootPath)
                        .Returns(webRootPath);

            AddressDTO address = null;

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.ExportUsers(addressId);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), viewResult.ActionName);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        [Fact]
        public void ExportUsers_AddressNotNull_IOException()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int addressId = 1;
            string webRootPath = _webRootPath + addressId.ToString();

            _environment.Setup(e => e.WebRootPath)
                        .Returns(webRootPath);

            AddressDTO address = new AddressDTO
            {
                ID = addressId
            };

            string path = $@"{webRootPath}\{_lists}\{addressId.ToString()}";

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _export.Setup(e => e.ExportUsers(It.IsAny<IZipFile>(), It.IsAny<IUsersRepository>(), It.IsAny<AddressDTO>(), It.IsAny<string>()))
                   .Throws(new IOException());

            // Act
            var result = _controller.ExportUsers(addressId);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), viewResult.ActionName);
            Assert.Null(viewResult.RouteValues);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _export.Verify(e => e.ExportUsers(_zipFile.Object, _usersRepository.Object, address, path), Times.Once);
        }

        [Fact]
        public void ExportUsers_AddressNotNull_ExportFailed()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int addressId = 1;
            string webRootPath = _webRootPath + addressId.ToString();

            _environment.Setup(e => e.WebRootPath)
                        .Returns(webRootPath);

            AddressDTO address = new AddressDTO
            {
                ID = addressId
            };

            string path = $@"{webRootPath}\{_lists}\{addressId.ToString()}";

            ZipResult zipResult = null;

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _export.Setup(e => e.ExportUsers(It.IsAny<IZipFile>(), It.IsAny<IUsersRepository>(), It.IsAny<AddressDTO>(), It.IsAny<string>()))
                   .Returns(zipResult);

            // Act
            var result = _controller.ExportUsers(addressId);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Details), viewResult.ActionName);
            Assert.True(viewResult.RouteValues.ContainsKey("id"));
            Assert.Equal(addressId, viewResult.RouteValues["id"]);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _export.Verify(e => e.ExportUsers(_zipFile.Object, _usersRepository.Object, address, path), Times.Once);
        }

        [Fact]
        public void ExportUsers_AddressNotNull_ExportSucceeded_MemoryStreamNull()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int addressId = 1;
            string webRootPath = _webRootPath + addressId.ToString();

            _environment.Setup(e => e.WebRootPath)
                        .Returns(webRootPath);

            AddressDTO address = new AddressDTO
            {
                ID = addressId
            };

            string path = $@"{webRootPath}\{_lists}\{addressId.ToString()}";

            MemoryStream memoryStream = null;
            string fileName = "someFileName";
            ZipResult zipResult = new ZipResult(fileName, memoryStream);

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _export.Setup(e => e.ExportUsers(It.IsAny<IZipFile>(), It.IsAny<IUsersRepository>(), It.IsAny<AddressDTO>(), It.IsAny<string>()))
                   .Returns(zipResult);

            // Act
            var result = _controller.ExportUsers(addressId);

            // Assert
            var viewResult = Assert.IsAssignableFrom<FileStreamResult>(result);
            Assert.Equal(fileName, viewResult.FileDownloadName);
            Assert.NotNull(viewResult.FileStream);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _export.Verify(e => e.ExportUsers(_zipFile.Object, _usersRepository.Object, address, path), Times.Once);
        }

        [Fact]
        public void ExportUsers_AddressNotNull_ExportSucceeded_MemoryStreamNotNull_FileNameNull()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int addressId = 1;
            string webRootPath = _webRootPath + addressId.ToString();

            _environment.Setup(e => e.WebRootPath)
                        .Returns(webRootPath);

            AddressDTO address = new AddressDTO
            {
                ID = addressId
            };

            string path = $@"{webRootPath}\{_lists}\{addressId.ToString()}";

            MemoryStream memoryStream = new MemoryStream();
            string fileName = null;
            ZipResult zipResult = new ZipResult(fileName, memoryStream);

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _export.Setup(e => e.ExportUsers(It.IsAny<IZipFile>(), It.IsAny<IUsersRepository>(), It.IsAny<AddressDTO>(), It.IsAny<string>()))
                   .Returns(zipResult);

            // Act
            var result = _controller.ExportUsers(addressId);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Details), viewResult.ActionName);
            Assert.True(viewResult.RouteValues.ContainsKey("id"));
            Assert.Equal(addressId, viewResult.RouteValues["id"]);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _export.Verify(e => e.ExportUsers(_zipFile.Object, _usersRepository.Object, address, path), Times.Once);
        }

        [Fact]
        public void ExportUsers_AddressNotNull_ExportSucceeded_MemoryStreamNotNull_FileNameEmpty()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int addressId = 1;
            string webRootPath = _webRootPath + addressId.ToString();

            _environment.Setup(e => e.WebRootPath)
                        .Returns(webRootPath);

            AddressDTO address = new AddressDTO
            {
                ID = addressId
            };

            string path = $@"{webRootPath}\{_lists}\{addressId.ToString()}";

            MemoryStream memoryStream = new MemoryStream();
            string fileName = string.Empty;
            ZipResult zipResult = new ZipResult(fileName, memoryStream);

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _export.Setup(e => e.ExportUsers(It.IsAny<IZipFile>(), It.IsAny<IUsersRepository>(), It.IsAny<AddressDTO>(), It.IsAny<string>()))
                   .Returns(zipResult);

            // Act
            var result = _controller.ExportUsers(addressId);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Details), viewResult.ActionName);
            Assert.True(viewResult.RouteValues.ContainsKey("id"));
            Assert.Equal(addressId, viewResult.RouteValues["id"]);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _export.Verify(e => e.ExportUsers(_zipFile.Object, _usersRepository.Object, address, path), Times.Once);
        }

        [Fact]
        public void ExportUsers_AddressNotNull_ExportSucceeded_MemoryStreamNotNull_CorrectFileName()
        {
            // Arrange
            NavigationProperties.NavigationPropertiesWrapper = new NavigationPropertiesWrapper();

            int addressId = 1;
            string webRootPath = _webRootPath + addressId.ToString();

            _environment.Setup(e => e.WebRootPath)
                        .Returns(webRootPath);

            AddressDTO address = new AddressDTO
            {
                ID = addressId
            };

            string path = $@"{webRootPath}\{_lists}\{addressId.ToString()}";

            MemoryStream memoryStream = new MemoryStream();
            string fileName = "someFileName";
            ZipResult zipResult = new ZipResult(fileName, memoryStream);

            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _export.Setup(e => e.ExportUsers(It.IsAny<IZipFile>(), It.IsAny<IUsersRepository>(), It.IsAny<AddressDTO>(), It.IsAny<string>()))
                   .Returns(zipResult);

            // Act
            var result = _controller.ExportUsers(addressId);

            // Assert
            var viewResult = Assert.IsAssignableFrom<FileStreamResult>(result);
            Assert.Equal(fileName, viewResult.FileDownloadName);
            Assert.Equal(memoryStream, viewResult.FileStream);

            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _export.Verify(e => e.ExportUsers(_zipFile.Object, _usersRepository.Object, address, path), Times.Once);
        }

        #endregion
    }
}
