using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator() =>
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");
    }
}
