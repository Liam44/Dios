using Dios.Controllers;
using Dios.Models;
using Dios.Repositories;
using Dios.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace DiosTest.Controllers
{
    public sealed class ParametersControllerTest
    {
        private readonly Mock<IParametersRepository> _parametersRepository;
        private readonly Mock<IFlatsRepository> _flatsRepository;

        private readonly ParameterController _controller;

        public ParametersControllerTest()
        {
            _parametersRepository = new Mock<IParametersRepository>();
            _flatsRepository = new Mock<IFlatsRepository>();

            _controller = new ParameterController(_parametersRepository.Object,
                                                  _flatsRepository.Object);
        }

        #region Edit

        #region Edit - Get

        [Fact]
        public void Edit_Get_FlatIdNotFound()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "User") }, "User"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            int flatId = 1;

            FlatDTO flat = null;

            _flatsRepository.Setup(f => f.Flat(It.IsAny<int>()))
                            .Returns(flat);

            // Act
            var result = _controller.Edit(flatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
        }

        [Fact]
        public void Edit_Get_FlatIdFound_UserIdNotFound()
        {
            // Arrange
            string userId = "someUserId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "User"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            int flatId = 1;
            int floor = 10;
            string number = "number";
            string entryDoorCode = "entryDoorCode";
            int addressId = 25;

            FlatDTO flat = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            ParameterDTO parameterDTO = null;

            _flatsRepository.Setup(f => f.Flat(It.IsAny<int>()))
                            .Returns(flat);
            _parametersRepository.Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>()))
                                 .Returns(parameterDTO);

            // Act
            var result = _controller.Edit(flatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _parametersRepository.Verify(p => p.Get(userId, flatId), Times.Once);

        }

        [Fact]
        public void Edit_Get_FlatIdFound_UserIdFound()
        {
            // Arrange
            string userId = "someUserId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "User"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            int flatId = 1;
            int floor = 10;
            string number = "number";
            string entryDoorCode = "entryDoorCode";
            int addressId = 25;

            FlatDTO flat = new FlatDTO
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
            ParameterDTO parameterDTO = new ParameterDTO
            {
                UserId = userId,
                Flat = flat,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            _flatsRepository.Setup(f => f.Flat(It.IsAny<int>()))
                            .Returns(flat);
            _parametersRepository.Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>()))
                                 .Returns(parameterDTO);

            // Act
            var result = _controller.Edit(flatId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ParameterEditVM>(viewResult.Model);

            Assert.Equal(flatId, model.FlatID);
            Assert.Equal(userId, model.UserId);
            Assert.Equal(isEmailVisible, model.IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible, model.IsPhoneNumberVisible);
            Assert.Equal(canBeContacted, model.CanBeContacted);

            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _parametersRepository.Verify(p => p.Get(userId, flatId), Times.Once);
        }

        #endregion

        #region Edit - Post

        [Fact]
        public void Edit_Post_NotFound()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "User") }, "User"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            string userId = "someUserId";
            int flatId = 1;
            bool isEmailVisible = true;
            bool isPhoneNumberVisible = true;
            bool canBeContacted = true;
            ParameterEditVM parameter = new ParameterEditVM
            {
                UserId = userId,
                FlatID = flatId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            ParameterDTO parameterDTO = null;

            _parametersRepository.Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>()))
                                 .Returns(parameterDTO);

            // Act
            var result = _controller.Edit(parameter);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _parametersRepository.Verify(p => p.Get(userId, flatId), Times.Once);
            _parametersRepository.Verify(p => p.Edit(It.IsAny<ParameterDTO>()), Times.Never);
        }

        [Fact]
        public void Edit_Post_Found()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "User") }, "User"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            string userId = "someUserId";
            int flatId = 1;
            bool isEmailVisible = true;
            bool isPhoneNumberVisible = true;
            bool canBeContacted = true;
            ParameterEditVM parameter = new ParameterEditVM
            {
                UserId = userId,
                FlatID = flatId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            ParameterDTO parameterDTO = new ParameterDTO
            {
                UserId = userId,
                Flat = new FlatDTO
                {
                    ID = flatId,
                    Floor = 10,
                    Number = "number",
                    EntryDoorCode = "entryDoorCode"
                },
                IsEmailVisible = !isEmailVisible,
                IsPhoneNumberVisible = !isPhoneNumberVisible,
                CanBeContacted = !canBeContacted
            };

            _parametersRepository.Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>()))
                                 .Returns(parameterDTO);
            _parametersRepository.Setup(p => p.Edit(It.IsAny<ParameterDTO>()))
                                 .Returns(1);

            // Act
            var result = _controller.Edit(parameter);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(UsersController.Flats), viewResult.ActionName);
            Assert.Equal("Users", viewResult.ControllerName);

            _parametersRepository.Verify(p => p.Get(userId, flatId), Times.Once);
            _parametersRepository.Verify(p => p.Edit(parameterDTO), Times.Once);
        }

        #endregion

        #endregion
    }
}
