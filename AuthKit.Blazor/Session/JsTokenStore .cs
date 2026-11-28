using AuthKit.Blazor.Configuration;
using AuthKit.Blazor.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AuthKit.Blazor.Session
{
    public sealed class JsTokenStore : ITokenStore
    {
        private readonly AuthKitJs _js;
        private readonly AuthKitUiOptions _options;
        private AkAuthTokens? _memory;

        public JsTokenStore(AuthKitJs js, AuthKitUiOptions options)
        {
            _js = js;
            _options = options;
        }

        private string Kind => _options.TokenStorage == AkStorageMode.Local ? "local" : "session";

        public async ValueTask<AkAuthTokens?> LoadAsync()
        {
            if (_options.TokenStorage == AkStorageMode.Memory) return _memory;

            var json = await _js.GetItemAsync(Kind, _options.StorageKey);
            if (string.IsNullOrEmpty(json)) return null;

            try { return JsonSerializer.Deserialize(json, AkJson.Default.AkAuthTokens); }
            catch (JsonException) { await ClearAsync(); return null; }
        }

        public async ValueTask<bool> SaveAsync(AkAuthTokens tokens)
        {
            if (_options.TokenStorage == AkStorageMode.Memory) { _memory = tokens; return true; }
            return await _js.SetItemAsync(Kind, _options.StorageKey, JsonSerializer.Serialize(tokens, AkJson.Default.AkAuthTokens));
        }

        public async ValueTask ClearAsync()
        {
            _memory = null;
            if (_options.TokenStorage != AkStorageMode.Memory) await _js.RemoveItemAsync(Kind, _options.StorageKey);
        }
    }

}
