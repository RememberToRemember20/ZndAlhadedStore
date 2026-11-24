using AuthKit.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Extensions
{
    internal static class IdentityResultExtensions
    {
        public static Result ToFailure(this IdentityResult result) =>
            Result.Failure(AuthErrorCodes.IdentityError, result.Errors.Select(e => e.Code));

        public static Result<T> ToFailure<T>(this IdentityResult result) =>
            Result<T>.Failure(AuthErrorCodes.IdentityError, result.Errors.Select(e => e.Code));
    }
}
