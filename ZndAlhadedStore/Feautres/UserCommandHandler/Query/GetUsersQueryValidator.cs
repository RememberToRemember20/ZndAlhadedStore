
using FluentValidation;
using AuthKit.Common;
namespace ZndAlhadedStore.Feautres.UserCommandHandler.Query
{

    public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        public GetUsersQueryValidator()
        {
            RuleFor(x => x.Page).ValidPage();
            RuleFor(x => x.PageSize).ValidPageSize();
            RuleFor(x => x.Search).MaximumLength(100);
        }
    }
}
