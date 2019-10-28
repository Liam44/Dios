using Dios.Data;
using Dios.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dios.Repositories
{
    public class ParametersRepository : IParametersRepository
    {
        private readonly ApplicationDbContext _context;

        public ParametersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ParameterDTO> Parameters(string userId, int? addressId = null)
        {
            return _context.Parameters
                           .Where(p => string.Compare(p.UserId, userId, false) == 0)
                           .Join(_context.Flats.Where(f => addressId == null || f.AddressID == addressId),
                                 p => p.FlatID,
                                 f => f.ID,
                                 (p, f) => new { Parameter = p, Flat = f })
                           .Join(_context.Users,
                                 pf => pf.Parameter.UserId,
                                 u => u.Id,
                                 (pf, u) => new ParameterDTO(pf.Flat, u, pf.Parameter))
                           .ToList();
        }

        public IEnumerable<ParameterDTO> Parameters(int flatId)
        {
            return _context.Parameters
                           .Where(p => p.FlatID == flatId)
                           .Join(_context.Flats,
                                 p => p.FlatID,
                                 f => f.ID,
                                 (p, f) => new { Parameter = p, Flat = f })
                           .Join(_context.Users,
                                 pf => pf.Parameter.UserId,
                                 u => u.Id,
                                 (pf, u) => new ParameterDTO(pf.Flat, u, pf.Parameter))
                           .ToList();
        }

        public IEnumerable<string> UserIds(int flatId)
        {
            return _context.Parameters
                           .Where(p => p.FlatID == flatId)
                           .Select(p => p.UserId)
                           .ToList();
        }

        private Parameter Parameter(string userId, int flatId)
        {
            return _context.Parameters
                           .FirstOrDefault(p => string.Compare(p.UserId, userId, false) == 0 && p.FlatID == flatId);
        }

        public int Add(ParameterDTO parameter)
        {
            Parameter parameterTmp = Parameter(parameter.User.Id, parameter.Flat.ID);

            if (parameterTmp != null)
            {
                return 0;
            }

            parameterTmp = new Parameter
            {
                UserId = parameter.User.Id,
                FlatID = parameter.Flat.ID,
                IsEmailVisible = parameter.IsEmailVisible,
                IsPhoneNumberVisible = parameter.IsPhoneNumberVisible,
                CanBeContacted = parameter.CanBeContacted
            };

            _context.Parameters.Add(parameterTmp);

            return _context.SaveChanges();
        }

        public int AddUsers(int flatId, List<string> userIds)
        {
            if (userIds == null)
            {
                return 0;
            }

            bool alteredDB = false;
            foreach (string userId in userIds)
            {
                _context.Add(new Parameter
                {
                    FlatID = flatId,
                    UserId = userId,
                    IsEmailVisible = true,
                    IsPhoneNumberVisible = true,
                    CanBeContacted = true
                });

                alteredDB = true;
            }

            if (alteredDB)
                return _context.SaveChanges();
            else
                return 0;
        }

        public int Delete(string userId, int flatId)
        {
            Parameter tmp = Parameter(userId, flatId);
            if (tmp == null)
            {
                return 0;
            }

            _context.Remove(tmp);

            return _context.SaveChanges();
        }

        public int Edit(ParameterDTO parameter)
        {
            if (parameter == null)
            {
                return 0;
            }

            Parameter tmp = Parameter(parameter.User.Id, parameter.Flat.ID);
            if (tmp == null)
            {
                return 0;
            }

            tmp.IsEmailVisible = parameter.IsEmailVisible;
            tmp.IsPhoneNumberVisible = parameter.IsPhoneNumberVisible;
            tmp.CanBeContacted = parameter.CanBeContacted;

            _context.Update(tmp);
            return _context.SaveChanges();
        }

        public ParameterDTO Get(string uid, int fid)
        {
            return _context.Parameters
                           .Where(p => p.FlatID == fid && string.Compare(p.UserId, uid, false) == 0)
                           .Join(_context.Flats,
                                 p => p.FlatID,
                                 f => f.ID,
                                 (p, f) => new { Parameter = p, Flat = f })
                           .Join(_context.Users,
                                 pf => pf.Parameter.UserId,
                                 u => u.Id,
                                 (pf, u) => new ParameterDTO(pf.Flat, u, pf.Parameter))
                           .FirstOrDefault();
        }

        public List<ParameterDTO> AllParameters()
        {
            return _context.Parameters
                           .Join(_context.Flats,
                                 p => p.FlatID,
                                 f => f.ID,
                                 (p, f) => new { Parameter = p, Flat = f })
                           .Join(_context.Users,
                                 pf => pf.Parameter.UserId,
                                 u => u.Id,
                                 (pf, u) => new ParameterDTO(pf.Flat, u, pf.Parameter))
                           .ToList();
        }
    }
}
