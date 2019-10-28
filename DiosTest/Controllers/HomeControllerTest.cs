using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Dios.Repositories;
using Dios.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DiosTest.Controllers
{
    public class HomeControllerTest
    {
        private Mock<IUsersRepository> _usersRepositoryMock;

        private HomeController controller;

        public HomeControllerTest()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>();

            controller = new HomeController(_usersRepositoryMock.Object);
        }

        [Fact]
        public void Index_UserNotAuthenticated()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, "test")
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            controller.ControllerContext = context;

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(AccountController.Login), viewResult.ActionName);
            Assert.Equal("Account", viewResult.ControllerName);

            _usersRepositoryMock.Verify(u => u.User(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Index_UserInRoleAdmin()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
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

            // Act
            var result = controller.Index();

            // Assert
            var viewresult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(AddressesController.Index), viewresult.ActionName);
            Assert.Equal("Addresses", viewresult.ControllerName);

            _usersRepositoryMock.Verify(u => u.User(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Index_UserInRoleUser_RegistrationCodeNotNull()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.NameIdentifier, "test")
            }, "user"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            controller.ControllerContext = context;

            _usersRepositoryMock.Setup(u => u.User(It.IsAny<string>()))
                                .Returns(new Dios.Models.UserDTO
                                {
                                    RegistrationCode = "Not null"
                                });

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(AccountController.ChangePassword), viewResult.ActionName);
            Assert.Equal("Account", viewResult.ControllerName);

            _usersRepositoryMock.Verify(u => u.User(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Index_UserInRoleUser_RegistrationCodeIsNull()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test"),
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

            _usersRepositoryMock.Setup(u => u.User(It.IsAny<string>()))
                                .Returns(new Dios.Models.UserDTO());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(UsersController.Flats), viewResult.ActionName);
            Assert.Equal("Users", viewResult.ControllerName);

            _usersRepositoryMock.Verify(u => u.User(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Index_UserInRoleHost()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, "Test")
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            controller.ControllerContext = context;

            _usersRepositoryMock.Setup(u => u.User(It.IsAny<string>()))
                                .Returns(new Dios.Models.UserDTO());

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(HostsController.Addresses), viewResult.ActionName);
            Assert.Equal("Hosts", viewResult.ControllerName);

            _usersRepositoryMock.Verify(u => u.User(It.IsAny<string>()), Times.Once);
        }
    }
}
