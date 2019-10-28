using Dios.Controllers;
using Dios.Data;
using Dios.Models;
using Dios.Repositories;
using Dios.Services;
using Dios.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;
namespace DiosTest.Controllers
{
    public class FlatsControllerTest
    {
        private Mock<IFlatsRepository> _flatsRepositoryMock;
        private Mock<IAddressesRepository> _addressesRepositoryMock;
        private Mock<IUsersRepository> _usersRepositoryMock;
        private Mock<IParametersRepository> _parametersRepositoryMock;
        private Mock<IRequestUserProvider> _requestUserProviderMock;

        private FlatsController controller;

        public FlatsControllerTest()
        {
            _flatsRepositoryMock = new Mock<IFlatsRepository>();
            _addressesRepositoryMock = new Mock<IAddressesRepository>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _parametersRepositoryMock = new Mock<IParametersRepository>();
            _requestUserProviderMock = new Mock<IRequestUserProvider>();

            controller = new FlatsController(_flatsRepositoryMock.Object,
                                             _addressesRepositoryMock.Object,
                                             _usersRepositoryMock.Object,
                                             _parametersRepositoryMock.Object,
                                             _requestUserProviderMock.Object);
        }

        #region Details view

        [Fact]
        public void Details_IdIsNull()
        {
            // Arrange

            // Act
            var result = controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Details_ValidId_FlatIsNull()
        {
            // Arrange
            var flatId = 2;
            _flatsRepositoryMock.Setup(f => f.Flat(flatId))
                                .Returns<FlatDTO>(null);

            // Act
            var result = controller.Details(flatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Once);
        }

        [Fact]
        public void Details_ValidId_ValidFlat()
        {
            // Arrange
            var flatId = 2;
            var flatNumber = "2";
            var flatFloor = 2;
            var entryDoorCode = "2";
            var addressId = 2;
            var address = new AddressDTO();

            var resultFlat = new FlatDTO
            {
                ID = flatId,
                Number = flatNumber,
                Floor = flatFloor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId,
                Address = address,
            };
            _flatsRepositoryMock.Setup(f => f.Flat(flatId))
                                .Returns(resultFlat);

            // Act
            var result = controller.Details(flatId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FlatDetailsVM>(viewResult.Model);

            Assert.Equal(flatId, model.ID);
            Assert.Equal(flatNumber, model.Number);
            Assert.Equal(flatFloor, model.Floor);
            Assert.Equal(entryDoorCode, model.EntryDoorCode);

            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Once);
        }

        #endregion

        #region Create View

        [Fact]
        public void Create_HttpGet_ValidAddress()
        {
            // Arrange
            var addressId = 2;
            var street = "street";
            var number = "2";
            var zipCode = "zipCode";
            var town = "town";
            var country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };
            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                   .Returns(address);

            // Act
            var result = controller.Create(addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FlatCreateVM>(viewResult.Model);

            Assert.Equal(0, model.ID);

            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Create_HttpGet_InvalidAddress()
        {
            // Arrange
            var addressId = 2;
            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                   .Returns<AddressDTO>(null);

            // Act
            var result = controller.Create(addressId);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Create_HttpPost_AddressIsNull()
        {
            // Arrange 
            var id = 2;
            var number = "2";
            var floor = 2;
            var entryDoorCode = "2";
            var addressId = 2;

            FlatCreateVM flat = new FlatCreateVM
            {
                ID = id,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId,
            };

            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                   .Returns<AddressDTO>(null);

            // Act
            var result = controller.Create(flat);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Once);

            _flatsRepositoryMock.Verify(f => f.Exists(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>()), Times.Never);
            _flatsRepositoryMock.Verify(f => f.Add(It.IsAny<FlatDTO>()), Times.Never);
        }

        [Fact]
        public void Create_HttpPost_AddressIsValid_FlatExists()
        {
            // Arrange
            var id = 2;
            var number = "2";
            var floor = 2;
            var entryDoorCode = "2";
            var addressId = 2;

            FlatCreateVM flat = new FlatCreateVM
            {
                ID = id,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId,
            };
            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                   .Returns(new AddressDTO { ID = id });
            _flatsRepositoryMock.Setup(f => f.Exists(addressId, floor, number))
                                .Returns(true);

            // Act
            var result = controller.Create(flat);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Once);
            _flatsRepositoryMock.Verify(f => f.Exists(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>()), Times.Once);
            _flatsRepositoryMock.Verify(f => f.Add(It.IsAny<FlatDTO>()), Times.Never);
        }

        [Fact]
        public void Create_HttpPost_AddressIsValid_FlatDoesNotExists()
        {
            // Arrange
            var id = 2;
            var number = "2";
            var floor = 2;
            var entryDoorCode = "2";
            var addressId = 2;

            var expectedReturn = 1;

            FlatCreateVM flat = new FlatCreateVM
            {
                ID = id,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId,
            };
            FlatDTO flatDTO = new FlatDTO
            {
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };
            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                   .Returns(new AddressDTO { ID = id });
            _flatsRepositoryMock.Setup(f => f.Exists(addressId, floor, number))
                                .Returns(false);
            _flatsRepositoryMock.Setup(f => f.Add(flatDTO))
                                .Returns(expectedReturn);

            // Act
            var result = controller.Create(flat);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = viewResult.Model;

            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Once);
            _flatsRepositoryMock.Verify(f => f.Exists(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>()), Times.Once);
            _flatsRepositoryMock.Verify(f => f.Add(It.IsAny<FlatDTO>()), Times.Once);
        }

        [Fact]
        public void Create_HttpPost_InvalidModelState()
        {
            // Arrange
            controller.ModelState.AddModelError("test", "test");

            var id = 2;
            var number = "2";
            var floor = 2;
            var entryDoorCode = "2";
            var addressId = 2;

            FlatCreateVM flat = new FlatCreateVM
            {
                ID = id,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId,
            };

            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                       .Returns(new AddressDTO { ID = id });

            // Act
            var result = controller.Create(flat);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.Model;

            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Once);
            _flatsRepositoryMock.Verify(f => f.Exists(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>()), Times.Never);
            _flatsRepositoryMock.Verify(f => f.Add(It.IsAny<FlatDTO>()), Times.Never);
        }

        #endregion

        #region Edit View

        [Fact]
        public void Edit_HttpGet_IdIsNull()
        {
            // Arrange

            // Act
            var result = controller.Edit(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public void Edit_HttpGet_ValidIdFlatIsNull()
        {
            // Arrange
            int id = 2;
            _flatsRepositoryMock.Setup(f => f.Flats(id))
                                .Returns<FlatDTO>(null);

            // Act
            var result = controller.Edit(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Once);
        }

        [Fact]
        public void Edit_HttpGet_ValidIdValidFlat()
        {
            // Arrange
            int id = 2;
            var number = "2";
            var floor = 2;
            var entryDoorCode = "2";
            var addressId = 2;
            FlatDTO flat = new FlatDTO
            {
                ID = id,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _flatsRepositoryMock.Setup(f => f.Flat(id))
                                .Returns(flat);

            // Act
            var result = controller.Edit(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FlatDTO>(viewResult.Model);

            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Once);

            Assert.Equal(id, model.ID);
            Assert.Equal(number, model.Number);
            Assert.Equal(floor, model.Floor);
            Assert.Equal(entryDoorCode, model.EntryDoorCode);
            Assert.Equal(addressId, model.AddressID);
        }

        #endregion

        #region Delete View

        [Fact]
        public void Delete_HttpGet_IdIsNull()
        {
            // Arrange

            // Act
            var result = controller.Delete(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public void Delete_HttpGet_ValidIdFlatIsNull()
        {
            // Arrange
            var id = 2;
            _flatsRepositoryMock.Setup(f => f.Flat(id))
                                .Returns<FlatDTO>(null);

            // Act
            var result = controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int>()), Times.Once);
            _parametersRepositoryMock.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public void Delete_HttpGet_ValidIdValidFlatNoParams()
        {
            var id = 2;
            var number = "2";
            var floor = 2;
            var entryDoorCode = "2";
            var addressId = 2;
            var street = "street";
            var zipcode = "zip";
            var town = "town";
            var country = "country";

            FlatDTO flat = new FlatDTO
            {
                ID = id,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId,
            };

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                ZipCode = zipcode,
                Town = town,
                Country = country
            };

            _flatsRepositoryMock.Setup(f => f.Flat(id))
                                .Returns(flat);
            _parametersRepositoryMock.Setup(p => p.Parameters(id))
                                     .Returns(new List<ParameterDTO>());
            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                   .Returns(address);

            // Act
            var result = controller.Delete(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FlatDetailsVM>(viewResult.Model);
            var flatAddress = Assert.IsType<AddressDetailsVM>(model.Address);

            Assert.Equal(id, model.ID);
            Assert.Equal(number, model.Number);
            Assert.Equal(floor, model.Floor);
            Assert.Equal(entryDoorCode, model.EntryDoorCode);

            Assert.Equal(addressId, flatAddress.ID);
            Assert.Equal(street, flatAddress.Street);
            Assert.Equal(zipcode, flatAddress.ZipCode);
            Assert.Equal(town, flatAddress.Town);
            Assert.Equal(country, flatAddress.Country);

            Assert.Empty(model.Parameters);

            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Once);
            _parametersRepositoryMock.Verify(p => p.Parameters(It.IsAny<int>()), Times.Once);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Once);
        }

        [Fact]
        public void Delete_HttpGet_ValidIdValidFlatOneParam()
        {
            // Arrange
            var id = 2;
            var number = "2";
            var floor = 2;
            var entryDoorCode = "2";
            var addressId = 2;
            var street = "street";
            var zipcode = "zip";
            var town = "town";
            var country = "country";

            FlatDTO flat = new FlatDTO
            {
                ID = id,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId,
            };

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                ZipCode = zipcode,
                Town = town,
                Country = country
            };

            _flatsRepositoryMock.Setup(f => f.Flat(id))
                                .Returns(flat);
            _parametersRepositoryMock.Setup(p => p.Parameters(id))
                                     .Returns(new List<ParameterDTO> { new ParameterDTO() });
            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                   .Returns(address);

            // Act
            var result = controller.Delete(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FlatDetailsVM>(viewResult.Model);
            var flatAddress = Assert.IsType<AddressDetailsVM>(model.Address);

            Assert.Equal(id, model.ID);
            Assert.Equal(number, model.Number);
            Assert.Equal(floor, model.Floor);
            Assert.Equal(entryDoorCode, model.EntryDoorCode);

            Assert.Equal(addressId, flatAddress.ID);
            Assert.Equal(street, flatAddress.Street);
            Assert.Equal(zipcode, flatAddress.ZipCode);
            Assert.Equal(town, flatAddress.Town);
            Assert.Equal(country, flatAddress.Country);

            Assert.Single(model.Parameters);

            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Once);
            _parametersRepositoryMock.Verify(p => p.Parameters(It.IsAny<int>()), Times.Once);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Once);

        }

        [Fact]
        public void Delete_HttpGet_ValidIdValidFlatSeveralParam()
        {
            // Arrange
            var id = 2;
            var number = "2";
            var floor = 2;
            var entryDoorCode = "2";
            var addressId = 2;
            var street = "street";
            var zipcode = "zip";
            var town = "town";
            var country = "country";

            FlatDTO flat = new FlatDTO
            {
                ID = id,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId,
            };

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                ZipCode = zipcode,
                Town = town,
                Country = country
            };

            _flatsRepositoryMock.Setup(f => f.Flat(id))
                                .Returns(flat);
            _parametersRepositoryMock.Setup(p => p.Parameters(id))
                                     .Returns(new List<ParameterDTO> { new ParameterDTO(), new ParameterDTO() });
            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                   .Returns(address);

            // Act
            var result = controller.Delete(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FlatDetailsVM>(viewResult.Model);
            var flatAddress = Assert.IsType<AddressDetailsVM>(model.Address);

            Assert.Equal(id, model.ID);
            Assert.Equal(number, model.Number);
            Assert.Equal(floor, model.Floor);
            Assert.Equal(entryDoorCode, model.EntryDoorCode);

            Assert.Equal(addressId, flatAddress.ID);
            Assert.Equal(street, flatAddress.Street);
            Assert.Equal(zipcode, flatAddress.ZipCode);
            Assert.Equal(town, flatAddress.Town);
            Assert.Equal(country, flatAddress.Country);

            Assert.Equal(2, model.Parameters.Count());

            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Once);
            _parametersRepositoryMock.Verify(p => p.Parameters(It.IsAny<int>()), Times.Once);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Once);

        }

        [Fact]
        public void DeleteConfirmed_HttpGet_FlatIsNull()
        {
            // Arrange
            var id = 2;
            _flatsRepositoryMock.Setup(f => f.Flat(id))
                                .Returns<FlatDTO>(null);

            // Act
            var result = controller.DeleteConfirmed(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteConfirmed_ValidId()
        {
            // Arrange
            var id = 2;
            _flatsRepositoryMock.Setup(f => f.Flat(id))
                                .Returns(new FlatDTO());
            _flatsRepositoryMock.Setup(f => f.Delete(id))
                                .Returns(1);

            // Act
            var result = controller.DeleteConfirmed(id);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(AddressesController.Details), viewResult.ActionName);
            Assert.Equal("Addresses", viewResult.ControllerName);

            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Once);
            _flatsRepositoryMock.Verify(f => f.Delete(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region Index View

        [Fact]
        public void Index_HttpGet_AddressIsNull()
        {
            // Arrange
            int id = 2;
            _addressesRepositoryMock.Setup(a => a.Address(id))
                                   .Returns<AddressDTO>(null);

            // Act
            var result = controller.Index(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Once);
        }

        [Fact]
        public void Index_HttpGet_ValidAddress()
        {
            // Arrange
            int id = 2;
            string street = "street";
            string number = "2";
            string zipCode = "zipcode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = id,
                Street = street,
                Number = number,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _addressesRepositoryMock.Setup(a => a.Address(id))
                                    .Returns(address);

            // Act
            var result = controller.Index(id);

            // Assert

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(id, viewResult.Model);

            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region API

        #region GetFlat API

        [Fact]
        public void GetFlat_invalidId()
        {
            // Arrange
            int fid = 2;
            _usersRepositoryMock.Setup(u => u.Users())
                                .Returns(new List<UserDTO>());
            _parametersRepositoryMock.Setup(p => p.Get(It.IsAny<string>(), fid))
                                     .Returns<ParameterDTO>(null);
            _flatsRepositoryMock.Setup(f => f.Flat(fid))
                                .Returns<FlatDTO>(null);
            _addressesRepositoryMock.Setup(a => a.Address(null))
                                    .Returns<AddressDTO>(null);

            // Act
            var result = controller.GetFlat(fid);

            // Assert
            var flatCreateVM = Assert.IsType<FlatCreateVM>(result);

            Assert.Null(flatCreateVM.Address);
            Assert.Null(flatCreateVM.EntryDoorCode);
            Assert.Null(flatCreateVM.Number);
            Assert.Null(flatCreateVM.StatusMessage);
            Assert.Null(flatCreateVM.AvailableUsers);
            Assert.Equal(1, flatCreateVM.Floor);

            _usersRepositoryMock.Verify(u => u.Users(), Times.Never);
            _parametersRepositoryMock.Verify(p => p.Get(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int>()), Times.Once);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public void GetFlat_ValidId()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                                                                    {
                                                                        new Claim(ClaimTypes.Role, "Admin")
                                                                    }, "Admin"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            controller.ControllerContext = context;

            int fid = 2;
            string number = "2";
            int floor = 2;
            string entryDoorCode = "code";
            int addressId = 2;
            string street = "street";
            string addressNumber = "22";
            string zipCode = "zipcode";
            string town = "town";
            string country = "country";

            FlatDTO flat = new FlatDTO
            {
                ID = fid,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _usersRepositoryMock.Setup(u => u.Users())
                                .Returns(new List<UserDTO>());
            _parametersRepositoryMock.Setup(p => p.Get(It.IsAny<string>(), fid))
                                     .Returns<ParameterDTO>(null);
            _flatsRepositoryMock.Setup(f => f.Flat(fid))
                                .Returns(flat);
            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                    .Returns(address);

            // Act
            var result = controller.GetFlat(fid);

            // Assert
            var flatCreateVM = Assert.IsType<FlatCreateVM>(result);

            Assert.True(flatCreateVM.CanDataBeEdited);

            _usersRepositoryMock.Verify(u => u.Users(), Times.Once);
            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int>()), Times.Once);
            _addressesRepositoryMock.Verify(a => a.Address(It.IsAny<int?>()), Times.Once);
        }

        [Fact]
        public void GetFlat_NotAdmin()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                                                                    {
                                                                        new Claim(ClaimTypes.Role, "User")
                                                                    }, "User"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            controller.ControllerContext = context;

            int fid = 2;
            string number = "2";
            int floor = 2;
            string entryDoorCode = "code";
            int addressId = 2;
            string street = "street";
            string addressNumber = "22";
            string zipCode = "zipcode";
            string town = "town";
            string country = "country";

            FlatDTO flat = new FlatDTO
            {
                ID = fid,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            _usersRepositoryMock.Setup(u => u.Users())
                                .Returns(new List<UserDTO>());
            _parametersRepositoryMock.Setup(p => p.Get(It.IsAny<string>(), fid))
                                     .Returns<ParameterDTO>(null);
            _flatsRepositoryMock.Setup(f => f.Flat(fid))
                                .Returns(flat);
            _addressesRepositoryMock.Setup(a => a.Address(addressId))
                                    .Returns(address);

            // Act
            var result = controller.GetFlat(fid);

            // Assert
            var flatCreateVM = Assert.IsType<FlatCreateVM>(result);

            Assert.False(flatCreateVM.CanDataBeEdited);
        }

        #endregion

        #region EditFlat API

        [Fact]
        public void EditFlat_NoNumberFlat()
        {
            // Arrange
            int id = 2;
            int flatId = 2;
            int floor = 0;
            string entryDoorCode = "entryDoorCode";
            int adressId = 2;
            bool canBeEdited = true;

            FlatCreateVM flat = new FlatCreateVM
            {
                ID = flatId,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = adressId,
                CanDataBeEdited = canBeEdited
            };

            FlatEditVM flatEdit = new FlatEditVM
            {
                ID = id,
                Flat = flat
            };

            // Act
            var result = controller.EditFlat(flatEdit);

            // Assert
            var EditResult = Assert.IsType<List<EditResultVM>>(result);

            _flatsRepositoryMock.Verify(f => f.Edit(It.IsAny<FlatDTO>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.UserIds(It.IsAny<int>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.Delete(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.AddUsers(It.IsAny<int>(), It.IsAny<List<string>>()), Times.Never);

            Assert.Single(EditResult);
            Assert.Equal("Nummer fältet är obligatoriskt!", EditResult.First().ErrorMessage);
            Assert.Equal(nameof(flatEdit.Flat.Number), EditResult.First().Property);
        }

        [Fact]
        public void EditFlat_NoNumberInvalidFloorFlat()
        {
            // Arrange
            int id = 2;
            int flatId = 2;
            int floor = -6;
            string entryDoorCode = "entryDoorCode";
            int adressId = 2;
            bool canBeEdited = true;

            FlatCreateVM flat = new FlatCreateVM
            {
                ID = flatId,
                Floor = floor,
                EntryDoorCode = entryDoorCode,
                AddressID = adressId,
                CanDataBeEdited = canBeEdited
            };

            FlatEditVM flatEdit = new FlatEditVM
            {
                ID = id,
                Flat = flat
            };

            // Act
            var result = controller.EditFlat(flatEdit);

            // Assert
            var EditResult = Assert.IsType<List<EditResultVM>>(result);

            Assert.Equal(2, EditResult.Count());

            _flatsRepositoryMock.Verify(f => f.Edit(It.IsAny<FlatDTO>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.UserIds(It.IsAny<int>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.Delete(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.AddUsers(It.IsAny<int>(), It.IsAny<List<string>>()), Times.Never);

            Assert.Equal("Nummer fältet är obligatoriskt!", EditResult[0].ErrorMessage);
            Assert.Equal(nameof(flatEdit.Flat.Number), EditResult[0].Property);
            Assert.Equal("Våning fältet måste vara mellan -5 och 200!", EditResult[1].ErrorMessage);
            Assert.Equal(nameof(flatEdit.Flat.Floor), EditResult[1].Property);
        }

        [Fact]
        public void EditFlat_InvalidFloorFlat()
        {
            // Arrange
            int id = 2;
            int flatId = 2;
            string number = "2";
            int floor = -6;
            string entryDoorCode = "entryDoorCode";
            int adressId = 2;
            bool canBeEdited = true;

            FlatCreateVM flat = new FlatCreateVM
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = adressId,
                CanDataBeEdited = canBeEdited
            };

            FlatEditVM flatEdit = new FlatEditVM
            {
                ID = id,
                Flat = flat
            };

            // Act
            var result = controller.EditFlat(flatEdit);

            // Assert
            var EditResult = Assert.IsType<List<EditResultVM>>(result);

            _flatsRepositoryMock.Verify(f => f.Edit(It.IsAny<FlatDTO>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.UserIds(It.IsAny<int>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.Delete(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _parametersRepositoryMock.Verify(p => p.AddUsers(It.IsAny<int>(), It.IsAny<List<string>>()), Times.Never);

            Assert.Single(EditResult);
            Assert.Equal("Våning fältet måste vara mellan -5 och 200!", EditResult.First().ErrorMessage);
            Assert.Equal(nameof(flatEdit.Flat.Floor), EditResult.First().Property);
        }

        [Fact]
        public void EditFlat_ValidFlat()
        {
            // Arrange
            int id = 2;
            int flatId = 2;
            string number = "2";
            int floor = 1;
            string entryDoorCode = "entryDoorCode";
            int adressId = 2;
            bool canBeEdited = true;

            FlatCreateVM flat = new FlatCreateVM
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = adressId,
                CanDataBeEdited = canBeEdited
            };

            FlatDTO flatDTO = new FlatDTO
            {
                ID = flatId,
                AddressID = adressId,
                Number = number,
                Floor = floor,
                EntryDoorCode = entryDoorCode
            };

            FlatEditVM flatEdit = new FlatEditVM
            {
                ID = id,
                Flat = flat
            };

            _flatsRepositoryMock.Setup(f => f.Flat(id))
                                .Returns(flatDTO);

            // Act
            var result = controller.EditFlat(flatEdit);

            // Assert
            var EditResult = Assert.IsType<List<EditResultVM>>(result);
            Assert.Empty(EditResult);

            _flatsRepositoryMock.Verify(f => f.Flat(It.IsAny<int?>()), Times.Once);
        }

        #endregion

        #endregion
    }
}
