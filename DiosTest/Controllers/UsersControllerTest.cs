using Dios.Controllers;
using Dios.Models;
using Dios.Repositories;
using Dios.Services;
using Dios.ViewModels;
using Dios.ViewModels.ErrorReports;
using Dios.ViewModels.UsersViewModels;
using Microsoft.AspNetCore.Http;
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
    public class UsersControllerTest
    {
        private readonly Mock<IRequestUserProvider> _requestUserProvider;
        private readonly Mock<IEmailSender> _emailSender;
        private readonly Mock<IUsersRepository> _usersRepository;
        private readonly Mock<IAddressesRepository> _addressesRepository;
        private readonly Mock<IFlatsRepository> _flatsRepository;
        private readonly Mock<IParametersRepository> _parameterRepository;
        private readonly Mock<IErrorReportsRepository> _errorReportsRepository;
        private readonly Mock<INewUser> _newUser;

        private readonly UsersController _controller;

        public UsersControllerTest()
        {
            _requestUserProvider = new Mock<IRequestUserProvider>();
            _emailSender = new Mock<IEmailSender>();
            _usersRepository = new Mock<IUsersRepository>();
            _addressesRepository = new Mock<IAddressesRepository>();
            _flatsRepository = new Mock<IFlatsRepository>();
            _parameterRepository = new Mock<IParametersRepository>();
            _errorReportsRepository = new Mock<IErrorReportsRepository>();
            _newUser = new Mock<INewUser>();

            _controller = new UsersController(_requestUserProvider.Object,
                                              _emailSender.Object,
                                              _usersRepository.Object,
                                              _addressesRepository.Object,
                                              _flatsRepository.Object,
                                              _parameterRepository.Object,
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

        #region ErrorReport

        #region ErrorReport - Get

        [Fact]
        public void ErrorReportGet_FlatNotFound()
        {
            // Arrange
            int flatId = 1;
            FlatDTO flatDTO = null;

            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>())).Returns(flatDTO);

            // Act
            var result = _controller.ErrorReport(flatId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);

            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
        }

        [Fact]
        public void ErrorReportGet_FlatFound_AddressNotFound()
        {
            // Arrange
            int flatId = 1;
            int floor = 20;
            string flatNumber = "flatNumber";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            FlatDTO flatDTO = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            AddressDTO addressDTO = null;

            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>())).Returns(flatDTO);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int?>())).Returns(addressDTO);

            // Act
            var result = _controller.ErrorReport(flatId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorReportCreateVM>(viewResult.Model);

            Assert.Equal(flatId, model.FlatId);
            Assert.Equal(flatNumber.ToString(), model.Flat);
            Assert.Null(model.Description);
            Assert.Null(model.Subject);

            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        [Fact]
        public void ErrorReportGet_FlatFound_AddressFound()
        {
            // Arrange
            int flatId = 1;
            int floor = 20;
            string flatNumber = "flatNumber";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            FlatDTO flatDTO = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            string street = "street";
            string addressNumber = "addressNumber";
            string zipCode = "zipCode";
            string town = "town";
            string country = "country";

            AddressDTO addressDTO = new AddressDTO
            {
                ID = addressId,
                Street = street,
                Number = addressNumber,
                ZipCode = zipCode,
                Town = town,
                Country = country
            };

            string modelFlat = string.Format("{0}, {1} {2}", flatNumber, street, addressNumber);

            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>())).Returns(flatDTO);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int?>())).Returns(addressDTO);

            // Act
            var result = _controller.ErrorReport(flatId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorReportCreateVM>(viewResult.Model);

            Assert.Equal(flatId, model.FlatId);
            Assert.Equal(modelFlat, model.Flat);
            Assert.Null(model.Description);
            Assert.Null(model.Subject);

            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        #endregion

        #region ErrorReport - Post

        [Fact]
        public void ErrorReportPost_InvalidModel()
        {
            // Arrange
            ErrorReportCreateVM model = null;

            _controller.ModelState.AddModelError("fakeError", "fakeErrorMsg");

            // Arrange
            var result = _controller.ErrorReport(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);

            _errorReportsRepository.Verify(er => er.Add(It.IsAny<ErrorReportDTO>()), Times.Never);
        }

        [Fact]
        public void ErrorReportPost_ValidModel_ModelNull()
        {
            // Arrange
            ErrorReportCreateVM model = null;

            // Arrange
            var result = _controller.ErrorReport(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);

            _errorReportsRepository.Verify(er => er.Add(It.IsAny<ErrorReportDTO>()), Times.Never);
        }

        [Fact]
        public void ErrorReportPost_ValidModel_ModelNotNull_FlatNotFound()
        {
            // Arrange
            int flatId = 1;
            string subject = "someSubject";
            string description = "someDescription";

            ErrorReportCreateVM model = new ErrorReportCreateVM
            {
                FlatId = flatId,
                Subject = subject,
                Description = description
            };

            FlatDTO flatDTO = null;

            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>())).Returns(flatDTO);

            // Arrange
            var result = _controller.ErrorReport(model);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);

            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _errorReportsRepository.Verify(er => er.Add(It.IsAny<ErrorReportDTO>()), Times.Never);
        }

        [Fact]
        public void ErrorReportPost_ValidModel_ModelNotNull_FlatFound()
        {
            // Arrange
            int flatId = 1;
            string subject = "someSubject";
            string description = "someDescription";

            ErrorReportCreateVM model = new ErrorReportCreateVM
            {
                FlatId = flatId,
                Subject = subject,
                Description = description
            };

            int floor = 20;
            string flatNumber = "flatNumber";
            string entryDoorCode = "entryDoorCode";
            int addressId = 1;

            FlatDTO flatDTO = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            _flatsRepository.Setup(f => f.Flat(It.IsAny<int?>())).Returns(flatDTO);
            _errorReportsRepository.Setup(er => er.Add(It.IsAny<ErrorReportDTO>()))
                                   .Returns(1);

            // Arrange
            var result = _controller.ErrorReport(model);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Flats), viewResult.ActionName);

            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _errorReportsRepository.Verify(er => er.Add(It.IsAny<ErrorReportDTO>()), Times.Once);
        }

        #endregion

        #endregion

        #region GetUser

        [Fact]
        public void GetUser_IdNull()
        {
            // Arrange
            string userId = null;
            UserDTO user = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            // Act
            var result = _controller.GetUser(userId);

            // Arrange
            Assert.NotNull(result);
            Assert.Null(result.Id);
            Assert.Null(result.PersonalNumber);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
            Assert.Null(result.Email);
            Assert.Null(result.PhoneNumber);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
        }

        [Fact]
        public void GetUser_IdNotFound()
        {
            // Arrange
            string userId = "someUserId";
            UserDTO user = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            // Act
            var result = _controller.GetUser(userId);

            // Arrange
            Assert.NotNull(result);
            Assert.Null(result.Id);
            Assert.Null(result.PersonalNumber);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
            Assert.Null(result.Email);
            Assert.Null(result.PhoneNumber);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
        }

        [Fact]
        public void GetUser_IdFound()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            // Act
            var result = _controller.GetUser(userId);

            // Arrange
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(personalNumber, result.PersonalNumber);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
            Assert.Equal(emailAddress, result.Email);
            Assert.Equal(phoneNumber, result.PhoneNumber);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
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
            Assert.False(model.IsPhoneNumber2Visible);
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
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserCreateVM model = new UserCreateVM
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            UserDTO userDTO = new UserDTO
            {
                Id = "someOtherUserId"
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

            _usersRepository.Verify(u => u.UserByPersonalNumber(personalNumber, "User"), Times.Once);
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
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserCreateVM model = new UserCreateVM
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
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

            _usersRepository.Verify(u => u.UserByPersonalNumber(personalNumber, "User"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Once);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Once);
            _newUser.Verify(nu => nu.Create(model,
                                            "User",
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
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserCreateVM model = new UserCreateVM
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
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

            _usersRepository.Verify(u => u.UserByPersonalNumber(personalNumber, "User"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Once);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Once);
            _newUser.Verify(nu => nu.Create(model,
                                            "User",
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
            string userId = null;

            // Act
            var result = _controller.Details(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()), Times.Never);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<string>()), Times.Never);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Details_IdNotFound()
        {
            // Arrange
            string userId = "someUserId";
            UserDTO user = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            // Act
            var result = _controller.Details(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()), Times.Never);
            _flatsRepository.Verify(f => f.Flats(It.IsAny<string>()), Times.Never);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Details_IdFound_UserParametersNull_FlatsNull()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            List<ParameterDTO> userParameters = null;
            List<FlatDTO> flats = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()))
                                .Returns(userParameters);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<string>()))
                            .Returns(flats);

            // Act
            var result = _controller.Details(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Empty(model.Addresses);
            Assert.Null(model.Flats);
            Assert.Empty(model.Parameters);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(userId, It.IsAny<int?>()), Times.Once);
            _flatsRepository.Verify(f => f.Flats(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Details_IdFound_NoUserParameters_NoFlats()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            List<ParameterDTO> userParameters = new List<ParameterDTO>();
            List<FlatDTO> flats = new List<FlatDTO>();

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()))
                                .Returns(userParameters);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<string>()))
                            .Returns(flats);

            // Act
            var result = _controller.Details(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Empty(model.Addresses);
            Assert.Empty(model.Flats);
            Assert.Empty(model.Parameters);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(userId, It.IsAny<int?>()), Times.Once);
            _flatsRepository.Verify(f => f.Flats(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Details_IdFound_OneUserParameter_OneFlat_FlatParametersNull_AddressesNull()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;
            ParameterDTO userParameter = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
            };

            int flatId = 1;
            int floor = 10;
            string flatNumber = "flatNumber";
            string entryDoorCode = "entryDoorCode";
            int addressId = 2;

            FlatDTO flat = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            List<ParameterDTO> userParameters = new List<ParameterDTO> { userParameter };
            List<FlatDTO> flats = new List<FlatDTO> { flat };
            List<ParameterDTO> flatParameters = null;
            AddressDTO flatAddress = null;

            string addressToString = new AddressDTO().ToString();

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<string>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(flatParameters);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(flatAddress);

            // Act
            var result = _controller.Details(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Empty(model.Addresses);
            Assert.Single(model.Flats);
            Assert.Equal(addressToString, model.Flats.First().Key);
            Assert.Equal(flatId, model.Flats[addressToString].First().ID);
            Assert.Equal(floor, model.Flats[addressToString].First().Floor);
            Assert.Equal(flatNumber, model.Flats[addressToString].First().Number);
            Assert.Equal(entryDoorCode, model.Flats[addressToString].First().EntryDoorCode);
            Assert.Empty(model.Flats[addressToString].First().Parameters);
            Assert.Empty(model.Parameters);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(userId, It.IsAny<int?>()), Times.Once);
            _flatsRepository.Verify(f => f.Flats(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        [Fact]
        public void Details_IdFound_OneUserParameter_OneFlat_NoFlatParameters()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;
            int parameterFlatFloor = -1;
            ParameterDTO userParameter = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted
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

            string addressToString = address.ToString();

            int flatId = 2;
            int floor = 10;
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

            List<ParameterDTO> userParameters = new List<ParameterDTO> { userParameter };
            List<FlatDTO> flats = new List<FlatDTO> { flat };
            List<ParameterDTO> parameters = new List<ParameterDTO>();

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()))
                                .Returns(userParameters);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<string>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Details(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Empty(model.Addresses);
            Assert.Single(model.Flats);
            Assert.Equal(addressToString, model.Flats.First().Key);
            Assert.Equal(flatId, model.Flats[addressToString].First().ID);
            Assert.Equal(floor, model.Flats[addressToString].First().Floor);
            Assert.Equal(flatNumber, model.Flats[addressToString].First().Number);
            Assert.Equal(entryDoorCode, model.Flats[addressToString].First().EntryDoorCode);
            Assert.Empty(model.Flats[addressToString].First().Parameters);
            Assert.Single(model.Parameters);
            Assert.Equal(parameterFlatFloor, model.Parameters.First().Key);
            Assert.Single(model.Parameters[parameterFlatFloor]);
            Assert.Equal(isEmailVisible, model.Parameters[parameterFlatFloor].First().IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible, model.Parameters[parameterFlatFloor].First().IsPhoneNumberVisible);
            Assert.Equal(canBeContacted, model.Parameters[parameterFlatFloor].First().CanBeContacted);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
            _flatsRepository.Verify(f => f.Flats(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        [Fact]
        public void Details_IdFound_OneUserParameter_OneFlat_OneFlatParameter()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            bool userIsEmailVisible = false;
            bool userIsPhoneNumberVisible = false;
            bool userCanBeContacted = false;
            int userParameterFlatFloor = -1;
            ParameterDTO userParameter = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = userIsEmailVisible,
                IsPhoneNumberVisible = userIsPhoneNumberVisible,
                CanBeContacted = userCanBeContacted
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

            string addressToString = address.ToString();

            int flatId = 2;
            int floor = 10;
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

            bool flatIsEmailVisible = false;
            bool flatIsPhoneNumberVisible = false;
            bool flatCanBeContacted = false;
            ParameterDTO flatParameter = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = flatIsEmailVisible,
                IsPhoneNumberVisible = flatIsPhoneNumberVisible,
                CanBeContacted = flatCanBeContacted
            };

            List<ParameterDTO> userParameters = new List<ParameterDTO> { userParameter };
            List<FlatDTO> flats = new List<FlatDTO> { flat };
            List<ParameterDTO> parameters = new List<ParameterDTO> { flatParameter };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()))
                                .Returns(userParameters);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<string>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns(parameters);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Details(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Empty(model.Addresses);
            Assert.Single(model.Flats);
            Assert.Equal(addressToString, model.Flats.First().Key);
            Assert.Equal(flatId, model.Flats[addressToString].First().ID);
            Assert.Equal(floor, model.Flats[addressToString].First().Floor);
            Assert.Equal(flatNumber, model.Flats[addressToString].First().Number);
            Assert.Equal(entryDoorCode, model.Flats[addressToString].First().EntryDoorCode);
            Assert.Single(model.Flats[addressToString].First().Parameters);
            Assert.Equal(flatIsEmailVisible, model.Flats[addressToString].First().Parameters.First().IsEmailVisible);
            Assert.Equal(flatIsPhoneNumberVisible, model.Flats[addressToString].First().Parameters.First().IsPhoneNumberVisible);
            Assert.Equal(flatCanBeContacted, model.Flats[addressToString].First().Parameters.First().CanBeContacted);
            Assert.Single(model.Parameters);
            Assert.Equal(userParameterFlatFloor, model.Parameters.First().Key);
            Assert.Single(model.Parameters[userParameterFlatFloor]);
            Assert.Equal(userIsEmailVisible, model.Parameters[userParameterFlatFloor].First().IsEmailVisible);
            Assert.Equal(userIsPhoneNumberVisible, model.Parameters[userParameterFlatFloor].First().IsPhoneNumberVisible);
            Assert.Equal(userCanBeContacted, model.Parameters[userParameterFlatFloor].First().CanBeContacted);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
            _flatsRepository.Verify(f => f.Flats(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        [Fact]
        public void Details_IdFound_TwoUserParameters_TwoFlats_TwoFlatParameters()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            bool userIsEmailVisible1 = false;
            bool userIsPhoneNumberVisible1 = true;
            bool userCanBeContacted1 = false;
            ParameterDTO userParameter1 = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = userIsEmailVisible1,
                IsPhoneNumberVisible = userIsPhoneNumberVisible1,
                CanBeContacted = userCanBeContacted1
            };

            bool userIsEmailVisible2 = true;
            bool userIsPhoneNumberVisible2 = false;
            bool userCanBeContacted2 = true;
            ParameterDTO userParameter2 = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = userIsEmailVisible2,
                IsPhoneNumberVisible = userIsPhoneNumberVisible2,
                CanBeContacted = userCanBeContacted2
            };

            int userParameterFlatFloor = -1;

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
            string addressNumber2 = "addressNumber2";
            string zipCode2 = "zipCode2";
            string town2 = "town2";
            string country2 = "country2";

            AddressDTO address2 = new AddressDTO
            {
                ID = addressId2,
                Street = street2,
                Number = addressNumber2,
                ZipCode = zipCode2,
                Town = town2,
                Country = country2
            };

            string addressToString1 = address1.ToString();
            string addressToString2 = address2.ToString();

            int flatId1 = 10;
            int floor1 = 10;
            string flatNumber1 = "flatNumber1";
            string entryDoorCode1 = "entryDoorCode1";

            FlatDTO flat1 = new FlatDTO
            {
                ID = flatId1,
                Floor = floor1,
                Number = flatNumber1,
                EntryDoorCode = entryDoorCode1,
                AddressID = addressId1
            };

            int flatId2 = 20;
            int floor2 = 20;
            string flatNumber2 = "flatNumber2";
            string entryDoorCode2 = "entryDoorCode2";

            FlatDTO flat2 = new FlatDTO
            {
                ID = flatId2,
                Floor = floor2,
                Number = flatNumber2,
                EntryDoorCode = entryDoorCode2,
                AddressID = addressId2
            };

            bool flatIsEmailVisible11 = true;
            bool flatIsPhoneNumberVisible11 = false;
            bool flatCanBeContacted11 = false;
            ParameterDTO flatParameter11 = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = flatIsEmailVisible11,
                IsPhoneNumberVisible = flatIsPhoneNumberVisible11,
                CanBeContacted = flatCanBeContacted11
            };

            bool flatIsEmailVisible12 = false;
            bool flatIsPhoneNumberVisible12 = true;
            bool flatCanBeContacted12 = false;
            ParameterDTO flatParameter12 = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = flatIsEmailVisible12,
                IsPhoneNumberVisible = flatIsPhoneNumberVisible12,
                CanBeContacted = flatCanBeContacted12
            };

            bool flatIsEmailVisible21 = true;
            bool flatIsPhoneNumberVisible21 = false;
            bool flatCanBeContacted21 = true;
            ParameterDTO flatParameter21 = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = flatIsEmailVisible21,
                IsPhoneNumberVisible = flatIsPhoneNumberVisible21,
                CanBeContacted = flatCanBeContacted21
            };

            bool flatIsEmailVisible22 = false;
            bool flatIsPhoneNumberVisible22 = true;
            bool flatCanBeContacted22 = false;
            ParameterDTO flatParameter22 = new ParameterDTO
            {
                UserId = userId,
                IsEmailVisible = flatIsEmailVisible22,
                IsPhoneNumberVisible = flatIsPhoneNumberVisible22,
                CanBeContacted = flatCanBeContacted22
            };

            List<ParameterDTO> userParameters = new List<ParameterDTO> { userParameter1, userParameter2 };
            List<FlatDTO> flats = new List<FlatDTO> { flat1, flat2 };
            List<ParameterDTO> flatParameters1 = new List<ParameterDTO> { flatParameter11, flatParameter12 };
            List<ParameterDTO> flatParameters2 = new List<ParameterDTO> { flatParameter21, flatParameter22 };

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()))
                                .Returns(userParameters);
            _flatsRepository.Setup(f => f.Flats(It.IsAny<string>()))
                            .Returns(flats);
            _parameterRepository.Setup(p => p.Parameters(It.IsAny<int>()))
                                .Returns((int flatId) => flatId == flatId1 ? flatParameters1 : flatParameters2);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns((int addressId) => addressId == addressId1 ? address1 : address2);

            // Act
            var result = _controller.Details(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDetailsVM>(viewResult.Model);

            Assert.Equal(userId, model.Id);
            Assert.Equal(personalNumber, model.PersonalNumber);
            Assert.Equal(firstName, model.FirstName);
            Assert.Equal(lastName, model.LastName);
            Assert.Equal(emailAddress, model.Email);
            Assert.Equal(phoneNumber, model.PhoneNumber);
            Assert.Empty(model.Addresses);
            Assert.Equal(2, model.Flats.Count);
            Assert.Equal(addressToString1, model.Flats.First().Key);
            Assert.Equal(flatId1, model.Flats[addressToString1].First().ID);
            Assert.Equal(floor1, model.Flats[addressToString1].First().Floor);
            Assert.Equal(flatNumber1, model.Flats[addressToString1].First().Number);
            Assert.Equal(entryDoorCode1, model.Flats[addressToString1].First().EntryDoorCode);
            Assert.Equal(2, model.Flats[addressToString1].First().Parameters.Count);
            Assert.Equal(flatIsEmailVisible11, model.Flats[addressToString1].First().Parameters.First().IsEmailVisible);
            Assert.Equal(flatIsPhoneNumberVisible11, model.Flats[addressToString1].First().Parameters.First().IsPhoneNumberVisible);
            Assert.Equal(flatCanBeContacted11, model.Flats[addressToString1].First().Parameters.First().CanBeContacted);
            Assert.Equal(flatIsEmailVisible12, model.Flats[addressToString1].First().Parameters.Last().IsEmailVisible);
            Assert.Equal(flatIsPhoneNumberVisible12, model.Flats[addressToString1].First().Parameters.Last().IsPhoneNumberVisible);
            Assert.Equal(flatCanBeContacted12, model.Flats[addressToString1].First().Parameters.Last().CanBeContacted);
            Assert.Equal(addressToString2, model.Flats.Last().Key);
            Assert.Equal(flatId2, model.Flats[addressToString2].First().ID);
            Assert.Equal(floor2, model.Flats[addressToString2].First().Floor);
            Assert.Equal(flatNumber2, model.Flats[addressToString2].First().Number);
            Assert.Equal(entryDoorCode2, model.Flats[addressToString2].First().EntryDoorCode);
            Assert.Equal(2, model.Flats[addressToString2].First().Parameters.Count);
            Assert.Equal(flatIsEmailVisible21, model.Flats[addressToString2].First().Parameters.First().IsEmailVisible);
            Assert.Equal(flatIsPhoneNumberVisible21, model.Flats[addressToString2].First().Parameters.First().IsPhoneNumberVisible);
            Assert.Equal(flatCanBeContacted21, model.Flats[addressToString2].First().Parameters.First().CanBeContacted);
            Assert.Equal(flatIsEmailVisible22, model.Flats[addressToString2].First().Parameters.Last().IsEmailVisible);
            Assert.Equal(flatIsPhoneNumberVisible22, model.Flats[addressToString2].First().Parameters.Last().IsPhoneNumberVisible);
            Assert.Equal(flatCanBeContacted22, model.Flats[addressToString2].First().Parameters.Last().CanBeContacted);
            Assert.Single(model.Parameters);
            Assert.Equal(userParameterFlatFloor, model.Parameters.First().Key);
            Assert.Equal(2, model.Parameters[userParameterFlatFloor].Count);
            Assert.Equal(userIsEmailVisible1, model.Parameters[userParameterFlatFloor].First().IsEmailVisible);
            Assert.Equal(userIsPhoneNumberVisible1, model.Parameters[userParameterFlatFloor].First().IsPhoneNumberVisible);
            Assert.Equal(userCanBeContacted1, model.Parameters[userParameterFlatFloor].First().CanBeContacted);
            Assert.Equal(userIsEmailVisible2, model.Parameters[userParameterFlatFloor].Last().IsEmailVisible);
            Assert.Equal(userIsPhoneNumberVisible2, model.Parameters[userParameterFlatFloor].Last().IsPhoneNumberVisible);
            Assert.Equal(userCanBeContacted2, model.Parameters[userParameterFlatFloor].Last().CanBeContacted);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
            _flatsRepository.Verify(f => f.Flats(userId), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId1), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId1), Times.Once);
            _parameterRepository.Verify(p => p.Parameters(flatId2), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId2), Times.Once);
        }

        #endregion

        #region Edit

        #region Edit - Get

        [Fact]
        public async Task EditGet_IdNull_CurrentUserNull()
        {
            // Arrange
            string userId = null;

            // Act
            Exception ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.Edit(userId));

            // Assert
            _usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            _usersRepository.Verify(u => u.GetUser(It.IsAny<string>()), Times.Never);
            _requestUserProvider.Verify(r => r.GetRolesAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task EditGet_IdNull_CurrentUserNotNull_UserNotFound()
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

            // Act
            Exception ex = await Assert.ThrowsAsync<ApplicationException>(() => _controller.Edit(id));

            // Assert
            _usersRepository.Verify(u => u.User(userId), Times.Once);
        }

        [Fact]
        public async Task EditGet_IdNotNull_UserNotFound()
        {
            // Arrange
            string userId = "someUserId";
            UserDTO user = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            // Act
            Exception ex = await Assert.ThrowsAsync<ApplicationException>(() => _controller.Edit(userId));

            // Assert
            _usersRepository.Verify(u => u.User(userId), Times.Once);
        }

        [Fact]
        public async Task EditGet_IdNull_UserFound_CurrentUserNotNull_CantChangeName1()
        {
            // Arrange
            string userId = null;
            string currentUserId = "someUserId";
            string userPersonalNumber = "personalNumber";
            string userFN = "John";
            string userLN = "Doe";
            string userEmail = "john.doe@some.email";
            string userPhoneNumber = "123456789";

            var currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, currentUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = currentUser
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

            User user = new User();

            IList<string> roles = null;

            bool expectedCanChangeName = false;
            bool expectedCanChangePassword = true;

            _usersRepository.Setup(u => u.User(It.IsAny<string>())).Returns(userDTO);
            _usersRepository.Setup(u => u.GetUser(It.IsAny<string>()))
                .Returns(user);
            _requestUserProvider.Setup(r => r.GetRolesAsync(It.IsAny<User>()))
                                .Returns(Task.FromResult(roles));

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserCreateVM>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(currentUserId, model.Id);
            Assert.Equal(userPersonalNumber, model.PersonalNumber);
            Assert.Equal(userFN, model.FirstName);
            Assert.Equal(userLN, model.LastName);
            Assert.Equal(expectedCanChangeName, model.CanChangeName);
            Assert.Equal(expectedCanChangePassword, model.CanChangePassword);
            Assert.Equal(userPhoneNumber, model.PhoneNumber);

            _usersRepository.Verify(u => u.User(currentUserId), Times.Once);
            _usersRepository.Verify(u => u.GetUser(It.IsAny<string>()), Times.Once);
            _requestUserProvider.Verify(r => r.GetRolesAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task EditGet_IdNotNull_UserFound_CurrentUserNotNull_CantChangeName2()
        {
            // Arrange
            string userId = "someUserId";
            string currentUserId = userId;
            string userFN = "John";
            string userLN = "Doe";
            string userPersonalNumber = "personalNumber";
            string userEmail = "john.doe@some.email";
            string userPhoneNumber = "123456789";

            var currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, currentUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = currentUser
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

            User user = new User();

            IList<string> roles = null;

            bool expectedCanChangeName = false;
            bool expectedCanChangePassword = false;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(userDTO);
            _usersRepository.Setup(u => u.GetUser(It.IsAny<string>()))
                            .Returns(user);
            _requestUserProvider.Setup(r => r.GetRolesAsync(It.IsAny<User>()))
                                .Returns(Task.FromResult(roles));

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserCreateVM>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(userId, model.Id);
            Assert.Equal(userPersonalNumber, model.PersonalNumber);
            Assert.Equal(userFN, model.FirstName);
            Assert.Equal(userLN, model.LastName);
            Assert.Equal(expectedCanChangeName, model.CanChangeName);
            Assert.Equal(expectedCanChangePassword, model.CanChangePassword);
            Assert.Equal(userPhoneNumber, model.PhoneNumber);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.GetUser(userId), Times.Once);
            _requestUserProvider.Verify(r => r.GetRolesAsync(user), Times.Once);
        }

        [Fact]
        public async Task EditGet_IdNotNull_UserFound_CurrentUserNotNull_CantChangeName3()
        {
            // Arrange
            string userId = "someUserId";
            string currentUserId = userId;
            string userFN = "John";
            string userLN = "Doe";
            string userPersonalNumber = "personalNumber";
            string userEmail = "john.doe@some.email";
            string userPhoneNumber = "123456789";

            var currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, currentUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = currentUser
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

            User user = new User();

            IList<string> roles = new List<string> { "Admin" };

            bool expectedCanChangeName = false;
            bool expectedCanChangePassword = false;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(userDTO);
            _usersRepository.Setup(u => u.GetUser(It.IsAny<string>()))
                            .Returns(user);
            _requestUserProvider.Setup(r => r.GetRolesAsync(It.IsAny<User>()))
                                .Returns(Task.FromResult(roles));

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserCreateVM>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(userId, model.Id);
            Assert.Equal(userPersonalNumber, model.PersonalNumber);
            Assert.Equal(userFN, model.FirstName);
            Assert.Equal(userLN, model.LastName);
            Assert.Equal(expectedCanChangeName, model.CanChangeName);
            Assert.Equal(expectedCanChangePassword, model.CanChangePassword);
            Assert.Equal(userPhoneNumber, model.PhoneNumber);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.GetUser(userId), Times.Once);
            _requestUserProvider.Verify(r => r.GetRolesAsync(user), Times.Once);
        }

        [Fact]
        public async Task EditGet_IdNotNull_UserFound_CurrentUserNotNull_CanChangeName()
        {
            // Arrange
            string userId = "someUserId";
            string currentUserId = userId;
            string userFN = "John";
            string userLN = "Doe";
            string userPersonalNumber = "personalNumber";
            string userEmail = "john.doe@some.email";
            string userPhoneNumber = "123456789";

            var currentUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, currentUserId)
            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = currentUser
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

            User user = new User();

            IList<string> roles = new List<string> { "User" };

            bool expectedCanChangeName = true;
            bool expectedCanChangePassword = false;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(userDTO);
            _usersRepository.Setup(u => u.GetUser(It.IsAny<string>()))
                            .Returns(user);
            _requestUserProvider.Setup(r => r.GetRolesAsync(It.IsAny<User>()))
                                .Returns(Task.FromResult(roles));

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserCreateVM>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(userId, model.Id);
            Assert.Equal(userPersonalNumber, model.PersonalNumber);
            Assert.Equal(userFN, model.FirstName);
            Assert.Equal(userLN, model.LastName);
            Assert.Equal(expectedCanChangeName, model.CanChangeName);
            Assert.Equal(expectedCanChangePassword, model.CanChangePassword);
            Assert.Equal(userPhoneNumber, model.PhoneNumber);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _usersRepository.Verify(u => u.GetUser(userId), Times.Once);
            _requestUserProvider.Verify(r => r.GetRolesAsync(user), Times.Once);
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

            // Act
            Exception ex = await Assert.ThrowsAsync<ApplicationException>(() => _controller.Edit(user));

            // Assert
            _usersRepository.Verify(u => u.User(userId), Times.Once);
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
                Id = "someOtherUserId"
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
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "User"), Times.Once);
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
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "User"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Once);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Once);
            _newUser.Verify(nu => nu.Create(model,
                                            "User",
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
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "User"), Times.Once);
            _usersRepository.Verify(u => u.GenerateRegistrationCode(), Times.Once);
            _usersRepository.Verify(u => u.GeneratePassword(), Times.Once);
            _newUser.Verify(nu => nu.Create(editedUser,
                                            "User",
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
            string userRegistrationCode = string.Empty;

            UserCreateVM editedUser = new UserCreateVM
            {
                Id = userId,
                FirstName = userFNOriginal,
                LastName = userLNOriginal,
                PersonalNumber = userPersonalNumberOriginal,
                Email = userEmailOriginal,
                PhoneNumber = userPhoneNumberOriginal
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
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberOriginal, "User"), Times.Once);
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
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "User"), Times.Once);
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
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "User"), Times.Once);
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
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "User"), Times.Once);
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
            _usersRepository.Verify(u => u.UserByPersonalNumber(userPersonalNumberEdited, "User"), Times.Once);
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

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
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
            int flatId = 1;

            // Act
            var result = _controller.Remove(userId, flatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(It.IsAny<string>()), Times.Never);
            _flatsRepository.Verify(f => f.Flat(It.IsAny<int>()), Times.Never);
            _parameterRepository.Verify(p => p.Get(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _addressesRepository.Verify(a => a.Address(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserNotFound()
        {
            // Arrange
            string userId = "someUserId";
            int flatId = 1;

            UserDTO user = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);

            // Act
            var result = _controller.Remove(userId, flatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(It.IsAny<int>()), Times.Never);
            _parameterRepository.Verify(p => p.Get(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _addressesRepository.Verify(a => a.Address(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_FlatNotFound()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            int flatId = 1;

            FlatDTO flat = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _flatsRepository.Setup(f => f.Flat(It.IsAny<int>()))
                            .Returns(flat);

            // Act
            var result = _controller.Remove(userId, flatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _parameterRepository.Verify(p => p.Get(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _addressesRepository.Verify(a => a.Address(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_FlatFound_ParametersNotFound()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            int flatId = 1;
            int floor = 10;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            FlatDTO flat = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode
            };

            ParameterDTO parameter = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(user);
            _flatsRepository.Setup(f => f.Flat(It.IsAny<int>()))
                            .Returns(flat);
            _parameterRepository.Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>()))
                                .Returns(parameter);

            // Act
            var result = _controller.Remove(userId, flatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _parameterRepository.Verify(p => p.Get(userId, flatId), Times.Once);
            _addressesRepository.Verify(a => a.Address(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_FlatFound_ParameterFound_AddressNull()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO userDTO = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

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
            int floor = 10;
            string number = "number";
            string entryDoorCode = "entryDoorCode";
            int addressId = 10;

            FlatDTO flatDTO = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

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
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted,
                FlatID = flatId,
                UserId = userId
            };
            ParameterDTO parameterDTO = new ParameterDTO(flat, user, parameter);

            AddressDTO address = null;

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(userDTO);
            _flatsRepository.Setup(f => f.Flat(It.IsAny<int>()))
                            .Returns(flatDTO);
            _parameterRepository.Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>()))
                                .Returns(parameterDTO);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Remove(userId, flatId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ParameterDetailsVM>(viewResult.Model);

            Assert.Equal(isEmailVisible, model.IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible, model.IsPhoneNumberVisible);
            Assert.Equal(canBeContacted, model.CanBeContacted);
            Assert.Equal(userId, model.User.Id);
            Assert.Equal(personalNumber, model.User.PersonalNumber);
            Assert.Equal(firstName, model.User.FirstName);
            Assert.Equal(lastName, model.User.LastName);
            Assert.Equal(emailAddress, model.User.Email);
            Assert.Equal(phoneNumber, model.User.PhoneNumber);
            Assert.Empty(model.User.Addresses);

            Assert.Equal(flatId, model.Flat.ID);
            Assert.Equal(floor, model.Flat.Floor);
            Assert.Equal(number, model.Flat.Number);
            Assert.Equal(entryDoorCode, model.Flat.EntryDoorCode);
            Assert.NotNull(model.Flat.Address);
            Assert.Equal(-1, model.Flat.Address.ID);
            Assert.Empty(model.Flat.Address.Street);
            Assert.Empty(model.Flat.Address.Number);
            Assert.Empty(model.Flat.Address.ZipCode);
            Assert.Empty(model.Flat.Address.Town);
            Assert.Empty(model.Flat.Address.Country);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _parameterRepository.Verify(p => p.Get(userId, flatId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        [Fact]
        public void Remove_UserIdNotNull_UserFound_FlatFound_ParameterFound_AddressNotNull()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO userDTO = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

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
            int floor = 10;
            string flatNumber = "flatNumber";
            string entryDoorCode = "entryDoorCode";
            int addressId = 10;

            FlatDTO flatDTO = new FlatDTO
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = flatNumber,
                EntryDoorCode = entryDoorCode,
                AddressID = addressId
            };

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

            bool isEmailVisible = false;
            bool isPhoneNumberVisible = false;
            bool canBeContacted = false;
            Parameter parameter = new Parameter
            {
                IsEmailVisible = isEmailVisible,
                IsPhoneNumberVisible = isPhoneNumberVisible,
                CanBeContacted = canBeContacted,
                FlatID = flatId,
                UserId = userId
            };
            ParameterDTO parameterDTO = new ParameterDTO(flat, user, parameter);

            _usersRepository.Setup(u => u.User(It.IsAny<string>()))
                            .Returns(userDTO);
            _flatsRepository.Setup(f => f.Flat(It.IsAny<int>()))
                            .Returns(flatDTO);
            _parameterRepository.Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>()))
                                .Returns(parameterDTO);
            _addressesRepository.Setup(a => a.Address(It.IsAny<int>()))
                                .Returns(address);

            // Act
            var result = _controller.Remove(userId, flatId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ParameterDetailsVM>(viewResult.Model);

            Assert.Equal(isEmailVisible, model.IsEmailVisible);
            Assert.Equal(isPhoneNumberVisible, model.IsPhoneNumberVisible);
            Assert.Equal(canBeContacted, model.CanBeContacted);
            Assert.Equal(userId, model.User.Id);
            Assert.Equal(personalNumber, model.User.PersonalNumber);
            Assert.Equal(firstName, model.User.FirstName);
            Assert.Equal(lastName, model.User.LastName);
            Assert.Equal(emailAddress, model.User.Email);
            Assert.Equal(phoneNumber, model.User.PhoneNumber);
            Assert.Empty(model.User.Addresses);

            Assert.Equal(flatId, model.Flat.ID);
            Assert.Equal(floor, model.Flat.Floor);
            Assert.Equal(flatNumber, model.Flat.Number);
            Assert.Equal(entryDoorCode, model.Flat.EntryDoorCode);
            Assert.NotNull(model.Flat.Address);
            Assert.Equal(addressId, model.Flat.Address.ID);
            Assert.Equal(street, model.Flat.Address.Street);
            Assert.Equal(addressNumber, model.Flat.Address.Number);
            Assert.Equal(zipCode, model.Flat.Address.ZipCode);
            Assert.Equal(town, model.Flat.Address.Town);
            Assert.Equal(country, model.Flat.Address.Country);

            _usersRepository.Verify(u => u.User(userId), Times.Once);
            _flatsRepository.Verify(f => f.Flat(flatId), Times.Once);
            _parameterRepository.Verify(p => p.Get(userId, flatId), Times.Once);
            _addressesRepository.Verify(a => a.Address(addressId), Times.Once);
        }

        #endregion

        #region RemoveConfirmed

        [Fact]
        public void RemoveConfirmed_ParameterNotFound()
        {
            // Arrange
            string userId = "someUserId";
            int flatId = 1;

            ParameterDTO parameter = null;

            _parameterRepository.Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>()))
                                .Returns(parameter);

            // Act
            var result = _controller.RemoveConfirmed(userId, flatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _parameterRepository.Verify(p => p.Get(userId, flatId), Times.Once);
            _parameterRepository.Verify(p => p.Delete(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void RemoveConfirmed_ParameterFound()
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
            int floor = 10;
            string number = "number";
            string entryDoorCode = "entryDoorCode";

            Flat flat = new Flat
            {
                ID = flatId,
                Floor = floor,
                Number = number,
                EntryDoorCode = entryDoorCode
            };

            ParameterDTO parameter = new ParameterDTO(flat, user);

            _parameterRepository.Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>()))
                                .Returns(parameter);

            // Act
            var result = _controller.RemoveConfirmed(userId, flatId);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(UsersController.Details), viewResult.ActionName);
            Assert.Equal(userId, viewResult.RouteValues.First().Value);

            _parameterRepository.Verify(p => p.Get(userId, flatId), Times.Once);
            _parameterRepository.Verify(p => p.Delete(userId, flatId), Times.Once);
        }

        #endregion

        #region GetUsers

        [Fact]
        public void GetUsers_Null()
        {
            // Arrange
            List<UserDTO> users = null;

            _usersRepository.Setup(u => u.Users())
                            .Returns(users);

            // Act
            var result = _controller.GetUsers();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetUsers_NoEntry()
        {
            // Arrange
            List<UserDTO> users = new List<UserDTO>();

            _usersRepository.Setup(u => u.Users())
                            .Returns(users);

            // Act
            var result = _controller.GetUsers();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetUsers_OneEntry()
        {
            // Arrange
            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            List<UserDTO> users = new List<UserDTO> { user };

            _usersRepository.Setup(u => u.Users())
                            .Returns(users);

            // Act
            var result = _controller.GetUsers();

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
        public void GetUsers_TwoEntries()
        {
            // Arrange
            string userId1 = "someUserId1";
            string personalNumber1 = "personalNumber1";
            string firstName1 = "firstName1";
            string lastName1 = "lastName1";
            string emailAddress1 = "emailAddress1";
            string phoneNumber1 = "phoneNumber1";

            UserDTO user1 = new UserDTO
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

            UserDTO user2 = new UserDTO
            {
                Id = userId2,
                PersonalNumber = personalNumber2,
                FirstName = firstName2,
                LastName = lastName2,
                Email = emailAddress2,
                PhoneNumber = phoneNumber2
            };

            List<UserDTO> users = new List<UserDTO> { user1, user2 };

            _usersRepository.Setup(u => u.Users())
                            .Returns(users);

            // Act
            var result = _controller.GetUsers();

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

        #region GetUsersAtAddress

        [Fact]
        public void GetUsersAtAddress_UsersAtAddressNull()
        {
            // Arrange
            int addressId = 1;

            List<UserDTO> users = null;

            _usersRepository.Setup(u => u.UsersAtAddress(It.IsAny<int>()))
                            .Returns(users);

            // Act
            var result = _controller.GetUsersAtAddress(addressId);

            // Arrange
            Assert.Null(result);
        }

        [Fact]
        public void GetUsersAtAddress_UsersAtAddressNoEntries()
        {
            // Arrange
            int addressId = 1;

            List<UserDTO> users = new List<UserDTO>();

            _usersRepository.Setup(u => u.UsersAtAddress(It.IsAny<int>()))
                            .Returns(users);

            // Act
            var result = _controller.GetUsersAtAddress(addressId);

            // Arrange
            Assert.Empty(result);
        }

        [Fact]
        public void GetUsersAtAddress_UsersAtAddressOneEntry()
        {
            // Arrange
            int addressId = 1;

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            List<UserDTO> users = new List<UserDTO> { user };

            string firstLetter = lastName.Substring(0, 1);

            _usersRepository.Setup(u => u.UsersAtAddress(It.IsAny<int>()))
                            .Returns(users);

            // Act
            var result = _controller.GetUsersAtAddress(addressId);

            // Arrange
            Assert.Single(result);
            Assert.Equal(firstLetter, result.First().Key);
            Assert.Equal(userId, result[firstLetter].First().Id);
            Assert.Equal(personalNumber, result[firstLetter].First().PersonalNumber);
            Assert.Equal(firstName, result[firstLetter].First().FirstName);
            Assert.Equal(lastName, result[firstLetter].First().LastName);
            Assert.Equal(emailAddress, result[firstLetter].First().Email);
            Assert.Equal(phoneNumber, result[firstLetter].First().PhoneNumber);
        }

        [Fact]
        public void GetUsersAtAddress_UsersAtAddressThreeEntries()
        {
            // Arrange
            int addressId = 1;

            string userId = "someUserId";
            string personalNumber = "personalNumber";
            string firstName = "firstName";
            string lastName = "lastName";
            string emailAddress = "emailAddress";
            string phoneNumber = "phoneNumber";

            UserDTO user1 = new UserDTO
            {
                Id = userId,
                PersonalNumber = personalNumber,
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                PhoneNumber = phoneNumber
            };

            string userId2 = "someUserId2";
            string personalNumber2 = "personalNumber2";
            string firstName2 = "firstName2";
            string lastName2 = lastName;
            string emailAddress2 = "emailAddress2";
            string phoneNumber2 = "phoneNumber2";

            UserDTO user2 = new UserDTO
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
            string firstName3 = "firstName3";
            string lastName3 = "someOtherLastName";
            string emailAddress3 = "emailAddress3";
            string phoneNumber3 = "phoneNumber3";

            UserDTO user3 = new UserDTO
            {
                Id = userId3,
                PersonalNumber = personalNumber3,
                FirstName = firstName3,
                LastName = lastName3,
                Email = emailAddress3,
                PhoneNumber = phoneNumber3
            };

            List<UserDTO> users = new List<UserDTO> { user3, user2, user1 };

            string firstLetter1 = lastName.Substring(0, 1);
            string firstLetter2 = lastName3.Substring(0, 1);

            _usersRepository.Setup(u => u.UsersAtAddress(It.IsAny<int>()))
                            .Returns(users);

            // Act
            var result = _controller.GetUsersAtAddress(addressId);

            // Arrange
            Assert.Equal(2, result.Count());
            Assert.Equal(firstLetter1, result.First().Key);
            Assert.Equal(2, result[firstLetter1].Count());
            Assert.Equal(userId, result[firstLetter1].First().Id);
            Assert.Equal(personalNumber, result[firstLetter1].First().PersonalNumber);
            Assert.Equal(firstName, result[firstLetter1].First().FirstName);
            Assert.Equal(lastName, result[firstLetter1].First().LastName);
            Assert.Equal(emailAddress, result[firstLetter1].First().Email);
            Assert.Equal(phoneNumber, result[firstLetter1].First().PhoneNumber);

            Assert.Equal(userId2, result[firstLetter1].Last().Id);
            Assert.Equal(personalNumber2, result[firstLetter1].Last().PersonalNumber);
            Assert.Equal(firstName2, result[firstLetter1].Last().FirstName);
            Assert.Equal(lastName2, result[firstLetter1].Last().LastName);
            Assert.Equal(emailAddress2, result[firstLetter1].Last().Email);
            Assert.Equal(phoneNumber2, result[firstLetter1].Last().PhoneNumber);

            Assert.Equal(firstLetter2, result.Last().Key);
            Assert.Equal(userId3, result[firstLetter2].First().Id);
            Assert.Equal(personalNumber3, result[firstLetter2].First().PersonalNumber);
            Assert.Equal(firstName3, result[firstLetter2].First().FirstName);
            Assert.Equal(lastName3, result[firstLetter2].First().LastName);
            Assert.Equal(emailAddress3, result[firstLetter2].First().Email);
            Assert.Equal(phoneNumber3, result[firstLetter2].First().PhoneNumber);
        }

        #endregion
    }
}
