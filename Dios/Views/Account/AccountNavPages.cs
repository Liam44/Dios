using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace Dios.Views.Account
{
    public static class AccountNavPages
    {
        public static string ActivePageKey => "ActivePage";

        public static string ChangePassword => "ChangePassword";

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);
    
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
    }
}
