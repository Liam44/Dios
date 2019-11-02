using Dios.Controllers;
using Dios.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Dios.Extensions
{
    public interface IUrlHelperWrapper
    {
        string ResetPasswordCallbackLink(IUrlHelper urlHelper, string userId, string code, string scheme);
        string GenerateRegistrationLink(IUrlHelper urlHelper, string code, string scheme);
    }

    public class UrlHelperWrapper : IUrlHelperWrapper
    {
        public string ResetPasswordCallbackLink(IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            try
            {
                return urlHelper.Action(action: nameof(AccountController.ResetPassword),
                                        controller: "Account",
                                        values: new { userId, code },
                                        protocol: scheme);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GenerateRegistrationLink(IUrlHelper urlHelper, string code, string scheme)
        {
            return urlHelper.Action(action: nameof(AccountController.Register),
                                    controller: "Account",
                                    values: new { code },
                                    protocol: scheme);
        }
    }

    public static class UrlHelperExtensions
    {
        public static IUrlHelperWrapper UrlHelperWrapper = new UrlHelperWrapper();

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            if (UrlHelperWrapper == null)
            {
                throw new UrlHelperWrapperUndefinedException();
            }

            return UrlHelperWrapper.ResetPasswordCallbackLink(urlHelper, userId, code, scheme);
        }

        public static string GenerateRegistrationLink(this IUrlHelper urlHelper, string code, string scheme)
        {
            if (UrlHelperWrapper == null)
            {
                throw new UrlHelperWrapperUndefinedException();
            }

            return UrlHelperWrapper.GenerateRegistrationLink(urlHelper, code, scheme);
        }
    }
}
