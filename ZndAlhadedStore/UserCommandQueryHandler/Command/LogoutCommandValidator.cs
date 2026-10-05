using FluentValidation;

namespace ZndAlhadedStore.UserCommandQueryHandler.Command
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
