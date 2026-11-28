using AuthKit.Blazor.Configuration;
using AuthKit.Blazor.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AuthKit.Blazor.Localization
{
    public sealed class AkLocalizer : IAkLocalizer
    {
        private static readonly HashSet<string> Rtl = new(StringComparer.OrdinalIgnoreCase) { "ar", "he", "fa", "ur" };
        private readonly AuthKitUiOptions _options;

        public AkLocalizer(AuthKitUiOptions options)
        {
            _options = options;
            Language = options.DefaultLanguage;
        }

        public string Language { get; private set; }
        public bool IsRtl => Rtl.Contains(Language.Split('-')[0]);
        public IReadOnlyList<string> Languages => (IReadOnlyList<string>)_options.Languages;
        public event Action? LanguageChanged;

        public string this[string key] => Lookup(key) ?? key;

        public string Format(string key, params object[] args) =>
            string.Format(CultureInfo.InvariantCulture, this[key], args);

        public string Error(ApiError? error)
        {
            if (error is null) return this["error.unknown"];
            if (error.Code == "rate_limited" && error.RetryAfterSeconds is > 0)
                return Format("error.rate_limited_after", error.RetryAfterSeconds.Value);
            return Error(error.Code, error.Detail);
        }

        public string Error(string? code, string? serverDetail = null)
        {
            if (!string.IsNullOrEmpty(code) && Lookup($"error.{code}") is { } text) return text;
            return !string.IsNullOrWhiteSpace(serverDetail) ? serverDetail : this["error.unknown"];
        }

        public void SetLanguage(string language)
        {
            if (!_options.Languages.Contains(language, StringComparer.OrdinalIgnoreCase) ||
                string.Equals(language, Language, StringComparison.OrdinalIgnoreCase)) return;

            Language = language;
            LanguageChanged?.Invoke();
        }

        private string? Lookup(string key)
        {
            if (_options.Texts.TryGetValue(Language, out var custom) && custom.TryGetValue(key, out var overridden))
                return overridden;

            if (AkTexts.For(Language) is { } builtIn && builtIn.TryGetValue(key, out var text)) return text;
            return AkTexts.En.TryGetValue(key, out var fallback) ? fallback : null;
        }
    }
}
