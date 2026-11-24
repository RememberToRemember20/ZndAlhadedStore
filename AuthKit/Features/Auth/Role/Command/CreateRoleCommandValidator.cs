using AuthKit.Common;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using AuthKit.Features.Auth.Role.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{

    internal sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
    }
}

