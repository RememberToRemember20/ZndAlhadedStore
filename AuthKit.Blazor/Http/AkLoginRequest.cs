using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    internal sealed record AkLoginRequest(string Email, string Password);
}
