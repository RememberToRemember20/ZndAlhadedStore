using AuthKit.Blazor.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Components
{
    internal static class AkValidation
    {
        public static string? Required(IAkLocalizer l, string? value) =>
            string.IsNullOrWhiteSpace(value) ? l["common.required"] : null;

        public static string? Email(IAkLocalizer l, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return l["common.required"];

            var at = value.IndexOf('@');
            return at > 0 && at < value.Length - 1 && !value.Contains(' ') ? null : l["common.invalid_email"];
        }

        public static string? NewPassword(IAkLocalizer l, string? value, int minLength)
        {
            if (string.IsNullOrEmpty(value)) return l["common.required"];
            return value.Length < minLength ? l.Format("common.too_short", minLength) : null;
        }

        public static string? Confirm(IAkLocalizer l, string? password, string? confirm) =>
            string.IsNullOrEmpty(confirm) ? l["common.required"]
            : confirm != password ? l["common.passwords_mismatch"] : null;
    }
}
