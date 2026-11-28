using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Components
{
    internal static class AkUrl
    {
        /// مسار داخلي فقط. يمنع Open Redirect بعد تسجيل الدخول (returnUrl=//evil.com)
        public static bool IsLocal(string? url) =>
            !string.IsNullOrEmpty(url) && url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'));
        internal static string? Query(NavigationManager nav, string key) =>
    System.Web.HttpUtility.ParseQueryString(new Uri(nav.Uri).Query)[key];

        internal static string WithoutQuery(NavigationManager nav) => new Uri(nav.Uri).GetLeftPart(UriPartial.Path);

    }
}
