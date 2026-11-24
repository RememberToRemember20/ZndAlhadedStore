using AuthKit.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithErrorCode(ValidationCodes.EmailRequired);
            RuleFor(x => x.Token).NotEmpty().WithErrorCode(ValidationCodes.TokenRequired);
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).WithErrorCode(ValidationCodes.NewPasswordInvalid);
        }
    }
}

