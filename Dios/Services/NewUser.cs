using Dios.Helpers;
using Dios.Models;
using Dios.ViewModels.UsersViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dios.Services
{
    public class NewUser : INewUser
    {
        public async Task<Dictionary<string, string>> Create(UserCreateVM user,
                                                             string role,
                                                             string registrationCode,
                                                             string password,
                                                             IRequestUserProvider requestUserProvider,
                                                             IEmailSender emailSender,
                                                             IUrlHelper url,
                                                             string scheme)
        {
            Dictionary<string, string> create = new Dictionary<string, string>();

            User newUser = new User
            {
                UserName = user.Email,
                PersonalNumber = user.PersonalNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PhoneNumber2 = user.PhoneNumber2,
                RegistrationCode = registrationCode
            };

            var result = await requestUserProvider.CreateAsync(newUser, password);

            if (result != IdentityResult.Success)
            {
                List<string> errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    switch (error.Code)
                    {
                        case "DuplicateUserName":
                            create.Add(nameof(user.Email), "En användare med samma e-post finns redan.");
                            break;
                    }
                }

                return create;
            }

            await requestUserProvider.AddToRoleAsync(newUser, role);

            // Once the user has been created and given a role,
            // those information are automatically sent to the email that has been given
            await SendActivationLink.Send(emailSender, url, user.Email, registrationCode, password, scheme);

            user.Id = newUser.Id;

            return create;
        }
    }
}
