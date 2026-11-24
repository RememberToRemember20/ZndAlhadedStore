using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Middleware
{
    public sealed class ForbiddenException : Exception
    {
        public bool IsAuthenticated { get; }
        public ForbiddenException(bool isAuthenticated) : base("Access denied.") => IsAuthenticated = isAuthenticated;
    }
}
