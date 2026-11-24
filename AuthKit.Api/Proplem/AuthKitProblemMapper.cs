using AuthKit.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Proplem
{
    public static class AuthKitProblemMapper
    {
        public static readonly IReadOnlyDictionary<string, int> DefaultStatusCodes = new Dictionary<string, int>
        {
            [AuthErrorCodes.InvalidCredentials] = 401,
            [AuthErrorCodes.LockedOut] = 401,
            [AuthErrorCodes.EmailNotConfirmed] = 401,
            [AuthErrorCodes.InvalidToken] = 401,
            [AuthErrorCodes.TokenReuseDetected] = 401,
            [AuthErrorCodes.SessionExpired] = 401,
            [AuthErrorCodes.AccountUnavailable] = 401,

            [AuthErrorCodes.Forbidden] = 403,
            [AuthErrorCodes.PrivilegeEscalation] = 403,
            [AuthErrorCodes.ProtectedRole] = 403,

            [AuthErrorCodes.UserNotFound] = 404,

            [AuthErrorCodes.EmailTaken] = 409,
            [AuthErrorCodes.RoleNameTaken] = 409,
            [AuthErrorCodes.RoleInUse] = 409,
            [AuthErrorCodes.UserAlreadyInRole] = 409,
            [AuthErrorCodes.PermissionAlreadyAssigned] = 409,
            [AuthErrorCodes.LastAdministrator] = 409,

            [AuthErrorCodes.InvalidRole] = 400,
            [AuthErrorCodes.PermissionNotFound] = 400,
            [AuthErrorCodes.InvalidLink] = 400,
            [AuthErrorCodes.InvalidCurrentPassword] = 400,
            [AuthErrorCodes.IdentityError] = 400,
            [AuthErrorCodes.ProvisioningRejected] = 400,

            [AuthErrorCodes.Exception] = 500
        };

        public static int StatusFor(string? code, IDictionary<string, int>? overrides = null)
        {
            if (code is null) return 400;
            if (overrides is not null && overrides.TryGetValue(code, out var custom)) return custom;
            return DefaultStatusCodes.TryGetValue(code, out var status) ? status : 400;
        }
    }
}
