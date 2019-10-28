using Dios.ViewModels.UsersViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dios.Services
{
    public interface INewUser
    {
        Task<Dictionary<string, string>> Create(UserCreateVM user,
                                                string role,
                                                string registrationCode,
                                                string password,
                                                IRequestUserProvider requestUserProvider,
                                                IEmailSender emailSender,
                                                IUrlHelper url,
                                                string scheme);
    }
}
