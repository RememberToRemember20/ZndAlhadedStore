using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Configuration
{
    public enum AkColorMode { Auto, Light, Dark }
    public enum AkMotion { None, Subtle, Full }
    public enum AkEnter { None, Fade, FadeUp, Scale }
    public enum AkStorageMode { Session, Local, Memory }
    public enum AkFieldType { Text, Email, Password, Phone }
    public enum AkRefreshMode { Cookie, Body }
   

    public sealed class AkRoutes
    {
        public string Login { get; set; } = "/login";
        public string Register { get; set; } = "/register";
        public string ForgotPassword { get; set; } = "/forgot-password";
        public string ResetPassword { get; set; } = "/reset-password";
        public string ConfirmEmail { get; set; } = "/confirm-email";
        public string Home { get; set; } = "/";
    }
    public sealed class AkFieldSpec
    {
        public required string Key { get; init; }          // اسم الخاصية في JSON (camelCase)
        public string? Label { get; init; }                // نص مباشر
        public string? LabelKey { get; init; }             // أو مفتاح ترجمة (الافتراضي: field.{Key})
        public AkFieldType Type { get; init; } = AkFieldType.Text;
        public bool Required { get; init; } = true;
        public string? Autocomplete { get; init; }
        public int? MaxLength { get; init; }
        public int? MinLength { get; init; }
    }

    public sealed class AkRegisterOptions
    {
        public string PasswordKey { get; set; } = "password";

        public IList<AkFieldSpec> Fields { get; } = new List<AkFieldSpec>
    {
        new() { Key = "email", LabelKey = "common.email", Type = AkFieldType.Email, Autocomplete = "email", MaxLength = 256 },
        new() { Key = "password", LabelKey = "common.password", Type = AkFieldType.Password, Autocomplete = "new-password", MaxLength = 128 }
    };

        public bool ConfirmPassword { get; set; } = true;
        public int PasswordMinLength { get; set; } = 8;
    }

    /// مسارات تخص تطبيقك (مسارات AuthKit.Api ثابتة)
    public sealed class AkEndpoints
    {
        public string Register { get; set; } = "api/auth/register";
        public string Staff { get; set; } = "api/staff";
        public string Users { get; set; } = "api/users";
    }

   

    public sealed class AuthKitUiOptions
    {
        public string ApiBaseAddress { get; set; } = string.Empty;
        public string DefaultLanguage { get; set; } = "ar";
        public IList<string> Languages { get; } = new List<string> { "ar", "en" };

        public AkTheme Theme { get; } = new();
        public AkRoutes Routes { get; } = new();
        public AkEndpoints Endpoints { get; } = new();
        public AkRegisterOptions Register { get; } = new();
        public AkRefreshMode RefreshTokenMode { get; set; } = AkRefreshMode.Cookie;

        public AkStorageMode TokenStorage { get; set; } = AkStorageMode.Session;
        public string StorageKey { get; set; } = "authkit.tokens";

        /// تعديل أي نص: Texts["ar"]["login.title"] = "..." (أو لغة جديدة بكل نصوصها)
        public IDictionary<string, IDictionary<string, string>> Texts { get; } =
            new Dictionary<string, IDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        internal Uri BaseUri => new(ApiBaseAddress.EndsWith('/') ? ApiBaseAddress : ApiBaseAddress + "/");

        internal void Validate()
        {
            if (!Uri.TryCreate(ApiBaseAddress, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new InvalidOperationException("AuthKitUiOptions.ApiBaseAddress must be an absolute http(s) URL.");

            if (!Languages.Contains(DefaultLanguage, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("AuthKitUiOptions.DefaultLanguage must be one of Languages.");

            var keys = Register.Fields.Select(f => f.Key).ToList();
            if (keys.Any(string.IsNullOrWhiteSpace) || keys.Distinct(StringComparer.OrdinalIgnoreCase).Count() != keys.Count)
                throw new InvalidOperationException("Register.Fields must have unique, non-empty keys.");
        }
    }
}
