using AuthKit.Common;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace AuthKit.Features.Auth.Register.Command
{
    internal class RegisterCommandValidator<TUser, TKey> : AbstractValidator<RegisterCommand<TUser, TKey>>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.User).NotNull().WithErrorCode(ValidationCodes.UserRequired);

            RuleFor(x => x.User.Email)
                .NotEmpty().WithErrorCode(ValidationCodes.EmailRequired)
                .EmailAddress().WithErrorCode(ValidationCodes.InvalidEmail )
                .When(x => x.User is not null);

            RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode(ValidationCodes.PasswordRequired)
                .MinimumLength(8).WithErrorCode(ValidationCodes.PasswordTooShort);
        }
    }
}

