using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Session
{
    public sealed class AuthKitJs : IAsyncDisposable
    {
        private readonly IJSRuntime _js;
        private IJSObjectReference? _module;

        public AuthKitJs(IJSRuntime js) => _js = js;

        private async ValueTask<IJSObjectReference> ModuleAsync() =>
            _module ??= await _js.InvokeAsync<IJSObjectReference>("import", "./_content/AuthKit.Blazor/authkit.js");

        public async ValueTask<string?> GetItemAsync(string kind, string key) =>
            await (await ModuleAsync()).InvokeAsync<string?>("getItem", kind, key);

        public async ValueTask<bool> SetItemAsync(string kind, string key, string value) =>
            await (await ModuleAsync()).InvokeAsync<bool>("setItem", kind, key, value);

        public async ValueTask RemoveItemAsync(string kind, string key) =>
            await (await ModuleAsync()).InvokeVoidAsync("removeItem", kind, key);

        public async ValueTask<bool> WithLockAsync<T>(string name, DotNetObjectReference<T> target, string method) where T : class =>
            await (await ModuleAsync()).InvokeAsync<bool>("withLock", name, target, method);

        public async ValueTask DisposeAsync()
        {
            if (_module is null) return;
            try { await _module.DisposeAsync(); } catch (JSDisconnectedException) { }
        }
    }

}
