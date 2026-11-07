using AuthKit.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.GetRoles.Query
{
    public class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
    {
        public GetRolesQueryValidator()
        {
            RuleFor(x => x.Page).ValidPage();
            RuleFor(x => x.PageSize).ValidPageSize();
        }
    }
}
