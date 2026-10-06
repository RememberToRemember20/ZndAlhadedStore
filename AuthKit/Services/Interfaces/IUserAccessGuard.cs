using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface IUserAccessGuard<TUser>
    {
        bool CanSignIn(TUser user);
    }
}
