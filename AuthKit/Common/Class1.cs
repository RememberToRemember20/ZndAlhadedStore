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
    }
}
