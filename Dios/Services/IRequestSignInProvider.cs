using Dios.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Dios.Services
{
    public interface IRequestSignInProvider
    {
        Task<SignInResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignInAsync(User user, bool isPersistent, string authenticationMethod = null);
        Task SignOutAsync();
    }
}
