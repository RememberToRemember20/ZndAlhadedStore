using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    public class CreateRoleCommandValidator<TRole, TKey> : AbstractValidator<CreateRoleCommand<TRole, TKey>>
     where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Role).NotNull();
            RuleFor(x => x.Role.Name)
                .NotEmpty().WithMessage("اسم الدور مطلوب.")
                .When(x => x.Role is not null);
        }
    }
}
