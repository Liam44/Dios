using Dios.Extensions;
using Dios.Models;
using Dios.Repositories;
using Dios.Services;
using Dios.ViewModels;
using Dios.ViewModels.UsersViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dios.Controllers
{
    [Authorize(Roles = "Host,Admin")]
    public class HostsController : Controller
    {
        private const string ROLE = "Host";

        private readonly IRequestUserProvider _requestUserProvider;
        private readonly IEmailSender _emailSender;
        private readonly IUsersRepository _usersRepository;
        private readonly IFlatsRepository _flatsRepository;
        private readonly IParametersRepository _parameterRepository;
        private readonly IAddressesRepository _addressesRepository;
        private readonly IAddressHostsRepository _addressHostsRepository;
        private readonly IErrorReportsRepository _errorRepository;
        private readonly INewUser _newUser;

        public HostsController(IRequestUserProvider requestUserProvider,
                               IEmailSender emailSender,
                               IUsersRepository usersRepository,
                               IFlatsRepository flatsRepository,
                               IParametersRepository parameterRepository,
                               IAddressesRepository addressesRepository,
                               IAddressHostsRepository addressHostsRepository,
                               IErrorReportsRepository errorRepository,
                               INewUser newUser)
        {
            _requestUserProvider = requestUserProvider;
            _emailSender = emailSender;
            _usersRepository = usersRepository;
            _flatsRepository = flatsRepository;
            _parameterRepository = parameterRepository;
            _addressesRepository = addressesRepository;
            _addressHostsRepository = addressHostsRepository;
            _newUser = newUser;
            _errorRepository = errorRepository;
        }

        // GET: Hosts
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IEnumerable<UserIndexVM> GetHosts()
        {
            IEnumerable<UserDTO> hosts = _usersRepository.Hosts();

            if (hosts == null)
            {
                return new List<UserIndexVM>();
            }

            return hosts.Select(u => new UserIndexVM(u))
                        .ToList();
        }

        public UserIndexVM GetHost(string hostId)
        {
            return new UserIndexVM(_usersRepository.User(hostId));
        }

        [HttpGet]
        [Authorize(Roles = ROLE)]
        public IActionResult ErrorReports()
        {
            return View(_errorRepository.ErrorReports(User.Id()));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new UserCreateVM() { IsPhoneNumber2Visible = true });
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

            user.Include(addresses: _addressHostsRepository.Addresses(id)?
                                                           .Select(a =>
                                                           {
                                                               a.Include(_flatsRepository.Flats(a.ID)?
                                                                                         .Select(f =>
                                                                                         {
                                                                                             f.Include(_parameterRepository.Parameters(f.ID)?.ToList());
                                                                                             return f;
                                                                                         })
                                                                                         .ToList(),
                                                                         _addressHostsRepository.Hosts(a.ID)?
                                                                                                .ToList());
                                                               return a;
                                                           }).ToList());

            return View(new UserDetailsVM(user, true));
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public IActionResult Edit(string id = null)
        {
            // If the user's ID is not provided, the current user's profile is displayed
            bool canChangePassword = false;
            if (string.IsNullOrEmpty(id))
            {
                id = User.Id();

                // Who is allowed to change their own password
                canChangePassword = true;
            }

            var user = _usersRepository.User(id);

            if (user == null)
            {
                throw new ApplicationException(/*$"Unable to load user with ID '{_userManager.GetUserId(Host)}'."*/);
            }

            var model = new UserCreateVM
            {
                Id = id,
                PersonalNumber = user.PersonalNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CanChangeName = false,
                CanChangePassword = canChangePassword,
                PhoneNumber = user.PhoneNumber,
                PhoneNumber2 = user.PhoneNumber2,
                IsPhoneNumber2Visible = true,
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

                    string previousHostId = model.Id;

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

                    _usersRepository.Delete(previousHostId);

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

                if (model.PhoneNumber2 != userDTO.PhoneNumber2)
                {
                    userDTO.PhoneNumber2 = model.PhoneNumber2;
                    edited = true;
                }

                if (edited)
                {
                    _usersRepository.Edit(userDTO);
                }
            }

            var userId = User.Id();
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

            user.Include(addresses: _addressHostsRepository.Addresses(id).ToList());

            return View(new UserDetailsVM(user, true));
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
        public IActionResult Remove(string id, int addressId)
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

            AddressDTO address = _addressesRepository.Address(addressId);
            if (address == null)
            {
                return NotFound();
            }

            AddressHostDTO addressHost = _addressHostsRepository.AddressHost(addressId, id);
            if (addressHost == null)
            {
                return NotFound();
            }

            address.Include(_flatsRepository.Flats(addressId)?
                                            .Select(f =>
                                            {
                                                f.Include(_parameterRepository.Parameters(f.ID)?.ToList());
                                                return f;
                                            })
                                            .ToList());

            return View(new AddressHostDetailsVM()
            {
                Address = new AddressDetailsVM(address),
                Host = new UserDetailsVM(host, true)
            });
        }

        // POST: Hosts/Remove/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveConfirmed(string id, int addressId)
        {
            AddressHostDTO addressHost = _addressHostsRepository.AddressHost(addressId, id);

            if (addressHost == null)
            {
                return NotFound();
            }

            _addressHostsRepository.Delete(addressId, id);

            return RedirectToAction(nameof(AddressesController.Details), "Addresses", new { id = addressId });
        }

        [Authorize(Roles = ROLE)]
        public IActionResult Addresses()
        {
            // Gets the list of buildings the current is user is responsible for
            List<AddressDetailsVM> addresses = _addressHostsRepository.Addresses(User.Id())?
                                                                      .Select(a =>
                                                                      {
                                                                          a.Flats = _flatsRepository.Flats(a.ID)?.ToList();
                                                                          return a;
                                                                      })
                                                                      .Select(a => new AddressDetailsVM(a,
                                                                                                        _flatsRepository.AmountAvailableFlats(a.ID),
                                                                                                        false))
                                                                      .OrderBy(a => a.Country)
                                                                      .ThenBy(a => a.Town)
                                                                      .ThenBy(a => a.Street)
                                                                      .ThenBy(a => a.Number)
                                                                      .ToList();

            return View(addresses);
        }

        #region API

        [AllowAnonymous]
        [Route("[controller]/[action]/{addressId}")]
        public Dictionary<string, List<UserDetailsVM>> GetHostsAtAddress(int addressId)
        {
            return _usersRepository.HostsAtAddress(addressId)?
                                   .GroupBy(u => u.LastName.Substring(0, 1))
                                   .OrderBy(ug => ug.Key)
                                   .ToDictionary(ug => ug.Key, ug => ug.Select(u => new UserDetailsVM(u, false))
                                                                       .OrderBy(u => u.FirstName)
                                                                       .ToList());
        }

        public DateTime Now()
        {
            return DateTime.Now;
        }

        [HttpPost]
        public void SaveErrors([FromBody]ErrorReportDTO error)
        {
            _errorRepository.Edit(error);
        }

        public Dictionary<int, ErrorReportAPI> GetErrors()
        {
            var tmp = _errorRepository.ErrorReports(User.Id());

            return tmp?
                                   .Select(e =>
                                   {
                                       IncludeNavigationProperties(e);
                                       return e;
                                   })
                                   .OrderBy(e => e.Flat?.Address.ToString())
                                   .GroupBy(e => e.Flat == null ? -1 : e.Flat.AddressID)
                                   .ToDictionary(egb => egb.Key,
                                                 egb => new ErrorReportAPI
                                                 {
                                                     AddressId = egb.Key,
                                                     Address = egb.FirstOrDefault().Flat?.Address?.ToString(),
                                                     Unseen = egb.Count(e => e.Seen == null) > 0,
                                                     ErrorReports = egb.ToDictionary(e => e.Id, e => e)
                                                 });
        }

        private void IncludeNavigationProperties(ErrorReportDTO errorReport)
        {
            errorReport.Flat = _flatsRepository.Flat(errorReport.FlatId);
            errorReport.Flat?.Include(_parameterRepository.Parameters(errorReport.FlatId)?.ToList(),
                                      _addressesRepository.Address(errorReport.Flat.AddressID));
        }

        #endregion

        #region Private classes

        public class ErrorReportAPI
        {
            public int AddressId { get; set; }
            public string Address { get; set; }
            public bool Unseen { get; set; }
            public Dictionary<int, ErrorReportDTO> ErrorReports { get; set; }
        }

        #endregion
    }
}
