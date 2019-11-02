using Dios.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Dios.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public sealed class EmailSender : IEmailSender
    {
        public EmailSettings EmailSettings { get; set; }

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            EmailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            Execute(email, subject, message).Wait();
            return Task.FromResult(0);
        }

        public async Task Execute(string email, string subject, string message)
        {
            try
            {
                // The email address the mail is sent to must be provided!
                if (string.IsNullOrEmpty(email))
                    return;

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(EmailSettings.NetworkCredentials.UserName, "Diös")
                };

                mail.To.Add(new MailAddress(email));

                if (!string.IsNullOrEmpty(EmailSettings.Bcc))
                    mail.Bcc.Add(new MailAddress(EmailSettings.Bcc));

                if (!string.IsNullOrEmpty(EmailSettings.ReplyToEmail))
                    mail.ReplyToList.Add(new MailAddress(EmailSettings.ReplyToEmail, EmailSettings.ReplyToName));

                mail.SubjectEncoding = Encoding.UTF8;
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                mail.BodyEncoding = Encoding.UTF8;

                using (SmtpClient smtp = new SmtpClient
                {
                    Host = EmailSettings.MailServer,
                    Port = EmailSettings.Port,
                    EnableSsl = EmailSettings.EnableSsl,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(EmailSettings.NetworkCredentials.UserName,
                                                        EmailSettings.NetworkCredentials.Password)
                })
                {
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception)
            {
                //do something here
            }
        }
    }
}
