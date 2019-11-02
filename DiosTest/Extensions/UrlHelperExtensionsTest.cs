using Dios.Controllers;
using Dios.Exceptions;
using Dios.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Xunit;
using UrlHelperExtensions = Dios.Extensions.UrlHelperExtensions;

namespace DiosTest.Extensions
{
    public sealed class UrlHelperExtensionsTest
    {
        #region UrlHelperWrapper

        [Fact]
        public void ResetPasswordCallbackLink()
        {
            // Arrange
            UrlHelperWrapper urlHelperWrapper = new UrlHelperWrapper();
            string actionResult = "someActionResult";

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                     .Returns(actionResult)
                     .Verifiable();
            IUrlHelper url = urlHelper.Object;

            string userId = "someUserId";
            string code = "someCode";
            string scheme = "someScheme";

            string actionName = nameof(AccountController.ResetPassword);

            // Act
            urlHelperWrapper.ResetPasswordCallbackLink(url, userId, code, scheme);

            // Assert
            Assert.NotEmpty(url.Action(actionName));
            Assert.Equal(actionResult, url.Action(actionName));
        }

        [Fact]
        public void GenerateRegistrationLink()
        {
            // Arrange
            UrlHelperWrapper urlHelperWrapper = new UrlHelperWrapper();
            string actionResult = "someActionResult";

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                     .Returns(actionResult)
                     .Verifiable();
            IUrlHelper url = urlHelper.Object;

            string code = "someCode";
            string scheme = "someScheme";

            string actionName = nameof(AccountController.ResetPassword);

            // Act
            urlHelperWrapper.GenerateRegistrationLink(url, code, scheme);

            // Assert
            Assert.NotEmpty(url.Action(actionName));
            Assert.Equal(actionResult, url.Action(actionName));
        }

        #endregion

        #region UrlHelperExtensions

        [Fact]
        public void ResetPasswordCallbackLink_UrlHelperWrapperNull()
        {
            // Arrange
            IUrlHelperWrapper urlHelperWrapper = null;
            UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper;

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;

            string userId = "someUserId";
            string code = "someCode";
            string scheme = "someScheme";

            // Act
            UrlHelperWrapperUndefinedException exception = Assert.Throws<UrlHelperWrapperUndefinedException>(() =>
                                                           urlHelper.ResetPasswordCallbackLink(userId, 
                                                                                               code, 
                                                                                               scheme));

            // Assert
            Assert.Equal("UrlHelperWrapper is null in UrlHelperExtensions.", exception.Message);
        }

        [Fact]
        public void ResetPasswordCallbackLink_UrlHelperWrapperNotNull()
        {
            // Arrange
            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;

            string userId = "someUserId";
            string code = "someCode";
            string scheme = "someScheme";

            string callbackLink = "someCallbackLink";

            urlHelperWrapper.Setup(u => u.ResetPasswordCallbackLink(It.IsAny<IUrlHelper>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()))
                            .Returns(callbackLink);

            // Act
            string result = urlHelper.ResetPasswordCallbackLink(userId, code, scheme);

            // Assert
            Assert.Equal(callbackLink, result);

            urlHelperWrapper.Verify(u => u.ResetPasswordCallbackLink(urlHelper, userId, code, scheme), Times.Once);
        }

        [Fact]
        public void GenerateRegistrationLink_UrlHelperWrapperNull()
        {
            // Arrange
            IUrlHelperWrapper urlHelperWrapper = null;
            UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper;

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;

            string code = "someCode";
            string scheme = "someScheme";

            // Act
            UrlHelperWrapperUndefinedException exception = Assert.Throws<UrlHelperWrapperUndefinedException>(() => 
                                                           urlHelper.GenerateRegistrationLink(code, 
                                                                                              scheme));

            // Assert
            Assert.Equal("UrlHelperWrapper is null in UrlHelperExtensions.", exception.Message);
        }

        [Fact]
        public void GenerateRegistrationLink_UrlHelperWrapperNotNull()
        {
            // Arrange
            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;

            string code = "someCode";
            string scheme = "someScheme";

            string registrationLink = "someRegistrationLink";

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(),
                                                                   It.IsAny<string>()))
                            .Returns(registrationLink);

            // Act
            string result = urlHelper.GenerateRegistrationLink(code, scheme);

            // Assert
            Assert.Equal(registrationLink, result);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper, code, scheme), Times.Once);
        }

        #endregion
    }
}
