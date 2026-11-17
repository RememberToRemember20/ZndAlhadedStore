using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.InventUser.Command
{
    public class InviteUserCommandValidator<TUser, TKey> : AbstractValidator<InviteUserCommand<TUser, TKey>>
      where TUser : IdentityUser<TKey>
      where TKey : IEquatable<TKey>
    {
        public InviteUserCommandValidator()
        {
            RuleFor(x => x.User).NotNull().WithMessage("بيانات المستخدم مطلوبة.");
            RuleFor(x => x.User.Email).NotEmpty().EmailAddress().MaximumLength(256).When(x => x.User is not null);
            RuleFor(x => x.RoleNames).Must(r => r is { Count: >= 1 and <= 10 }).WithMessage("حدد من دور واحد إلى 10 أدوار.");
            RuleFor(x => x.InvitedByUserId).NotEmpty();
        }
    }
}
