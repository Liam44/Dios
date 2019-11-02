using Dios.Data;
using Dios.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dios.Repositories
{
    public sealed class FlatsRepository : IFlatsRepository
    {
        private readonly ApplicationDbContext _context;

        public FlatsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<FlatDTO> Flats(int addressID)
        {
            return _context.Flats
                           .Where(f => f.AddressID == addressID)
                           .Select(f => new FlatDTO(f))
                           .ToList();
        }

        public int AmountAvailableFlats(int addressId)
        {
            return _context.Flats
                           .Where(f => f.AddressID == addressId)
                           .GroupJoin(_context.Parameters,
                                      f => f.ID,
                                      p => p.FlatID,
                                      (f, p) => new { f, p })
                           .Where(fp => fp.p.Count() == 0)
                           .Count();
        }

        public IEnumerable<FlatDTO> Flats(string userId)
        {
            return _context.Parameters
                           .Where(p => string.Compare(p.UserId, userId, false) == 0)
                           .Join(_context.Flats,
                                 p => p.FlatID,
                                 f => f.ID,
                                 (p, f) => new FlatDTO(f));
        }

        public FlatDTO Flat(int? id)
        {
            return _context.Flats.Where(f => f.ID == id)
                                 .Select(f => new FlatDTO(f))
                                 .FirstOrDefault();
        }

        private Flat GetFlat(int? id)
        {
            return _context.Flats.FirstOrDefault(f => f.ID == id);
        }

        public bool Exists(int addressId, int? floor, string number)
        {
            return _context.Flats
                           .FirstOrDefault(f => f.AddressID == addressId &&
                                                f.Floor == floor &&
                                                string.Compare(f.Number, number, false) == 0) != null;
        }

        public int Add(FlatDTO flat)
        {
            Flat flatTmp = new Flat
            {
                Floor = flat.Floor,
                Number = flat.Number,
                EntryDoorCode = flat.EntryDoorCode,
                AddressID = flat.AddressID
            };

            _context.Flats.Add(flatTmp);

            return _context.SaveChanges();
        }

        public int Edit(FlatDTO flat)
        {
            if (flat == null)
            {
                return 0;
            }

            Flat flatTmp = GetFlat(flat.ID);
            if (flatTmp == null)
            {
                return 0;
            }

            flatTmp.Floor = flat.Floor;
            flatTmp.Number = flat.Number;
            flatTmp.EntryDoorCode = flat.EntryDoorCode;

            _context.Update(flatTmp);

            return _context.SaveChanges();
        }

        public int Delete(int flatId)
        {
            Flat flatTmp = GetFlat(flatId);
            if (flatTmp == null)
            {
                return 0;
            }

            _context.Remove(flatTmp);

            return _context.SaveChanges();
        }
    }
}
