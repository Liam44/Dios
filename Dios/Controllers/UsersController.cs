using Dios.Extensions;
using Dios.Models;
using Dios.Repositories;
using Dios.Services;
using Dios.ViewModels;
using Dios.ViewModels.ErrorReports;
using Dios.ViewModels.UsersViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dios.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public sealed class UsersController : Controller
    {
        private const string ROLE = "User";

        private readonly IRequestUserProvider _requestUserProvider;
        private readonly IEmailSender _emailSender;
        private readonly IUsersRepository _usersRepository;
        private readonly IAddressesRepository _addressesRepository;
        private readonly IFlatsRepository _flatsRepository;
        private readonly IParametersRepository _parametersRepository;
        private readonly IErrorReportsRepository _errorRepository;
        private readonly INewUser _newUser;

        public UsersController(IRequestUserProvider requestUserProvider,
                               IEmailSender emailSender,
                               IUsersRepository usersRepository,
                               IAddressesRepository addressesRepository,
                               IFlatsRepository flatsRepository,
                               IParametersRepository parameterRepository,
                               IErrorReportsRepository errorRepository,
                               INewUser newUser)
        {
            _requestUserProvider = requestUserProvider;
            _emailSender = emailSender;
            _usersRepository = usersRepository;
            _addressesRepository = addressesRepository;
            _flatsRepository = flatsRepository;
            _parametersRepository = parameterRepository;
            _errorRepository = errorRepository;
            _newUser = newUser;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = ROLE)]
        public IActionResult ErrorReports(int flatId)
        {
            FlatDTO flat = _flatsRepository.Flat(flatId);

            if (flat == null)
            {
                return NotFound();
            }

            flat.Include(address: _addressesRepository.Address(flat.AddressID));

            ErrorReportsVM model = new ErrorReportsVM
            {
                FlatId = flatId,
                Flat = flat.Number + (flat.Address == null ? string.Empty : $", {flat.Address.Street} {flat.Address.Number}"),
                ErrorReports = _errorRepository.ErrorReports(flatId)?.ToList()
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = ROLE)]
        public IActionResult ErrorReport(int flatId)
        {
            FlatDTO flat = _flatsRepository.Flat(flatId);

            if (flat == null)
            {
                return NotFound();
            }

            AddressDTO addressDTO = _addressesRepository.Address(flat.AddressID);

            flat.Include(address: addressDTO);

            ErrorReportCreateVM model = new ErrorReportCreateVM
            {
                FlatId = flatId,
                Flat = flat.Number + (addressDTO == null ? string.Empty : $", {addressDTO.Street} {addressDTO.Number}")
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = ROLE)]
        public IActionResult ErrorReport(ErrorReportCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model == null)
            {
                return View(new ErrorReportCreateVM());
            }

            FlatDTO flat = _flatsRepository.Flat(model.FlatId);

            if (flat == null)
            {
                return NotFound();
            }

            ErrorReportDTO report = new ErrorReportDTO
            {
                Description = model.Description,
                Subject = model.Subject,
                FlatId = model.FlatId
            };

            _errorRepository.Add(report);

            return RedirectToAction(nameof(Flats));
        }

        public UserIndexVM GetUser(string userId)
        {
            return new UserIndexVM(_usersRepository.User(userId));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new UserCreateVM() { IsPhoneNumber2Visible = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(UserCreateVM userVM)
        {
            if (!ModelState.IsValid)
            {
                return View(userVM);
            }

            // Checks if the given personal number doesn't match with another user
            UserDTO userDTOPN = _usersRepository.UserByPersonalNumber(userVM.PersonalNumber, ROLE);
            if (userDTOPN != null)
            {
                ModelState.AddModelError(nameof(userVM.PersonalNumber), "En användare med samma personnummer finns redan i databasen.");

                return View(userVM);
            }

            // An authentication code is generated, along with a default password
            string registrationCode = _usersRepository.GenerateRegistrationCode();
            string password = _usersRepository.GeneratePassword();

            Dictionary<string, string> result = await _newUser.Create(userVM,
                                                                      ROLE,
                                                                      registrationCode,
                                                                      password,
                                                                      _requestUserProvider,
                                                                      _emailSender,
                                                                      Url,
                                                                      Request.Scheme);

            if (result.Count > 0)
            {
                foreach (string key in result.Keys)
                {
                    ModelState.AddModelError(key, result[key]);
                }

                return View(userVM);
            }

            return RedirectToAction(nameof(UsersController.Index));
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Admin")]
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserDTO user = _usersRepository.User(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Include(_parametersRepository.Parameters(id)?.ToList());

            return View(new UserDetailsVM(user, false, GetFlats(id)));
        }

        [Authorize(Roles = "User,Host")]
        public IActionResult Flats()
        {
            string userId = User.Id();
            IEnumerable<FlatDTO> flats = _flatsRepository.Flats(userId)?
                                                         .Select(f =>
                                                         {
                                                             f.Include(_parametersRepository.Parameters(f.ID)?.ToList(),
                                                                       _addressesRepository.Address(f.AddressID));
                                                             return f;
                                                         })
                                                         .ToList();

            return View(flats.GroupBy(f => f.Floor)
                             .ToDictionary(fg => fg.Key, fg => fg.Select(f => new FlatDetailsVM(f,
                                                                                                _parametersRepository.Get(userId, f.ID)))
                                                                 .OrderBy(f => f.Number)
                                                                 .ToList()));
        }

        public async Task SendMessage([FromBody]SendMessageVM sendMessageVM)
        {
            // A model must be provided
            if (sendMessageVM == null)
            {
                return;
            }

            // A 'To' address must be provided
            if (string.IsNullOrEmpty(sendMessageVM.To))
            {
                return;
            }

            // A message must be provided
            if (string.IsNullOrEmpty(sendMessageVM.Message))
            {
                return;
            }

            // The user the message is sent to must be valid
            UserDTO userTo = _usersRepository.User(sendMessageVM.To);
            if (userTo == null)
            {
                return;
            }

            // This case should never happen, but one never knows...
            User userFrom = await _requestUserProvider.GetUserAsync();
            if (userFrom == null)
            {
                return;
            }

            _emailSender.EmailSettings.Bcc = userFrom.Email;
            _emailSender.EmailSettings.ReplyToEmail = userFrom.Email;
            _emailSender.EmailSettings.ReplyToName = userFrom.ToString();

            await _emailSender.SendEmailAsync(userTo.Email,
                                              $"Meddelande från {userFrom.ToString()}",
                                              sendMessageVM.Message);
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Edit(string id = null)
        {
            // If the user's ID is not provided, the current user's profile is displayed
            bool canChangePassword = false;
            if (string.IsNullOrEmpty(id))
            {
                id = User.Id();
                canChangePassword = true;
            }

            var user = _usersRepository.User(id);

            if (user == null)
            {
                throw new ApplicationException(/*$"Unable to load user with ID '{_userManager.GetUserId(User)}'."*/);
            }

            var roles = await _requestUserProvider.GetRolesAsync(_usersRepository.GetUser(User.Id()));

            var model = new UserCreateVM
            {
                Id = id,
                PersonalNumber = user.PersonalNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CanChangeName = id != null && roles != null && !roles.Contains("Admin"),
                CanChangePassword = canChangePassword,
                PhoneNumber = user.PhoneNumber,
                IsPhoneNumber2Visible = false,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UserDTO userDTO = _usersRepository.User(model.Id);

            if (userDTO == null)
            {
                throw new ApplicationException();
            }

            // Checks if the given personal number doesn't match with another user
            UserDTO userDTOPN = _usersRepository.UserByPersonalNumber(model.PersonalNumber, ROLE);
            if (userDTOPN != null && string.Compare(userDTO.Id, userDTOPN.Id, false) != 0)
            {
                ModelState.AddModelError(nameof(model.PersonalNumber), "En användare med samma personnummer finns redan i databasen.");

                return View(model);
            }

            bool edited = false;
            bool recreated = false;

            if (model.Email != userDTO.Email)
            {
                if (string.IsNullOrEmpty(userDTO.RegistrationCode))
                {
                    userDTO.NormalizedEmail = model.Email.ToUpper();
                    userDTO.Email = model.Email;
                    edited = true;
                }
                else
                {
                    // The user hasn't activated their account yet
                    // A new user then needs to be created

                    // An authentication code is generated, along with a default password
                    string registrationCode = _usersRepository.GenerateRegistrationCode();
                    string password = _usersRepository.GeneratePassword();

                    string previousUserId = model.Id;

                    Dictionary<string, string> result = await _newUser.Create(model,
                                                                              ROLE,
                                                                              registrationCode,
                                                                              password,
                                                                              _requestUserProvider,
                                                                              _emailSender,
                                                                              Url,
                                                                              Request.Scheme);

                    if (result.Count > 0)
                    {
                        foreach (string key in result.Keys)
                        {
                            ModelState.AddModelError(key, result[key]);
                        }

                        return View(model);
                    }

                    _usersRepository.Delete(previousUserId);

                    recreated = true;
                }
            }

            if (!recreated)
            {
                if (model.PersonalNumber != userDTO.PersonalNumber)
                {
                    userDTO.PersonalNumber = model.PersonalNumber;
                    edited = true;
                }

                if (model.FirstName != userDTO.FirstName)
                {
                    userDTO.FirstName = model.FirstName;
                    edited = true;
                }

                if (model.LastName != userDTO.LastName)
                {
                    userDTO.LastName = model.LastName;
                    edited = true;
                }

                if (model.PhoneNumber != userDTO.PhoneNumber)
                {
                    userDTO.PhoneNumber = model.PhoneNumber;
                    edited = true;
                }

                if (edited)
                {
                    _usersRepository.Edit(userDTO);
                }
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userName = "Din";

            if (userId != userDTO.Id)
            {
                userName = userDTO.FirstName;
                if (string.IsNullOrEmpty(userName))
                {
                    userName = userDTO.ToString();
                }

                userName += "s";
            }

            StatusMessage = userName + " profil uppdateras!";
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserDTO user = _usersRepository.User(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Include(_parametersRepository.Parameters(id).ToList());

            return View(new UserDetailsVM(user, false, GetFlats(id)));
        }

        // POST: Users/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var user = _usersRepository.User(id);

            if (user == null)
            {
                return NotFound();
            }

            _usersRepository.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Hosts/Remove/5
        [Authorize(Roles = "Admin")]
        public IActionResult Remove(string id, int flatId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var host = _usersRepository.User(id);
            if (host == null)
            {
                return NotFound();
            }

            FlatDTO flat = _flatsRepository.Flat(flatId);
            if (flat == null)
            {
                return NotFound();
            }

            ParameterDTO parameter = _parametersRepository.Get(id, flatId);
            if (parameter == null)
            {
                return NotFound();
            }

            parameter.Flat.Include(address: _addressesRepository.Address(parameter.Flat.AddressID));

            return View(new ParameterDetailsVM(parameter));
        }

        // POST: Hosts/Remove/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveConfirmed(string id, int flatId)
        {
            var parameter = _parametersRepository.Get(id, flatId);

            if (parameter == null)
            {
                return NotFound();
            }

            _parametersRepository.Delete(id, flatId);

            return RedirectToAction(nameof(Details), new { id });
        }

        #region API

        public IEnumerable<UserIndexVM> GetUsers()
        {
            IEnumerable<UserDTO> users = _usersRepository.Users();

            if (users == null)
            {
                return new List<UserIndexVM>();
            }

            return users.Select(u => new UserIndexVM(u))
                        .ToList();
        }

        [AllowAnonymous]
        [Route("[controller]/[action]/{addressId}")]
        public Dictionary<string, List<UserDetailsVM>> GetUsersAtAddress(int addressId)
        {
            return _usersRepository.UsersAtAddress(addressId)?
                                   .Select(u =>
                                   {
                                       u.Include(_parametersRepository.Parameters(u.Id, addressId)?.ToList());
                                       return u;
                                   })
                                   .GroupBy(u => u.LastName.Substring(0, 1))
                                   .OrderBy(ug => ug.Key)
                                   .ToDictionary(ug => ug.Key, ug => ug.Select(u => new UserDetailsVM(u, false))
                                                                       .OrderBy(u => u.LastName)
                                                                       .ThenBy(u => u.FirstName)
                                                                       .ToList());
        }

        #endregion

        #region Helpers

        private Dictionary<string, List<FlatDetailsVM>> GetFlats(string userId)
        {
            return _flatsRepository.Flats(userId)?
                                   .Select(f =>
                                   {
                                       f.Include(_parametersRepository.Parameters(f.ID)?.ToList(),
                                                 _addressesRepository.Address(f.AddressID));
                                       return f;
                                   })
                                   .OrderBy(f => f.Address.Country)
                                   .ThenBy(f => f.Address.Town)
                                   .ThenBy(f => f.Address.Street)
                                   .ThenBy(f => f.Address.Number)
                                   .GroupBy(f => f.AddressID, f => f)
                                   .ToDictionary(fg => fg.Select(f => f.Address.ToString())
                                                         .FirstOrDefault(),
                                                 fg => fg.Select(f => new FlatDetailsVM(f))
                                                         .OrderBy(f => f.Number)
                                                         .ToList());
        }

        #endregion
    }
}
