using Dios.Controllers;
using Dios.Helpers;
using Dios.Models;
using Dios.Repositories;
using Dios.Services;
using Dios.ViewModels;
using Dios.ViewModels.UsersViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace DiosTest.Controllers
{
    public class HostsControllerTest
    {
        private readonly Mock<IRequestUserProvider> _requestUserProvider;
        private readonly Mock<IEmailSender> _emailSender;
        private readonly Mock<IUsersRepository> _usersRepository;
        private readonly Mock<IFlatsRepository> _flatsRepository;
        private readonly Mock<IParametersRepository> _parameterRepository;
        private readonly Mock<IAddressesRepository> _addressesRepository;
        private readonly Mock<IAddressHostsRepository> _addressHostsRepository;
        private readonly Mock<IErrorReportsRepository> _errorReportsRepository;
        private readonly Mock<INewUser> _newUser;

        private readonly HostsController _controller;

        public HostsControllerTest()
        {
            _requestUserProvider = new Mock<IRequestUserProvider>();
            _emailSender = new Mock<IEmailSender>();
            _usersRepository = new Mock<IUsersRepository>();
            _flatsRepository = new Mock<IFlatsRepository>();
            _parameterRepository = new Mock<IParametersRepository>();
            _addressesRepository = new Mock<IAddressesRepository>();
            _addressHostsRepository = new Mock<IAddressHostsRepository>();
            _errorReportsRepository = new Mock<IErrorReportsRepository>();
            _newUser = new Mock<INewUser>();

            _controller = new HostsController(_requestUserProvider.Object,
                                              _emailSender.Object,
                                              _usersRepository.Object,
                                              _flatsRepository.Object,
                                              _parameterRepository.Object,
                                              _addressesRepository.Object,
                                              _addressHostsRepository.Object,
                                              _errorReportsRepository.Object,
                                              _newUser.Object);
        }

        #region Index

        [Fact]
        public void Index()
        {
            // Arrange

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region GetHosts

        [Fact]
        public void GetHosts_Null()
        {
            // Arrange
            List<UserDTO> hosts = null;

            _usersRepository.Setup(u => u.Hosts())
                            .Returns(hosts);

            // Act
            var result = _controller.GetHosts();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetHosts_NoEntry()
        {
            // Arrange
            List<UserDTO> hosts = new List<UserDTO>();

            _usersRepository.Setup(u => u.Hosts())
                            .Returns(hosts);

            // Act
            var result = _controller.GetHosts();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetHosts_OneEntry()
        {
            // Arrange
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

            _usersRepository.Setup(u => u.Hosts())
                            .Returns(hosts);

            // Act
            var result = _controller.GetHosts();

            // Assert
            Assert.Single(result);
            Assert.Equal(hostId, result.First().Id);
            Assert.Equal(personalNumber, result.First().PersonalNumber);
            Assert.Equal(firstName, result.First().FirstName);
            Assert.Equal(lastName, result.First().LastName);
            Assert.Equal(emailAddress, result.First().Email);
            Assert.Equal(phoneNumber, result.First().PhoneNumber);
            Assert.Equal(phoneNumber2, result.First().PhoneNumber2);
        }

        [Fact]
        public void GetHosts_TwoEntries()
        {
            // Arrange
            string hostId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";
            string phoneNumber21 = "phoneNumber21";

            UserDTO host1 = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1,
                PhoneNumber2 = phoneNumber21
            };

            string hostId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";
            string phoneNumber22 = "phoneNumber22";

            UserDTO host2 = new UserDTO
            {
                Id = hostId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2,
                PhoneNumber2 = phoneNumber22
            };

            List<UserDTO> hosts = new List<UserDTO> { host1, host2 };

            _usersRepository.Setup(u => u.Hosts())
                            .Returns(hosts);

            // Act
            var result = _controller.GetHosts();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(hostId1, result.First().Id);
            Assert.Equal(personalNumber1, result.First().PersonalNumber);
            Assert.Equal(firstName1, result.First().FirstName);
            Assert.Equal(lastName1, result.First().LastName);
            Assert.Equal(emailAddress1, result.First().Email);
            Assert.Equal(phoneNumber1, result.First().PhoneNumber);
            Assert.Equal(phoneNumber21, result.First().PhoneNumber2);
            Assert.Equal(hostId2, result.Last().Id);
            Assert.Equal(personalNumber2, result.Last().PersonalNumber);
            Assert.Equal(firstName2, result.Last().FirstName);
            Assert.Equal(lastName2, result.Last().LastName);
            Assert.Equal(emailAddress2, result.Last().Email);
            Assert.Equal(phoneNumber2, result.Last().PhoneNumber);
            Assert.Equal(phoneNumber22, result.Last().PhoneNumber2);
        }

        #endregion

        #region GetHost

        [Fact]
        public void GetHost_IdNull()
        {
            // Arrange
            string hostId = null;
            UserDTO host = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host);

            // Act
            var result = _controller.GetHost(hostId);

            // Arrange
            Assert.NotNull(result);
            Assert.Null(result.Id);
            Assert.Null(result.PersonalNumber);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
            Assert.Null(result.Email);
            Assert.Null(result.PhoneNumber);
            Assert.Null(result.PhoneNumber2);

            _usersRepository.Verify(u => u.User(hostId), Times.Once);
        }

        [Fact]
        public void GetHost_IdNotFound()
        {
            // Arrange
            string hostId = "someHostId";
            UserDTO host = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host);

            // Act
            var result = _controller.GetHost(hostId);

            // Arrange
            Assert.NotNull(result);
            Assert.Null(result.Id);
            Assert.Null(result.PersonalNumber);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
            Assert.Null(result.Email);
            Assert.Null(result.PhoneNumber);
            Assert.Null(result.PhoneNumber2);

            _usersRepository.Verify(u => u.User(hostId), Times.Once);
        }

        [Fact]
        public void GetHost_IdFound()
        {
            // Arrange
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

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host);

            // Act
            var result = _controller.GetHost(hostId);

            // Arrange
            Assert.NotNull(result);
            Assert.Equal(hostId, result.Id);
            Assert.Equal(personalNumber, result.PersonalNumber);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
            Assert.Equal(emailAddress, result.Email);
            Assert.Equal(phoneNumber, result.PhoneNumber);
            Assert.Equal(phoneNumber2, result.PhoneNumber2);

            _usersRepository.Verify(u => u.User(hostId), Times.Once);
        }

        #endregion

        #region ErrorReports

        [Fact]
        public void ErrorReports_NoEntry()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            List<ErrorReportDTO> errorReports = new List<ErrorReportDTO>();

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errorReports);

            // Act
            var result = _controller.ErrorReports();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ErrorReportDTO>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void ErrorReports_OneEntry()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            int errorReportId = 1;
            DateTime seen = DateTime.Now.AddMinutes(20);
            int flatId = 2;
            string description = "someDescription";
            string subject = "someSubject";
            DateTime submitted = DateTime.Now;
            Status currentStatus = Status.finished;
            Priority currentPriority = Priority.low;

            ErrorReportDTO errorReport = new ErrorReportDTO
            {
                Id = errorReportId,
                Seen = seen,
                FlatId = flatId,
                Description = description,
                Subject = subject,
                Submitted = submitted,
                CurrentStatus = currentStatus,
                CurrentPriority = currentPriority
            };

            List<ErrorReportDTO> errorReports = new List<ErrorReportDTO> { errorReport };

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errorReports);

            // Act
            var result = _controller.ErrorReports();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ErrorReportDTO>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal(errorReportId, model.First().Id);
            Assert.Equal(seen, model.First().Seen);
            Assert.Equal(flatId, model.First().FlatId);
            Assert.Equal(description, model.First().Description);
            Assert.Equal(subject, model.First().Subject);
            Assert.Equal(submitted, model.First().Submitted);
            Assert.Equal(currentStatus, model.First().CurrentStatus);
            Assert.Equal(currentPriority, model.First().CurrentPriority);
        }

        [Fact]
        public void ErrorReports_TwoEntries()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            int errorReportId1 = 1;
            DateTime seen1 = DateTime.Now.AddMinutes(20);
            int flatId1 = 2;
            string description1 = "someDescription1";
            string subject1 = "someSubject1";
            DateTime submitted1 = DateTime.Now;
            Status currentStatus1 = Status.finished;
            Priority currentPriority1 = Priority.low;

            ErrorReportDTO errorReport1 = new ErrorReportDTO
            {
                Id = errorReportId1,
                Seen = seen1,
                FlatId = flatId1,
                Description = description1,
                Subject = subject1,
                Submitted = submitted1,
                CurrentStatus = currentStatus1,
                CurrentPriority = currentPriority1
            };

            int errorReportId2 = 2;
            DateTime? seen2 = null;
            int flatId2 = 20;
            string description2 = "someDescription2";
            string subject2 = "someSubject2";
            DateTime submitted2 = DateTime.Now.AddMinutes(40); ;
            Status currentStatus2 = Status.finished;
            Priority currentPriority2 = Priority.low;

            ErrorReportDTO errorReport2 = new ErrorReportDTO
            {
                Id = errorReportId2,
                Seen = seen2,
                FlatId = flatId2,
                Description = description2,
                Subject = subject2,
                Submitted = submitted2,
                CurrentStatus = currentStatus2,
                CurrentPriority = currentPriority2
            };

            List<ErrorReportDTO> errorReports = new List<ErrorReportDTO> { errorReport1, errorReport2 };

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errorReports);

            // Act
            var result = _controller.ErrorReports();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ErrorReportDTO>>(viewResult.Model);
            Assert.Equal(2, model.Count());
            Assert.Equal(errorReportId1, model.First().Id);
            Assert.Equal(seen1, model.First().Seen);
            Assert.Equal(flatId1, model.First().FlatId);
            Assert.Equal(description1, model.First().Description);
            Assert.Equal(subject1, model.First().Subject);
            Assert.Equal(submitted1, model.First().Submitted);
            Assert.Equal(currentStatus1, model.First().CurrentStatus);
            Assert.Equal(currentPriority1, model.First().CurrentPriority);
            Assert.Equal(errorReportId2, model.Last().Id);
            Assert.Null(model.Last().Seen);
            Assert.Equal(flatId2, model.Last().FlatId);
            Assert.Equal(description2, model.Last().Description);
            Assert.Equal(subject2, model.Last().Subject);
            Assert.Equal(submitted2, model.Last().Submitted);
            Assert.Equal(currentStatus2, model.Last().CurrentStatus);
            Assert.Equal(currentPriority2, model.Last().CurrentPriority);
        }

        #endregion

        #region Create

        #region Create - Get

        [Fact]
        public void Create_Get()
        {
            // Arrange

            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserCreateVM>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Null(model.Id);
            Assert.Null(model.PersonalNumber);
            Assert.Null(model.FirstName);
            Assert.Null(model.LastName);
            Assert.False(model.CanChangeName);
            Assert.Null(model.Email);
            Assert.Null(model.PhoneNumber);
            Assert.Null(model.PhoneNumber2);
            Assert.True(model.IsPhoneNumber2Visible);
            Assert.False(model.CanChangePassword);
            Assert.Null(model.StatusMessage);
        }

        #endregion

        #region Create - Post

        [Fact]
        public async Task Create_Post_InvalidModelState()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Error");

            // Act
            var result = await _controller.Create(new UserCreateVM());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<UserCreateVM>(viewResult.Model);
        }

        [Fact]
        public async Task Create_Post_ValidModelState_DuplicatePersonalNumber()
        {
            // Arrange
            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserCreateVM model = new UserCreateVM
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            UserDTO userDTO = new UserDTO
            {
                Id = "someOtherHostId"
            };

            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTO);

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<UserCreateVM>(viewResult.Model);
            Assert.NotEmpty(_controller.ModelState);
            Assert.Equal(nameof(model.PersonalNumber), _controller.ModelState.First().Key);
            Assert.Equal("En användare med samma personnummer finns redan i databasen.", _controller.ModelState.First().Value.Errors.First().ErrorMessage);

            _usersRepository.Verify(u => u.UserByPersonalNumber(personalNumber, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Never);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Never);
            _newUser.Verify(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<IRequestUserProvider>(),
                                            It.IsAny<IEmailSender>(),
                                            It.IsAny<IUrlHelper>(),
                                            It.IsAny<string>()),
                                  Times.Never);
        }

        [Fact]
        public async Task Create_Post_ValidModelState_DuplicateUsername()
        {
            // Arrange
            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserCreateVM model = new UserCreateVM
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            UserDTO userDTO = null;

            string registrationCode = "notThatRandomRegistrationCode";
            string password = "n07ThatRand0mPa$$w0rd!";

            string key = nameof(UserDTO.Email);
            string value = "En användare med samma e-post finns redan.";
            Dictionary<string, string> createResult = new Dictionary<string, string> { { key, value } };

            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            string requestString = "https";
            var url = new Mock<IUrlHelper>(MockBehavior.Strict);
            url.Setup(v => v.Action(It.IsAny<UrlActionContext>()))
               .Returns("callbackUrl")
               .Verifiable();

            context.Setup(v => v.Request)
                   .Returns(request.Object);
            request.Setup(v => v.Scheme)
                   .Returns(requestString);

            var ctrctx = new ControllerContext
            {
                HttpContext = context.Object
            };

            _controller.ControllerContext = ctrctx;
            _controller.Url = url.Object;

            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTO);
            _usersRepository.Setup(u => u.GenerateRegistrationCode())
                            .Returns(registrationCode);
            _usersRepository.Setup(u => u.GeneratePassword())
                            .Returns(password);
            _newUser.Setup(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                           It.IsAny<string>(),
                                           It.IsAny<string>(),
                                           It.IsAny<string>(),
                                           It.IsAny<IRequestUserProvider>(),
                                           It.IsAny<IEmailSender>(),
                                           It.IsAny<IUrlHelper>(),
                                           It.IsAny<string>()))
                    .Returns(Task.FromResult(createResult));

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<UserCreateVM>(viewResult.Model);
            Assert.Single(_controller.ModelState);
            Assert.Equal(key, _controller.ModelState.First().Key);
            Assert.Equal(value, _controller.ModelState.First().Value.Errors.First().ErrorMessage);

            _usersRepository.Verify(u => u.UserByPersonalNumber(personalNumber, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Once);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Once);
            _newUser.Verify(nu => nu.Create(model,
                                            "Host",
                                            registrationCode,
                                            password,
                                            _requestUserProvider.Object,
                                            _emailSender.Object,
                                            url.Object,
                                            requestString),
                                  Times.Once);
        }

        [Fact]
        public async Task Create_Post_ValidModelState_Success()
        {
            // Arrange
            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserCreateVM model = new UserCreateVM
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            UserDTO userDTO = null;

            string registrationCode = "notThatRandomRegistrationCode";
            string password = "n07ThatRand0mPa$$w0rd!";

            Dictionary<string, string> createResult = new Dictionary<string, string> { };

            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            string requestString = "https";
            var url = new Mock<IUrlHelper>(MockBehavior.Strict);
            url.Setup(v => v.Action(It.IsAny<UrlActionContext>()))
               .Returns("callbackUrl")
               .Verifiable();

            context.Setup(v => v.Request)
                   .Returns(request.Object);
            request.Setup(v => v.Scheme)
                   .Returns(requestString);

            var ctrctx = new ControllerContext
            {
                HttpContext = context.Object
            };

            _controller.ControllerContext = ctrctx;
            _controller.Url = url.Object;

            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTO);
            _usersRepository.Setup(u => u.GenerateRegistrationCode())
                            .Returns(registrationCode);
            _usersRepository.Setup(u => u.GeneratePassword())
                            .Returns(password);
            _newUser.Setup(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                           It.IsAny<string>(),
                                           It.IsAny<string>(),
                                           It.IsAny<string>(),
                                           It.IsAny<IRequestUserProvider>(),
                                           It.IsAny<IEmailSender>(),
                                           It.IsAny<IUrlHelper>(),
                                           It.IsAny<string>()))
                    .Returns(Task.FromResult(createResult));

            // Act
            var result = await _controller.Create(model);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(_controller.ModelState);

            _usersRepository.Verify(u => u.UserByPersonalNumber(personalNumber, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Once);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Once);
            _newUser.Verify(nu => nu.Create(model,
                                            "Host",
                                            registrationCode,
                                            password,
                                            _requestUserProvider.Object,
                                            _emailSender.Object,
                                            url.Object,
                                            requestString),
                                  Times.Once);
        }

        #endregion

        #endregion

        #region Details

        [Fact]
        public void Details_IdNull()
        {
            // Arrange
            string hostId = null;

            // Act
            var result = _controller.Details(hostId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Details_IdNotFound()
        {
            // Arrange
            string hostId = "someHostId";
            UserDTO host = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host);

            // Act
            var result = _controller.Details(hostId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(hostId), Times.Once);
        }

        [Fact]
        public void Details_IdFound_AddressesNull()
        {
            // Arrange
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

            List<AddressDTO> addresses = null;
            List<FlatDTO> flats = null;
            List<ParameterDTO> parameters = null;
            List<UserDTO> hosts = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host);
            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(hostId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(hostId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Equal(phoneNumber2, model.PhoneNumber2);
            Assert.Empty(model.Addresses);
            Assert.Null(model.Flats);
            Assert.Empty(model.Parameters);

            _usersRepository.Verify(u => u.User(hostId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<int>()), Times.Never);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
            _addressHostsRepository.Verify(ah => ah.Hosts(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Details_IdFound_NoAddresses_FlatsNull_HostsNull()
        {
            // Arrange
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

            List<AddressDTO> addresses = new List<AddressDTO>();
            List<FlatDTO> flats = null;
            List<ParameterDTO> parameters = null;
            List<UserDTO> hosts = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host);
            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(hostId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(hostId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Equal(phoneNumber2, model.PhoneNumber2);
            Assert.Empty(model.Addresses);
            Assert.Null(model.Flats);
            Assert.Empty(model.Parameters);

            _usersRepository.Verify(u => u.User(hostId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<int>()), Times.Never);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
            _addressHostsRepository.Verify(ah => ah.Hosts(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Details_IdFound_OneAddress_FlatsNull_HostsNull()
        {
            // Arrange
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

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address };
            List<FlatDTO> flats = null;
            List<ParameterDTO> parameters = null;
            List<UserDTO> hosts = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host);
            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(hostId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(hostId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Equal(phoneNumber2, model.PhoneNumber2);
            Assert.Single(model.Addresses);
            Assert.Equal(addressId, model.Addresses.First().Key);
            Assert.Equal(addressId, model.Addresses[addressId].First().ID);
            Assert.Equal(street, model.Addresses[addressId].First().Street);
            Assert.Equal(addressNumber, model.Addresses[addressId].First().Number);
            Assert.Equal(zipCode, model.Addresses[addressId].First().ZipCode);
            Assert.Equal(town, model.Addresses[addressId].First().Town);
            Assert.Equal(country, model.Addresses[addressId].First().Country);
            Assert.True(model.Addresses[addressId].First().CanDataBeDeleted);
            Assert.Equal(0, model.Addresses[addressId].First().AmountAvailableFlats);
            Assert.Equal(0, model.Addresses[addressId].First().AmountFlats);
            Assert.Equal(0, model.Addresses[addressId].First().AmountHosts);
            Assert.Equal(0, model.Addresses[addressId].First().AmountUsers);
            Assert.Null(model.Addresses[addressId].First().Flats);
            Assert.Null(model.Flats);
            Assert.Empty(model.Parameters);

            _usersRepository.Verify(u => u.User(hostId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
            _addressHostsRepository.Verify(ah => ah.Hosts(addressId), Times.Once);
        }

        [Fact]
        public void Details_IdFound_OneAddress_NoFlats_NoHosts()
        {
            // Arrange
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

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address };
            List<FlatDTO> flats = new List<FlatDTO>();
            List<ParameterDTO> parameters = null;
            List<UserDTO> hosts = new List<UserDTO>();

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host);
            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(hostId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(hostId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Equal(phoneNumber2, model.PhoneNumber2);
            Assert.Single(model.Addresses);
            Assert.Equal(addressId, model.Addresses.First().Key);
            Assert.Equal(addressId, model.Addresses[addressId].First().ID);
            Assert.Equal(street, model.Addresses[addressId].First().Street);
            Assert.Equal(addressNumber, model.Addresses[addressId].First().Number);
            Assert.Equal(zipCode, model.Addresses[addressId].First().ZipCode);
            Assert.Equal(town, model.Addresses[addressId].First().Town);
            Assert.Equal(country, model.Addresses[addressId].First().Country);
            Assert.True(model.Addresses[addressId].First().CanDataBeDeleted);
            Assert.Equal(0, model.Addresses[addressId].First().AmountAvailableFlats);
            Assert.Equal(0, model.Addresses[addressId].First().AmountFlats);
            Assert.Equal(0, model.Addresses[addressId].First().AmountHosts);
            Assert.Equal(0, model.Addresses[addressId].First().AmountUsers);
            Assert.Empty(model.Addresses[addressId].First().Flats);
            Assert.Null(model.Flats);
            Assert.Empty(model.Parameters);

            _usersRepository.Verify(u => u.User(hostId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
            _addressHostsRepository.Verify(ah => ah.Hosts(addressId), Times.Once);
        }

        [Fact]
        public void Details_IdFound_OneAddress_OneFlat_ParametersNull_OneHost()
        {
            // Arrange
            string hostId1 = "someHostId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";
            string phoneNumber21 = "phoneNumber21";

            UserDTO host1 = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1,
                PhoneNumber2 = phoneNumber21
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            int flatId = 2;
            int floor = 20;
            string flatNumber = "flatNumber";
            string entryDoorCode = "entryDoorCode";

            FlatDTO flat = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            string hostId2 = "someHostId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";
            string phoneNumber22 = "phoneNumber22";

            UserDTO host2 = new UserDTO
            {
                Id = hostId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2,
                PhoneNumber2 = phoneNumber22
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address };
            List<FlatDTO> flats = new List<FlatDTO> { flat };
            List<ParameterDTO> parameters = null;
            List<UserDTO> hosts = new List<UserDTO> { host2 };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host1);
            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(hostId1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(hostId1, model.Id);
            Assert.Equal(personalNumber1, model.PersonalNumber);
            Assert.Equal(firstName1, model.FirstName);
            Assert.Equal(lastName1, model.LastName);
            Assert.Equal(emailAddress1, model.Email);
            Assert.Equal(phoneNumber1, model.PhoneNumber);
            Assert.Equal(phoneNumber21, model.PhoneNumber2);
            Assert.Single(model.Addresses);
            Assert.Equal(addressId, model.Addresses.First().Key);
            Assert.Equal(addressId, model.Addresses[addressId].First().ID);
            Assert.Equal(street, model.Addresses[addressId].First().Street);
            Assert.Equal(addressNumber, model.Addresses[addressId].First().Number);
            Assert.Equal(zipCode, model.Addresses[addressId].First().ZipCode);
            Assert.Equal(town, model.Addresses[addressId].First().Town);
            Assert.Equal(country, model.Addresses[addressId].First().Country);
            Assert.True(model.Addresses[addressId].First().CanDataBeDeleted);
            Assert.Equal(0, model.Addresses[addressId].First().AmountAvailableFlats);
            Assert.Equal(1, model.Addresses[addressId].First().AmountFlats);
            Assert.Equal(1, model.Addresses[addressId].First().AmountHosts);
            Assert.Equal(0, model.Addresses[addressId].First().AmountUsers);
            Assert.Single(model.Addresses[addressId].First().Flats);
            Assert.Equal(floor, model.Addresses[addressId].First().Flats.Keys.First());
            Assert.Equal(flatId, model.Addresses[addressId].First().Flats[floor].First().ID);
            Assert.Equal(floor, model.Addresses[addressId].First().Flats[floor].First().Floor);
            Assert.Equal(flatNumber, model.Addresses[addressId].First().Flats[floor].First().Number);
            Assert.Equal(entryDoorCode, model.Addresses[addressId].First().Flats[floor].First().EntryDoorCode);
            Assert.NotNull(model.Addresses[addressId].First().Flats[floor].First().Address);
            Assert.Empty(model.Addresses[addressId].First().Flats[floor].First().Parameters);
            Assert.Null(model.Flats);
            Assert.Empty(model.Parameters);

            _usersRepository.Verify(u => u.User(hostId1), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Addresses(hostId1), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Hosts(addressId), Times.Once);
        }

        [Fact]
        public void Details_IdFound_OneAddress_TwoFlats_NoParameters_TwoHosts()
        {
            // Arrange
            string hostId1 = "someHostId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";
            string phoneNumber21 = "phoneNumber21";

            UserDTO hostDetails = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1,
                PhoneNumber2 = phoneNumber21
            };

            UserDTO host1 = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1,
                PhoneNumber2 = phoneNumber21
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            int flatId1 = 2;
            int floor1 = 20;
            string flatNumber1 = "flatNumber1";
            string entryDoorCode1 = "entryDoorCode1";

            FlatDTO flat1 = new FlatDTO
            {
                ID = flatId1,
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId
            };

            int flatId2 = 3;
            int floor2 = 10;
            string flatNumber2 = "flatNumber2";
            string entryDoorCode2 = "entryDoorCode2";

            FlatDTO flat2 = new FlatDTO
            {
                ID = flatId2,
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId
            };

            string hostId2 = "someHostId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";
            string phoneNumber22 = "phoneNumber22";

            UserDTO host2 = new UserDTO
            {
                Id = hostId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2,
                PhoneNumber2 = phoneNumber22
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address };
            List<FlatDTO> flats = new List<FlatDTO> { flat1, flat2 };
            List<ParameterDTO> parameters1 = new List<ParameterDTO>();
            List<ParameterDTO> parameters2 = new List<ParameterDTO>();
            List<UserDTO> hosts = new List<UserDTO> { host1, host2 };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(hostDetails);
            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns((int flatId) => flatId == flatId1 ? parameters1 : parameters2);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(hostId1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(hostId1, model.Id);
            Assert.Equal(personalNumber1, model.PersonalNumber);
            Assert.Equal(firstName1, model.FirstName);
            Assert.Equal(lastName1, model.LastName);
            Assert.Equal(emailAddress1, model.Email);
            Assert.Equal(phoneNumber1, model.PhoneNumber);
            Assert.Equal(phoneNumber21, model.PhoneNumber2);
            Assert.Single(model.Addresses);
            Assert.Equal(addressId, model.Addresses.First().Key);
            Assert.Equal(addressId, model.Addresses[addressId].First().ID);
            Assert.Equal(street, model.Addresses[addressId].First().Street);
            Assert.Equal(addressNumber, model.Addresses[addressId].First().Number);
            Assert.Equal(zipCode, model.Addresses[addressId].First().ZipCode);
            Assert.Equal(town, model.Addresses[addressId].First().Town);
            Assert.Equal(country, model.Addresses[addressId].First().Country);
            Assert.True(model.Addresses[addressId].First().CanDataBeDeleted);
            Assert.Equal(0, model.Addresses[addressId].First().AmountAvailableFlats);
            Assert.Equal(2, model.Addresses[addressId].First().AmountFlats);
            Assert.Equal(2, model.Addresses[addressId].First().AmountHosts);
            Assert.Equal(0, model.Addresses[addressId].First().AmountUsers);
            Assert.Equal(2, model.Addresses[addressId].First().Flats.Count);
            Assert.Equal(floor1, model.Addresses[addressId].First().Flats.Keys.First());
            Assert.Equal(flatId1, model.Addresses[addressId].First().Flats[floor1].First().ID);
            Assert.Equal(floor1, model.Addresses[addressId].First().Flats[floor1].First().Floor);
            Assert.Equal(flatNumber1, model.Addresses[addressId].First().Flats[floor1].First().Number);
            Assert.Equal(entryDoorCode1, model.Addresses[addressId].First().Flats[floor1].First().EntryDoorCode);
            Assert.NotNull(model.Addresses[addressId].First().Flats[floor1].First().Address);
            Assert.Empty(model.Addresses[addressId].First().Flats[floor1].First().Parameters);
            Assert.Equal(floor2, model.Addresses[addressId].First().Flats.Keys.Last());
            Assert.Equal(flatId2, model.Addresses[addressId].First().Flats[floor2].First().ID);
            Assert.Equal(floor2, model.Addresses[addressId].First().Flats[floor2].First().Floor);
            Assert.Equal(flatNumber2, model.Addresses[addressId].First().Flats[floor2].First().Number);
            Assert.Equal(entryDoorCode2, model.Addresses[addressId].First().Flats[floor2].First().EntryDoorCode);
            Assert.NotNull(model.Addresses[addressId].First().Flats[floor2].First().Address);
            Assert.Empty(model.Addresses[addressId].First().Flats[floor2].First().Parameters);
            Assert.Null(model.Flats);
            Assert.Empty(model.Parameters);

            _usersRepository.Verify(u => u.User(hostId1), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Addresses(hostId1), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId1), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId2), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Hosts(addressId), Times.Once);
        }

        [Fact]
        public void Details_IdFound_OneAddress_TwoFlats_TwoParameters_TwoHosts()
        {
            // Arrange
            string hostId1 = "someHostId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";
            string phoneNumber21 = "phoneNumber21";

            UserDTO hostDetails = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1,
                PhoneNumber2 = phoneNumber21
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            int flatId1 = 2;
            int floor1 = 20;
            string flatNumber1 = "flatNumber1";
            string entryDoorCode1 = "entryDoorCode1";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId
            };

            FlatDTO flatDTO1 = new FlatDTO(flat1);

            int flatId2 = 3;
            int floor2 = 10;
            string flatNumber2 = "flatNumber2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId
            };

            FlatDTO flatDTO2 = new FlatDTO(flat2);

            string userId1 = "someUserId1";
            string personalNumberUser1 = "personalNumberUser1";
            string firstNameUser1 = "firstNameUser1";
            string lastNameUser1 = "lastNameUser1";
            string emailAddressUser1 = "emailAddressUser1";
            string phoneNumberUser1 = "phoneNumberUser1";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumberUser1,
                FirstName = firstNameUser1,
                LastName = lastNameUser1,
                Email = emailAddressUser1,
                PhoneNumber = phoneNumberUser1
            };

            string userId2 = "someUserId2";
            string personalNumberUser2 = "personalNumberUser2";
            string firstNameUser2 = "firstNameUser2";
            string lastNameUser2 = "lastNameUser2";
            string emailAddressUser2 = "emailAddressUser2";
            string phoneNumberUser2 = "phoneNumberUser2";

            User user2 = new User
            {
                Id = userId2,
                PersonalNumber = personalNumberUser2,
                FirstName = firstNameUser2,
                LastName = lastNameUser2,
                Email = emailAddressUser2,
                PhoneNumber = phoneNumberUser2
            };

            ParameterDTO parameter1 = new ParameterDTO(flat1, user1);
            ParameterDTO parameter2 = new ParameterDTO(flat1, user2);

            UserDTO host1 = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1,
                PhoneNumber2 = phoneNumber21
            };

            string hostId2 = "someHostId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";
            string phoneNumber22 = "phoneNumber22";

            UserDTO host2 = new UserDTO
            {
                Id = hostId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2,
                PhoneNumber2 = phoneNumber22
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address };
            List<FlatDTO> flats = new List<FlatDTO> { flatDTO1, flatDTO2 };
            List<ParameterDTO> parameters1 = new List<ParameterDTO> { parameter1, parameter2 };
            List<ParameterDTO> parameters2 = new List<ParameterDTO>();
            List<UserDTO> hosts = new List<UserDTO> { host1, host2 };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(hostDetails);
            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns((int flatId) => flatId == flatId1 ? parameters1 : parameters2);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(hostId1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            // HOST //
            Assert.Equal(hostId1, model.Id);
            Assert.Equal(personalNumber1, model.PersonalNumber);
            Assert.Equal(firstName1, model.FirstName);
            Assert.Equal(lastName1, model.LastName);
            Assert.Equal(emailAddress1, model.Email);
            Assert.Equal(phoneNumber1, model.PhoneNumber);
            Assert.Equal(phoneNumber21, model.PhoneNumber2);
            // / HOST.FLATS / //
            Assert.Null(model.Flats);
            // / HOST.PARAMETERS / //
            Assert.Empty(model.Parameters);
            // / HOST.ADDRESSES / //
            Assert.Single(model.Addresses);
            Assert.Equal(addressId, model.Addresses.First().Key);
            Assert.Equal(addressId, model.Addresses[addressId].First().ID);
            Assert.Equal(street, model.Addresses[addressId].First().Street);
            Assert.Equal(addressNumber, model.Addresses[addressId].First().Number);
            Assert.Equal(zipCode, model.Addresses[addressId].First().ZipCode);
            Assert.Equal(town, model.Addresses[addressId].First().Town);
            Assert.Equal(country, model.Addresses[addressId].First().Country);
            Assert.True(model.Addresses[addressId].First().CanDataBeDeleted);
            Assert.Equal(0, model.Addresses[addressId].First().AmountAvailableFlats);
            Assert.Equal(2, model.Addresses[addressId].First().AmountFlats);
            Assert.Equal(2, model.Addresses[addressId].First().AmountHosts);
            Assert.Equal(2, model.Addresses[addressId].First().AmountUsers);
            Assert.Equal(2, model.Addresses[addressId].First().Flats.Count);
            // // HOST.ADDRESSES.FLATS // //
            Assert.Equal(floor1, model.Addresses[addressId].First().Flats.Keys.First());
            Assert.Equal(flatId1, model.Addresses[addressId].First().Flats[floor1].First().ID);
            Assert.Equal(floor1, model.Addresses[addressId].First().Flats[floor1].First().Floor);
            Assert.Equal(flatNumber1, model.Addresses[addressId].First().Flats[floor1].First().Number);
            Assert.Equal(entryDoorCode1, model.Addresses[addressId].First().Flats[floor1].First().EntryDoorCode);
            Assert.NotNull(model.Addresses[addressId].First().Flats[floor1].First().Address);
            // /// HOST.ADDRESSES.FLATS.PARAMETERS /// //
            Assert.Equal(2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Count);
            Assert.Equal(userId1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.Id);
            Assert.Equal(personalNumberUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.PersonalNumber);
            Assert.Equal(firstNameUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.FirstName);
            Assert.Equal(lastNameUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.LastName);
            Assert.Equal(emailAddressUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.Email);
            Assert.Equal(phoneNumberUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.PhoneNumber);
            Assert.Equal(userId2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.Id);
            Assert.Equal(personalNumberUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.PersonalNumber);
            Assert.Equal(firstNameUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.FirstName);
            Assert.Equal(lastNameUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.LastName);
            Assert.Equal(emailAddressUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.Email);
            Assert.Equal(phoneNumberUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.PhoneNumber);
            Assert.Equal(floor2, model.Addresses[addressId].First().Flats.Keys.Last());
            Assert.Equal(flatId2, model.Addresses[addressId].First().Flats[floor2].First().ID);
            Assert.Equal(floor2, model.Addresses[addressId].First().Flats[floor2].First().Floor);
            Assert.Equal(flatNumber2, model.Addresses[addressId].First().Flats[floor2].First().Number);
            Assert.Equal(entryDoorCode2, model.Addresses[addressId].First().Flats[floor2].First().EntryDoorCode);
            Assert.NotNull(model.Addresses[addressId].First().Flats[floor2].First().Address);
            Assert.Empty(model.Addresses[addressId].First().Flats[floor2].First().Parameters);

            _usersRepository.Verify(u => u.User(hostId1), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Addresses(hostId1), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId1), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId2), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Hosts(addressId), Times.Once);
        }

        [Fact]
        public void Details_IdFound_OneAddress_TwoFlats_ThreeParameters_TwoHosts()
        {
            // Arrange
            string hostId1 = "someHostId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";
            string phoneNumber21 = "phoneNumber21";

            UserDTO hostDetails = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1,
                PhoneNumber2 = phoneNumber21
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            int flatId1 = 2;
            int floor1 = 20;
            string flatNumber1 = "flatNumber1";
            string entryDoorCode1 = "entryDoorCode1";

            Flat flat1 = new Flat
            {
                ID = flatId1,
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId
            };

            FlatDTO flatDTO1 = new FlatDTO(flat1);

            int flatId2 = 3;
            int floor2 = 10;
            string flatNumber2 = "flatNumber2";
            string entryDoorCode2 = "entryDoorCode2";

            Flat flat2 = new Flat
            {
                ID = flatId2,
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId
            };

            FlatDTO flatDTO2 = new FlatDTO(flat2);

            string userId1 = "someUserId1";
            string personalNumberUser1 = "personalNumberUser1";
            string firstNameUser1 = "firstNameUser1";
            string lastNameUser1 = "lastNameUser1";
            string emailAddressUser1 = "emailAddressUser1";
            string phoneNumberUser1 = "phoneNumberUser1";

            User user1 = new User
            {
                Id = userId1,
                PersonalNumber = personalNumberUser1,
                FirstName = firstNameUser1,
                LastName = lastNameUser1,
                Email = emailAddressUser1,
                PhoneNumber = phoneNumberUser1
            };

            string userId2 = "someUserId2";
            string personalNumberUser2 = "personalNumberUser2";
            string firstNameUser2 = "firstNameUser2";
            string lastNameUser2 = "lastNameUser2";
            string emailAddressUser2 = "emailAddressUser2";
            string phoneNumberUser2 = "phoneNumberUser2";

            User user2 = new User
            {
                Id = userId2,
                PersonalNumber = personalNumberUser2,
                FirstName = firstNameUser2,
                LastName = lastNameUser2,
                Email = emailAddressUser2,
                PhoneNumber = phoneNumberUser2
            };

            string userId3 = "someUserId3";
            string personalNumberUser3 = "personalNumberUser3";
            string firstNameUser3 = "firstNameUser3";
            string lastNameUser3 = "lastNameUser3";
            string emailAddressUser3 = "emailAddressUser3";
            string phoneNumberUser3 = "phoneNumberUser3";

            User user3 = new User
            {
                Id = userId3,
                PersonalNumber = personalNumberUser3,
                FirstName = firstNameUser3,
                LastName = lastNameUser3,
                Email = emailAddressUser3,
                PhoneNumber = phoneNumberUser3
            };

            ParameterDTO parameter1 = new ParameterDTO(flat1, user1);
            ParameterDTO parameter2 = new ParameterDTO(flat1, user2);
            ParameterDTO parameter3 = new ParameterDTO(flat2, user3);

            UserDTO host1 = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1,
                PhoneNumber2 = phoneNumber21
            };

            string hostId2 = "someHostId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = "lastName2";
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";
            string phoneNumber22 = "phoneNumber22";

            UserDTO host2 = new UserDTO
            {
                Id = hostId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2,
                PhoneNumber2 = phoneNumber22
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address };
            List<FlatDTO> flats = new List<FlatDTO> { flatDTO1, flatDTO2 };
            List<ParameterDTO> parameters1 = new List<ParameterDTO> { parameter1, parameter2 };
            List<ParameterDTO> parameters2 = new List<ParameterDTO> { parameter3 };
            List<UserDTO> hosts = new List<UserDTO> { host1, host2 };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(hostDetails);
            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns((int flatId) => flatId == flatId1 ? parameters1 : parameters2);
            _addressHostsRepository.Setup(ah => ah.Hosts(It.IsAny<int>()))
                                   .Returns(hosts);

            // Act
            var result = _controller.Details(hostId1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            // HOST //
            Assert.Equal(hostId1, model.Id);
            Assert.Equal(personalNumber1, model.PersonalNumber);
            Assert.Equal(firstName1, model.FirstName);
            Assert.Equal(lastName1, model.LastName);
            Assert.Equal(emailAddress1, model.Email);
            Assert.Equal(phoneNumber1, model.PhoneNumber);
            Assert.Equal(phoneNumber21, model.PhoneNumber2);
            // / HOST.FLATS / //
            Assert.Null(model.Flats);
            // / HOST.PARAMETERS / //
            Assert.Empty(model.Parameters);
            // / HOST.ADDRESSES / //
            Assert.Single(model.Addresses);
            Assert.Equal(addressId, model.Addresses.First().Key);
            Assert.Equal(addressId, model.Addresses[addressId].First().ID);
            Assert.Equal(street, model.Addresses[addressId].First().Street);
            Assert.Equal(addressNumber, model.Addresses[addressId].First().Number);
            Assert.Equal(zipCode, model.Addresses[addressId].First().ZipCode);
            Assert.Equal(town, model.Addresses[addressId].First().Town);
            Assert.Equal(country, model.Addresses[addressId].First().Country);
            Assert.True(model.Addresses[addressId].First().CanDataBeDeleted);
            Assert.Equal(0, model.Addresses[addressId].First().AmountAvailableFlats);
            Assert.Equal(2, model.Addresses[addressId].First().AmountFlats);
            Assert.Equal(2, model.Addresses[addressId].First().AmountHosts);
            Assert.Equal(3, model.Addresses[addressId].First().AmountUsers);
            Assert.Equal(2, model.Addresses[addressId].First().Flats.Count);
            // // HOST.ADDRESSES.FLATS // //
            Assert.Equal(floor1, model.Addresses[addressId].First().Flats.Keys.First());
            Assert.Equal(flatId1, model.Addresses[addressId].First().Flats[floor1].First().ID);
            Assert.Equal(floor1, model.Addresses[addressId].First().Flats[floor1].First().Floor);
            Assert.Equal(flatNumber1, model.Addresses[addressId].First().Flats[floor1].First().Number);
            Assert.Equal(entryDoorCode1, model.Addresses[addressId].First().Flats[floor1].First().EntryDoorCode);
            Assert.NotNull(model.Addresses[addressId].First().Flats[floor1].First().Address);
            // /// HOST.ADDRESSES.FLATS.PARAMETERS /// //
            Assert.Equal(2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Count);
            Assert.Equal(userId1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.Id);
            Assert.Equal(personalNumberUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.PersonalNumber);
            Assert.Equal(firstNameUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.FirstName);
            Assert.Equal(lastNameUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.LastName);
            Assert.Equal(emailAddressUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.Email);
            Assert.Equal(phoneNumberUser1, model.Addresses[addressId].First().Flats[floor1].First().Parameters.First().User.PhoneNumber);
            Assert.Equal(userId2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.Id);
            Assert.Equal(personalNumberUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.PersonalNumber);
            Assert.Equal(firstNameUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.FirstName);
            Assert.Equal(lastNameUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.LastName);
            Assert.Equal(emailAddressUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.Email);
            Assert.Equal(phoneNumberUser2, model.Addresses[addressId].First().Flats[floor1].First().Parameters.Last().User.PhoneNumber);
            Assert.Equal(floor2, model.Addresses[addressId].First().Flats.Keys.Last());
            Assert.Equal(flatId2, model.Addresses[addressId].First().Flats[floor2].First().ID);
            Assert.Equal(floor2, model.Addresses[addressId].First().Flats[floor2].First().Floor);
            Assert.Equal(flatNumber2, model.Addresses[addressId].First().Flats[floor2].First().Number);
            Assert.Equal(entryDoorCode2, model.Addresses[addressId].First().Flats[floor2].First().EntryDoorCode);
            Assert.NotNull(model.Addresses[addressId].First().Flats[floor2].First().Address);
            Assert.Single(model.Addresses[addressId].First().Flats[floor2].First().Parameters);
            Assert.Equal(userId3, model.Addresses[addressId].First().Flats[floor2].First().Parameters.First().User.Id);
            Assert.Equal(personalNumberUser3, model.Addresses[addressId].First().Flats[floor2].First().Parameters.First().User.PersonalNumber);
            Assert.Equal(firstNameUser3, model.Addresses[addressId].First().Flats[floor2].First().Parameters.First().User.FirstName);
            Assert.Equal(lastNameUser3, model.Addresses[addressId].First().Flats[floor2].First().Parameters.First().User.LastName);
            Assert.Equal(emailAddressUser3, model.Addresses[addressId].First().Flats[floor2].First().Parameters.First().User.Email);
            Assert.Equal(phoneNumberUser3, model.Addresses[addressId].First().Flats[floor2].First().Parameters.First().User.PhoneNumber);

            _usersRepository.Verify(u => u.User(hostId1), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Addresses(hostId1), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId1), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId2), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Hosts(addressId), Times.Once);
        }

        #endregion

        #region Edit

        #region Edit - Get

        [Fact]
        public void EditGet_IdNull_UserNotFound()
        {
            // Arrange
            string id = null;
            string userId = "someUserId";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns<User>(null);

            try
            {
                // Act
                var result = _controller.Edit(id);
            }
            catch (ApplicationException ex)
            {
                // Assert
                Assert.NotNull(ex);

                _usersRepository.Verify(u => u.User(userId), Times.Once);
            }
        }

        [Fact]
        public void EditGet_IdNotNull_UserNotFound()
        {
            // Arrange
            string userId = "someUserId";
            UserDTO user = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            try
            {
                // Act
                var result = _controller.Edit(userId);
            }
            catch (ApplicationException ex)
            {
                // Assert
                Assert.NotNull(ex);

                _usersRepository.Verify(u => u.User(userId), Times.Once);
            }
        }

        [Fact]
        public void EditGet_IdNull_UserFound()
        {
            // Arrange
            string userId = null;
            string currentUserId = "someUserId";
            string userPersonalNumber = "personalNumber";
            string userFN = "John";
            string userLN = "Doe";
            string userEmail = "john.doe@some.email";
            string userPhoneNumber = "123456789";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, currentUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };

            _controller.ControllerContext = context;

            UserDTO userDTO = new UserDTO
            {
                Id = userId,
                PersonalNumber = userPersonalNumber,
                FirstName = userFN,
                LastName = userLN,
                Email = userEmail,
                PhoneNumber = userPhoneNumber
            };

            bool expectedCanChangePassword = true;

            _usersRepository.Setup(u => u.User(It.IsAny<string>())).Returns(userDTO);

            // Act
            var result = _controller.Edit(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserCreateVM>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(currentUserId, model.Id);
            Assert.Equal(userPersonalNumber, model.PersonalNumber);
            Assert.Equal(userFN, model.FirstName);
            Assert.Equal(userLN, model.LastName);
            Assert.Equal(expectedCanChangePassword, model.CanChangePassword);
            Assert.Equal(userPhoneNumber, model.PhoneNumber);

            _usersRepository.Verify(u => u.User(currentUserId), Times.Once);
        }

        [Fact]
        public void EditGet_IdNotNull_UserFound()
        {
            // Arrange
            string userId = "someUserId";
            string currentUserId = userId;
            string userFN = "John";
            string userLN = "Doe";
            string userPersonalNumber = "personalNumber";
            string userEmail = "john.doe@some.email";
            string userPhoneNumber = "123456789";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = userPersonalNumber,
                FirstName = userFN,
                LastName = userLN,
                Email = userEmail,
                PhoneNumber = userPhoneNumber
            };

            bool expectedCanChangePassword = false;

            _usersRepository.Setup(u => u.User(It.IsAny<string>())).Returns(user);

            // Act
            var result = _controller.Edit(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserCreateVM>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(userId, model.Id);
            Assert.Equal(userPersonalNumber, model.PersonalNumber);
            Assert.Equal(userFN, model.FirstName);
            Assert.Equal(userLN, model.LastName);
            Assert.Equal(expectedCanChangePassword, model.CanChangePassword);
            Assert.Equal(userPhoneNumber, model.PhoneNumber);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
        }

        #endregion

        #region Edit - Post

        [Fact]
        public async Task EditPost_InvalidModel()
        {
            // Arrange
            string userId = "someUserId";
            string userFN = "John";
            string userLN = "Doe";
            string userPersonalNumber = "john.doe@some.email";
            string userEmail = "john.doe@some.email";
            string userPhoneNumber = "123456789";

            UserCreateVM user = new UserCreateVM
            {
                Id = userId,
                FirstName = userFN,
                LastName = userLN,
                PersonalNumber = userPersonalNumber,
                Email = userEmail,
                PhoneNumber = userPhoneNumber
            };

            _usersRepository.Setup(u => u.Edit(It.IsAny<UserDTO>()));

            _controller.ModelState.AddModelError("fakeError", "fakeErrorMsg");

            // Act
            var result = await _controller.Edit(user);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserCreateVM>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(user, model);

            _usersRepository.Verify(u => u.Edit(It.IsAny<UserDTO>()), Times.Never);
            _usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EditPost_ValidModel_UserNotFound()
        {
            // Arrange
            string userId = "someUserId";
            string userFN = "John";
            string userLN = "Doe";
            string userPersonalNumber = "john.doe@some.email";
            string userEmail = "john.doe@some.email";
            string userPhoneNumber = "123456789";

            UserCreateVM user = new UserCreateVM
            {
                Id = userId,
                FirstName = userFN,
                LastName = userLN,
                PersonalNumber = userPersonalNumber,
                Email = userEmail,
                PhoneNumber = userPhoneNumber
            };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                                .Returns<User>(null);

            try
            {
                // Act
                var result = await _controller.Edit(user);
            }
            catch (ApplicationException ex)
            {
                // Assert
                Assert.NotNull(ex);

                _usersRepository.Verify(u => u.User(userId), Times.Once);
            }
        }

        [Fact]
        public async Task EditPost_ValidModel_UserExists_DuplicatedPersonalNumber()
        {
            // Arrange
            string userId = "someUserId";
            string userFNEdited = "firstNameEdited";
            string userLNEdited = "lastNameEdited";
            string userPersonalNumberEdited = "editedPersonalNumber";
            string userEmailEdited = "editedEmailAddress";
            string userPhoneNumberEdited = "phoneNumberEdited";

            UserCreateVM editedUser = new UserCreateVM
            {
                Id = userId,
                FirstName = userFNEdited,
                LastName = userLNEdited,
                PersonalNumber = userPersonalNumberEdited,
                Email = userEmailEdited,
                PhoneNumber = userPhoneNumberEdited
            };

            string userFNOriginal = "firstName";
            string userLNOriginal = "lastName";
            string userPersonalNumberOriginal = "personalNumber";
            string userEmailOriginal = "emailAddress";
            string userPhoneNumberOriginal = "phoneNumber";

            UserDTO originalUser = new UserDTO
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                NormalizedEmail = userEmailOriginal.ToUpper(),
                PhoneNumber = userPhoneNumberOriginal
            };

            UserDTO userDTOPN = new UserDTO
            {
                Id = "someOtherHostId"
            };

            string editorUserId = userId;
            var editorUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, editorUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = editorUser
                }
            };

            _controller.ControllerContext = context;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(originalUser);
            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTOPN);

            // Act
            var result = await _controller.Edit(editedUser);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserCreateVM>(viewResult.Model);

            Assert.Equal(userEmailOriginal.ToUpper(), originalUser.NormalizedEmail);
            Assert.Equal(userEmailOriginal, originalUser.Email);
            Assert.Equal(userPersonalNumberOriginal, originalUser.PersonalNumber);
            Assert.Equal(userPhoneNumberOriginal, originalUser.PhoneNumber);
            Assert.Equal(userFNOriginal, originalUser.FirstName);
            Assert.Equal(userLNOriginal, originalUser.LastName);

            Assert.Null(_controller.ViewBag.Message);
            Assert.Single(_controller.ModelState);
            Assert.Equal(nameof(UserDTO.PersonalNumber), _controller.ModelState.First().Key);
            Assert.Equal("En användare med samma personnummer finns redan i databasen.", _controller.ModelState.First().Value.Errors.First().ErrorMessage);
            Assert.Null(_controller.StatusMessage);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "Host"), Times.Once);
            _usersRepository.Verify(u => u.Edit(It.IsAny<UserDTO>()), Times.Never);
        }

        [Fact]
        public async Task EditPost_ValidModel_UserExists_PersonalNumberNotFound_EmailAddressEdited_AccountNotRegistered_DuplicatedUser()
        {
            // Arrange
            string userId = "someUserId";
            string userFNEdited = "firstNameEdited";
            string userLNEdited = "lastNameEdited";
            string userPersonalNumberEdited = "editedPersonalNumber";
            string userEmailEdited = "editedEmailAddress";
            string userPhoneNumberEdited = "phoneNumberEdited";

            UserCreateVM editedUser = new UserCreateVM
            {
                Id = userId,
                FirstName = userFNEdited,
                LastName = userLNEdited,
                PersonalNumber = userPersonalNumberEdited,
                Email = userEmailEdited,
                PhoneNumber = userPhoneNumberEdited
            };

            string userFNOriginal = "firstName";
            string userLNOriginal = "lastName";
            string userPersonalNumberOriginal = "personalNumber";
            string userEmailOriginal = "emailAddress";
            string userPhoneNumberOriginal = "phoneNumber";
            string userRegistrationCode = "registrationCode";

            UserDTO originalUser = new UserDTO
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                NormalizedEmail = userEmailOriginal.ToUpper(),
                PhoneNumber = userPhoneNumberOriginal,
                RegistrationCode = userRegistrationCode
            };

            UserDTO userDTOPN = null;

            string generatedRegistrationCode = "generatedRegistrationCode";
            string generatedPassword = "generatedPassword";

            string key = nameof(UserDTO.Email);
            string value = "En användare med samma e-post finns redan.";
            Dictionary<string, string> createResult = new Dictionary<string, string> { { key, value } };

            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            string requestString = "https";
            var url = new Mock<IUrlHelper>(MockBehavior.Strict);
            url.Setup(v => v.Action(It.IsAny<UrlActionContext>()))
               .Returns("callbackUrl")
               .Verifiable();

            context.Setup(v => v.Request)
                   .Returns(request.Object);
            request.Setup(v => v.Scheme)
                   .Returns(requestString);

            var ctrctx = new ControllerContext
            {
                HttpContext = context.Object
            };

            _controller.ControllerContext = ctrctx;
            _controller.Url = url.Object;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(originalUser);
            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTOPN);
            _usersRepository.Setup(u => u.GenerateRegistrationCode()).Returns(generatedRegistrationCode);
            _usersRepository.Setup(u => u.GeneratePassword()).Returns(generatedPassword);
            _newUser.Setup(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                           It.IsAny<string>(),
                                           It.IsAny<string>(),
                                           It.IsAny<string>(),
                                           It.IsAny<IRequestUserProvider>(),
                                           It.IsAny<IEmailSender>(),
                                           It.IsAny<IUrlHelper>(),
                                           It.IsAny<string>()))
                    .Returns(Task.FromResult(createResult));

            // Act
            var result = await _controller.Edit(editedUser);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserCreateVM>(viewResult.Model);

            Assert.Equal(userEmailOriginal.ToUpper(), originalUser.NormalizedEmail);
            Assert.Equal(userEmailOriginal, originalUser.Email);
            Assert.Equal(userPersonalNumberOriginal, originalUser.PersonalNumber);
            Assert.Equal(userPhoneNumberOriginal, originalUser.PhoneNumber);
            Assert.Equal(userFNOriginal, originalUser.FirstName);
            Assert.Equal(userLNOriginal, originalUser.LastName);

            Assert.Null(_controller.ViewBag.Message);
            Assert.Null(_controller.StatusMessage);

            Assert.Single(_controller.ModelState);
            Assert.Equal(key, _controller.ModelState.First().Key);
            Assert.Equal(value, _controller.ModelState.First().Value.Errors.First().ErrorMessage);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Once);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Once);
            _newUser.Verify(nu => nu.Create(model,
                                            "Host",
                                            generatedRegistrationCode,
                                            generatedPassword,
                                            _requestUserProvider.Object,
                                            _emailSender.Object,
                                            url.Object,
                                            requestString),
                                  Times.Once);
            _usersRepository.Verify(u => u.Delete(It.IsAny<string>()), Times.Never);
            _usersRepository.Verify(u => u.Edit(originalUser), Times.Never);
        }

        [Fact]
        public async Task EditPost_ValidModel_UserExists_PersonalNumberNotFound_EmailAddressEdited_AccountNotRegistered_NoDuplicatedUser()
        {
            // Arrange
            string userId = "someUserId";
            string userFNEdited = "firstNameEdited";
            string userLNEdited = "lastNameEdited";
            string userPersonalNumberEdited = "editedPersonalNumber";
            string userEmailEdited = "editedEmailAddress";
            string userPhoneNumberEdited = "phoneNumberEdited";

            UserCreateVM editedUser = new UserCreateVM
            {
                Id = userId,
                FirstName = userFNEdited,
                LastName = userLNEdited,
                PersonalNumber = userPersonalNumberEdited,
                Email = userEmailEdited,
                PhoneNumber = userPhoneNumberEdited
            };

            string userFNOriginal = "firstName";
            string userLNOriginal = "lastName";
            string userPersonalNumberOriginal = "personalNumber";
            string userEmailOriginal = "emailAddress";
            string userPhoneNumberOriginal = "phoneNumber";
            string userRegistrationCode = "registrationCode";

            UserDTO originalUser = new UserDTO
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                NormalizedEmail = userEmailOriginal.ToUpper(),
                PhoneNumber = userPhoneNumberOriginal,
                RegistrationCode = userRegistrationCode
            };

            UserDTO userDTOPN = null;

            string generatedRegistrationCode = "generatedRegistrationCode";
            string generatedPassword = "generatedPassword";

            Dictionary<string, string> createResult = new Dictionary<string, string> { };

            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            string requestString = "https";
            var url = new Mock<IUrlHelper>(MockBehavior.Strict);
            url.Setup(v => v.Action(It.IsAny<UrlActionContext>()))
               .Returns("callbackUrl")
               .Verifiable();

            string editorUserId = userId;
            var editorUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, editorUserId)
            }));

            httpContext.Setup(c => c.Request)
                       .Returns(request.Object);
            httpContext.Setup(c => c.User)
                       .Returns(editorUser);
            request.Setup(v => v.Scheme)
                   .Returns(requestString);

            var ctrctx = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            _controller.ControllerContext = ctrctx;
            _controller.Url = url.Object;

            string expectedStatusMessage = "Din profil uppdateras!";

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(originalUser);
            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTOPN);
            _usersRepository.Setup(u => u.GenerateRegistrationCode()).Returns(generatedRegistrationCode);
            _usersRepository.Setup(u => u.GeneratePassword()).Returns(generatedPassword);
            _newUser.Setup(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                           It.IsAny<string>(),
                                           It.IsAny<string>(),
                                           It.IsAny<string>(),
                                           It.IsAny<IRequestUserProvider>(),
                                           It.IsAny<IEmailSender>(),
                                           It.IsAny<IUrlHelper>(),
                                           It.IsAny<string>()))
                    .Returns(Task.FromResult(createResult));
            _usersRepository.Setup(u => u.Delete(It.IsAny<string>()))
                            .Returns(1);
            _usersRepository.Setup(u => u.Edit(It.IsAny<UserDTO>()));

            // Act
            var result = await _controller.Edit(editedUser);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Edit), viewResult.ActionName);

            Assert.Equal(userEmailOriginal.ToUpper(), originalUser.NormalizedEmail);
            Assert.Equal(userEmailOriginal, originalUser.Email);
            Assert.Equal(userPersonalNumberOriginal, originalUser.PersonalNumber);
            Assert.Equal(userPhoneNumberOriginal, originalUser.PhoneNumber);
            Assert.Equal(userFNOriginal, originalUser.FirstName);
            Assert.Equal(userLNOriginal, originalUser.LastName);

            Assert.Null(_controller.ViewBag.Message);
            Assert.Equal(expectedStatusMessage, _controller.StatusMessage);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Once);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Once);
            _newUser.Verify(nu => nu.Create(editedUser,
                                            "Host",
                                            generatedRegistrationCode,
                                            generatedPassword,
                                            _requestUserProvider.Object,
                                            _emailSender.Object,
                                            url.Object,
                                            requestString),
                                  Times.Once);
            _usersRepository.Verify(u => u.Delete(userId), Times.Once);
            _usersRepository.Verify(u => u.Edit(It.IsAny<UserDTO>()), Times.Never);
        }

        [Fact]
        public async Task EditPost_ValidModel_UserExists_PersonalNumberNotFound_NoChanges_EditedByCurrentUser()
        {
            // Arrange
            string userId = "someUserId";
            string userFNOriginal = "firstName";
            string userLNOriginal = "lastName";
            string userPersonalNumberOriginal = "personalNumber";
            string userEmailOriginal = "emailAddress";
            string userPhoneNumberOriginal = "phoneNumber";
            string userPhoneNumber2Original = string.Empty;
            string userRegistrationCode = string.Empty;

            UserCreateVM editedUser = new UserCreateVM
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                PhoneNumber = userPhoneNumberOriginal,
                PhoneNumber2 = userPhoneNumber2Original
            };

            UserDTO originalUser = new UserDTO
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                NormalizedEmail = userEmailOriginal.ToUpper(),
                PhoneNumber = userPhoneNumberOriginal,
                RegistrationCode = userRegistrationCode
            };

            UserDTO userDTOPN = null;

            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            string requestString = "https";
            var url = new Mock<IUrlHelper>(MockBehavior.Strict);
            url.Setup(v => v.Action(It.IsAny<UrlActionContext>()))
               .Returns("callbackUrl")
               .Verifiable();

            httpContext.Setup(c => c.Request)
                       .Returns(request.Object);
            request.Setup(v => v.Scheme)
                   .Returns(requestString);

            var ctrctx = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            _controller.ControllerContext = ctrctx;
            _controller.Url = url.Object;

            string editorUserId = userId;
            var editorUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, editorUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = editorUser
                }
            };

            _controller.ControllerContext = context;

            string expectedStatusMessage = "Din profil uppdateras!";

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(originalUser);
            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTOPN);
            _usersRepository.Setup(u => u.Edit(It.IsAny<UserDTO>()));

            // Act
            var result = await _controller.Edit(editedUser);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Edit), viewResult.ActionName);

            Assert.Equal(userEmailOriginal.ToUpper(), originalUser.NormalizedEmail);
            Assert.Equal(userEmailOriginal, originalUser.Email);
            Assert.Equal(userPersonalNumberOriginal, originalUser.PersonalNumber);
            Assert.Equal(userPhoneNumberOriginal, originalUser.PhoneNumber);
            Assert.Equal(userFNOriginal, originalUser.FirstName);
            Assert.Equal(userLNOriginal, originalUser.LastName);

            Assert.Null(_controller.ViewBag.Message);
            Assert.Equal(expectedStatusMessage, _controller.StatusMessage);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberOriginal, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Never);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Never);
            _newUser.Verify(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<IRequestUserProvider>(),
                                            It.IsAny<IEmailSender>(),
                                            It.IsAny<IUrlHelper>(),
                                            It.IsAny<string>()),
                                  Times.Never);
            _usersRepository.Verify(u => u.Delete(It.IsAny<string>()), Times.Never);
            _usersRepository.Verify(u => u.Edit(originalUser), Times.Never);
        }

        [Fact]
        public async Task EditPost_ValidModel_UserExists_PersonalNumberNotFound_EmailAddressEdited_AccountRegistered_EditedByCurrentUser()
        {
            // Arrange
            string userId = "someUserId";
            string userFNEdited = "firstNameEdited";
            string userLNEdited = "lastNameEdited";
            string userPersonalNumberEdited = "editedPersonalNumber";
            string userEmailEdited = "editedEmailAddress";
            string userPhoneNumberEdited = "phoneNumberEdited";

            UserCreateVM editedUser = new UserCreateVM
            {
                Id = userId,
                FirstName = userFNEdited,
                LastName = userLNEdited,
                PersonalNumber = userPersonalNumberEdited,
                Email = userEmailEdited,
                PhoneNumber = userPhoneNumberEdited
            };

            string userFNOriginal = "firstName";
            string userLNOriginal = "lastName";
            string userPersonalNumberOriginal = "personalNumber";
            string userEmailOriginal = "emailAddress";
            string userPhoneNumberOriginal = "phoneNumber";
            string userRegistrationCode = string.Empty;

            UserDTO originalUser = new UserDTO
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                NormalizedEmail = userEmailOriginal.ToUpper(),
                PhoneNumber = userPhoneNumberOriginal,
                RegistrationCode = userRegistrationCode
            };

            UserDTO userDTOPN = null;

            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            string requestString = "https";
            var url = new Mock<IUrlHelper>(MockBehavior.Strict);
            url.Setup(v => v.Action(It.IsAny<UrlActionContext>()))
               .Returns("callbackUrl")
               .Verifiable();

            httpContext.Setup(c => c.Request)
                       .Returns(request.Object);
            request.Setup(v => v.Scheme)
                   .Returns(requestString);

            var ctrctx = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            _controller.ControllerContext = ctrctx;
            _controller.Url = url.Object;

            string editorUserId = userId;
            var editorUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, editorUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = editorUser
                }
            };

            _controller.ControllerContext = context;

            string expectedStatusMessage = "Din profil uppdateras!";

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(originalUser);
            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTOPN);
            _usersRepository.Setup(u => u.Edit(It.IsAny<UserDTO>()));

            // Act
            var result = await _controller.Edit(editedUser);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Edit), viewResult.ActionName);

            Assert.Equal(userEmailEdited.ToUpper(), originalUser.NormalizedEmail);
            Assert.Equal(userEmailEdited, originalUser.Email);
            Assert.Equal(userPersonalNumberEdited, originalUser.PersonalNumber);
            Assert.Equal(userPhoneNumberEdited, originalUser.PhoneNumber);
            Assert.Equal(userFNEdited, originalUser.FirstName);
            Assert.Equal(userLNEdited, originalUser.LastName);

            Assert.Null(_controller.ViewBag.Message);
            Assert.Equal(expectedStatusMessage, _controller.StatusMessage);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Never);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Never);
            _newUser.Verify(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<IRequestUserProvider>(),
                                            It.IsAny<IEmailSender>(),
                                            It.IsAny<IUrlHelper>(),
                                            It.IsAny<string>()),
                                  Times.Never);
            _usersRepository.Verify(u => u.Delete(It.IsAny<string>()), Times.Never);
            _usersRepository.Verify(u => u.Edit(originalUser), Times.Once);
        }

        [Fact]
        public async Task EditPost_ValidModel_UserExists_PersonalNumberNotFound_EmailAddressEdited_AccountRegistered_EditedBySomeoneElse_FirstNameNotNullOrEmpty()
        {
            // Arrange
            string userId = "someUserId";
            string userFNEdited = "John";
            string userLNEdited = "Doe";
            string userPersonalNumberEdited = "editedPersonalNumber";
            string userEmailEdited = "editedEmailAddress";
            string userPhoneNumberEdited = "123456789";

            UserCreateVM editedUser = new UserCreateVM
            {
                Id = userId,
                FirstName = userFNEdited,
                LastName = userLNEdited,
                PersonalNumber = userPersonalNumberEdited,
                Email = userEmailEdited,
                PhoneNumber = userPhoneNumberEdited
            };

            string userFNOriginal = "Jane";
            string userLNOriginal = "Dough";
            string userPersonalNumberOriginal = "originalPersonalNumber";
            string userEmailOriginal = "originalEmailAddress";
            string userPhoneNumberOriginal = "987654321";
            string userRegistrationCode = string.Empty;

            UserDTO originalUser = new UserDTO
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                NormalizedEmail = userEmailOriginal.ToUpper(),
                PhoneNumber = userPhoneNumberOriginal,
                RegistrationCode = userRegistrationCode
            };

            UserDTO userDTOPN = null;

            string editorUserId = "someEditedUserId";
            var editorUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, editorUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = editorUser
                }
            };

            _controller.ControllerContext = context;

            string expectedStatusMessage = string.Format("{0}s profil uppdateras!", userFNEdited);

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                                .Returns(originalUser);
            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTOPN);
            _usersRepository.Setup(u => u.Edit(It.IsAny<UserDTO>()));

            // Act
            var result = await _controller.Edit(editedUser);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Edit), viewResult.ActionName);

            Assert.Equal(userEmailEdited.ToUpper(), originalUser.NormalizedEmail);
            Assert.Equal(userEmailEdited, originalUser.Email);
            Assert.Equal(userPersonalNumberEdited, originalUser.PersonalNumber);
            Assert.Equal(userPhoneNumberEdited, originalUser.PhoneNumber);
            Assert.Equal(userFNEdited, originalUser.FirstName);
            Assert.Equal(userLNEdited, originalUser.LastName);

            Assert.Null(_controller.ViewBag.Message);
            Assert.Equal(expectedStatusMessage, _controller.StatusMessage);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Never);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Never);
            _newUser.Verify(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<IRequestUserProvider>(),
                                            It.IsAny<IEmailSender>(),
                                            It.IsAny<IUrlHelper>(),
                                            It.IsAny<string>()),
                                  Times.Never);
            _usersRepository.Verify(u => u.Delete(It.IsAny<string>()), Times.Never);
            _usersRepository.Verify(u => u.Edit(originalUser), Times.Once);
        }

        [Fact]
        public async Task EditPost_ValidModel_UserExists_PersonalNumberNotFound_EmailAddressEdited_AccountRegistered_EditedBySomeoneElse_FirstNameNull()
        {
            // Arrange
            string userId = "someUserId";
            string userFNEdited = null;
            string userLNEdited = "Doe";
            string userPersonalNumberEdited = "editedPersonalNumber";
            string userEmailEdited = "editedEmailAddress";
            string userPhoneNumberEdited = "123456789";

            UserCreateVM editedUser = new UserCreateVM
            {
                Id = userId,
                FirstName = userFNEdited,
                LastName = userLNEdited,
                PersonalNumber = userPersonalNumberEdited,
                Email = userEmailEdited,
                PhoneNumber = userPhoneNumberEdited
            };

            string userFNOriginal = "Jane";
            string userLNOriginal = "Dough";
            string userPersonalNumberOriginal = "originalPersonalNumber";
            string userEmailOriginal = "originalEmailAddress";
            string userPhoneNumberOriginal = "987654321";
            string userRegistrationCode = string.Empty;

            UserDTO originalUser = new UserDTO
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                NormalizedEmail = userEmailOriginal.ToUpper(),
                PhoneNumber = userPhoneNumberOriginal,
                RegistrationCode = userRegistrationCode
            };

            UserDTO userDTOPN = null;

            string editorUserId = "someEditedUserId";
            var editorUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, editorUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = editorUser
                }
            };

            _controller.ControllerContext = context;

            string expectedStatusMessage = string.Format("{0}s profil uppdateras!", userLNEdited);

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                                .Returns(originalUser);
            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTOPN);
            _usersRepository.Setup(u => u.Edit(It.IsAny<UserDTO>()));

            // Act
            var result = await _controller.Edit(editedUser);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Edit), viewResult.ActionName);

            Assert.Equal(userEmailEdited.ToUpper(), originalUser.NormalizedEmail);
            Assert.Equal(userEmailEdited, originalUser.Email);
            Assert.Equal(userPersonalNumberEdited, originalUser.PersonalNumber);
            Assert.Equal(userPhoneNumberEdited, originalUser.PhoneNumber);
            Assert.Equal(userFNEdited, originalUser.FirstName);
            Assert.Equal(userLNEdited, originalUser.LastName);

            Assert.Null(_controller.ViewBag.Message);
            Assert.Equal(expectedStatusMessage, _controller.StatusMessage);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Never);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Never);
            _newUser.Verify(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<IRequestUserProvider>(),
                                            It.IsAny<IEmailSender>(),
                                            It.IsAny<IUrlHelper>(),
                                            It.IsAny<string>()),
                                  Times.Never);
            _usersRepository.Verify(u => u.Delete(It.IsAny<string>()), Times.Never);
            _usersRepository.Verify(u => u.Edit(originalUser), Times.Once);
        }

        [Fact]
        public async Task EditPost_ValidModel_UserExists_PersonalNumberNotFound_EmailAddressEdited_AccountRegistered_EditedBySomeoneElse_FirstNameEmpty()
        {
            // Arrange
            string userId = "someUserId";
            string userFNEdited = string.Empty;
            string userLNEdited = "Doe";
            string userPersonalNumberEdited = "editedPersonalNumber";
            string userEmailEdited = "editedEmailAddress";
            string userPhoneNumberEdited = "123456789";

            UserCreateVM editedUser = new UserCreateVM
            {
                Id = userId,
                FirstName = userFNEdited,
                LastName = userLNEdited,
                PersonalNumber = userPersonalNumberEdited,
                Email = userEmailEdited,
                PhoneNumber = userPhoneNumberEdited
            };

            string userFNOriginal = "Jane";
            string userLNOriginal = "Dough";
            string userPersonalNumberOriginal = "originalPersonalNumber";
            string userEmailOriginal = "originalEmailAddress";
            string userPhoneNumberOriginal = "987654321";
            string userRegistrationCode = string.Empty;

            UserDTO originalUser = new UserDTO
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                NormalizedEmail = userEmailOriginal.ToUpper(),
                PhoneNumber = userPhoneNumberOriginal,
                RegistrationCode = userRegistrationCode
            };

            UserDTO userDTOPN = null;

            string editorUserId = "someEditedUserId";
            var editorUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, editorUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = editorUser
                }
            };

            _controller.ControllerContext = context;

            string expectedStatusMessage = string.Format("{0}s profil uppdateras!", userLNEdited);

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                                .Returns(originalUser);
            _usersRepository.Setup(u => u.UserByPersonalNumber(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(userDTOPN);
            _usersRepository.Setup(u => u.Edit(It.IsAny<UserDTO>()));

            // Act
            var result = await _controller.Edit(editedUser);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Edit), viewResult.ActionName);

            Assert.Equal(userEmailEdited.ToUpper(), originalUser.NormalizedEmail);
            Assert.Equal(userEmailEdited, originalUser.Email);
            Assert.Equal(userPersonalNumberEdited, originalUser.PersonalNumber);
            Assert.Equal(userPhoneNumberEdited, originalUser.PhoneNumber);
            Assert.Equal(userFNEdited, originalUser.FirstName);
            Assert.Equal(userLNEdited, originalUser.LastName);

            Assert.Null(_controller.ViewBag.Message);
            Assert.Equal(expectedStatusMessage, _controller.StatusMessage);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "Host"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Never);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Never);
            _newUser.Verify(nu => nu.Create(It.IsAny<UserCreateVM>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>(),
                                            It.IsAny<IRequestUserProvider>(),
                                            It.IsAny<IEmailSender>(),
                                            It.IsAny<IUrlHelper>(),
                                            It.IsAny<string>()),
                                  Times.Never);
            _usersRepository.Verify(u => u.Delete(It.IsAny<string>()), Times.Never);
            _usersRepository.Verify(u => u.Edit(originalUser), Times.Once);
        }

        #endregion

        #endregion

        #region Delete

        [Fact]
        public void Delete_IdNull()
        {
            // Arrange
            string userId = null;

            // Act
            var result = _controller.Delete(userId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Delete_IdNotNull_UserNotFound()
        {
            // Arrange
            string userId = "someUserId";
            UserDTO user = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            // Act
            var result = _controller.Delete(userId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
        }

        [Fact]
        public void Delete_ValidId()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            // Act
            var result = _controller.Delete(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserDetailsVM>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(userId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Equal(phoneNumber2, model.PhoneNumber2);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
        }

        #endregion

        #region DeleteConfirmed

        [Fact]
        public void DeleteConfirmed_UserNotFound()
        {
            // Arrange
            string userId = "someUserId";
            UserDTO user = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            _usersRepository.Setup(u => u.Delete(It.IsAny<string>()))
                                .Returns(0);

            // Act
            var result = _controller.DeleteConfirmed(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.Delete(userId), Times.Never);

        }

        [Fact]
        public void DeleteConfirmed_UserFound()
        {
            // Arrange
            string id = "someUserId";
            UserDTO user = new UserDTO();

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                                .Returns(user);

            _usersRepository.Setup(u => u.Delete(It.IsAny<string>()))
                                .Returns(0);

            // Act
            var result = _controller.DeleteConfirmed(id);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), viewResult.ActionName);

            _usersRepository.Verify(u => u.User(id), Times.Once);
            _usersRepository.Verify(u => u.Delete(id), Times.Once);

        }

        #endregion

        #region Remove

        [Fact]
        public void Remove_UserIdNull()
        {
            // Arrange
            string userId = null;
            int addressId = 1;

            // Act
            var result = _controller.Remove(userId, addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            _addressesRepository.Verify(a => a.Address(It.IsAny<int>()), Times.Never);
            _addressHostsRepository.Verify(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<int>()), Times.Never);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserNotFound()
        {
            // Arrange
            string userId = "someUserId";
            int addressId = 1;

            UserDTO user = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            // Act
            var result = _controller.Remove(userId, addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Once);
            _addressesRepository.Verify(a => a.Address(It.IsAny<int>()), Times.Never);
            _addressHostsRepository.Verify(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<int>()), Times.Never);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_AddressNotFound()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            int addressId = 1;

            AddressDTO address = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Remove(userId, addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Once);
            _addressesRepository.Verify(a => a.Address(It.IsAny<int>()), Times.Once);
            _addressHostsRepository.Verify(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<int>()), Times.Never);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_AddressFound_AddressHostNotFound()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            AddressHostDTO addressHost = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _addressHostsRepository.Setup(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()))
                                   .Returns(addressHost);

            // Act
            var result = _controller.Remove(userId, addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.AddressHost(addressId, userId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<int>()), Times.Never);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_AddressFound_AddressHostFound_FlatsNull()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            AddressHostDTO addressHost = new AddressHostDTO(address, user);

            List<FlatDTO> flats = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _addressHostsRepository.Setup(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()))
                                   .Returns(addressHost);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);

            // Act
            var result = _controller.Remove(userId, addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddressHostDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Host.Id);
            Assert.Equal(personalNumber, model.Host.PersonalNumber);
            Assert.Equal(firstName, model.Host.FirstName);
            Assert.Equal(lastName, model.Host.LastName);
            Assert.Equal(emailAddress, model.Host.Email);
            Assert.Equal(phoneNumber, model.Host.PhoneNumber);
            Assert.Equal(phoneNumber2, model.Host.PhoneNumber2);
            Assert.Empty(model.Host.Addresses);

            Assert.Equal(addressId, model.Address.ID);
            Assert.Equal(street, model.Address.Street);
            Assert.Equal(addressNumber, model.Address.Number);
            Assert.Equal(zipCode, model.Address.ZipCode);
            Assert.Equal(town, model.Address.Town);
            Assert.Equal(country, model.Address.Country);
            Assert.Null(model.Address.Flats);
            Assert.Null(model.Address.Hosts);
            Assert.Equal(0, model.Address.AmountFlats);
            Assert.Equal(0, model.Address.AmountAvailableFlats);
            Assert.Equal(0, model.Address.AmountUsers);
            Assert.Equal(0, model.Address.AmountHosts);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.AddressHost(addressId, userId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_AddressFound_AddressHostFound_FlatsNoEntry()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            AddressHostDTO addressHost = new AddressHostDTO(address, user);

            List<FlatDTO> flats = new List<FlatDTO>();

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _addressHostsRepository.Setup(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()))
                                   .Returns(addressHost);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);

            // Act
            var result = _controller.Remove(userId, addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddressHostDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Host.Id);
            Assert.Equal(personalNumber, model.Host.PersonalNumber);
            Assert.Equal(firstName, model.Host.FirstName);
            Assert.Equal(lastName, model.Host.LastName);
            Assert.Equal(emailAddress, model.Host.Email);
            Assert.Equal(phoneNumber, model.Host.PhoneNumber);
            Assert.Equal(phoneNumber2, model.Host.PhoneNumber2);
            Assert.Empty(model.Host.Addresses);

            Assert.Equal(addressId, model.Address.ID);
            Assert.Equal(street, model.Address.Street);
            Assert.Equal(addressNumber, model.Address.Number);
            Assert.Equal(zipCode, model.Address.ZipCode);
            Assert.Equal(town, model.Address.Town);
            Assert.Equal(country, model.Address.Country);
            Assert.Empty(model.Address.Flats);
            Assert.Null(model.Address.Hosts);
            Assert.Equal(0, model.Address.AmountFlats);
            Assert.Equal(0, model.Address.AmountAvailableFlats);
            Assert.Equal(0, model.Address.AmountUsers);
            Assert.Equal(0, model.Address.AmountHosts);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.AddressHost(addressId, userId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_AddressFound_AddressHostFound_FlatsOneEntry_ParametersNull()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            AddressHostDTO addressHost = new AddressHostDTO(address, user);

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
            List<ParameterDTO> parameters = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _addressHostsRepository.Setup(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()))
                                   .Returns(addressHost);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);

            // Act
            var result = _controller.Remove(userId, addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddressHostDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Host.Id);
            Assert.Equal(personalNumber, model.Host.PersonalNumber);
            Assert.Equal(firstName, model.Host.FirstName);
            Assert.Equal(lastName, model.Host.LastName);
            Assert.Equal(emailAddress, model.Host.Email);
            Assert.Equal(phoneNumber, model.Host.PhoneNumber);
            Assert.Equal(phoneNumber2, model.Host.PhoneNumber2);
            Assert.True(model.Host.IsPhoneNumber2Visible);
            Assert.Empty(model.Host.Addresses);

            Assert.Equal(addressId, model.Address.ID);
            Assert.Equal(street, model.Address.Street);
            Assert.Equal(addressNumber, model.Address.Number);
            Assert.Equal(zipCode, model.Address.ZipCode);
            Assert.Equal(town, model.Address.Town);
            Assert.Equal(country, model.Address.Country);
            Assert.Single(model.Address.Flats);
            Assert.Equal(floor, model.Address.Flats.First().Key);
            Assert.Equal(flatId, model.Address.Flats[floor].First().ID);
            Assert.Equal(floor, model.Address.Flats[floor].First().Floor);
            Assert.Equal(flatNumber, model.Address.Flats[floor].First().Number);
            Assert.Equal(entryDoorCode, model.Address.Flats[floor].First().EntryDoorCode);
            Assert.Empty(model.Address.Flats[floor].First().Parameters);
            Assert.Null(model.Address.Hosts);
            Assert.Equal(1, model.Address.AmountFlats);
            Assert.Equal(0, model.Address.AmountAvailableFlats);
            Assert.Equal(0, model.Address.AmountUsers);
            Assert.Equal(0, model.Address.AmountHosts);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.AddressHost(addressId, userId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId), Times.Once);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_AddressFound_AddressHostFound_FlatsOneEntry_ParametersNoEntry()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            AddressHostDTO addressHost = new AddressHostDTO(address, user);

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

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _addressHostsRepository.Setup(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()))
                                   .Returns(addressHost);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);

            // Act
            var result = _controller.Remove(userId, addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddressHostDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Host.Id);
            Assert.Equal(personalNumber, model.Host.PersonalNumber);
            Assert.Equal(firstName, model.Host.FirstName);
            Assert.Equal(lastName, model.Host.LastName);
            Assert.Equal(emailAddress, model.Host.Email);
            Assert.Equal(phoneNumber, model.Host.PhoneNumber);
            Assert.Equal(phoneNumber2, model.Host.PhoneNumber2);
            Assert.True(model.Host.IsPhoneNumber2Visible);
            Assert.Empty(model.Host.Addresses);

            Assert.Equal(addressId, model.Address.ID);
            Assert.Equal(street, model.Address.Street);
            Assert.Equal(addressNumber, model.Address.Number);
            Assert.Equal(zipCode, model.Address.ZipCode);
            Assert.Equal(town, model.Address.Town);
            Assert.Equal(country, model.Address.Country);
            Assert.Single(model.Address.Flats);
            Assert.Equal(floor, model.Address.Flats.First().Key);
            Assert.Equal(flatId, model.Address.Flats[floor].First().ID);
            Assert.Equal(floor, model.Address.Flats[floor].First().Floor);
            Assert.Equal(flatNumber, model.Address.Flats[floor].First().Number);
            Assert.Equal(entryDoorCode, model.Address.Flats[floor].First().EntryDoorCode);
            Assert.Empty(model.Address.Flats[floor].First().Parameters);
            Assert.Null(model.Address.Hosts);
            Assert.Equal(1, model.Address.AmountFlats);
            Assert.Equal(0, model.Address.AmountAvailableFlats);
            Assert.Equal(0, model.Address.AmountUsers);
            Assert.Equal(0, model.Address.AmountHosts);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.AddressHost(addressId, userId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId), Times.Once);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_AddressFound_AddressHostFound_FlatsOneEntry_ParametersOneEntry()
        {
            // Arrange
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

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            AddressHostDTO addressHost = new AddressHostDTO(address, host);

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

            string userId = "someUserId";
            string personalNumberUser = "personalNumberUser";
            string firstNameUser = "firstNameUser";
            string lastNameUser = "lastNameUser";
            string emailAddressUser = "emailAddressUser";
            string phoneNumberUser = "phoneNumberUser";

            ParameterDTO parameter = new ParameterDTO(
                new Flat
                {
                    ID = flatId,
                    Floor = floor,
                    Number = flatNumber,
                    EntryDoorCode = entryDoorCode
                },
                new User
                {
                    Id = userId,
                    PersonalNumber = personalNumberUser,
                    FirstName = firstNameUser,
                    LastName = lastNameUser,
                    Email = emailAddressUser,
                    PhoneNumber = phoneNumberUser
                }
            );

            List<ParameterDTO> parameters = new List<ParameterDTO> { parameter };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(host);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);
            _addressHostsRepository.Setup(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()))
                                   .Returns(addressHost);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);

            // Act
            var result = _controller.Remove(hostId, addressId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddressHostDetailsVM>(viewResult.Model);

            Assert.Equal(hostId, model.Host.Id);
            Assert.Equal(personalNumber, model.Host.PersonalNumber);
            Assert.Equal(firstName, model.Host.FirstName);
            Assert.Equal(lastName, model.Host.LastName);
            Assert.Equal(emailAddress, model.Host.Email);
            Assert.Equal(phoneNumber, model.Host.PhoneNumber);
            Assert.Equal(phoneNumber2, model.Host.PhoneNumber2);
            Assert.True(model.Host.IsPhoneNumber2Visible);
            Assert.Empty(model.Host.Addresses);

            Assert.Equal(addressId, model.Address.ID);
            Assert.Equal(street, model.Address.Street);
            Assert.Equal(addressNumber, model.Address.Number);
            Assert.Equal(zipCode, model.Address.ZipCode);
            Assert.Equal(town, model.Address.Town);
            Assert.Equal(country, model.Address.Country);
            Assert.Single(model.Address.Flats);
            Assert.Equal(floor, model.Address.Flats.First().Key);
            Assert.Equal(flatId, model.Address.Flats[floor].First().ID);
            Assert.Equal(floor, model.Address.Flats[floor].First().Floor);
            Assert.Equal(flatNumber, model.Address.Flats[floor].First().Number);
            Assert.Equal(entryDoorCode, model.Address.Flats[floor].First().EntryDoorCode);
            Assert.Single(model.Address.Flats[floor].First().Parameters);
            Assert.Equal(userId, model.Address.Flats[floor].First().Parameters.First().User.Id);
            Assert.Equal(personalNumberUser, model.Address.Flats[floor].First().Parameters.First().User.PersonalNumber);
            Assert.Equal(firstNameUser, model.Address.Flats[floor].First().Parameters.First().User.FirstName);
            Assert.Equal(lastNameUser, model.Address.Flats[floor].First().Parameters.First().User.LastName);
            Assert.Equal(emailAddressUser, model.Address.Flats[floor].First().Parameters.First().User.Email);
            Assert.Equal(phoneNumberUser, model.Address.Flats[floor].First().Parameters.First().User.PhoneNumber);
            Assert.Equal(flatId, model.Address.Flats[floor].First().Parameters.First().Flat.ID);
            Assert.Equal(floor, model.Address.Flats[floor].First().Parameters.First().Flat.Floor);
            Assert.Equal(flatNumber, model.Address.Flats[floor].First().Parameters.First().Flat.Number);
            Assert.Equal(entryDoorCode, model.Address.Flats[floor].First().Parameters.First().Flat.EntryDoorCode);
            Assert.Null(model.Address.Hosts);
            Assert.Equal(1, model.Address.AmountFlats);
            Assert.Equal(0, model.Address.AmountAvailableFlats);
            Assert.Equal(1, model.Address.AmountUsers);
            Assert.Equal(0, model.Address.AmountHosts);

            _usersRepository.Verify(u => u.User(hostId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.AddressHost(addressId, hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId), Times.Once);
        }

        #endregion

        #region RemoveConfirmed

        [Fact]
        public void RemoveConfirmed_AddressHostNotFound()
        {
            // Arrange
            string hostId = "someHostId";
            int addressId = 1;

            AddressHostDTO addressHost = null;

            _addressHostsRepository.Setup(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()))
                                   .Returns(addressHost);

            // Act
            var result = _controller.RemoveConfirmed(hostId, addressId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _addressHostsRepository.Verify(ah => ah.AddressHost(addressId, hostId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Delete(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void RemoveConfirmed_AddressHostFound()
        {
            // Arrange
            string hostId = "someHostId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";
            string phoneNumber2 = "phoneNumber2";

            UserDTO user = new UserDTO
            {
                Id = hostId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneNumber2 = phoneNumber2
            };

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            AddressHostDTO addressHost = new AddressHostDTO(address, user);

            _addressHostsRepository.Setup(ah => ah.AddressHost(It.IsAny<int>(), It.IsAny<string>()))
                                   .Returns(addressHost);

            // Act
            var result = _controller.RemoveConfirmed(hostId, addressId);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(AddressesController.Details), viewResult.ActionName);
            Assert.Equal("Addresses", viewResult.ControllerName);
            Assert.Equal(addressId, viewResult.RouteValues.First().Value);

            _addressHostsRepository.Verify(ah => ah.AddressHost(addressId, hostId), Times.Once);
            _addressHostsRepository.Verify(ah => ah.Delete(addressId, hostId), Times.Once);
        }

        #endregion

        #region Addresses

        [Fact]
        public void Addresses_AddressesNull()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            List<AddressDTO> addresses = null;

            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);

            // Act
            var result = _controller.Addresses();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);

            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<int>()), Times.Never);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Addresses_AddressesNoEntry()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            List<AddressDTO> addresses = new List<AddressDTO>();

            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);

            // Act
            var result = _controller.Addresses();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<AddressDetailsVM>>(viewResult.Model);

            Assert.Empty(model);

            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<int>()), Times.Never);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Addresses_AddressesOneEntry_FlatsNull_NegativeAmountAvailableFlats()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address };

            List<FlatDTO> flats = null;

            int amountAvailableFlats = -1;
            int expectedAmountAvailableFlats = 0;

            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _flatsRepository.Setup(f => f.AmountAvailableFlats(It.IsAny<int>()))
                            .Returns(amountAvailableFlats);

            // Act
            var result = _controller.Addresses();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<AddressDetailsVM>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal(addressId, model.First().ID);
            Assert.Equal(street, model.First().Street);
            Assert.Equal(addressNumber, model.First().Number);
            Assert.Equal(zipCode, model.First().ZipCode);
            Assert.Equal(town, model.First().Town);
            Assert.Equal(country, model.First().Country);
            Assert.False(model.First().CanDataBeDeleted);
            Assert.Null(model.First().Flats);
            Assert.Equal(expectedAmountAvailableFlats, model.First().AmountAvailableFlats);
            Assert.Equal(0, model.First().AmountFlats);
            Assert.Equal(0, model.First().AmountUsers);
            Assert.Equal(0, model.First().AmountHosts);

            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(addressId), Times.Once);
        }

        [Fact]
        public void Addresses_AddressesOneEntry_FlatsNoEntry_ZeroAmountAvailableFlats()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address };

            List<FlatDTO> flats = new List<FlatDTO>();

            int amountAvailableFlats = 0;
            int expectedAmountAvailableFlats = amountAvailableFlats;

            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _flatsRepository.Setup(f => f.AmountAvailableFlats(It.IsAny<int>()))
                            .Returns(amountAvailableFlats);

            // Act
            var result = _controller.Addresses();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<AddressDetailsVM>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal(addressId, model.First().ID);
            Assert.Equal(street, model.First().Street);
            Assert.Equal(addressNumber, model.First().Number);
            Assert.Equal(zipCode, model.First().ZipCode);
            Assert.Equal(town, model.First().Town);
            Assert.Equal(country, model.First().Country);
            Assert.False(model.First().CanDataBeDeleted);
            Assert.Empty(model.First().Flats);
            Assert.Equal(expectedAmountAvailableFlats, model.First().AmountAvailableFlats);
            Assert.Equal(0, model.First().AmountFlats);
            Assert.Equal(0, model.First().AmountUsers);
            Assert.Equal(0, model.First().AmountHosts);

            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(addressId), Times.Once);
        }

        [Fact]
        public void Addresses_AddressesOneEntry_FlatsOneEntry_PositiveAmountAvailableFlats()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            int addressId = 1;
            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO address = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address };

            int flatId = 2;
            int floor = 10;
            string flatNumer = "flatNumber";
            string entryDoorCode = "entryDoorCode";
            FlatDTO flat = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumer,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            List<FlatDTO> flats = new List<FlatDTO> { flat };

            int amountAvailableFlats = 5;
            int expectedAmountAvailableFlats = amountAvailableFlats;

            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _flatsRepository.Setup(f => f.AmountAvailableFlats(It.IsAny<int>()))
                            .Returns(amountAvailableFlats);

            // Act
            var result = _controller.Addresses();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<AddressDetailsVM>>(viewResult.Model);

            Assert.Single(model);
            Assert.Equal(addressId, model.First().ID);
            Assert.Equal(street, model.First().Street);
            Assert.Equal(addressNumber, model.First().Number);
            Assert.Equal(zipCode, model.First().ZipCode);
            Assert.Equal(town, model.First().Town);
            Assert.Equal(country, model.First().Country);
            Assert.False(model.First().CanDataBeDeleted);
            Assert.Single(model.First().Flats);
            Assert.Equal(floor, model.First().Flats.First().Key);
            Assert.Equal(flatId, model.First().Flats[floor].First().ID);
            Assert.Equal(floor, model.First().Flats[floor].First().Floor);
            Assert.Equal(flatNumer, model.First().Flats[floor].First().Number);
            Assert.Equal(entryDoorCode, model.First().Flats[floor].First().EntryDoorCode);
            Assert.Equal(expectedAmountAvailableFlats, model.First().AmountAvailableFlats);
            Assert.Equal(1, model.First().AmountFlats);
            Assert.Equal(0, model.First().AmountUsers);
            Assert.Equal(0, model.First().AmountHosts);

            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId), Times.Once);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(addressId), Times.Once);
        }

        [Fact]
        public void Addresses_AddressesSixEntries_FlatsNoEntries_PositiveAmountAvailableFlats()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            int addressId1 = 1;
            string street1 = "street1";
            string addressNumber1 = "addressNumber1";
            string zipCode1 = "zipCode1";
            string town1 = "town1";
            string country1 = "country1";

            AddressDTO address1 = new AddressDTO
            {
                ID = addressId1,
                Street = street1,
                Number = addressNumber1,
                ZipCode = zipCode1,
                Town = town1,
                Country = country1
            };

            int addressId2 = 2;
            string street2 = "street2";
            string addressNumber2 = addressNumber1;
            string zipCode2 = zipCode1;
            string town2 = town1;
            string country2 = country1;

            AddressDTO address2 = new AddressDTO
            {
                ID = addressId2,
                Street = street2,
                Number = addressNumber2,
                ZipCode = zipCode2,
                Town = town2,
                Country = country2
            };

            int addressId3 = 3;
            string street3 = "street3";
            string addressNumber3 = "addressNumber3";
            string zipCode3 = zipCode1;
            string town3 = town1;
            string country3 = country1;

            AddressDTO address3 = new AddressDTO
            {
                ID = addressId3,
                Street = street3,
                Number = addressNumber3,
                ZipCode = zipCode3,
                Town = town3,
                Country = country3
            };

            int addressId4 = 4;
            string street4 = "street4";
            string addressNumber4 = "addressNumber4";
            string zipCode4 = "zipCode4";
            string town4 = town1;
            string country4 = country1;

            AddressDTO address4 = new AddressDTO
            {
                ID = addressId4,
                Street = street4,
                Number = addressNumber4,
                ZipCode = zipCode4,
                Town = town4,
                Country = country4
            };

            int addressId5 = 5;
            string street5 = "street5";
            string addressNumber5 = "addressNumber5";
            string zipCode5 = "zipCode5";
            string town5 = "town5";
            string country5 = country1;

            AddressDTO address5 = new AddressDTO
            {
                ID = addressId5,
                Street = street5,
                Number = addressNumber5,
                ZipCode = zipCode5,
                Town = town5,
                Country = country5
            };

            int addressId6 = 6;
            string street6 = "street6";
            string addressNumber6 = "addressNumber6";
            string zipCode6 = "zipCode6";
            string town6 = "town6";
            string country6 = "country6";

            AddressDTO address6 = new AddressDTO
            {
                ID = addressId6,
                Street = street6,
                Number = addressNumber6,
                ZipCode = zipCode6,
                Town = town6,
                Country = country6
            };

            List<AddressDTO> addresses = new List<AddressDTO> { address6, address5, address4, address3, address2, address1 };

            List<FlatDTO> flats = new List<FlatDTO>();

            int amountAvailableFlats = 5;
            int expectedAmountAvailableFlats = amountAvailableFlats;

            _addressHostsRepository.Setup(ah => ah.Addresses(It.IsAny<string>()))
                                   .Returns(addresses);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<int>()))
                            .Returns(flats);
            _flatsRepository.Setup(f => f.AmountAvailableFlats(It.IsAny<int>()))
                            .Returns(amountAvailableFlats);

            // Act
            var result = _controller.Addresses();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<AddressDetailsVM>>(viewResult.Model);

            Assert.Equal(6, model.Count);
            Assert.Equal(addressId1, model.First().ID);
            Assert.Equal(street1, model.First().Street);
            Assert.Equal(addressNumber1, model.First().Number);
            Assert.Equal(zipCode1, model.First().ZipCode);
            Assert.Equal(town1, model.First().Town);
            Assert.Equal(country1, model.First().Country);
            Assert.False(model.First().CanDataBeDeleted);
            Assert.Empty(model.First().Flats);
            Assert.Equal(expectedAmountAvailableFlats, model.First().AmountAvailableFlats);
            Assert.Equal(0, model.First().AmountFlats);
            Assert.Equal(0, model.First().AmountUsers);
            Assert.Equal(0, model.First().AmountHosts);

            Assert.Equal(addressId2, model[1].ID);
            Assert.Equal(street2, model[1].Street);
            Assert.Equal(addressNumber2, model[1].Number);
            Assert.Equal(zipCode2, model[1].ZipCode);
            Assert.Equal(town2, model[1].Town);
            Assert.Equal(country2, model[1].Country);
            Assert.False(model[1].CanDataBeDeleted);
            Assert.Empty(model[1].Flats);
            Assert.Equal(expectedAmountAvailableFlats, model[1].AmountAvailableFlats);
            Assert.Equal(0, model[1].AmountFlats);
            Assert.Equal(0, model[1].AmountUsers);
            Assert.Equal(0, model[1].AmountHosts);

            Assert.Equal(addressId3, model[2].ID);
            Assert.Equal(street3, model[2].Street);
            Assert.Equal(addressNumber3, model[2].Number);
            Assert.Equal(zipCode3, model[2].ZipCode);
            Assert.Equal(town3, model[2].Town);
            Assert.Equal(country3, model[2].Country);
            Assert.False(model[2].CanDataBeDeleted);
            Assert.Empty(model[2].Flats);
            Assert.Equal(expectedAmountAvailableFlats, model[2].AmountAvailableFlats);
            Assert.Equal(0, model[2].AmountFlats);
            Assert.Equal(0, model[2].AmountUsers);
            Assert.Equal(0, model[2].AmountHosts);

            Assert.Equal(addressId4, model[3].ID);
            Assert.Equal(street4, model[3].Street);
            Assert.Equal(addressNumber4, model[3].Number);
            Assert.Equal(zipCode4, model[3].ZipCode);
            Assert.Equal(town4, model[3].Town);
            Assert.Equal(country4, model[3].Country);
            Assert.False(model[3].CanDataBeDeleted);
            Assert.Empty(model[3].Flats);
            Assert.Equal(expectedAmountAvailableFlats, model[3].AmountAvailableFlats);
            Assert.Equal(0, model[3].AmountFlats);
            Assert.Equal(0, model[3].AmountUsers);
            Assert.Equal(0, model[3].AmountHosts);

            Assert.Equal(addressId5, model[4].ID);
            Assert.Equal(street5, model[4].Street);
            Assert.Equal(addressNumber5, model[4].Number);
            Assert.Equal(zipCode5, model[4].ZipCode);
            Assert.Equal(town5, model[4].Town);
            Assert.Equal(country5, model[4].Country);
            Assert.False(model[4].CanDataBeDeleted);
            Assert.Empty(model[4].Flats);
            Assert.Equal(expectedAmountAvailableFlats, model[4].AmountAvailableFlats);
            Assert.Equal(0, model[4].AmountFlats);
            Assert.Equal(0, model[4].AmountUsers);
            Assert.Equal(0, model[4].AmountHosts);

            Assert.Equal(addressId6, model.Last().ID);
            Assert.Equal(street6, model.Last().Street);
            Assert.Equal(addressNumber6, model.Last().Number);
            Assert.Equal(zipCode6, model.Last().ZipCode);
            Assert.Equal(town6, model.Last().Town);
            Assert.Equal(country6, model.Last().Country);
            Assert.False(model.Last().CanDataBeDeleted);
            Assert.Empty(model.Last().Flats);
            Assert.Equal(expectedAmountAvailableFlats, model.Last().AmountAvailableFlats);
            Assert.Equal(0, model.Last().AmountFlats);
            Assert.Equal(0, model.Last().AmountUsers);
            Assert.Equal(0, model.Last().AmountHosts);

            _addressHostsRepository.Verify(ah => ah.Addresses(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId6), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId5), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId4), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId3), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId2), Times.Once);
            _flatsRepository.Verify(f => f.Flats(addressId1), Times.Once);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(addressId6), Times.Once);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(addressId5), Times.Once);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(addressId4), Times.Once);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(addressId3), Times.Once);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(addressId2), Times.Once);
            _flatsRepository.Verify(f => f.AmountAvailableFlats(addressId1), Times.Once);
        }

        #endregion

        #region GetHostsAtAddress

        [Fact]
        public void GetHostsAtAddress_HostsAtAddressNull()
        {
            // Arrange
            int addressId = 1;

            List<UserDTO> hosts = null;

            _usersRepository.Setup(u => u.HostsAtAddress(It.IsAny<int>()))
                            .Returns(hosts);

            // Act
            var result = _controller.GetHostsAtAddress(addressId);

            // Arrange
            Assert.Null(result);
        }

        [Fact]
        public void GetHostsAtAddress_HostsAtAddressNoEntries()
        {
            // Arrange
            int addressId = 1;

            List<UserDTO> hosts = new List<UserDTO>();

            _usersRepository.Setup(u => u.HostsAtAddress(It.IsAny<int>()))
                            .Returns(hosts);

            // Act
            var result = _controller.GetHostsAtAddress(addressId);

            // Arrange
            Assert.Empty(result);
        }

        [Fact]
        public void GetHostsAtAddress_HostsAtAddressOneEntry()
        {
            // Arrange
            int addressId = 1;

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

            string firstLetter = lastName.Substring(0, 1);

            _usersRepository.Setup(u => u.HostsAtAddress(It.IsAny<int>()))
                            .Returns(hosts);

            // Act
            var result = _controller.GetHostsAtAddress(addressId);

            // Arrange
            Assert.Single(result);
            Assert.Equal(firstLetter, result.First().Key);
            Assert.Equal(hostId, result[firstLetter].First().Id);
            Assert.Equal(personalNumber, result[firstLetter].First().PersonalNumber);
            Assert.Equal(firstName, result[firstLetter].First().FirstName);
            Assert.Equal(lastName, result[firstLetter].First().LastName);
            Assert.Equal(emailAddress, result[firstLetter].First().Email);
            Assert.Equal(phoneNumber, result[firstLetter].First().PhoneNumber);
            Assert.Equal(phoneNumber2, result[firstLetter].First().PhoneNumber2);
        }

        [Fact]
        public void GetHostsAtAddress_HostsAtAddressThreeEntries()
        {
            // Arrange
            int addressId = 1;

            string hostId1 = "someHostId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";
            string phoneNumber21 = "phoneNumber21";

            UserDTO host1 = new UserDTO
            {
                Id = hostId1,
                PersonalNumber = personalNumber1,
                FirstName = firstName1,
                LastName = lastName1,
                Email = emailAddress1,
                PhoneNumber = phoneNumber1,
                PhoneNumber2 = phoneNumber21
            };

            string hostId2 = "someHostId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = lastName1;
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";
            string phoneNumber22 = "phoneNumber22";

            UserDTO host2 = new UserDTO
            {
                Id = hostId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2,
                PhoneNumber2 = phoneNumber22
            };

            string hostId3 = "someHostId3";
            string personalNumber3 = "personalNumber3";
            string firstName3 = "firstName3";
            string lastName3 = "someOtherLastName";
            string emailAddress3 = "emailAddress3";
            string phoneNumber3 = "phoneNumber3";
            string phoneNumber23 = "phoneNumber23";

            UserDTO host3 = new UserDTO
            {
                Id = hostId3,
                PersonalNumber = personalNumber3,
                FirstName = firstName3,
                LastName = lastName3,
                Email = emailAddress3,
                PhoneNumber = phoneNumber3,
                PhoneNumber2 = phoneNumber23
            };

            List<UserDTO> hosts = new List<UserDTO> { host3, host2, host1 };

            string firstLetter1 = lastName1.Substring(0, 1);
            string firstLetter2 = lastName3.Substring(0, 1);

            _usersRepository.Setup(u => u.HostsAtAddress(It.IsAny<int>()))
                            .Returns(hosts);

            // Act
            var result = _controller.GetHostsAtAddress(addressId);

            // Arrange
            Assert.Equal(2, result.Count());
            Assert.Equal(firstLetter1, result.First().Key);
            Assert.Equal(2, result[firstLetter1].Count());
            Assert.Equal(hostId1, result[firstLetter1].First().Id);
            Assert.Equal(personalNumber1, result[firstLetter1].First().PersonalNumber);
            Assert.Equal(firstName1, result[firstLetter1].First().FirstName);
            Assert.Equal(lastName1, result[firstLetter1].First().LastName);
            Assert.Equal(emailAddress1, result[firstLetter1].First().Email);
            Assert.Equal(phoneNumber1, result[firstLetter1].First().PhoneNumber);
            Assert.Equal(phoneNumber21, result[firstLetter1].First().PhoneNumber2);

            Assert.Equal(hostId2, result[firstLetter1].Last().Id);
            Assert.Equal(personalNumber2, result[firstLetter1].Last().PersonalNumber);
            Assert.Equal(firstName2, result[firstLetter1].Last().FirstName);
            Assert.Equal(lastName2, result[firstLetter1].Last().LastName);
            Assert.Equal(emailAddress2, result[firstLetter1].Last().Email);
            Assert.Equal(phoneNumber2, result[firstLetter1].Last().PhoneNumber);
            Assert.Equal(phoneNumber22, result[firstLetter1].Last().PhoneNumber2);

            Assert.Equal(firstLetter2, result.Last().Key);
            Assert.Equal(hostId3, result[firstLetter2].First().Id);
            Assert.Equal(personalNumber3, result[firstLetter2].First().PersonalNumber);
            Assert.Equal(firstName3, result[firstLetter2].First().FirstName);
            Assert.Equal(lastName3, result[firstLetter2].First().LastName);
            Assert.Equal(emailAddress3, result[firstLetter2].First().Email);
            Assert.Equal(phoneNumber3, result[firstLetter2].First().PhoneNumber);
            Assert.Equal(phoneNumber23, result[firstLetter2].First().PhoneNumber2);
        }

        #endregion

        #region SaveErrors

        [Fact]
        public void SaveErrors_Null()
        {
            // Arrange
            ErrorReportDTO errorReport = null;

            // Act
            _controller.SaveErrors(errorReport);

            _errorReportsRepository.Setup(er => er.Edit(It.IsAny<ErrorReportDTO>()))
                                   .Returns(0);

            // Assert
            _errorReportsRepository.Verify(er => er.Edit(null), Times.Once);
        }

        [Fact]
        public void SaveErrors_NotNull()
        {
            // Arrange
            int errorId = 1;
            DateTime seen = DateTime.Now.AddMinutes(30);
            int flatId = 12;
            string description = "someDescription";
            string subject = "someSubject";
            DateTime submitted = DateTime.Now;
            Status currentStatus = Status.irrelevant;
            Priority currentPriority = Priority.low;
            List<CommentDTO> comments = new List<CommentDTO> { new CommentDTO(new Comment()) };

            ErrorReportDTO errorReport = new ErrorReportDTO
            {
                Id = errorId,
                Seen = seen,
                Subject = subject,
                Description = description,
                Submitted = submitted,
                CurrentStatus = currentStatus,
                CurrentPriority = currentPriority,
                Comments = comments,
                FlatId = flatId
            };

            // Act
            _controller.SaveErrors(errorReport);

            _errorReportsRepository.Setup(er => er.Edit(It.IsAny<ErrorReportDTO>()))
                                   .Returns(0);

            // Assert
            _errorReportsRepository.Verify(er => er.Edit(errorReport), Times.Once);
        }

        #endregion

        #region GetErrors

        [Fact]
        public void GetErrors_Null()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            List<ErrorReportDTO> errors = null;

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errors);

            // Act
            var result = _controller.GetErrors();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetErrors_NoEntries()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            List<ErrorReportDTO> errors = new List<ErrorReportDTO>();

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errors);

            // Act
            var result = _controller.GetErrors();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetErrors_OneEntry_FlatNull()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            CommentDTO comment = new CommentDTO(new Comment());

            int flatId = 12;
            FlatDTO flatDTO = null;

            int errorId = 1;
            DateTime seen = DateTime.Now.AddMinutes(30);
            string description = "someDescription";
            string subject = "someSubject";
            DateTime submitted = DateTime.Now;
            Status currentStatus = Status.irrelevant;
            Priority currentPriority = Priority.low;
            List<CommentDTO> comments = new List<CommentDTO> { comment };

            ErrorReportDTO errorReport = new ErrorReportDTO
            {
                Id = errorId,
                Seen = seen,
                Subject = subject,
                Description = description,
                Submitted = submitted,
                CurrentStatus = currentStatus,
                CurrentPriority = currentPriority,
                Comments = comments,
                FlatId = flatId,
                Flat = flatDTO
            };

            List<ErrorReportDTO> errors = new List<ErrorReportDTO> { errorReport };

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errors);

            // Act
            var result = _controller.GetErrors();

            // Assert
            Assert.Single(result);
            Assert.Equal(errorId, result.First().Value.ErrorReports.First().Value.Id);
            Assert.Equal(seen, result.First().Value.ErrorReports.First().Value.Seen);
            Assert.Equal(subject, result.First().Value.ErrorReports.First().Value.Subject);
            Assert.Equal(description, result.First().Value.ErrorReports.First().Value.Description);
            Assert.Equal(submitted, result.First().Value.ErrorReports.First().Value.Submitted);
            Assert.Equal(currentStatus, result.First().Value.ErrorReports.First().Value.CurrentStatus);
            Assert.Equal(currentPriority, result.First().Value.ErrorReports.First().Value.CurrentPriority);
            Assert.Single(result.First().Value.ErrorReports.First().Value.Comments);
            Assert.Equal(comment, result.First().Value.ErrorReports.First().Value.Comments.First());
            Assert.Equal(flatId, result.First().Value.ErrorReports.First().Value.FlatId);
        }

        [Fact]
        public void GetErrors_OneEntry_FlatNotNull()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            CommentDTO comment = new CommentDTO(new Comment());

            int flatId = 12;
            int addressId = 1;
            int floor = 20;
            string flatNumber = "flatNumber";
            string entryDoorCode = "entryDoorCode";

            FlatDTO flatDTO = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            int errorId = 1;
            DateTime seen = DateTime.Now.AddMinutes(30);
            string description = "someDescription";
            string subject = "someSubject";
            DateTime submitted = DateTime.Now;
            Status currentStatus = Status.irrelevant;
            Priority currentPriority = Priority.low;
            List<CommentDTO> comments = new List<CommentDTO> { comment };

            ErrorReportDTO errorReport = new ErrorReportDTO
            {
                Id = errorId,
                Seen = seen,
                Subject = subject,
                Description = description,
                Submitted = submitted,
                CurrentStatus = currentStatus,
                CurrentPriority = currentPriority,
                Comments = comments,
                FlatId = flatId
            };

            List<ErrorReportDTO> errors = new List<ErrorReportDTO> { errorReport };

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errors);
            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>()))
                            .Returns(flatDTO);

            // Act
            var result = _controller.GetErrors();

            // Assert
            Assert.Single(result);
            Assert.Equal(errorId, result.First().Value.ErrorReports.First().Value.Id);
            Assert.Equal(seen, result.First().Value.ErrorReports.First().Value.Seen);
            Assert.Equal(subject, result.First().Value.ErrorReports.First().Value.Subject);
            Assert.Equal(description, result.First().Value.ErrorReports.First().Value.Description);
            Assert.Equal(submitted, result.First().Value.ErrorReports.First().Value.Submitted);
            Assert.Equal(currentStatus, result.First().Value.ErrorReports.First().Value.CurrentStatus);
            Assert.Equal(currentPriority, result.First().Value.ErrorReports.First().Value.CurrentPriority);
            Assert.Single(result.First().Value.ErrorReports.First().Value.Comments);
            Assert.Equal(comment, result.First().Value.ErrorReports.First().Value.Comments.First());
            Assert.Equal(flatId, result.First().Value.ErrorReports.First().Value.FlatId);

            _errorReportsRepository.Verify(er => er.ErrorReports(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
        }

        [Fact]
        public void GetErrors_TwoEntries_Flat1Null_Flat2Null()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            CommentDTO comment1 = new CommentDTO(new Comment());
            CommentDTO comment2 = new CommentDTO(new Comment());

            int flatId1 = 12;
            FlatDTO flatDTO1 = null;

            int errorId1 = 1;
            DateTime seen1 = DateTime.Now.AddMinutes(30);
            string description1 = "someDescription1";
            string subject1 = "someSubject1";
            DateTime submitted1 = DateTime.Now;
            Status currentStatus1 = Status.irrelevant;
            Priority currentPriority1 = Priority.low;
            List<CommentDTO> comments1 = new List<CommentDTO> { comment1 };

            ErrorReportDTO errorReport1 = new ErrorReportDTO
            {
                Id = errorId1,
                Seen = seen1,
                Subject = subject1,
                Description = description1,
                Submitted = submitted1,
                CurrentStatus = currentStatus1,
                CurrentPriority = currentPriority1,
                Comments = comments1,
                FlatId = flatId1,
                Flat = flatDTO1
            };

            int flatId2 = 22;
            FlatDTO flatDTO2 = null;

            int errorId2 = 2;
            DateTime? seen2 = null;
            string description2 = "someDescription2";
            string subject2 = "someSubject2";
            DateTime submitted2 = DateTime.Now.AddMinutes(-30);
            Status currentStatus2 = Status.finished;
            Priority currentPriority2 = Priority.medium;
            List<CommentDTO> comments2 = new List<CommentDTO> { comment2 };

            ErrorReportDTO errorReport2 = new ErrorReportDTO
            {
                Id = errorId2,
                Seen = seen2,
                Subject = subject2,
                Description = description2,
                Submitted = submitted2,
                CurrentStatus = currentStatus2,
                CurrentPriority = currentPriority2,
                Comments = comments2,
                FlatId = flatId2,
                Flat = flatDTO2
            };

            List<ErrorReportDTO> errors = new List<ErrorReportDTO> { errorReport1, errorReport2 };

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errors);
            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>()))
                            .Returns<int?>(flatId => { return flatId == flatId1 ? flatDTO1 : flatDTO2; });

            // Act
            var result = _controller.GetErrors();

            // Assert
            Assert.Single(result);
            Assert.Equal(errorId1, result.First().Value.ErrorReports.First().Value.Id);
            Assert.Equal(seen1, result.First().Value.ErrorReports.First().Value.Seen);
            Assert.Equal(subject1, result.First().Value.ErrorReports.First().Value.Subject);
            Assert.Equal(description1, result.First().Value.ErrorReports.First().Value.Description);
            Assert.Equal(submitted1, result.First().Value.ErrorReports.First().Value.Submitted);
            Assert.Equal(currentStatus1, result.First().Value.ErrorReports.First().Value.CurrentStatus);
            Assert.Equal(currentPriority1, result.First().Value.ErrorReports.First().Value.CurrentPriority);
            Assert.Single(result.First().Value.ErrorReports.First().Value.Comments);
            Assert.Equal(comment1, result.First().Value.ErrorReports.First().Value.Comments.First());
            Assert.Equal(flatId1, result.First().Value.ErrorReports.First().Value.FlatId);

            Assert.Equal(errorId2, result.First().Value.ErrorReports.Last().Value.Id);
            Assert.Equal(seen2, result.First().Value.ErrorReports.Last().Value.Seen);
            Assert.Equal(subject2, result.First().Value.ErrorReports.Last().Value.Subject);
            Assert.Equal(description2, result.First().Value.ErrorReports.Last().Value.Description);
            Assert.Equal(submitted2, result.First().Value.ErrorReports.Last().Value.Submitted);
            Assert.Equal(currentStatus2, result.First().Value.ErrorReports.Last().Value.CurrentStatus);
            Assert.Equal(currentPriority2, result.First().Value.ErrorReports.Last().Value.CurrentPriority);
            Assert.Single(result.First().Value.ErrorReports.Last().Value.Comments);
            Assert.Equal(comment2, result.First().Value.ErrorReports.Last().Value.Comments.First());
            Assert.Equal(flatId2, result.First().Value.ErrorReports.Last().Value.FlatId);

            _errorReportsRepository.Verify(er => er.ErrorReports(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId1), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId2), Times.Once);
        }

        [Fact]
        public void GetErrors_TwoEntries_Flat1NotNull_Flat2NotNull_Address1Null_Address2Null_SameAddressId()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            CommentDTO comment1 = new CommentDTO(new Comment());
            CommentDTO comment2 = new CommentDTO(new Comment());

            int addressId = 1;
            AddressDTO addressDTO = null;

            int flatId1 = 12;
            int floor1 = 20;
            string flatNumber1 = "flatNumber1";
            string entryDoorCode1 = "entryDoorCode1";

            FlatDTO flatDTO1 = new FlatDTO
            {
                ID = flatId1,
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId
            };

            int errorId1 = 1;
            DateTime seen1 = DateTime.Now.AddMinutes(30);
            string description1 = "someDescription1";
            string subject1 = "someSubject1";
            DateTime submitted1 = DateTime.Now;
            Status currentStatus1 = Status.irrelevant;
            Priority currentPriority1 = Priority.low;
            List<CommentDTO> comments1 = new List<CommentDTO> { comment1 };

            ErrorReportDTO errorReport1 = new ErrorReportDTO
            {
                Id = errorId1,
                Seen = seen1,
                Subject = subject1,
                Description = description1,
                Submitted = submitted1,
                CurrentStatus = currentStatus1,
                CurrentPriority = currentPriority1,
                Comments = comments1,
                FlatId = flatId1
            };

            int flatId2 = 22;
            int floor2 = 20;
            string flatNumber2 = "flatNumber2";
            string entryDoorCode2 = "entryDoorCode2";

            FlatDTO flatDTO2 = new FlatDTO
            {
                ID = flatId2,
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId
            };

            int errorId2 = 2;
            DateTime? seen2 = null;
            string description2 = "someDescription2";
            string subject2 = "someSubject2";
            DateTime submitted2 = DateTime.Now.AddMinutes(-30);
            Status currentStatus2 = Status.finished;
            Priority currentPriority2 = Priority.medium;
            List<CommentDTO> comments2 = new List<CommentDTO> { comment2 };

            ErrorReportDTO errorReport2 = new ErrorReportDTO
            {
                Id = errorId2,
                Seen = seen2,
                Subject = subject2,
                Description = description2,
                Submitted = submitted2,
                CurrentStatus = currentStatus2,
                CurrentPriority = currentPriority2,
                Comments = comments2,
                FlatId = flatId2
            };

            List<ErrorReportDTO> errors = new List<ErrorReportDTO> { errorReport1, errorReport2 };

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errors);
            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>()))
                            .Returns<int?>(flatId => { return flatId == flatId1 ? flatDTO1 : flatDTO2; });
            _addressesRepository.Setup(a => a.Address(It.IsAny<int?>()))
                                .Returns(addressDTO);

            // Act
            var result = _controller.GetErrors();

            // Assert
            Assert.Single(result);
            Assert.Equal(errorId1, result.First().Value.ErrorReports.First().Value.Id);
            Assert.Equal(seen1, result.First().Value.ErrorReports.First().Value.Seen);
            Assert.Equal(subject1, result.First().Value.ErrorReports.First().Value.Subject);
            Assert.Equal(description1, result.First().Value.ErrorReports.First().Value.Description);
            Assert.Equal(submitted1, result.First().Value.ErrorReports.First().Value.Submitted);
            Assert.Equal(currentStatus1, result.First().Value.ErrorReports.First().Value.CurrentStatus);
            Assert.Equal(currentPriority1, result.First().Value.ErrorReports.First().Value.CurrentPriority);
            Assert.Single(result.First().Value.ErrorReports.First().Value.Comments);
            Assert.Equal(comment1, result.First().Value.ErrorReports.First().Value.Comments.First());
            Assert.Equal(flatId1, result.First().Value.ErrorReports.First().Value.FlatId);

            Assert.Equal(errorId2, result.First().Value.ErrorReports.Last().Value.Id);
            Assert.Equal(seen2, result.First().Value.ErrorReports.Last().Value.Seen);
            Assert.Equal(subject2, result.First().Value.ErrorReports.Last().Value.Subject);
            Assert.Equal(description2, result.First().Value.ErrorReports.Last().Value.Description);
            Assert.Equal(submitted2, result.First().Value.ErrorReports.Last().Value.Submitted);
            Assert.Equal(currentStatus2, result.First().Value.ErrorReports.Last().Value.CurrentStatus);
            Assert.Equal(currentPriority2, result.First().Value.ErrorReports.Last().Value.CurrentPriority);
            Assert.Single(result.First().Value.ErrorReports.Last().Value.Comments);
            Assert.Equal(comment2, result.First().Value.ErrorReports.Last().Value.Comments.First());
            Assert.Equal(flatId2, result.First().Value.ErrorReports.Last().Value.FlatId);

            _errorReportsRepository.Verify(er => er.ErrorReports(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId1), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId2), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Exactly(2));
        }

        [Fact]
        public void GetErrors_TwoEntries_Flat1NotNull_Flat2NotNull_AddressesNotNull_SameAddress()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            CommentDTO comment1 = new CommentDTO(new Comment());
            CommentDTO comment2 = new CommentDTO(new Comment());

            int addressId1 = 1;
            string street1 = "street1";
            string addressNumber1 = "addressNumber1";
            string zipCode1 = "zipCode1";
            string town1 = "town1";
            string country1 = "country1";

            AddressDTO addressDTO1 = new AddressDTO
            {
                ID = addressId1,
                Street = street1,
                Number = addressNumber1,
                ZipCode = zipCode1,
                Town = town1,
                Country = country1
            };

            int flatId1 = 12;
            int floor1 = 20;
            string flatNumber1 = "flatNumber1";
            string entryDoorCode1 = "entryDoorCode1";

            FlatDTO flatDTO1 = new FlatDTO
            {
                ID = flatId1,
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId1
            };

            int errorId1 = 1;
            DateTime seen1 = DateTime.Now.AddMinutes(30);
            string description1 = "someDescription1";
            string subject1 = "someSubject1";
            DateTime submitted1 = DateTime.Now;
            Status currentStatus1 = Status.irrelevant;
            Priority currentPriority1 = Priority.low;
            List<CommentDTO> comments1 = new List<CommentDTO> { comment1 };

            ErrorReportDTO errorReport1 = new ErrorReportDTO
            {
                Id = errorId1,
                Seen = seen1,
                Subject = subject1,
                Description = description1,
                Submitted = submitted1,
                CurrentStatus = currentStatus1,
                CurrentPriority = currentPriority1,
                Comments = comments1,
                FlatId = flatId1
            };

            int addressId2 = addressId1;
            AddressDTO addressDTO2 = addressDTO1;

            int flatId2 = 22;
            int floor2 = 20;
            string flatNumber2 = "flatNumber2";
            string entryDoorCode2 = "entryDoorCode2";

            FlatDTO flatDTO2 = new FlatDTO
            {
                ID = flatId2,
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId2
            };

            int errorId2 = 2;
            DateTime? seen2 = null;
            string description2 = "someDescription2";
            string subject2 = "someSubject2";
            DateTime submitted2 = DateTime.Now.AddMinutes(-30);
            Status currentStatus2 = Status.finished;
            Priority currentPriority2 = Priority.medium;
            List<CommentDTO> comments2 = new List<CommentDTO> { comment2 };

            ErrorReportDTO errorReport2 = new ErrorReportDTO
            {
                Id = errorId2,
                Seen = seen2,
                Subject = subject2,
                Description = description2,
                Submitted = submitted2,
                CurrentStatus = currentStatus2,
                CurrentPriority = currentPriority2,
                Comments = comments2,
                FlatId = flatId2
            };

            List<ErrorReportDTO> errors = new List<ErrorReportDTO> { errorReport1, errorReport2 };

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errors);
            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>()))
                            .Returns<int?>(flatId => { return flatId == flatId1 ? flatDTO1 : flatDTO2; });
            _addressesRepository.Setup(a => a.Address(It.IsAny<int?>()))
                                .Returns<int?>(addressId => { return addressId == addressId1 ? addressDTO1 : addressDTO2; });

            // Act
            var result = _controller.GetErrors();

            // Assert
            Assert.Single(result);
            Assert.Equal(errorId1, result.First().Value.ErrorReports.First().Value.Id);
            Assert.Equal(seen1, result.First().Value.ErrorReports.First().Value.Seen);
            Assert.Equal(subject1, result.First().Value.ErrorReports.First().Value.Subject);
            Assert.Equal(description1, result.First().Value.ErrorReports.First().Value.Description);
            Assert.Equal(submitted1, result.First().Value.ErrorReports.First().Value.Submitted);
            Assert.Equal(currentStatus1, result.First().Value.ErrorReports.First().Value.CurrentStatus);
            Assert.Equal(currentPriority1, result.First().Value.ErrorReports.First().Value.CurrentPriority);
            Assert.Single(result.First().Value.ErrorReports.First().Value.Comments);
            Assert.Equal(comment1, result.First().Value.ErrorReports.First().Value.Comments.First());
            Assert.Equal(flatId1, result.First().Value.ErrorReports.First().Value.FlatId);

            Assert.Equal(errorId2, result.First().Value.ErrorReports.Last().Value.Id);
            Assert.Equal(seen2, result.First().Value.ErrorReports.Last().Value.Seen);
            Assert.Equal(subject2, result.First().Value.ErrorReports.Last().Value.Subject);
            Assert.Equal(description2, result.First().Value.ErrorReports.Last().Value.Description);
            Assert.Equal(submitted2, result.First().Value.ErrorReports.Last().Value.Submitted);
            Assert.Equal(currentStatus2, result.First().Value.ErrorReports.Last().Value.CurrentStatus);
            Assert.Equal(currentPriority2, result.First().Value.ErrorReports.Last().Value.CurrentPriority);
            Assert.Single(result.First().Value.ErrorReports.Last().Value.Comments);
            Assert.Equal(comment2, result.First().Value.ErrorReports.Last().Value.Comments.First());
            Assert.Equal(flatId2, result.First().Value.ErrorReports.Last().Value.FlatId);

            _errorReportsRepository.Verify(er => er.ErrorReports(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId1), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId2), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId1), Times.Exactly(2));
        }

        [Fact]
        public void GetErrors_TwoEntries_Flat1NotNull_Flat2NotNull_AddressesNotNull_DifferentAddresses()
        {
            // Arrange
            string hostId = "someHostId";
            var host = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Host"),
                new Claim(ClaimTypes.NameIdentifier, hostId)
            }, "Host"));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = host
                }
            };

            _controller.ControllerContext = context;

            CommentDTO comment1 = new CommentDTO(new Comment());
            CommentDTO comment2 = new CommentDTO(new Comment());

            int addressId1 = 1;
            string street1 = "street1";
            string addressNumber1 = "addressNumber1";
            string zipCode1 = "zipCode1";
            string town1 = "town1";
            string country1 = "country1";

            AddressDTO addressDTO1 = new AddressDTO
            {
                ID = addressId1,
                Street = street1,
                Number = addressNumber1,
                ZipCode = zipCode1,
                Town = town1,
                Country = country1
            };

            int flatId1 = 12;
            int floor1 = 20;
            string flatNumber1 = "flatNumber1";
            string entryDoorCode1 = "entryDoorCode1";

            FlatDTO flatDTO1 = new FlatDTO
            {
                ID = flatId1,
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId1
            };

            int errorId1 = 1;
            DateTime seen1 = DateTime.Now.AddMinutes(30);
            string description1 = "someDescription1";
            string subject1 = "someSubject1";
            DateTime submitted1 = DateTime.Now;
            Status currentStatus1 = Status.irrelevant;
            Priority currentPriority1 = Priority.low;
            List<CommentDTO> comments1 = new List<CommentDTO> { comment1 };

            ErrorReportDTO errorReport1 = new ErrorReportDTO
            {
                Id = errorId1,
                Seen = seen1,
                Subject = subject1,
                Description = description1,
                Submitted = submitted1,
                CurrentStatus = currentStatus1,
                CurrentPriority = currentPriority1,
                Comments = comments1,
                FlatId = flatId1
            };

            int addressId2 = 2;
            string street2 = "street2";
            string addressNumber2 = "addressNumber2";
            string zipCode2 = "zipCode2";
            string town2 = "town2";
            string country2 = "country2";

            AddressDTO addressDTO2 = new AddressDTO
            {
                ID = addressId2,
                Street = street2,
                Number = addressNumber2,
                ZipCode = zipCode2,
                Town = town2,
                Country = country2
            };

            int flatId2 = 22;
            int floor2 = 20;
            string flatNumber2 = "flatNumber2";
            string entryDoorCode2 = "entryDoorCode2";

            FlatDTO flatDTO2 = new FlatDTO
            {
                ID = flatId2,
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId2
            };

            int errorId2 = 2;
            DateTime? seen2 = null;
            string description2 = "someDescription2";
            string subject2 = "someSubject2";
            DateTime submitted2 = DateTime.Now.AddMinutes(-30);
            Status currentStatus2 = Status.finished;
            Priority currentPriority2 = Priority.medium;
            List<CommentDTO> comments2 = new List<CommentDTO> { comment2 };

            ErrorReportDTO errorReport2 = new ErrorReportDTO
            {
                Id = errorId2,
                Seen = seen2,
                Subject = subject2,
                Description = description2,
                Submitted = submitted2,
                CurrentStatus = currentStatus2,
                CurrentPriority = currentPriority2,
                Comments = comments2,
                FlatId = flatId2
            };

            List<ErrorReportDTO> errors = new List<ErrorReportDTO> { errorReport1, errorReport2 };

            _errorReportsRepository.Setup(er => er.ErrorReports(It.IsAny<string>()))
                                   .Returns(errors);
            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>()))
                            .Returns<int?>(flatId => { return flatId == flatId1 ? flatDTO1 : flatDTO2; });
            _addressesRepository.Setup(a => a.Address(It.IsAny<int?>()))
                                .Returns<int?>(addressId => { return addressId == addressId1 ? addressDTO1 : addressDTO2; });

            // Act
            var result = _controller.GetErrors();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(errorId1, result.First().Value.ErrorReports.First().Value.Id);
            Assert.Equal(seen1, result.First().Value.ErrorReports.First().Value.Seen);
            Assert.Equal(subject1, result.First().Value.ErrorReports.First().Value.Subject);
            Assert.Equal(description1, result.First().Value.ErrorReports.First().Value.Description);
            Assert.Equal(submitted1, result.First().Value.ErrorReports.First().Value.Submitted);
            Assert.Equal(currentStatus1, result.First().Value.ErrorReports.First().Value.CurrentStatus);
            Assert.Equal(currentPriority1, result.First().Value.ErrorReports.First().Value.CurrentPriority);
            Assert.Single(result.First().Value.ErrorReports.First().Value.Comments);
            Assert.Equal(comment1, result.First().Value.ErrorReports.First().Value.Comments.First());
            Assert.Equal(flatId1, result.First().Value.ErrorReports.First().Value.FlatId);

            Assert.Equal(errorId2, result.Last().Value.ErrorReports.First().Value.Id);
            Assert.Equal(seen2, result.Last().Value.ErrorReports.First().Value.Seen);
            Assert.Equal(subject2, result.Last().Value.ErrorReports.First().Value.Subject);
            Assert.Equal(description2, result.Last().Value.ErrorReports.First().Value.Description);
            Assert.Equal(submitted2, result.Last().Value.ErrorReports.First().Value.Submitted);
            Assert.Equal(currentStatus2, result.Last().Value.ErrorReports.First().Value.CurrentStatus);
            Assert.Equal(currentPriority2, result.Last().Value.ErrorReports.First().Value.CurrentPriority);
            Assert.Single(result.Last().Value.ErrorReports.First().Value.Comments);
            Assert.Equal(comment2, result.Last().Value.ErrorReports.First().Value.Comments.First());
            Assert.Equal(flatId2, result.Last().Value.ErrorReports.First().Value.FlatId);

            _errorReportsRepository.Verify(er => er.ErrorReports(hostId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId1), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId2), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId1), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId2), Times.Once);
        }

        #endregion
    }
}
