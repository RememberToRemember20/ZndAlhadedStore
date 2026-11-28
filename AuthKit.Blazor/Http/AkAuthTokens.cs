using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    public sealed record AkAuthTokens(string AccessToken, string? RefreshToken, DateTimeOffset AccessTokenExpiresAt);

}
