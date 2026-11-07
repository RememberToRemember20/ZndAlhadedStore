using AuthKit.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.Audit.Query
{
    public class GetAuditLogsQueryValidator : AbstractValidator<GetAuditLogsQuery>
    {
        public GetAuditLogsQueryValidator()
        {
            RuleFor(x => x.Page).ValidPage();
            RuleFor(x => x.PageSize).ValidPageSize();
            RuleFor(x => x.EventType).MaximumLength(64);
            RuleFor(x => x.ActorUserId).MaximumLength(128);
            RuleFor(x => x).Must(x => x.FromUtc is null || x.ToUtc is null || x.FromUtc <= x.ToUtc)
                .WithMessage("تاريخ البداية يجب أن يسبق تاريخ النهاية.");
        }
    }

}
