using AuthKit.Blazor.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Session
{
    public interface ITokenStore
    {
        ValueTask<AkAuthTokens?> LoadAsync();
        ValueTask<bool> SaveAsync(AkAuthTokens tokens);
        ValueTask ClearAsync();
    }
}
