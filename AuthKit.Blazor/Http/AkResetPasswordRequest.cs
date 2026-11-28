using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    internal sealed record AkResetPasswordRequest(string Email, string Token, string NewPassword);

}
