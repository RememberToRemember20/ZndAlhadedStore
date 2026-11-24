using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Common
{
    public static class AuthErrorCodes
    {
        public const string InvalidCredentials = "invalid_credentials";
        public const string LockedOut = "locked_out";
        public const string EmailNotConfirmed = "email_not_confirmed";
        public const string TokenReuseDetected = "token_reuse_detected";
        public const string InvalidToken = "invalid_token";
        public const string AccountUnavailable = "account_unavailable";
        public const string Exception = "exception";
        public const string InvalidRole = "invalid_role";
        public const string PrivilegeEscalation = "privilege_escalation";
        public const string ProvisioningRejected = "provisioning_rejected";
        public const string SessionExpired = "session_expired";
        public const string Forbidden = "forbidden";
        public const string LastAdministrator = "last_administrator";
        public const string ProtectedRole = "protected_role";
        public const string EmailTaken = "email_taken";
        public const string UserNotFound = "user_not_found";
        public const string InvalidLink = "invalid_link";
        public const string InvalidCurrentPassword = "invalid_current_password";
        public const string RoleNameTaken = "role_name_taken";
        public const string RoleInUse = "role_in_use";
        public const string UserAlreadyInRole = "user_already_in_role";
        public const string PermissionNotFound = "permission_not_found";
        public const string PermissionAlreadyAssigned = "permission_already_assigned";
        public const string IdentityError = "identity_error";
        
    }
    public static class ValidationCodes
    {
        public const string PageInvalid = "page_invalid";
        public const string PageSizeInvalid = "page_size_invalid";
        public const string RolesCountInvalid = "roles_count_invalid";
        public const string DateRangeInvalid = "date_range_invalid";
        public const string UserRequired = "user_required";
        public const string RoleIdRequired = "role_id_required";
        public const string PermissionIdInvalid = "permission_id_invalid";  
        public const string TokenRequired = "token_required";
        public const string RefreshTokenRequired = "refresh_token_required";
        public const string CurrentPasswordRequired = "current_password_required";
        public const string NewPasswordInvalid = "new_password_invalid";
        public const string NewPasswordCannotBeSame = "new_password_cannot_be_same";
        public const string EmailRequired = "email_required";
        public const string AccessTokenRequired = "access_token_required";
        public const string InvalidEmail = "invalid_email";
        public const string PasswordRequired = "password_required";
        public const string PasswordTooShort = "password_too_short";
        public const string RoleNameRequired = "role_name_required";
    }
    public static class AuthKitRateLimitPolicies
    {
        public const string Sensitive = "auth-sensitive";
        public const string Moderate = "auth-moderate";
    }
}
