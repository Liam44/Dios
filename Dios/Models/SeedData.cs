using Dios.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Dios.Models
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            #region Roles

            // Looks for any admin role
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(store, null, null, null, null);

                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Looks for any user role
            if (!context.Roles.Any(r => r.Name == "User"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(store, null, null, null, null);

                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Looks for any host role
            if (!context.Roles.Any(r => r.Name == "Host"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(store, null, null, null, null);

                await roleManager.CreateAsync(new IdentityRole("Host"));
            }
            
            #endregion

            #region Users

            if (!context.Users.Any(u => u.UserName == "admin@millbyte.se"))
            {
                var store = new UserStore<User>(context);
                var userManager = new UserManager<User>(store, null, new PasswordHasher<User>(), null, null, null, null, null, null);
                var newuser = new User
                {
                    UserName = "admin@millbyte.se",
                    PersonalNumber = string.Empty,
                    Email = "admin@millbyte.se",
                    FirstName = "Admin",
                    LastName = "Admin",
                    RegistrationCode = string.Empty
                };

                await userManager.CreateAsync(newuser, "Admin-password1");
                await userManager.AddToRoleAsync(newuser, "Admin");
            }

            #endregion
        }
    }
}
