using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Email.Command
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Token).NotEmpty().WithMessage("رمز التأكيد مطلوب.");
        }
    }

}
