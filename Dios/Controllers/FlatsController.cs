using Dios.Extensions;
using Dios.Models;
using Dios.Repositories;
using Dios.Services;
using Dios.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Dios.Controllers
{
    public class FlatsController : Controller
    {
        private readonly IFlatsRepository _flatsRepository;
        private readonly IAddressesRepository _addressesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IParametersRepository _parameterRepository;
        private readonly IRequestUserProvider _requestUserProvider;

        public FlatsController(IFlatsRepository flatsRepository,
                               IAddressesRepository addressesRepository,
                               IUsersRepository usersRepository,
                               IParametersRepository parameterRepository,
                               IRequestUserProvider requestUserProvider)
        {
            _flatsRepository = flatsRepository;
            _addressesRepository = addressesRepository;
            _usersRepository = usersRepository;
            _parameterRepository = parameterRepository;
            _requestUserProvider = requestUserProvider;
        }

        // GET: Flats/Details/5
        [Authorize(Roles = "Admin,Host")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FlatDTO flat = _flatsRepository.Flat(id);
            if (flat == null)
            {
                return NotFound();
            }

            flat.Include(_parameterRepository.Parameters(flat.ID).ToList(), _addressesRepository.Address(flat.AddressID));

            return View(new FlatDetailsVM(flat));
        }

        // GET: Flats/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int addressId)
        {
            AddressDTO address = _addressesRepository.Address(addressId);

            if (address == null)
            {
                return RedirectToAction(nameof(AddressesController.Index), "Addresses");
            }

            return View(new FlatCreateVM(address));
        }

        // POST: Flats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([Bind("ID,Number,Floor,EntryDoorCode,AddressID")] FlatCreateVM flatVM)
        {
            AddressDTO address = _addressesRepository.Address(flatVM.AddressID);

            if (address == null)
            {
                return RedirectToAction(nameof(AddressesController.Index), "Addresses");
            }

            if (ModelState.IsValid)
            {
                // Check if any flat having the same number on the same floor already exists at that address
                if (_flatsRepository.Exists(address.ID, flatVM.Floor, flatVM.Number))
                {
                    flatVM.StatusMessage = string.Format("Error: En lägenhet med samma nummer finns redan på våning <b>{0}</b> på <b>{1}</b>",
                                                         flatVM.Floor,
                                                         address.ToString());

                    flatVM.Address = new AddressDetailsVM(address);

                    return View(flatVM);
                }

                FlatDTO flat = new FlatDTO
                {
                    Number = flatVM.Number,
                    Floor = flatVM.Floor,
                    EntryDoorCode = flatVM.EntryDoorCode,
                    AddressID = flatVM.AddressID
                };

                _flatsRepository.Add(flat);

                return View(new FlatCreateVM(address)
                {
                    StatusMessage = string.Format("Lägenhet <b>#{0}</b> med portkod <b>{1}</b> skapades på våningen <b>{2}</b> på <b>{3}</b>",
                                                  flat.Number,
                                                  flat.EntryDoorCode,
                                                  flat.Floor,
                                                  address.ToString())
                });
            }

            flatVM.Address = new AddressDetailsVM(address);
            return View(flatVM);
        }

        // GET: Flats/Edit/5
        [Authorize(Roles = "Admin,Host")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FlatDTO flat = _flatsRepository.Flat(id);
            if (flat == null)
            {
                return NotFound();
            }

            return View(flat);
        }

        // GET: Flats/Delete/5
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FlatDTO flat = _flatsRepository.Flat(id);
            if (flat == null)
            {
                return NotFound();
            }

            flat.Include(_parameterRepository.Parameters(flat.ID).ToList(), _addressesRepository.Address(flat.AddressID));

            return View(new FlatDetailsVM(flat));
        }

        // POST: Flats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var flat = _flatsRepository.Flat(id);

            if (flat == null)
            {
                return NotFound();
            }

            _flatsRepository.Delete(id);
            return RedirectToAction(nameof(AddressesController.Details), "Addresses", new { id = flat.AddressID });
        }

        [Route("[controller]/[action]/{addressId}")]
        public IActionResult Index(int addressId)
        {
            AddressDTO address = _addressesRepository.Address(addressId);
            if (address == null)
            {
                return NotFound();
            }

            return View(addressId);
        }

        #region API

        [Authorize(Roles = "Admin,Host")]
        [Route("[controller]/[action]/{flatId}")]
        public FlatCreateVM GetFlat(int flatId)     
        {
            FlatDTO flat = _flatsRepository.Flat(flatId);

            if (flat == null)
            {
                return new FlatCreateVM();
            }

            IEnumerable<UserDTO> allUsers = _usersRepository.Users();

            List<UserDetailsVM> availableUsers = allUsers.Where(u => (_parameterRepository.Get(u.Id, flatId)) == null)
                                                         .Select(u => new UserDetailsVM(u, false))
                                                         .OrderBy(u => u.LastName)
                                                         .ThenBy(u => u.FirstName)
                                                         .ToList();

            List<UserDetailsVM> users = allUsers.Where(u => (_parameterRepository.Get(u.Id, flatId)) != null)
                                                .Select(u => new UserDetailsVM(u, false))
                                                .OrderBy(u => u.LastName)
                                                .ThenBy(u => u.FirstName)
                                                .ToList();

            return new FlatCreateVM(_addressesRepository.Address(flat.AddressID), flat, users, availableUsers, User.IsInRole("Admin"));
        }

        [HttpPost]
        public IEnumerable<EditResultVM> EditFlat([FromBody] FlatEditVM flatVM)
        {
            List<EditResultVM> result = new List<EditResultVM>();

            if (string.IsNullOrEmpty(flatVM.Flat.Number))
            {
                result.Add(new EditResultVM
                {
                    Property = nameof(flatVM.Flat.Number),
                    ErrorMessage = "Nummer fältet är obligatoriskt!"
                });
            }

            if (flatVM.Flat.Floor < -5 || flatVM.Flat.Floor > 200)
            {
                result.Add(new EditResultVM
                {
                    Property = nameof(flatVM.Flat.Floor),
                    ErrorMessage = "Våning fältet måste vara mellan -5 och 200!"
                });
            }

            if (flatVM.ID == flatVM.Flat.ID && result.Count == 0)
            {
                // Everything went fine so far: all information can be saved
                FlatDTO flat = _flatsRepository.Flat(flatVM.ID);
                flat.Number = flatVM.Flat.Number;
                flat.Floor = flatVM.Flat.Floor;
                flat.EntryDoorCode = flatVM.Flat.EntryDoorCode;

                _flatsRepository.Edit(flat);

                // Update the list of users
                List<string> originalUserIds = _parameterRepository.UserIds(flatVM.ID).ToList();

                // Delete all the unselected users
                if (flatVM.UsersId == null)
                {
                    flatVM.UsersId = new List<string>().ToArray();
                }

                foreach (string userId in originalUserIds.Where(hi => !flatVM.UsersId.Contains(hi)))
                {
                    _parameterRepository.Delete(userId, flatVM.ID);
                }

                // Add the newly selected users, if any
                _parameterRepository.AddUsers(flatVM.ID, flatVM.UsersId.Where(ti => !originalUserIds.Contains(ti)).ToList());
            }

            return result;
        }

        #endregion
    }
}
