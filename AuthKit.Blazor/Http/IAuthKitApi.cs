using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    public interface IAuthKitApi
    {
        Task<ApiResult<AkAuthTokens>> LoginAsync(string email, string password, CancellationToken ct = default);
        Task<ApiResult<AkRegisterResult>> RegisterAsync(IReadOnlyDictionary<string, string> fields, CancellationToken ct = default);
        Task<ApiResult> ForgotPasswordAsync(string email, CancellationToken ct = default);
        Task<ApiResult> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default);
        Task<ApiResult> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default);
        Task<ApiResult> ResendConfirmationAsync(string email, CancellationToken ct = default);
        Task<ApiResult<AkAuthTokens>> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken ct = default);
        Task<ApiResult<AkMe>> GetMeAsync(CancellationToken ct = default);
    }
}
