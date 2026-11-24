using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Contracts
{
    public record LogoutRequest(string RefreshToken);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
    public record CreateRoleRequest(string Name);
    public record UpdateRoleRequest(string NewName);
    public record AssignPermissionRequest(int PermissionId);
    public record AssignRoleRequest(string RoleName);
    public record CreatedResponse(string Id);
}
