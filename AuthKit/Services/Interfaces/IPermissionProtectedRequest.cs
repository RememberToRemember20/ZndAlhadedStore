using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface IPermissionProtectedRequest
    {
        string RequiredPermission { get; }
    }
}
