using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Common
{
    internal static class UrlTokenEncoder
    {
        public static string Encode(string token) =>
            WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        public static bool TryDecode(string encoded, out string token)
        {
            try
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encoded));
                return true;
            }
            catch (FormatException)
            {
                token = string.Empty;
                return false;
            }
        }
    }
}
