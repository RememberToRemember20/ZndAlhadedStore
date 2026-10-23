using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface IAuthEmailSender<TUser>
    {
        Task SendPasswordResetAsync(TUser user, string encodedToken, CancellationToken cancellationToken = default);
        Task SendEmailConfirmationAsync(TUser user, string encodedToken, CancellationToken cancellationToken = default);

    }
}
