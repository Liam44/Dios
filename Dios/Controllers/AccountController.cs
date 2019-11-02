using Dios.Extensions;
using Dios.Models;
using Dios.Repositories;
using Dios.Services;
using Dios.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dios.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public sealed class AccountController : Controller
    {
        private readonly IRequestUserProvider _requestUserProvider;
        private readonly IRequestSignInProvider _requestSignInProvider;
        private readonly IEmailSender _emailSender;
        private readonly ILog<AccountController> _logger;
        private readonly IUsersRepository _usersRepository;

        public AccountController(IRequestUserProvider requestUserProvider,
                                 IRequestSignInProvider requestSignInProvider,
                                 IEmailSender emailSender,
                                 ILog<AccountController> logger,
                                 IUsersRepository usersRepository)
        {
            _requestUserProvider = requestUserProvider;
            _requestSignInProvider = requestSignInProvider;
            _emailSender = emailSender;
            _logger = logger;
            _usersRepository = usersRepository;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _requestSignInProvider.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ogiltigt inloggningsförsök.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{code}")]
        public IActionResult Register(string code)
        {
            // Checks that there actually is any user associated to the given registration code
            UserDTO user = _usersRepository.UserByRegistrationCode(code);

            if (user == null)
            {
                return NotFound();
            }

            return View(new RegisterViewModel { RegistrationCode = code });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("{code}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string code, RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.Compare(code, model.RegistrationCode, false) != 0)
            {
                ViewData["StatusMessage"] = "Ogiltig registreringskod.";

                return View();
            }

            // Checks that there actually is any user associated to the given registration code
            UserDTO user = _usersRepository.UserByRegistrationCode(model.RegistrationCode);

            if (user == null)
            {
                ViewData["StatusMessage"] = "Ogiltigt inloggningsförsök.";

                return View();
            }

            // Checks that the given email address actually matches the one associated with the registration code
            if (string.Compare(model.Email, user.Email, false) != 0)
            {
                ViewData["StatusMessage"] = "Ogiltigt inloggningsförsök.";

                return View();
            }

            // We now only need to get the user logged in
            return await Login(new LoginViewModel { Email = model.Email, Password = model.Password, RememberMe = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _requestSignInProvider.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _requestUserProvider.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _requestUserProvider.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);

                _emailSender.EmailSettings.Bcc = string.Empty;
                _emailSender.EmailSettings.ReplyToEmail = string.Empty;
                _emailSender.EmailSettings.ReplyToName = string.Empty;

                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _requestUserProvider.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _requestUserProvider.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _requestUserProvider.GetUserAsync();
            if (user == null)
            {
                throw new ApplicationException(/*$"Unable to load user with ID '{_requestUserProvider.GetUserId()}'."*/);
            }

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _requestUserProvider.GetUserAsync();
            if (user == null)
            {
                throw new ApplicationException(/*$"Unable to load user with ID '{_requestUserProvider.GetUserId()}'."*/);
            }

            // Checks that the new password actually is different from the previous one
            if (string.Compare(model.OldPassword, model.NewPassword, false) == 0)
            {
                return View(model);
            }

            var changePasswordResult = await _requestUserProvider.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _requestSignInProvider.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");

            // If it's the first time the user logs in, the registration code has to be reset
            UserDTO userDTO = new UserDTO(user)
            {
                RegistrationCode = string.Empty
            };
            _usersRepository.Edit(userDTO);

            return RedirectToAction(nameof(ChangedPassword));
        }

        public IActionResult ChangedPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
