using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    internal sealed record AkConfirmEmailRequest(string UserId, string Token);

}
