using AuthKit.Common;
using FluentValidation;

namespace AuthKit.Features.Auth.RefreshToken.Command
{
    internal sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().MaximumLength(512);
            RuleFor(x => x.AccessToken).MaximumLength(4096);
        }
    }
}

