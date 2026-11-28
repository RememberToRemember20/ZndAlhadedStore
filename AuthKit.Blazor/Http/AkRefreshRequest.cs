using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    internal sealed record AkRefreshRequest(string? AccessToken, string? RefreshToken);

}
