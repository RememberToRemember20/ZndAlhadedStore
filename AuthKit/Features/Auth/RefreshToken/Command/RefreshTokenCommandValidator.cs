using AuthKit.Common;
using FluentValidation;

namespace AuthKit.Features.Auth.RefreshToken.Command
{
    internal class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty().WithErrorCode(ValidationCodes.AccessTokenRequired);
            RuleFor(x => x.RefreshToken).NotEmpty().WithErrorCode(ValidationCodes.RefreshTokenRequired);
        }
    }
}

