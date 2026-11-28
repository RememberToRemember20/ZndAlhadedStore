using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{
    public sealed record AkMe(string Id, string? Email, bool EmailConfirmed, IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions);

}
