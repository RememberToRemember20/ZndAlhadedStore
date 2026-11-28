using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Session
{
    public interface ISessionHint
    {
        ValueTask<bool> GetAsync();
        ValueTask SetAsync(bool value);
    }

    public sealed class JsSessionHint : ISessionHint
    {
        private const string Key = "authkit.hint";
        private readonly AuthKitJs _js;
        public JsSessionHint(AuthKitJs js) => _js = js;

        public async ValueTask<bool> GetAsync() => await _js.GetItemAsync("local", Key) == "1";

        public async ValueTask SetAsync(bool value)
        {
            if (value) await _js.SetItemAsync("local", Key, "1");
            else await _js.RemoveItemAsync("local", Key);
        }
    }

    internal sealed class CookieModeHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            request.Headers.TryAddWithoutValidation("X-AuthKit-Token-Delivery", "cookie");
            return base.SendAsync(request, cancellationToken);
        }
    }
}
