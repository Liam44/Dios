using Dios.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dios.Services
{
    public sealed class RequestUserProvider : IRequestUserProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public RequestUserProvider(IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return _userManager.AddToRoleAsync(user, role);
        }

        public Task<IdentityResult> CreateAsync(User user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<User> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return _userManager.GetRolesAsync(user);
        }

        public Task<User> GetUserAsync()
        {
            return _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
        }

        public string GetUserId()
        {
            return _userManager.GetUserId(_contextAccessor.HttpContext.User);
        }

        public Task<IdentityResult> ResetPasswordAsync(User user, string code, string password)
        {
            return _userManager.ResetPasswordAsync(user, code, password);
        }

        public Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }
    }
}
