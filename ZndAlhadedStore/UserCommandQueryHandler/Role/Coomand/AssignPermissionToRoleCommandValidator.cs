using FluentValidation;

namespace ZndAlhadedStore.UserCommandQueryHandler.Role.Coomand
{
    public class AssignPermissionToRoleCommandValidator : AbstractValidator<AssignPermissionToRoleCommand>
    {
        public AssignPermissionToRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("معرّف الدور مطلوب.");

            RuleFor(x => x.PermissionId)
                .GreaterThan(0).WithMessage("معرّف الصلاحية غير صالح.");
        }
    }
}
