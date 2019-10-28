using System;
using System.Security.Claims;

namespace Dios.Extensions
{
    public static class CurrentUserId
    {
        /// <summary>
        /// Returns the user's Id
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string Id(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
