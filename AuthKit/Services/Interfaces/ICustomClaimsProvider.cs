using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface ICustomClaimsProvider<TUser>
    {
        IEnumerable<Claim> GetClaims(TUser user);
    }
}
