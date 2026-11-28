using AuthKit.Blazor.Localization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Components
{
    public abstract class AkComponentBase : ComponentBase, IDisposable
    {
        [Inject] protected IAkLocalizer L { get; set; } = default!;

        protected override void OnInitialized() => L.LanguageChanged += OnLanguageChanged;

        private void OnLanguageChanged() => _ = InvokeAsync(StateHasChanged);

        public virtual void Dispose() => L.LanguageChanged -= OnLanguageChanged;
        internal static class AkUrlGuard
        {
            /// مسار نسبي أو https فقط. بدون data: وبدون // (للشعار المضمّن استخدم Theme.Logo)
            public static bool IsSafeImageUrl(string? url)
            {
                if (string.IsNullOrWhiteSpace(url) || url.Length > 2048) return false;
                if (url.StartsWith("//") || url.StartsWith("/\\")) return false;
                return Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri) && (!uri.IsAbsoluteUri || uri.Scheme == Uri.UriSchemeHttps);
            }
        }
    }

}
