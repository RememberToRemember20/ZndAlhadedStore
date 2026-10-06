using FluentValidation;

namespace AuthKit.Features.Auth.RefreshToken.Command
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("الـ Access Token مطلوب.");
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("الـ Refresh Token مطلوب.");
        }
    }
}
