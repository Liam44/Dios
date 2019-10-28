using Dios.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dios.Services
{
    public interface IRequestUserProvider
    {
        string GetUserId();
        Task<User> GetUserAsync();
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<IdentityResult> ResetPasswordAsync(User user, string code, string password);
        Task<User> FindByEmailAsync(string email);
        Task<IList<string>> GetRolesAsync(User user);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    }
}
