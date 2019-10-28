using Dios.Controllers;
using Dios.Helpers;
using Dios.Models;
using Dios.Repositories;
using Dios.Services;
using Dios.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DiosTest.Controllers
{
    public class AccountControllerTest
    {
        private Mock<IRequestUserProvider> requestUserProviderMock;
        private Mock<IRequestSignInProvider> requestSignInProviderMock;
        private Mock<IEmailSender> emailSenderMock;
        private Mock<ILog<AccountController>> loggerMock;
        private Mock<IUsersRepository> usersRepoMock;

        private AccountController controller;

        public AccountControllerTest()
        {
            requestUserProviderMock = new Mock<IRequestUserProvider>();
            requestSignInProviderMock = new Mock<IRequestSignInProvider>();
            emailSenderMock = new Mock<IEmailSender>();

            loggerMock = new Mock<ILog<AccountController>>();
            usersRepoMock = new Mock<IUsersRepository>();

            controller = new AccountController(requestUserProviderMock.Object,
                                               requestSignInProviderMock.Object,
                                               emailSenderMock.Object,
                                               loggerMock.Object,
                                               usersRepoMock.Object);
        }

        #region Login

        #region Login - Get

        //[Fact]
        //public async Task Login()
        //{
        //    // Arrange
        //    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //                                                            {
        //                                                                new Claim(ClaimTypes.Role, "Admin")
        //                                                            }, "Admin"));

        //    var context = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = user
        //        }
        //    };

        //    controller.ControllerContext = context;

        //    // Act
        //    var result = await controller.Login();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //}

        #endregion

        #region Login - Post

        [Fact]
        public async Task Login_Post_InvalidModelState()
        {
            // Arrange
            controller.ControllerContext.ModelState.AddModelError("Error", "Error");

            // Act
            var result = await controller.Login(new LoginViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(1, viewResult.ViewData.ModelState.ErrorCount);
        }

        [Fact]
        public async Task Login_Post_SuccessfulLogin()
        {
            // Arrange
            requestSignInProviderMock.Setup(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                                     .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            controller.Url = new UrlHelper(new ActionContext());

            // Act
            var result = await controller.Login(new LoginViewModel());

            // Assert
            requestSignInProviderMock.Verify(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public async Task Login_Post_InvalidLogin()
        {
            // Arrange
            requestSignInProviderMock.Setup(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                                     .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await controller.Login(new LoginViewModel());

            // Assert
            requestSignInProviderMock.Verify(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(1, viewResult.ViewData.ModelState.ErrorCount);
        }

        [Fact]
        public async Task Login_Post_UserLockedOut()
        {
            // Arrange
            requestSignInProviderMock.Setup(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                                     .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            // Act
            var result = await controller.Login(new LoginViewModel());

            // Assert
            requestSignInProviderMock.Verify(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Lockout), viewResult.ActionName);
        }

        #endregion

        #endregion

        #region Lockout

        [Fact]
        public void Lockout()
        {
            // Arrange

            // Act
            var result = controller.Lockout();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        #endregion

        #region Register

        #region Register - Get

        [Fact]
        public void RegisterTest_Get_EmptyRegistrationCode()
        {
            // Arrange
            string emptyRegistrationCode = string.Empty;

            // Act
            var result = controller.Register(emptyRegistrationCode);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void RegisterTest_Get_InvalidCode()
        {
            // Arrange
            string invalidCode = "invalid";
            usersRepoMock.Setup(v => v.UserByRegistrationCode(invalidCode))
                         .Returns<int?>(null);

            // Act
            var result = controller.Register(invalidCode);

            // Assert
            usersRepoMock.Verify(v => v.UserByRegistrationCode(It.IsAny<string>()), Times.Once);

            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void RegisterTest_Get_GoodCode()
        {
            // Arrange
            string validCode = "valid";
            usersRepoMock.Setup(v => v.UserByRegistrationCode(validCode))
                         .Returns(new UserDTO());

            // Act
            var result = controller.Register(validCode);

            // Assert
            usersRepoMock.Verify(v => v.UserByRegistrationCode(It.IsAny<string>()), Times.Once);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<RegisterViewModel>(viewResult.ViewData.Model);
            Assert.Equal(validCode, model.RegistrationCode);
        }

        #endregion

        #region Register - Post

        [Fact]
        public async Task Register_Post_InvalidModelState()
        {
            // Arrange
            controller.ModelState.AddModelError("Error", "Error");

            // Act
            var result = await controller.Register(string.Empty, new RegisterViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(1, viewResult.ViewData.ModelState.ErrorCount);
        }

        [Fact]
        public async Task Register_Post_ValidModelState_IllicitRegistrationCode()
        {
            // Arrange
            string code = "code";

            string email = "email";
            string registrationCode = "registrationCode";
            string password = "password";
            RegisterViewModel model = new RegisterViewModel
            {
                Email = email,
                RegistrationCode = registrationCode,
                Password = password
            };

            string statusMessage = "Ogiltig registreringskod.";

            // Act
            var result = await controller.Register(code, model);

            // Assert
            usersRepoMock.Verify(u => u.UserByRegistrationCode(It.IsAny<string>()), Times.Never);

            var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(viewResult.Model);
            Assert.NotNull(controller.ViewData["StatusMessage"]);
            Assert.Equal(statusMessage, controller.ViewData["StatusMessage"].ToString());
        }

        [Fact]
        public async Task Register_Post_ValidModel_ValidRegistrationCode_InvalidRegistrationCode()
        {
            // Arrange
            string email = "email";
            string registrationCode = "ValidID";
            string password = "password";
            RegisterViewModel model = new RegisterViewModel
            {
                Email = email,
                RegistrationCode = registrationCode,
                Password = password
            };

            UserDTO user = null;

            usersRepoMock.Setup(u => u.UserByRegistrationCode(registrationCode))
                         .Returns(user);

            string statusMessage = "Ogiltigt inloggningsförsök.";

            // Act
            var result = await controller.Register(registrationCode, model);

            // Assert
            usersRepoMock.Verify(u => u.UserByRegistrationCode(registrationCode), Times.Once);

            var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(viewResult.Model);
            Assert.NotNull(controller.ViewData["StatusMessage"]);
            Assert.Equal(statusMessage, controller.ViewData["StatusMessage"].ToString());
        }

        [Fact]
        public async Task Register_Post_ValidModel_ValidRegistrationCode_ValidRegistrationCode_InvalidEmailAddress()
        {
            // Arrange
            string email = "email";
            string registrationCode = "ValidID";
            string password = "password";
            RegisterViewModel model = new RegisterViewModel
            {
                Email = email,
                RegistrationCode = registrationCode,
                Password = password
            };

            string userId = "userId";
            string emailAddress = "emailAddress";
            UserDTO user = new UserDTO
            {
                Id = userId,
                Email = emailAddress
            };

            usersRepoMock.Setup(u => u.UserByRegistrationCode(registrationCode))
                         .Returns(user);

            string statusMessage = "Ogiltigt inloggningsförsök.";

            // Act
            var result = await controller.Register(registrationCode, model);

            // Assert
            usersRepoMock.Verify(u => u.UserByRegistrationCode(registrationCode), Times.Once);

            var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(viewResult.Model);
            Assert.NotNull(controller.ViewData["StatusMessage"]);
            Assert.Equal(statusMessage, controller.ViewData["StatusMessage"].ToString());
        }

        [Fact]
        public async Task Register_Post_ValidModel_ValidRegistrationCode_ValidRegistrationCode_ValidEmailAddress()
        {
            // Arrange
            string email = "email";
            string registrationCode = "ValidID";
            string password = "password";
            RegisterViewModel model = new RegisterViewModel
            {
                Email = email,
                RegistrationCode = registrationCode,
                Password = password
            };

            string userId = "userId";
            UserDTO user = new UserDTO
            {
                Id = userId,
                Email = email
            };

            usersRepoMock.Setup(u => u.UserByRegistrationCode(registrationCode))
                         .Returns(user);

            requestSignInProviderMock.Setup(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                                     .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            controller.Url = new UrlHelper(new ActionContext());

            // Act
            var result = await controller.Register(registrationCode, model);

            // Assert
            usersRepoMock.Verify(u => u.UserByRegistrationCode(registrationCode), Times.Once);

            var viewResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);

            Assert.Null(controller.ViewData["StatusMessage"]);
        }

        #endregion

        #endregion

        #region Logout

        [Fact]
        public async Task Logout()
        {
            // Arrange
            requestSignInProviderMock.Setup(s => s.SignOutAsync())
                                     .Returns(Task.FromResult(IdentityResult.Success));
            loggerMock.Setup(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));

            // Act
            var result = await controller.Logout();

            // Assert
            requestSignInProviderMock.Verify(s => s.SignOutAsync(), Times.Once);
            loggerMock.Verify(l => l.LogInformation("User logged out."), Times.Once);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
        }

        #endregion

        #region ForgotPassword

        #region ForgotPassword - Get

        [Fact]
        public void ForgotPassword_Get()
        {
            // Arrange

            // Act
            var result = controller.ForgotPassword();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        #endregion

        #region ForgotPassword - Post

        [Fact]
        public async Task ForgotPassword_Post_InvalidModelState()
        {
            // Arrange
            controller.ModelState.AddModelError("Error", "Error");

            // Act
            var result = await controller.ForgotPassword(new ForgotPasswordViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ForgotPasswordViewModel>(viewResult.ViewData.Model);

            Assert.Null(model.Email);
        }

        [Fact]
        public async Task ForgotPassword_Post_UserDoesNotExist()
        {
            // Arrange
            var email = "email";
            ForgotPasswordViewModel model = new ForgotPasswordViewModel
            {
                Email = email
            };

            requestUserProviderMock.Setup(v => v.FindByEmailAsync(It.IsAny<string>()))
                                   .Returns(Task.FromResult<User>(null));

            // Act
            var result = await controller.ForgotPassword(model);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.ForgotPasswordConfirmation), viewResult.ActionName);
        }

        [Fact]
        public async Task ForgotPassword_Post_ValidUser()
        {
            // Arrange
            var email = "email";
            ForgotPasswordViewModel model = new ForgotPasswordViewModel
            {
                Email = email
            };

            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var url = new Mock<IUrlHelper>(MockBehavior.Strict);
            url.Setup(v => v.Action(It.IsAny<UrlActionContext>()))
               .Returns("callbackUrl")
               .Verifiable();

            context.Setup(v => v.Request)
                   .Returns(request.Object);
            request.Setup(v => v.Scheme)
                   .Returns("https");

            var ctrctx = new ControllerContext
            {
                HttpContext = context.Object
            };

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = "bcc",
                ReplyToEmail = "replyToEmail",
                ReplyToName = "replyToMail"
            };

            emailSenderMock.Setup(s => s.EmailSettings)
                           .Returns(emailSettings);

            requestUserProviderMock.Setup(v => v.FindByEmailAsync(It.IsAny<string>()))
                                   .Returns(Task.FromResult(new User()));
            requestUserProviderMock.Setup(v => v.GeneratePasswordResetTokenAsync(It.IsAny<User>()))
                                   .Returns(Task.FromResult("code"));
            emailSenderMock.Setup(e => e.SendEmailAsync(It.IsAny<String>(),
                                                        It.IsAny<string>(),
                                                        It.IsAny<string>()))
                           .Returns(Task.FromResult(IdentityResult.Success));

            controller.ControllerContext = ctrctx;
            controller.Url = url.Object;

            // Act
            var result = await controller.ForgotPassword(model);

            // Assert
            requestUserProviderMock.Verify(v => v.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            requestUserProviderMock.Verify(v => v.GeneratePasswordResetTokenAsync(It.IsAny<User>()), Times.Once);
            emailSenderMock.Verify(v => v.SendEmailAsync(It.IsAny<String>(),
                                                         It.IsAny<string>(),
                                                         It.IsAny<string>()), Times.Once);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(controller.ForgotPasswordConfirmation), viewResult.ActionName);
            Assert.Empty(emailSettings.Bcc);
            Assert.Empty(emailSettings.ReplyToEmail);
            Assert.Empty(emailSettings.ReplyToName);
        }

        #endregion

        #endregion

        #region ForgotPasswordConfirmation

        [Fact]
        public void ForgotPasswordConfirmation() {
            // Arrange

            // Act
            var result = controller.ForgotPasswordConfirmation();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region ResetPassword

        #region ResetPassword - Get

        [Fact]
        public void ResetPassword_Get_CodeIsNull()
        {
            // Arrange

            // Act

            // Assert
            Exception ex = Assert.Throws<ApplicationException>(() => controller.ResetPassword());
            Assert.Equal("A code must be supplied for password reset.", ex.Message);
        }

        [Fact]
        public void ResetPassword_Get_ValidCode()
        {
            // Arrange
            string code = "code";

            // Act
            var result = controller.ResetPassword(code);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ResetPasswordViewModel>(viewResult.ViewData.Model);

            Assert.Equal(code, model.Code);
            Assert.Null(model.ConfirmPassword);
            Assert.Null(model.Email);
            Assert.Null(model.Password);
        }

        #endregion

        #region ResetPassword - Post

        [Fact]
        public async Task ResetPassword_Post_InvalidModelState()
        {
            // Arrange
            controller.ModelState.AddModelError("error", "error");

            // Act
            var result = await controller.ResetPassword(new ResetPasswordViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(1, viewResult.ViewData.ModelState.ErrorCount);
        }

        [Fact]
        public async Task ResetPassword_Post_NoUser()
        {
            // Arrange
            string email = "email";

            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = email
            };

            requestUserProviderMock.Setup(v => v.FindByEmailAsync(It.IsAny<string>()))
                                   .Returns(Task.FromResult<User>(null));

            // Act
            var result = await controller.ResetPassword(model);

            // Assert
            requestUserProviderMock.Verify(v => v.FindByEmailAsync(email), Times.Once);
            requestUserProviderMock.Verify(v => v.ResetPasswordAsync(It.IsAny<User>(),
                                                                     It.IsAny<string>(),
                                                                     It.IsAny<string>()), Times.Never);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.ResetPasswordConfirmation), viewResult.ActionName);
        }

        [Fact]
        public async Task ResetPassword_Post_Success()
        {
            // Arrange
            string email = "email";
            string code = "code";
            string password = "password";

            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = email,
                Code = code,
                Password = password
            };


            requestUserProviderMock.Setup(v => v.FindByEmailAsync(It.IsAny<string>()))
                                   .Returns(Task.FromResult(new User()));

            requestUserProviderMock.Setup(v => v.ResetPasswordAsync(It.IsAny<User>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()))
                                   .Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await controller.ResetPassword(model);

            // Assert
            requestUserProviderMock.Verify(v => v.FindByEmailAsync(email), Times.Once);
            requestUserProviderMock.Verify(v => v.ResetPasswordAsync(It.IsAny<User>(),
                                                                     It.IsAny<string>(),
                                                                     It.IsAny<string>()), Times.Once);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.ResetPasswordConfirmation), viewResult.ActionName);
        }

        [Fact]
        public async Task ResetPassword_Post_Error()
        {
            // Assert
            string email = "email";

            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = email
            };

            requestUserProviderMock.Setup(v => v.FindByEmailAsync(It.IsAny<string>()))
                                   .Returns(Task.FromResult(new User()));
            requestUserProviderMock.Setup(v => v.ResetPasswordAsync(It.IsAny<User>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()))
                                   .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "error" })));

            // Act
            var result = await controller.ResetPassword(model);

            // Assert   
            requestUserProviderMock.Verify(v => v.FindByEmailAsync(email), Times.Once);
            requestUserProviderMock.Verify(v => v.ResetPasswordAsync(It.IsAny<User>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()), Times.Once);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(1, viewResult.ViewData.ModelState.ErrorCount);
        }

        #endregion

        #endregion

        #region ResetPasswordConfirmation

        [Fact]
        public void ResetPasswordConfirmation()
        {
            // Arrange

            // Act
            var result = controller.ResetPasswordConfirmation();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        #endregion

        #region ChangePassword

        #region ChangePassword - Get

        [Fact]
        public async Task ChangePassword_UserNull()
        {
            // Arrange
            User user = null;

            requestUserProviderMock.Setup(r => r.GetUserAsync())
                                   .Returns(Task.FromResult(user));

            try
            {
                // Act
                Exception ex = await Assert.ThrowsAsync<ApplicationException>(() => controller.ChangePassword());
            }
            catch (ApplicationException ex)
            {
                // Assert
                Assert.NotNull(ex);
            }
            finally
            {
                requestUserProviderMock.Verify(r => r.GetUserAsync(), Times.Once);
            }
        }

        [Fact]
        public async Task ChangePassword_UserNotNull()
        {
            // Arrange
            User user = new User();

            requestUserProviderMock.Setup(r => r.GetUserAsync())
                                   .Returns(Task.FromResult(user));

            //Act
            var result = await controller.ChangePassword();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ChangePasswordViewModel>(viewResult.Model);

            Assert.NotNull(model);
        }

        #endregion

        #region ChangePassword - Post

        [Fact]
        public async Task ChangePassword_InvalidModel()
        {
            // Arrange
            string oldPassword = "oldPassword";
            string newPassword = "newPassword";
            string confirmedPassword = "confirmedPassword";

            ChangePasswordViewModel inputModel = new ChangePasswordViewModel
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmedPassword
            };

            controller.ModelState.AddModelError("fakeError", "fakeErrorMessage");

            // Act
            var result = await controller.ChangePassword(inputModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ChangePasswordViewModel>(viewResult.Model);

            Assert.Equal(inputModel.OldPassword, model.OldPassword);
            Assert.Equal(inputModel.NewPassword, model.NewPassword);
            Assert.Equal(inputModel.ConfirmPassword, model.ConfirmPassword);
        }

        [Fact]
        public async Task ChangePassword_ValidModel_UserNull()
        {
            // Arrange
            string oldPassword = "oldPassword";
            string newPassword = "newPassword";
            string confirmedPassword = "confirmedPassword";

            ChangePasswordViewModel inputModel = new ChangePasswordViewModel
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmedPassword
            };

            User user = null;

            requestUserProviderMock.Setup(r => r.GetUserAsync())
                                   .Returns(Task.FromResult(user));

            try
            {
                // Act
                Exception ex = await Assert.ThrowsAsync<ApplicationException>(() => controller.ChangePassword(inputModel));
            }
            catch (ApplicationException ex)
            {
                // Assert
                Assert.NotNull(ex);
            }
            finally
            {
                requestUserProviderMock.Verify(r => r.GetUserAsync(), Times.Once);
            }
        }

        [Fact]
        public async Task ChangePassword_ValidModel_UserNotNull_ChangePasswordFailed()
        {
            // Arrange
            string oldPassword = "oldPassword";
            string newPassword = "newPassword";
            string confirmedPassword = "confirmedPassword";

            ChangePasswordViewModel inputModel = new ChangePasswordViewModel
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmedPassword
            };

            User user = new User();
            string errorDescription = "errorMessage";
            List<IdentityError> errors = new List<IdentityError>
            {
                new IdentityError { Description = errorDescription }
            };

            requestUserProviderMock.Setup(r => r.GetUserAsync())
                                   .Returns(Task.FromResult(user));
            requestUserProviderMock.Setup(r => r.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns(Task.FromResult(IdentityResult.Failed(errors.ToArray())));

            // Act
            var result = await controller.ChangePassword(inputModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ChangePasswordViewModel>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(inputModel, model);
            Assert.Equal(errorDescription, controller.ModelState[string.Empty].Errors.First().ErrorMessage);

            requestUserProviderMock.Verify(r => r.GetUserAsync(), Times.Once);
            requestUserProviderMock.Verify(r => r.ChangePasswordAsync(user, oldPassword, newPassword), Times.Once);
        }


        [Fact]
        public async Task ChangePassword_ValidModel_UserNotNull_ChangePasswordPassed()
        {
            // Arrange
            string oldPassword = "oldPassword";
            string newPassword = "newPassword";
            string confirmedPassword = "confirmedPassword";

            ChangePasswordViewModel inputModel = new ChangePasswordViewModel
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmedPassword
            };

            User user = new User();
            bool isPersistent = false;
            string authenticationMethod = null;

            string logInformationMessage = "User changed their password successfully.";

            requestUserProviderMock.Setup(r => r.GetUserAsync())
                                   .Returns(Task.FromResult(user));
            requestUserProviderMock.Setup(r => r.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns(Task.FromResult(IdentityResult.Success));
            requestSignInProviderMock.Setup(r => r.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<string>()))
                                     .Returns(Task.FromResult(IdentityResult.Success));
            loggerMock.Setup(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));

            // Act
            var result = await controller.ChangePassword(inputModel);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.ChangedPassword), viewResult.ActionName);

            requestUserProviderMock.Verify(r => r.GetUserAsync(), Times.Once);
            requestUserProviderMock.Verify(r => r.ChangePasswordAsync(user, oldPassword, newPassword), Times.Once);
            requestSignInProviderMock.Verify(r => r.SignInAsync(user, isPersistent, authenticationMethod), Times.Once);
            loggerMock.Verify(l => l.LogInformation(logInformationMessage), Times.Once);
        }

        #endregion

        #endregion

        #region ChangedPassword

        [Fact]
        public void ChangedPassword()
        {
            // Arrange

            // Act
            var result = controller.ChangedPassword();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region AccessDenied

        [Fact]
        public void AccessDeniedTest()
        {
            // Arrange

            // Act
            var result = controller.AccessDenied();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        #endregion
    }
}

