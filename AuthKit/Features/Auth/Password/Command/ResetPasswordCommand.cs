using AuthKit.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>;
}
