using AuthKit.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZndAlhadedStore.Feautres.UserCommandHandler.Command
{
    public record SetUserActiveCommand(string CurrentUserId, string TargetUserId, bool IsActive) : IRequest<Result>;

}
