using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace AuthKit.Features.Auth.Register.Command
{
    public class RegisterCommandValidator<TUser, TKey> : AbstractValidator<RegisterCommand<TUser, TKey>>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.User).NotNull().WithMessage("بيانات المستخدم مطلوبة.");

            RuleFor(x => x.User.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.")
                .When(x => x.User is not null);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة.")
                .MinimumLength(8).WithMessage("كلمة المرور يجب أن تكون 8 أحرف على الأقل.");
        }
    }
}
