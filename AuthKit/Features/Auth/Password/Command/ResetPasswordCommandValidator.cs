using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Token).NotEmpty().WithMessage("رمز الاسترجاع مطلوب.");
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).WithMessage("كلمة المرور يجب أن تكون 8 أحرف على الأقل.");
        }
    }
}
