using Dios.Extensions;
using Dios.Helpers;
using Dios.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DiosTest.Helpers
{
    public sealed class SendActivationLinkTest
    {
        [Fact]
        public async Task Send_EmailSenderNull()
        {
            // Arrange
            IEmailSender emailSender = null;
            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;
            string email = null;
            string registrationCode = null;
            string password = null;
            string scheme = null;

            // Act
            await SendActivationLink.Send(emailSender, urlHelper, email, registrationCode, password, scheme);

            // Assert
            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNull()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            IUrlHelper urlHelper = null;
            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;
            string email = null;
            string registrationCode = null;
            string password = null;
            string scheme = null;

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()), Times.Never);
            emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(),
                                                     It.IsAny<string>(),
                                                     It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNull()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;
            string email = null;
            string registrationCode = null;
            string password = null;
            string scheme = null;

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailEmpty()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;
            string email = string.Empty;
            string registrationCode = null;
            string password = null;
            string scheme = null;

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNull()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;
            string email = "someEmailAddress";
            string registrationCode = null;
            string password = null;
            string scheme = null;

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()), Times.Never);
            emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(),
                                                     It.IsAny<string>(),
                                                     It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeEmpty()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;
            string email = "someEmailAddress";
            string registrationCode = null;
            string password = null;
            string scheme = string.Empty;

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>()), Times.Never);
            emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(),
                                                     It.IsAny<string>(),
                                                     It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNotEmpty_RegistrationCodeNull_PasswordNull_CallbackUrlNull()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            string bcc = "someBcc";
            string replyToEmail = "someReplyToEmail";
            string replyToName = "someReplyToName";

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = bcc,
                ReplyToEmail = replyToEmail,
                ReplyToName = replyToName
            };

            emailSender.Setup(s => s.EmailSettings)
                       .Returns(emailSettings);

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            string email = "someEmailAddress";
            string registrationCode = null;
            string password = null;
            string scheme = "someScheme";

            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            string callbackUrl = null;

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(),
                                                                   It.IsAny<string>()))
                            .Returns(callbackUrl);

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            Assert.Equal(bcc, emailSettings.Bcc);
            Assert.Equal(replyToEmail, emailSettings.ReplyToEmail);
            Assert.Equal(replyToName, emailSettings.ReplyToName);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper,
                                                                    registrationCode,
                                                                    scheme), Times.Once);
            emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(),
                                                     It.IsAny<string>(),
                                                     It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNotEmpty_RegistrationCodeEmpty_PasswordEmpty_CallbackUrlNull()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            string bcc = "someBcc";
            string replyToEmail = "someReplyToEmail";
            string replyToName = "someReplyToName";

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = bcc,
                ReplyToEmail = replyToEmail,
                ReplyToName = replyToName
            };

            emailSender.Setup(s => s.EmailSettings)
                       .Returns(emailSettings);

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            string email = "someEmailAddress";
            string registrationCode = string.Empty;
            string password = string.Empty;
            string scheme = "someScheme";

            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            string callbackUrl = null;

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(),
                                                                   It.IsAny<string>()))
                            .Returns(callbackUrl);

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            Assert.Equal(bcc, emailSettings.Bcc);
            Assert.Equal(replyToEmail, emailSettings.ReplyToEmail);
            Assert.Equal(replyToName, emailSettings.ReplyToName);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper,
                                                                    registrationCode,
                                                                    scheme), Times.Once);
            emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(),
                                                     It.IsAny<string>(),
                                                     It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNotEmpty_RegistrationCodeNotEmpty_PasswordNotEmpty_CallbackUrlNull()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            string bcc = "someBcc";
            string replyToEmail = "someReplyToEmail";
            string replyToName = "someReplyToName";

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = bcc,
                ReplyToEmail = replyToEmail,
                ReplyToName = replyToName
            };

            emailSender.Setup(s => s.EmailSettings)
                       .Returns(emailSettings);

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            string email = "someEmailAddress";
            string registrationCode = "someRegistrationCode";
            string password = "somePassword";
            string scheme = "someScheme";

            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            string callbackUrl = null;

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(), 
                                                                   It.IsAny<string>()))
                            .Returns(callbackUrl);

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            Assert.Equal(bcc, emailSettings.Bcc);
            Assert.Equal(replyToEmail, emailSettings.ReplyToEmail);
            Assert.Equal(replyToName, emailSettings.ReplyToName);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper,
                                                                    registrationCode,
                                                                    scheme), Times.Once);
            emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(),
                                                     It.IsAny<string>(),
                                                     It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNotEmpty_RegistrationCodeNull_PasswordNull_CallbackUrlEmpty()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            string bcc = "someBcc";
            string replyToEmail = "someReplyToEmail";
            string replyToName = "someReplyToName";

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = bcc,
                ReplyToEmail = replyToEmail,
                ReplyToName = replyToName
            };

            emailSender.Setup(s => s.EmailSettings)
                       .Returns(emailSettings);

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            string email = "someEmailAddress";
            string registrationCode = null;
            string password = null;
            string scheme = "someScheme";

            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            string callbackUrl = string.Empty;

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(), 
                                                                   It.IsAny<string>()))
                            .Returns(callbackUrl);

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            Assert.Equal(bcc, emailSettings.Bcc);
            Assert.Equal(replyToEmail, emailSettings.ReplyToEmail);
            Assert.Equal(replyToName, emailSettings.ReplyToName);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper,
                                                                    registrationCode,
                                                                    scheme), Times.Once);
            emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(),
                                                     It.IsAny<string>(),
                                                     It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNotEmpty_RegistrationCodeEmpty_PasswordEmpty_CallbackUrlEmpty()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            string bcc = "someBcc";
            string replyToEmail = "someReplyToEmail";
            string replyToName = "someReplyToName";

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = bcc,
                ReplyToEmail = replyToEmail,
                ReplyToName = replyToName
            };

            emailSender.Setup(s => s.EmailSettings)
                       .Returns(emailSettings);

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            string email = "someEmailAddress";
            string registrationCode = string.Empty;
            string password = string.Empty;
            string scheme = "someScheme";

            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            string callbackUrl = string.Empty;

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(), 
                                                                   It.IsAny<string>()))
                            .Returns(callbackUrl);

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            Assert.Equal(bcc, emailSettings.Bcc);
            Assert.Equal(replyToEmail, emailSettings.ReplyToEmail);
            Assert.Equal(replyToName, emailSettings.ReplyToName);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper,
                                                                    registrationCode,
                                                                    scheme), Times.Once);
            emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(),
                                                     It.IsAny<string>(),
                                                     It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNotEmpty_RegistrationCodeNotEmpty_PasswordNotEmpty_CallbackUrlEmpty()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            string bcc = "someBcc";
            string replyToEmail = "someReplyToEmail";
            string replyToName = "someReplyToName";

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = bcc,
                ReplyToEmail = replyToEmail,
                ReplyToName = replyToName
            };

            emailSender.Setup(s => s.EmailSettings)
                       .Returns(emailSettings);

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            string email = "someEmailAddress";
            string registrationCode = "someRegistrationCode";
            string password = "somePassword";
            string scheme = "someScheme";

            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            string callbackUrl = string.Empty;

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(),
                                                                   It.IsAny<string>()))
                            .Returns(callbackUrl);

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            Assert.Equal(bcc, emailSettings.Bcc);
            Assert.Equal(replyToEmail, emailSettings.ReplyToEmail);
            Assert.Equal(replyToName, emailSettings.ReplyToName);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper,
                                                                    registrationCode,
                                                                    scheme), Times.Once);
            emailSender.Verify(e => e.SendEmailAsync(It.IsAny<string>(),
                                                     It.IsAny<string>(),
                                                     It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNotEmpty_RegistrationCodeNull_PasswordNull_CallbackUrlNotEmpty()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            string bcc = "someBcc";
            string replyToEmail = "someReplyToEmail";
            string replyToName = "someReplyToName";

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = bcc,
                ReplyToEmail = replyToEmail,
                ReplyToName = replyToName
            };

            emailSender.Setup(s => s.EmailSettings)
                       .Returns(emailSettings);

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            string email = "someEmailAddress";
            string registrationCode = null;
            string password = null;
            string scheme = "someScheme";

            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            string callbackUrl = "someCallbackUrl";

            string subject = "Diös - Kontot skapades";
            string message = $"Var god och klicka på länken för att aktivera ditt konto: <a href='{callbackUrl}'>länk</a>" +
                             $"<p>Användarsnamn: {email}</p>" +
                             $"<p>Lösenord: {password}</p>";

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(), 
                                                                   It.IsAny<string>()))
                            .Returns(callbackUrl);

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            Assert.Empty(emailSettings.Bcc);
            Assert.Empty(emailSettings.ReplyToEmail);
            Assert.Empty(emailSettings.ReplyToName);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper,
                                                                    registrationCode,
                                                                    scheme), Times.Once);
            emailSender.Verify(e => e.SendEmailAsync(email, subject, message), Times.Once);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNotEmpty_RegistrationCodeEmpty_PasswordEmpty_CallbackUrlNotEmpty()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            string bcc = "someBcc";
            string replyToEmail = "someReplyToEmail";
            string replyToName = "someReplyToName";

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = bcc,
                ReplyToEmail = replyToEmail,
                ReplyToName = replyToName
            };

            emailSender.Setup(s => s.EmailSettings)
                       .Returns(emailSettings);

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            string email = "someEmailAddress";
            string registrationCode = string.Empty;
            string password = string.Empty;
            string scheme = "someScheme";

            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            string callbackUrl = "someCallbackUrl";

            string subject = "Diös - Kontot skapades";
            string message = $"Var god och klicka på länken för att aktivera ditt konto: <a href='{callbackUrl}'>länk</a>" +
                             $"<p>Användarsnamn: {email}</p>" +
                             $"<p>Lösenord: {password}</p>";

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(), 
                                                                   It.IsAny<string>()))
                            .Returns(callbackUrl);

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            Assert.Empty(emailSettings.Bcc);
            Assert.Empty(emailSettings.ReplyToEmail);
            Assert.Empty(emailSettings.ReplyToName);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper,
                                                                    registrationCode,
                                                                    scheme), Times.Once);
            emailSender.Verify(e => e.SendEmailAsync(email, subject, message), Times.Once);
        }

        [Fact]
        public async Task Send_EmailSenderNotNull_UrlHelperNotNull_EmailNotEmpty_SchemeNotEmpty_RegistrationCodeNotEmpty_PasswordNotEmpty_CallbackUrlNotEmpty()
        {
            // Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            string bcc = "someBcc";
            string replyToEmail = "someReplyToEmail";
            string replyToName = "someReplyToName";

            EmailSettings emailSettings = new EmailSettings
            {
                Bcc = bcc,
                ReplyToEmail = replyToEmail,
                ReplyToName = replyToName
            };

            emailSender.Setup(s => s.EmailSettings)
                       .Returns(emailSettings);

            IUrlHelper urlHelper = new Mock<IUrlHelper>().Object;
            string email = "someEmailAddress";
            string registrationCode = "someRegistrationCode";
            string password = "somePassword";
            string scheme = "someScheme";

            Mock<IUrlHelperWrapper> urlHelperWrapper = new Mock<IUrlHelperWrapper>();
            Dios.Extensions.UrlHelperExtensions.UrlHelperWrapper = urlHelperWrapper.Object;

            string callbackUrl = "someCallbackUrl";

            string subject = "Diös - Kontot skapades";
            string message = $"Var god och klicka på länken för att aktivera ditt konto: <a href='{callbackUrl}'>länk</a>" +
                             $"<p>Användarsnamn: {email}</p>" +
                             $"<p>Lösenord: {password}</p>";

            urlHelperWrapper.Setup(u => u.GenerateRegistrationLink(It.IsAny<IUrlHelper>(),
                                                                   It.IsAny<string>(), 
                                                                   It.IsAny<string>()))
                            .Returns(callbackUrl);

            // Act
            await SendActivationLink.Send(emailSender.Object, urlHelper, email, registrationCode, password, scheme);

            // Assert
            Assert.Empty(emailSettings.Bcc);
            Assert.Empty(emailSettings.ReplyToEmail);
            Assert.Empty(emailSettings.ReplyToName);

            urlHelperWrapper.Verify(u => u.GenerateRegistrationLink(urlHelper,
                                                                    registrationCode,
                                                                    scheme), Times.Once);
            emailSender.Verify(e => e.SendEmailAsync(email, subject, message), Times.Once);
        }
    }
}
