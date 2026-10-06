using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Auth
{
    public record AuthResult(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
}
