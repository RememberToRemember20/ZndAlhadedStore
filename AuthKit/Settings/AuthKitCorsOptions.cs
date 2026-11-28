using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Settings
{
    public sealed class AuthKitCorsOptions
    {
        public IReadOnlyList<string> AllowedOrigins { get; set; } = Array.Empty<string>();
    }
}
