using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public record UserProvisioningContext<TUser>(TUser User, IReadOnlyCollection<string> RoleNames, string InvitedByUserId);

    public interface IUserProvisioningValidator<TUser>
    {
        /// يرجع رسالة خطأ لرفض الإنشاء، أو null للموافقة
        /// 
        Task<string?> ValidateAsync(UserProvisioningContext<TUser> context, CancellationToken cancellationToken);
    }
}
