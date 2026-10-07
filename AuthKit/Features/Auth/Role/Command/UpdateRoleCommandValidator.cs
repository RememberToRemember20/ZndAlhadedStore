using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    public class UpdateRoleCommandValidator<TRole, TKey> : AbstractValidator<UpdateRoleCommand<TRole, TKey>>
     where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>
    {
        public UpdateRoleCommandValidator() =>
            RuleFor(x => x.NewName).NotEmpty().WithMessage("اسم الدور الجديد مطلوب.");
    }
}
