using Dios.Controllers;
using System;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            try
            {
                return urlHelper.Action(
                    action: nameof(AccountController.ResetPassword),
                    controller: "Account",
                    values: new { userId, code },
                    protocol: scheme);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string GenerateRegistrationLink(this IUrlHelper urlHelper, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.Register),
                controller: "Account",
                values: new { code },
                protocol: scheme);
        }
    }
}
