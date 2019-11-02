using System;
using System.Security.Claims;

namespace Dios.Extensions
{
    public interface ICurrentUserIdWrapper
    {
        string Id(ClaimsPrincipal user);
    }

    public class CurrentUserIdWrapper : ICurrentUserIdWrapper
    {
        public string Id(ClaimsPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }

    public static class CurrentUserId
    {
        public static ICurrentUserIdWrapper CurrentUserIdWrapper = new CurrentUserIdWrapper();

        /// <summary>
        /// Returns the user's Id
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string Id(this ClaimsPrincipal user)
        {
            return CurrentUserIdWrapper.Id(user);
        }
    }
}
