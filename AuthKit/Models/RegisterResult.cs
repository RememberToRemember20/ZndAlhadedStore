using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Models
{
    // AuthKit/Models/RegisterResult.cs
    public record RegisterResult(AuthResult? Tokens, bool RequiresEmailConfirmation);
}
