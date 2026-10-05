using FluentValidation;

namespace ZndAlhadedStore.UserCommandQueryHandler.Command
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة.")
                .MinimumLength(8).WithMessage("كلمة المرور يجب أن تكون 8 أحرف على الأقل.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("الاسم الكامل مطلوب.")
                .MaximumLength(100).WithMessage("الاسم الكامل طويل جداً.");
        }
    }
}
