using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Models
{
    public record AuthResult(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
}
