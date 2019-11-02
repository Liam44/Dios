using Dios.Data;
using Dios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dios.Repositories
{
    public sealed class UsersRepository : IUsersRepository
    {
        private const string NUMBERS = "1234567890";
        private const string TOKENS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string SPECIAL_TOKENS = "!:/;-_&%?";
        private const int REGISTRATION_LENGTH = 20;
        private const int PASSWORD_LENGTH = 15;

        private readonly ApplicationDbContext _context;

        public UsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserDTO> Users()
        {
            return UsersByRole("User");
        }

        public IEnumerable<UserDTO> UsersAtAddress(int addressId)
        {
            return _context.Flats
                           .Where(f => f.AddressID == addressId)
                           .Join(_context.Parameters,
                                 f => f.ID,
                                 p => p.FlatID,
                                 (f, p) => p.UserId)
                           .Join(_context.Users,
                                 uId => uId,
                                 u => u.Id,
                                 (uId, u) => new UserDTO(u))
                           .Distinct()  // A same user can live in several flats at the same address
                           .ToList();
        }

        public UserDTO User(string id)
        {
            return _context.Users
                           .Where(u => string.Compare(u.Id, id, false) == 0)
                           .Select(u => new UserDTO(u))
                           .FirstOrDefault();
        }

        public UserDTO UserByPersonalNumber(string personalNumber, string role)
        {
            return UsersByRole(role).FirstOrDefault(u => string.Compare(u.PersonalNumber, personalNumber, false) == 0);
        }

        public User GetUser(string id)
        {
            return _context.Users
                           .FirstOrDefault(u => string.Compare(u.Id, id, false) == 0);
        }

        public UserDTO UserByRegistrationCode(string code)
        {
            return _context.Users
                           .Where(u => string.Compare(u.RegistrationCode, code, false) == 0)
                           .Select(u => new UserDTO(u))
                           .FirstOrDefault();
        }

        public IEnumerable<UserDTO> Hosts()
        {
            return UsersByRole("Host");
        }

        public IEnumerable<UserDTO> HostsAtAddress(int addressId)
        {
            return _context.AddressHosts
                           .Where(ah => ah.AddressID == addressId)
                           .Join(_context.Users,
                                 ah => ah.UserId,
                                 u => u.Id,
                                 (ah, u) => new UserDTO(u))
                           .ToList();
        }

        public int Edit(UserDTO user)
        {
            if (user == null)
            {
                return 0;
            }

            User userTmp = GetUser(user.Id);

            if (userTmp == null)
            {
                return 0;
            }

            userTmp.PersonalNumber = user.PersonalNumber;
            userTmp.LastName = user.LastName;
            userTmp.FirstName = user.FirstName;
            userTmp.Email = user.Email;
            userTmp.NormalizedEmail = user.NormalizedEmail;
            userTmp.PhoneNumber = user.PhoneNumber;
            userTmp.PhoneNumber2 = user.PhoneNumber2;
            userTmp.RegistrationCode = user.RegistrationCode;

            _context.Update(userTmp);

            return _context.SaveChanges();
        }

        public int Delete(string userId)
        {
            User userTmp = GetUser(userId);

            if (userTmp == null)
            {
                return 0;
            }

            _context.Remove(userTmp);

            return _context.SaveChanges();
        }

        private IEnumerable<UserDTO> UsersByRole(string role)
        {
            var roleId = _context.Roles.Where(r => string.Compare(r.Name, role, false) == 0).Select(r => r.Id).SingleOrDefault();

            return _context.Users
                           .Where(u => (_context.UserRoles
                                                .Where(ur => ur.RoleId == roleId)
                                                .Select(ur => ur.UserId)).Contains(u.Id))
                           .Select(u => new UserDTO(u))
                           .ToList();
        }

        public string GenerateRegistrationCode()
        {
            string tokens = TOKENS + NUMBERS;

            string registrationCode = string.Empty;
            Random random = new Random();

            while (registrationCode.Length < REGISTRATION_LENGTH)
            {
                registrationCode += tokens[random.Next(0, tokens.Length)];
            }

            return registrationCode;
        }

        public string GeneratePassword()
        {
            string tokens = TOKENS + SPECIAL_TOKENS + NUMBERS;

            string password = string.Empty;
            Random random = new Random();

            while (password.Length < PASSWORD_LENGTH)
            {
                password += tokens[random.Next(0, tokens.Length)];
            }

            // The password must contain at least one number in order to be valid
            if (password.Count(c => NUMBERS.Contains(c)) == 0)
            {
                // All we need to do is to replace the first character by a random number
                int index = password.IndexOf(password.First(c => TOKENS.Contains(c)));
                StringBuilder stringBuilder = new StringBuilder(password);
                stringBuilder[index] = NUMBERS[random.Next(NUMBERS.Length)];
                password = stringBuilder.ToString();
            }

            // The password must contain at least one special character in order to be valid
            if (password.Count(c => SPECIAL_TOKENS.Contains(c)) == 0)
            {
                // All we need to do is to replace the first character by a random special character
                int index = password.IndexOf(password.First(c => TOKENS.Contains(c)));
                StringBuilder stringBuilder = new StringBuilder(password);
                stringBuilder[index] = SPECIAL_TOKENS[random.Next(SPECIAL_TOKENS.Length)];
                password = stringBuilder.ToString();
            }

            return password;
        }
    }
}
