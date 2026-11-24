using AuthKit.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Email.Command
{
    internal class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Token).NotEmpty().WithErrorCode(ValidationCodes.TokenRequired);
        }
    }

}

