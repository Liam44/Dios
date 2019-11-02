using Dios.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Dios.Services
{
    public sealed class RequestSignInProvider : IRequestSignInProvider
    {
        private readonly SignInManager<User> _signInManager;

        public RequestSignInProvider(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public Task<SignInResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return _signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure);
        }

        public Task SignInAsync(User user, bool isPersistent, string authenticationMethod = null)
        {
            return _signInManager.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public Task SignOutAsync()
        {
            return _signInManager.SignOutAsync();
        }
    }
}