using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    public sealed record AkRegisterResult(AkAuthTokens? Tokens, bool RequiresEmailConfirmation);

}
