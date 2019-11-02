using Dios.Extensions;
using Dios.Models;
using Dios.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Dios.Controllers
{
    public sealed class HomeController : Controller
    {
        IUsersRepository _usersRepository;

        public HomeController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction(nameof(AccountController.Login), "Account");
            else if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(AddressesController.Index), "Addresses");
            else
            {
                // Checks if the user has just logged in for the first time
                UserDTO user = _usersRepository.User(User.Id());

                if (!string.IsNullOrEmpty(user.RegistrationCode))
                {
                    // If so, they're redirected to the "Change password" view
                    return RedirectToAction(nameof(AccountController.ChangePassword), "Account");
                }

                // Otherwise, they're redirected to the "Flats/Index" view depending on their current role
                if (User.IsInRole("User"))
                {
                    return RedirectToAction(nameof(UsersController.Flats), "Users");
                }
                else
                {
                    return RedirectToAction(nameof(HostsController.Addresses), "Hosts");
                }

            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
