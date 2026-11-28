using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    internal sealed record AkLogoutRequest(string? RefreshToken);

}
