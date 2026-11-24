using AuthKit.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    internal class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CurrentPassword).NotEmpty().WithErrorCode(ValidationCodes.CurrentPasswordRequired);
            RuleFor(x => x.NewPassword)
                .NotEmpty().MinimumLength(8).WithErrorCode(ValidationCodes.NewPasswordInvalid)
                .NotEqual(x => x.CurrentPassword).WithErrorCode(ValidationCodes.NewPasswordCannotBeSame);
        }
    }
}

