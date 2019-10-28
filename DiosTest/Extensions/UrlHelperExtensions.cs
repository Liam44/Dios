using Dios.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Xunit;

namespace DiosTest.Extensions
{
    public class UrlHelperExtensions
    {
        [Fact]
        public void ResetPasswordCallbackLink()
        {
            // Arrange
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
            url.ResetPasswordCallbackLink(userId, code, scheme);

            // Assert
            Assert.NotEmpty(url.Action(actionName));
            Assert.Equal(actionResult, url.Action(actionName));
        }

        [Fact]
        public void GenerateRegistrationLink()
        {
            // Arrange
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
            url.GenerateRegistrationLink(code, scheme);

            // Assert
            Assert.NotEmpty(url.Action(actionName));
            Assert.Equal(actionResult, url.Action(actionName));
        }
    }
}
