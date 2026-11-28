using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuthKit.Api.Contracts
{
    public record LogoutRequest(string RefreshToken);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
    public record CreateRoleRequest(string Name);
    public record UpdateRoleRequest(string NewName);
    public record AssignPermissionRequest(int PermissionId);
    public record AssignRoleRequest(string RoleName);
    public record CreatedResponse(string Id);

    public sealed record TokenResponse(
        string AccessToken,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? RefreshToken,
        DateTime AccessTokenExpiresAt);

    public sealed record RegisterResponse(TokenResponse? Tokens, bool RequiresEmailConfirmation);

    public record RefreshRequest(string? AccessToken, string? RefreshToken);
}
