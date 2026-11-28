using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AuthKit.Blazor.Configuration
{
    public sealed class AkTheme
    {
        // الهوية
        public string? AppName { get; set; }
        public string? LogoUrl { get; set; }
        public string? LogoAlt { get; set; }
        public RenderFragment? Logo { get; set; }          // شعار مخصص (بيغلب LogoUrl)

        // الألوان (أي قيمة CSS آمنة: #hex أو rgb() أو hsl()...)
        public string? Primary { get; set; }
        public string? PrimaryContrast { get; set; }
        public string? Background { get; set; }
        public string? Surface { get; set; }
        public string? Text { get; set; }
        public string? Muted { get; set; }
        public string? Border { get; set; }
        public string? Danger { get; set; }
        public string? Success { get; set; }

        // الشكل
        public string? Radius { get; set; }                // "12px"
        public string? FontFamily { get; set; }
        public string? LogoHeight { get; set; }            // "56px"

        // الوضع والحركة
        public AkColorMode ColorMode { get; set; } = AkColorMode.Auto;
        public AkMotion Motion { get; set; } = AkMotion.Subtle;
        public AkEnter Enter { get; set; } = AkEnter.FadeUp;
        public int? AnimationMs { get; set; }              // 1 - 2000

        public AkTheme Clone() => (AkTheme)MemberwiseClone();

        internal string ModeAttribute => ColorMode.ToString().ToLowerInvariant();
        internal string MotionAttribute => Motion.ToString().ToLowerInvariant();
        internal string EnterAttribute => Enter switch
        {
            AkEnter.None => "none",
            AkEnter.Fade => "fade",
            AkEnter.FadeUp => "fade-up",
            _ => "scale"
        };

        /// متغيرات CSS للقيم المحددة فقط. القيم غير الآمنة بتتجاهل.
        public string ToStyle()
        {
            var sb = new StringBuilder();

            void Add(string name, string? value)
            {
                if (AkCss.IsSafe(value)) sb.Append(name).Append(':').Append(value!.Trim()).Append(';');
            }

            Add("--ak-color-primary", Primary);
            Add("--ak-color-primary-contrast", PrimaryContrast);
            Add("--ak-color-bg", Background);
            Add("--ak-color-surface", Surface);
            Add("--ak-color-text", Text);
            Add("--ak-color-muted", Muted);
            Add("--ak-color-border", Border);
            Add("--ak-color-danger", Danger);
            Add("--ak-color-success", Success);
            Add("--ak-radius", Radius);
            Add("--ak-font", FontFamily);
            Add("--ak-logo-height", LogoHeight);

            if (AnimationMs is > 0 and <= 2000) sb.Append("--ak-ms:").Append(AnimationMs).Append("ms;");
            return sb.ToString();
        }
    }

    internal static class AkCss
    {
        private static readonly Regex Allowed = new(@"^[A-Za-z0-9#%.,\s()\-_'""/+*]+$", RegexOptions.Compiled);
        private static readonly string[] Forbidden = ["url(", "expression(", "javascript:", "@import"];

        /// الثيم قد يجي من قاعدة بيانات (مستأجرين)، فنمنع حقن CSS
        public static bool IsSafe(string? value) =>
            !string.IsNullOrWhiteSpace(value) && value.Length <= 200 && Allowed.IsMatch(value) &&
            !Forbidden.Any(f => value.Contains(f, StringComparison.OrdinalIgnoreCase));
    }

    /// ثيم كل مستخدم (نسخة مستقلة، فالتعديل في وقت التشغيل ما يأثرش على باقي المستخدمين في Blazor Server)
    public sealed class AkThemeState
    {
        public AkThemeState(AuthKitUiOptions options) => Current = options.Theme.Clone();

        public AkTheme Current { get; private set; }
        public event Action? Changed;

        public void Update(Action<AkTheme> change) { change(Current); Changed?.Invoke(); }
        public void Replace(AkTheme theme) { Current = theme; Changed?.Invoke(); }
    }
}
