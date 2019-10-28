using Dios.Helpers;
using System.Threading.Tasks;

namespace Dios.Services
{
    public interface IEmailSender
    {
        EmailSettings EmailSettings { get; set; }
        Task SendEmailAsync(string email, string subject, string message);
    }
}
