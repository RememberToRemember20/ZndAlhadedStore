using AuthKit.Blazor.Configuration;
using AuthKit.Blazor.Session;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    public sealed class AuthKitApi : IAuthKitApi
    {
        private readonly AuthKitTransport _transport;
        private readonly AuthKitSession _session;
        private readonly AuthKitUiOptions _options;

        public AuthKitApi(AuthKitTransport transport, AuthKitSession session, AuthKitUiOptions options)
        {
            _transport = transport;
            _session = session;
            _options = options;
        }

        private ApiResult<AkAuthTokens> CheckDelivery(ApiResult<AkAuthTokens> result) =>
    result.IsSuccess && _options.RefreshTokenMode == AkRefreshMode.Cookie && result.Value?.RefreshToken is not null
        ? ApiResult<AkAuthTokens>.Fail(ApiError.Local(0, "misconfigured_cookie_mode"))
        : result;

// LoginAsync:
public async Task<ApiResult<AkAuthTokens>> LoginAsync(string email, string password, CancellationToken ct = default) =>
    CheckDelivery(await _transport.SendAsync<AkAuthTokens>(HttpMethod.Post, "api/auth/login", new AkLoginRequest(email, password), null, ct));
        public async Task<ApiResult<AkRegisterResult>> RegisterAsync(IReadOnlyDictionary<string, string> fields, CancellationToken ct = default)
        {
            var result = await _transport.SendAsync<AkRegisterResult>(
                HttpMethod.Post, _options.Endpoints.Register, fields.ToDictionary(p => p.Key, p => p.Value), null, ct);

            return _options.RefreshTokenMode == AkRefreshMode.Cookie && result.Value?.Tokens?.RefreshToken is not null
                ? ApiResult<AkRegisterResult>.Fail(ApiError.Local(0, "misconfigured_cookie_mode"))
                : result;
        }
        public async Task<ApiResult> ForgotPasswordAsync(string email, CancellationToken ct = default) =>
            await _transport.SendAsync<AkNone>(HttpMethod.Post, "api/auth/forgot-password", new AkEmailRequest(email), null, ct);

        public async Task<ApiResult> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default) =>
            await _transport.SendAsync<AkNone>(HttpMethod.Post, "api/auth/reset-password", new AkResetPasswordRequest(email, token, newPassword), null, ct);

        public async Task<ApiResult> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default) =>
            await _transport.SendAsync<AkNone>(HttpMethod.Post, "api/auth/confirm-email", new AkConfirmEmailRequest(userId, token), null, ct);

        public async Task<ApiResult> ResendConfirmationAsync(string email, CancellationToken ct = default) =>
            await _transport.SendAsync<AkNone>(HttpMethod.Post, "api/auth/resend-confirmation", new AkEmailRequest(email), null, ct);

        public Task<ApiResult<AkAuthTokens>> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken ct = default) =>
            AuthedAsync<AkAuthTokens>(HttpMethod.Post, "api/auth/change-password", new AkChangePasswordRequest(currentPassword, newPassword), ct);

        public Task<ApiResult<AkMe>> GetMeAsync(CancellationToken ct = default) =>
            AuthedAsync<AkMe>(HttpMethod.Get, "api/auth/me", null, ct);

        /// طلب موثّق: يجدد التوكن لو منتهي، ويعيد المحاولة مرة واحدة لو رجع 401
        private async Task<ApiResult<T>> AuthedAsync<T>(HttpMethod method, string path, object? body, CancellationToken ct)
        {
            var token = await _session.GetAccessTokenAsync();
            if (token is null) return ApiResult<T>.Fail(ApiError.Local(401, "unauthorized"));

            var result = await _transport.SendAsync<T>(method, path, body, token, ct);

            if (result.Error is { Status: 401 } &&
                await _session.RefreshAsync(token) &&
                await _session.GetAccessTokenAsync() is { } fresh)
                result = await _transport.SendAsync<T>(method, path, body, fresh, ct);

            return result;
        }
    }
}
