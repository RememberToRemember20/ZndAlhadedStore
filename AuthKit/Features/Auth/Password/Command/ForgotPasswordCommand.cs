using AuthKit.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result>;
}
