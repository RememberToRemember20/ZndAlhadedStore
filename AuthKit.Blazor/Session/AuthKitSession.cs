using AuthKit.Blazor.Configuration;
using AuthKit.Blazor.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AuthKit.Blazor.Session
{
    public sealed class AuthKitSession
    {
        private static readonly TimeSpan RefreshWindow = TimeSpan.FromSeconds(30);
        private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

        private readonly ITokenStore _store;
        private readonly ISessionHint _hint;
        private readonly AuthKitTransport _transport;
        private readonly IRefreshLock _lock;
        private readonly AuthKitUiOptions _options;
        private readonly TimeProvider _time;

        private AkAuthTokens? _tokens;
        private bool _loaded;
        private Task<bool>? _loading;

        public AuthKitSession(ITokenStore store, ISessionHint hint, AuthKitTransport transport,
            IRefreshLock refreshLock, AuthKitUiOptions options, TimeProvider time)
        {
            _store = store; _hint = hint; _transport = transport; _lock = refreshLock; _options = options; _time = time;
        }

        private bool CookieMode => _options.RefreshTokenMode == AkRefreshMode.Cookie;

        public event Action? Changed;
        public bool IsSignedIn => _tokens is not null;

        /// false أثناء الـ Prerendering (JS غير متاح): الحالة "لسه مش معروفة" مش "غير مسجّل"
        public async ValueTask<bool> EnsureLoadedAsync()
        {
            if (_loaded) return true;
            return await (_loading ??= LoadAsync());
        }

        private async Task<bool> LoadAsync()
        {
            try
            {
                if (CookieMode)
                {
                    // الجلسة الحقيقية في كوكي HttpOnly. التوكن الوحيد اللي بنعرفه هو اللي بيرجع من التجديد
                    if (await _hint.GetAsync())
                        await _lock.RunExclusiveAsync(() => RefreshCoreAsync(null));
                }
                else
                    _tokens = await _store.LoadAsync();

                _loaded = true;
                return true;
            }
            catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException)
            {
                _loading = null;
                return false;
            }
        }

        public async Task<ClaimsPrincipal> GetPrincipalAsync()
        {
            if (!await EnsureLoadedAsync() || _tokens is null) return Anonymous;
            return AkJwt.ToPrincipal(_tokens.AccessToken) ?? Anonymous;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            if (!await EnsureLoadedAsync() || _tokens is null) return null;

            if (_tokens.AccessTokenExpiresAt - _time.GetUtcNow() > RefreshWindow)
                return _tokens.AccessToken;

            return await RefreshAsync(_tokens.AccessToken) ? _tokens?.AccessToken : null;
        }

        public async Task<bool> RefreshAsync(string? staleAccessToken = null)
        {
            if (!await EnsureLoadedAsync()) return false;
            return await _lock.RunExclusiveAsync(() => RefreshCoreAsync(staleAccessToken));
        }

        private async Task<bool> RefreshCoreAsync(string? staleAccessToken)
        {
            AkAuthTokens? stored = null;

            if (CookieMode)
            {
                // طلب تاني في نفس التبويب جدّد بالفعل
                if (staleAccessToken is not null && _tokens is not null && _tokens.AccessToken != staleAccessToken)
                    return true;
            }
            else
            {
                stored = await _store.LoadAsync();
                if (stored is null) { await ClearLocalAsync(); return false; }

                // تبويب تاني جدّد بالفعل: نتبنى نتيجته من غير طلب
                if (staleAccessToken is not null && stored.AccessToken != staleAccessToken)
                {
                    _tokens = stored;
                    Changed?.Invoke();
                    return true;
                }
            }

            var request = CookieMode ? new AkRefreshRequest(null, null) : new AkRefreshRequest(stored!.AccessToken, stored.RefreshToken);
            var result = await _transport.SendAsync<AkAuthTokens>(HttpMethod.Post, "api/auth/refresh-token", request);

            // في وضع الكوكي الخادم ما يرجّعش Refresh Token في الجسم. لو رجّعه فالإعدادات غير متوافقة
            var misconfigured = CookieMode && result.IsSuccess && result.Value?.RefreshToken is not null;

            if (result.IsSuccess && result.Value is { } fresh && !misconfigured)
            {
                await AdoptAsync(fresh);
                return true;
            }

            // رفض نهائي: الجلسة انتهت. أي سبب تاني (شبكة، 5xx، 429): نحتفظ بحالتنا ونحاول لاحقاً
            if (misconfigured || result.Error is { Status: 400 or 401 or 403 })
                await ClearLocalAsync();

            return false;
        }

        public Task SignInAsync(AkAuthTokens tokens) =>
            AdoptAsync(CookieMode ? tokens with { RefreshToken = null } : tokens);

        public async Task SignOutAsync(bool allDevices = false)
        {
            var access = await GetAccessTokenAsync();

            if (access is not null)
            {
                if (allDevices)
                    await _transport.SendAsync<AkNone>(HttpMethod.Post, "api/auth/logout-all", null, access);
                else
                    await _transport.SendAsync<AkNone>(HttpMethod.Post, "api/auth/logout",
                        new AkLogoutRequest(CookieMode ? null : _tokens?.RefreshToken), access);
            }

            await ClearLocalAsync();   // الكوكي بيتمسح من الخادم؛ الخروج المحلي لازم يحصل حتى لو الطلب فشل
        }

        private async Task AdoptAsync(AkAuthTokens tokens)
        {
            _tokens = tokens;
            _loaded = true;

            if (CookieMode) await _hint.SetAsync(true);
            else await _store.SaveAsync(tokens);

            Changed?.Invoke();
        }

        private async Task ClearLocalAsync()
        {
            _tokens = null;
            _loaded = true;

            if (CookieMode) await _hint.SetAsync(false);
            else await _store.ClearAsync();

            Changed?.Invoke();
        }
    }
}
