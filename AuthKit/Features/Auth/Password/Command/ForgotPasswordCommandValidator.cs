using AuthKit.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    internal class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator() =>
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithErrorCode(ValidationCodes.EmailRequired)    ;
    }
}

