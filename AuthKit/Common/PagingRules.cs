using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Common
{
    public static class PagingRules
    {
        public static IRuleBuilderOptions<T, int> ValidPage<T>(this IRuleBuilder<T, int> rule) =>
     rule.GreaterThanOrEqualTo(1).WithErrorCode(ValidationCodes.PageInvalid);

        public static IRuleBuilderOptions<T, int> ValidPageSize<T>(this IRuleBuilder<T, int> rule) =>
            rule.InclusiveBetween(1, PagingDefaults.MaxPageSize).WithErrorCode(ValidationCodes.PageSizeInvalid);

    }
}
