using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Localization
{
    public static partial class AkTexts
    {
        public static IReadOnlyDictionary<string, string>? For(string language) =>
            language.Split('-')[0].ToLowerInvariant() switch { "ar" => Ar, "en" => En, _ => null };
    }
}
