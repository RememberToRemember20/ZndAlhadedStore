using AuthKit.Common;
using FluentValidation;

namespace AuthKit.Features.Auth.Logout.Command
{
    internal class LogoutCommandValidator:AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty().WithErrorCode(ValidationCodes.RefreshTokenRequired);
        }
    }
}

