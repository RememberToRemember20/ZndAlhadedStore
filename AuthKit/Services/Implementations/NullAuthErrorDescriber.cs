using AuthKit.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Implementations
{
    internal sealed class NullAuthErrorDescriber : IAuthErrorDescriber
    {
        public string? Describe(string code) => null;
    }
}
