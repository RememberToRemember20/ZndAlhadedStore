using AuthKit.Common;
using FluentValidation;

namespace ZndAlhadedStore.Feautres.Staff.Query
{
    public class GetStaffQueryValidator : AbstractValidator<GetStaffQuery>
    {
        public GetStaffQueryValidator()
        {
            RuleFor(x => x.Page).ValidPage();
            RuleFor(x => x.PageSize).ValidPageSize();
            RuleFor(x => x.Role).MaximumLength(256);
        }
    }
}
