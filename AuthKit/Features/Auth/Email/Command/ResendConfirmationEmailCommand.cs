using AuthKit.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Email.Command
{
    public record ResendConfirmationEmailCommand(string Email) : IRequest<Result>;

}
