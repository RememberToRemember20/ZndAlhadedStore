using AuthKit.Common;
using FluentValidation;

namespace AuthKit.Features.AccessControl.AssignPermissionToRole.Command
{
    internal class AssignPermissionToRoleCommandValidator : AbstractValidator<AssignPermissionToRoleCommand>
    {
        public AssignPermissionToRoleCommandValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty().WithErrorCode(ValidationCodes.RoleIdRequired);
            RuleFor(x => x.PermissionId).GreaterThan(0).WithErrorCode(ValidationCodes.PermissionIdInvalid);
        }
    }
}

