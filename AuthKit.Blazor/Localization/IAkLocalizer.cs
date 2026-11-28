using AuthKit.Blazor.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Localization
{
    public interface IAkLocalizer
    {
        string Language { get; }
        bool IsRtl { get; }
        IReadOnlyList<string> Languages { get; }
        string this[string key] { get; }
        string Format(string key, params object[] args);
        string Error(ApiError? error);
        string Error(string? code, string? serverDetail = null);
        void SetLanguage(string language);
        event Action? LanguageChanged;
    }
}
