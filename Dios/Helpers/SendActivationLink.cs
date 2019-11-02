using Dios.Extensions;
using Dios.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dios.Helpers
{
    public static class SendActivationLink
    {
        public static async Task Send(IEmailSender emailSender, IUrlHelper url, string email, string registrationCode, string password, string scheme)
        {
            if (emailSender == null)
            {
                return;
            }

            if (url == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(scheme))
            {
                return;
            }

            var callbackUrl = url.GenerateRegistrationLink(registrationCode, scheme);

            if (string.IsNullOrEmpty(callbackUrl))
            {
                return;
            }

            emailSender.EmailSettings.Bcc = string.Empty;
            emailSender.EmailSettings.ReplyToEmail = string.Empty;
            emailSender.EmailSettings.ReplyToName = string.Empty;

            await emailSender.SendEmailAsync(email, "Diös - Kontot skapades",
               $"Var god och klicka på länken för att aktivera ditt konto: <a href='{callbackUrl}'>länk</a>" +
               $"<p>Användarsnamn: {email}</p>" +
               $"<p>Lösenord: {password}</p>");
        }
    }
}
