using System;

namespace Dios.Exceptions
{
    public class UrlHelperWrapperUndefinedException : Exception
    {
        public UrlHelperWrapperUndefinedException(): base("UrlHelperWrapper is null in UrlHelperExtensions.")
        {

        }
    }
}
