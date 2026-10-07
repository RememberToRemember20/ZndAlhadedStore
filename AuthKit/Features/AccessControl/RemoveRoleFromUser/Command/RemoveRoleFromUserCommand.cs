using AuthKit.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.RemoveRoleFromUser.Command
{
    public record RemoveRoleFromUserCommand(string TargetUserId, string RoleName) : IRequest<Result>;
}
