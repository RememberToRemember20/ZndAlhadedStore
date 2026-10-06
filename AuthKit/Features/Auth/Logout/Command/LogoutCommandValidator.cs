using FluentValidation;

namespace AuthKit.Features.Auth.Logout.Command
{
    public class LogoutCommandValidator:AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("الـ Refresh Token مطلوب.");
        }
    }
}
