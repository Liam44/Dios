using Dios.Data;
using Dios.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dios.Repositories
{
    public class AddressesRepository : IAddressesRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public AddressDTO Address(int? addressId)
        {
            return _context.Addresses
                           .Where(a => a.ID == addressId)
                           .Select(a => new AddressDTO(a))
                           .FirstOrDefault();
        }

        private Address GetAddress(int addressId)
        {
            return _context.Addresses.FirstOrDefault(a => a.ID == addressId);
        }

        public IEnumerable<AddressDTO> Addresses()
        {
            return _context.Addresses
                           .Select(a => new AddressDTO(a))
                           .ToList();
        }

        public int Add(AddressDTO address)
        {
            Address addressTmp = GetAddress(address.ID);

            if (addressTmp != null)
            {
                return 0;
            }

            addressTmp = new Address
            {
                Street = address.Street,
                Number = address.Number,
                ZipCode = address.ZipCode,
                Town = address.Town,
                Country = address.Country
            };

            _context.Addresses.Add(addressTmp);

            return _context.SaveChanges();
        }

        public int Edit(AddressDTO address)
        {
            if (address == null)
            {
                return 0;
            }

            Address addressTmp = GetAddress(address.ID);
            if (addressTmp == null)
            {
                return 0;
            }

            addressTmp.Street = address.Street;
            addressTmp.Number = address.Number;
            addressTmp.ZipCode = address.ZipCode;
            addressTmp.Town = address.Town;
            addressTmp.Country = address.Country;

            _context.Update(addressTmp);
            return _context.SaveChanges();
        }

        public int Delete(int addressId)
        {
            Address addressTmp = GetAddress(addressId);
            if (addressTmp == null)
            {
                return 0;
            }

            _context.Remove(addressTmp);
            return _context.SaveChanges();
        }
    }
}
