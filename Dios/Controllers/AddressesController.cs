using Dios.Exceptions;
using Dios.Extensions;
using Dios.Helpers;
using Dios.Models;
using Dios.Repositories;
using Dios.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dios.Controllers
{
    [Authorize(Roles = "Admin,Host")]
    public class AddressesController : Controller
    {
        private readonly IAddressesRepository _addressesRepository;
        private readonly IFlatsRepository _flatsRepository;
        private readonly IParametersRepository _parameterRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IAddressHostsRepository _addressHostsRepository;
        private readonly IHostingEnvironment _environment;
        private readonly IExport _export;

        private string RootPath
        {
            get { return Path.Combine(_environment.WebRootPath, "lists"); }
            set { }
        }

        public AddressesController(IAddressesRepository addressesRepository,
                                   IFlatsRepository flatsRepository,
                                   IParametersRepository parameterRepository,
                                   IUsersRepository usersRepository,
                                   IAddressHostsRepository addressHostsRepository,
                                   IHostingEnvironment environment,
                                   IExport export)
        {
            _addressesRepository = addressesRepository;
            _flatsRepository = flatsRepository;
            _parameterRepository = parameterRepository;
            _usersRepository = usersRepository;
            _addressHostsRepository = addressHostsRepository;
            _environment = environment;
            _export = export;
        }

        // GET: Addresses
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Addresses/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AddressDTO address = _addressesRepository.Address(id);
            if (address == null)
            {
                return NotFound();
            }

            IncludeNavigationProperties(address);

            return View(new AddressDetailsVM(address, canDataBeDeleted: User.IsInRole("Admin")));
        }

        // GET: Addresses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Addresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([Bind("ID,Street,Number,ZipCode,Town,Country")] AddressCreateVM addressVM)
        {
            if (ModelState.IsValid)
            {
                AddressDTO address = new AddressDTO
                {
                    Street = addressVM.Street.Trim(),
                    Number = addressVM.Number.Trim(),
                    ZipCode = addressVM.ZipCode.Trim(),
                    Town = addressVM.Town.Trim(),
                    Country = addressVM.Country.ToUpper().Trim()
                };

                _addressesRepository.Add(address);
                return RedirectToAction(nameof(FlatsController.Create), "Flats", new { addressId = address.ID });
            }

            return View(addressVM);
        }

        // GET: Addresses/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = _addressesRepository.Address(id);
            if (address == null)
            {
                return NotFound();
            }

            return View(id);
        }

        // GET: Addresses/Delete/5
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AddressDTO address = _addressesRepository.Address(id);
            if (address == null)
            {
                return NotFound();
            }

            IncludeNavigationProperties(address);

            return View(new AddressDetailsVM(address));

        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var address = _addressesRepository.Address(id);

            if (address == null)
            {
                return NotFound();
            }

            _addressesRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ExportUsers(int id)
        {
            AddressDTO address = _addressesRepository.Address(id);

            if (address == null)
            {
                return RedirectToAction(nameof(Index));
            }

            IncludeNavigationProperties(address);

            string path = Path.Combine(RootPath, address.ID.ToString());

            // Export users lists in different formats and prepare all generated files to be downloaded
            try
            {
                ZipResult zipResult = _export.ExportUsers(_usersRepository, address, path);

                if (zipResult == null || string.IsNullOrEmpty(zipResult.FileName))
                {
                    // Something went wrong: redirect to the details view
                    return RedirectToAction(nameof(Details), new { id });
                }

                return File(zipResult.MemoryStream, zipResult.ContentType, zipResult.FileName);
            }
            catch (IOException)
            {
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is ExportUsersException || ex is FormatException)
            {
                // Something went wrong: redirect to the details view
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #region API

        public IEnumerable<AddressDetailsVM> GetAddresses()
        {
            return _addressesRepository.Addresses()
                                       .Select(a =>
                                       {
                                           IncludeNavigationProperties(a);

                                           return a;
                                       })
                                       .Select(a => new AddressDetailsVM(a,
                                                                       _flatsRepository.AmountAvailableFlats(a.ID)))
                                       .ToList();
        }

        [Route("[controller]/[action]/{addressId}")]
        public AddressCreateVM GetAddress(int addressId)
        {
            IEnumerable<UserDTO> allHosts = _usersRepository.Hosts();

            List<UserDetailsVM> availableHosts = allHosts.Where(u => (_addressHostsRepository.AddressHost(addressId, u.Id)) == null)
                                                         .Select(u => new UserDetailsVM(u, true))
                                                         .OrderBy(u => u.LastName)
                                                         .ThenBy(u => u.FirstName)
                                                         .ToList();

            List<UserDetailsVM> hosts = allHosts.Where(u => (_addressHostsRepository.AddressHost(addressId, u.Id)) != null)
                                                .Select(u => new UserDetailsVM(u, true))
                                                .OrderBy(u => u.LastName)
                                                .ThenBy(u => u.FirstName)
                                                .ToList();

            return new AddressCreateVM(_addressesRepository.Address(addressId), hosts, availableHosts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IEnumerable<EditResultVM> EditAddress([FromBody] AddressEditVM addressVM)
        {
            List<EditResultVM> result = new List<EditResultVM>();

            if (string.IsNullOrEmpty(addressVM.Address.Country))
            {
                result.Add(new EditResultVM
                {
                    Property = nameof(addressVM.Address.Country),
                    ErrorMessage = "Land fältet är obligatoriskt!"
                });
            }

            if (string.IsNullOrEmpty(addressVM.Address.Town))
            {
                result.Add(new EditResultVM
                {
                    Property = nameof(addressVM.Address.Town),
                    ErrorMessage = "Ort fältet är obligatoriskt!"
                });
            }

            if (string.IsNullOrEmpty(addressVM.Address.ZipCode))
            {
                result.Add(new EditResultVM
                {
                    Property = nameof(addressVM.Address.ZipCode),
                    ErrorMessage = "Postkod fältet är obligatoriskt!"
                });
            }

            if (string.IsNullOrEmpty(addressVM.Address.Number))
            {
                result.Add(new EditResultVM
                {
                    Property = nameof(addressVM.Address.Number),
                    ErrorMessage = "Gatansnummer fältet är obligatoriskt!"
                });
            }

            if (string.IsNullOrEmpty(addressVM.Address.Street))
            {
                result.Add(new EditResultVM
                {
                    Property = nameof(addressVM.Address.Street),
                    ErrorMessage = "Gata fältet är obligatoriskt!"
                });
            }

            if (addressVM.ID == addressVM.Address.ID && result.Count == 0)
            {
                // Everything went fine so far: all information can be saved
                AddressDTO address = _addressesRepository.Address(addressVM.ID);
                address.Street = addressVM.Address.Street;
                address.Number = addressVM.Address.Number;
                address.ZipCode = addressVM.Address.ZipCode;
                address.Town = addressVM.Address.Town;
                address.Country = addressVM.Address.Country;

                _addressesRepository.Edit(address);

                // Update the list of hosts
                List<string> originalHostIds = _addressHostsRepository.HostIds(addressVM.ID).ToList();

                // Delete all the unselected hosts
                if (addressVM.HostsId == null)
                {
                    addressVM.HostsId = new List<string>().ToArray();
                }

                foreach (string hostId in originalHostIds.Where(hi => !addressVM.HostsId.Contains(hi)))
                {
                    _addressHostsRepository.Delete(addressVM.ID, hostId);
                }

                // Add the newly selected hosts, if any
                _addressHostsRepository.AddHosts(addressVM.ID, addressVM.HostsId.Where(ti => !originalHostIds.Contains(ti)).ToList());
            }

            return result;
        }

        #endregion

        #region Helpers

        private void IncludeNavigationProperties(AddressDTO address)
        {
            address.Include(_flatsRepository.Flats(address.ID)
                                            .Select(f =>
                                            {
                                                f.Include(_parameterRepository.Parameters(f.ID)
                                                                              .OrderBy(p => p.User.LastName)
                                                                              .ThenBy(p => p.User.FirstName)
                                                                              .ToList());
                                                return f;
                                            })
                                            .ToList(),
                            _addressHostsRepository.Hosts(address.ID)
                                                   .OrderBy(h => h.LastName)
                                                   .ThenBy(h => h.FirstName)
                                                   .ToList());
        }

        #endregion
    }
}
