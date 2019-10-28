using Dios.Data;
using Dios.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dios.Repositories
{
    public class AddressHostsRepository : IAddressHostsRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressHostsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public AddressHostDTO AddressHost(int addressId, string hostId)
        {
            return _context.AddressHosts
                           .Where(ah => ah.AddressID == addressId &&
                                        string.Compare(ah.UserId, hostId, false) == 0)
                           .Join(_context.Users,
                                 ah => ah.UserId,
                                 u => u.Id,
                                 (ah, u) => new { ah, User = new UserDTO(u) })
                           .Join(_context.Addresses,
                                 ahu => ahu.ah.AddressID,
                                 a => a.ID,
                                 (ahu, a) => new { Address = new AddressDTO(a), ahu.User })
                           .Select(au => new AddressHostDTO(au.Address,
                                                            au.User))
                           .FirstOrDefault();
        }

        private User GetHost(string hostId)
        {
            return _context.Users.FirstOrDefault(u => string.Compare(u.Id, hostId, false) == 0);
        }

        private AddressHost GetAddressHost(int addressId, string hostId)
        {
            return _context.AddressHosts.FirstOrDefault(ah => ah.AddressID == addressId &&
                                                              string.Compare(ah.UserId, hostId, false) == 0);
        }

        public IEnumerable<AddressHostDTO> AddressHosts(string hostId)
        {
            return _context.AddressHosts
                           .Where(ah => string.Compare(ah.UserId, hostId, false) == 0)
                           .Join(_context.Users,
                                 ah => ah.UserId,
                                 u => u.Id,
                                 (ah, u) => new { AddressId = ah.AddressID, Host = u })
                           .Join(_context.Addresses,
                                 ahu => ahu.AddressId,
                                 a => a.ID,
                                 (ahu, a) => new { Address = a, ahu.Host })
                           .Select(ah => new AddressHostDTO(new AddressDTO(ah.Address), new UserDTO(ah.Host)))
                           .ToList();
        }

        public IEnumerable<AddressDTO> Addresses(string hostId)
        {
            return _context.AddressHosts
                           .Where(ah => string.Compare(ah.UserId, hostId, false) == 0)
                           .Join(_context.Addresses,
                                 ah => ah.AddressID,
                                 a => a.ID,
                                 (ah, a) => new AddressDTO(a));
        }

        public IEnumerable<UserDTO> Hosts(int addressId)
        {
            return _context.AddressHosts
                           .Where(ah => ah.AddressID == addressId)
                           .Join(_context.Users,
                                 ah => ah.UserId,
                                 u => u.Id,
                                 (ah, u) => new UserDTO(u))
                           .ToList();
        }

        public IEnumerable<string> HostIds(int addressId)
        {
            return _context.AddressHosts.Where(ah => ah.AddressID == addressId).Select(ah => ah.UserId);
        }

        public int Add(AddressHostDTO addressHost)
        {
            AddressHost addressHostTmp = GetAddressHost(addressHost.Address.ID, addressHost.Host.Id);

            if (addressHostTmp != null)
            {
                return 0;
            }

            _context.AddressHosts.Add(new AddressHost { AddressID = addressHost.Address.ID, UserId = addressHost.Host.Id });

            return _context.SaveChanges();
        }

        public int AddHosts(int addressId, List<string> hostIds)
        {
            if (hostIds == null)
            {
                return 0;
            }

            bool alteredDB = false;
            foreach (string hostId in hostIds)
            {
                _context.Add(new AddressHost
                {
                    AddressID = addressId,
                    UserId = hostId
                });

                alteredDB = true;
            }

            if (alteredDB)
                return _context.SaveChanges();
            else
                return 0;
        }

        public int Delete(int addressId, string hostId)
        {
            AddressHost addressHostTmp = GetAddressHost(addressId, hostId);
            if (addressHostTmp == null)
            {
                return 0;
            }

            _context.Remove(addressHostTmp);

            return _context.SaveChanges();
        }
    }
}
