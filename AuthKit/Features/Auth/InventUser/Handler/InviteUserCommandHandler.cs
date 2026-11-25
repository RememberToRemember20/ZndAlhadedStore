using AuthKit.Common;
using AuthKit.Features.Auth.InventUser.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AuthKit.Features.Auth.InventUser.Handler
{
    internal class InviteUserCommandHandler<TUser, TRole, TKey, TContext>
    : IRequestHandler<InviteUserCommand<TUser, TKey>, Result<string>>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TContext : DbContext
    {
        private readonly UserManager<TUser> _userManager;
        private readonly RoleManager<TRole> _roleManager;
        private readonly IUserPermissionResolver<TKey> _permissions;
        private readonly IAuthEmailSender<TUser> _emailSender;
        private readonly IEnumerable<IUserProvisioningValidator<TUser>> _validators;
        private readonly TContext _dbContext;
        private readonly ILogger<InviteUserCommandHandler<TUser, TRole, TKey, TContext>> _logger;

        public InviteUserCommandHandler(
            UserManager<TUser> userManager, RoleManager<TRole> roleManager,
            IUserPermissionResolver<TKey> permissions, IAuthEmailSender<TUser> emailSender,
            IEnumerable<IUserProvisioningValidator<TUser>> validators, TContext dbContext,
            ILogger<InviteUserCommandHandler<TUser, TRole, TKey, TContext>> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _permissions = permissions;
            _emailSender = emailSender;
            _validators = validators;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(InviteUserCommand<TUser, TKey> request, CancellationToken cancellationToken)
        {
            // 1) الأدوار المطلوبة موجودة
            var roles = new List<TRole>();
            foreach (var name in request.RoleNames.Select(r => r.Trim()).Where(r => r.Length > 0)
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var role = await _roleManager.FindByNameAsync(name);
                if (role is null)
                    return Result<string>.Failure(AuthErrorCodes.InvalidRole);
                roles.Add(role);
            }

            // 2) منع تصعيد الصلاحيات: لا تمنح إلا ما تملكه
            var caller = await _userManager.FindByIdAsync(request.InvitedByUserId);
            if (caller is null)
                return Result<string>.Failure( AuthErrorCodes.AccountUnavailable);

            var callerPermissions = await _permissions.GetUserPermissionsAsync(caller.Id, cancellationToken);
            var requested = await _permissions.GetRolePermissionsAsync(roles.Select(r => r.Id).ToList(), cancellationToken);

            if (requested.Except(callerPermissions, StringComparer.Ordinal).Any())
                return Result<string>.Failure(AuthErrorCodes.PrivilegeEscalation);

            // 3) قواعد التطبيق الخاصة
            var roleNames = roles.Select(r => r.Name!).ToList();
            var context = new UserProvisioningContext<TUser>(request.User, roleNames, request.InvitedByUserId);

            foreach (var validator in _validators)
            {
                var error = await validator.ValidateAsync(context, cancellationToken);
                if (error is not null)
                    return Result<string>.Failure(AuthErrorCodes.ProvisioningRejected, [error]);   

            }

            if (await _userManager.FindByEmailAsync(request.User.Email!) is not null)
                return Result<string>.Failure(AuthErrorCodes.EmailTaken);

            // 4) إنشاء ذري: المستخدم (ومعه بروفايله لو موجود في الـ Graph) + الأدوار
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var created = await _userManager.CreateAsync(request.User, GenerateUnknownPassword());
                if (!created.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<string>.Failure(string.Join(" | ", created.Errors.Select(e => e.Description)));
                }

                var added = await _userManager.AddToRolesAsync(request.User, roleNames);
                if (!added.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<string>.Failure(string.Join(" | ", added.Errors.Select(e => e.Description)));
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "فشل إنشاء المستخدم المدعو، تم التراجع الكامل.");
                return Result<string>.Failure(AuthErrorCodes.Exception);
            }

            // 5) الدعوة بعد نجاح الحفظ؛ فشلها ما بيلغيش الحساب
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(request.User);
                await _emailSender.
                    SendInvitationAsync(request.User, UrlTokenEncoder.Encode(token), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "تم إنشاء الحساب لكن فشل إرسال الدعوة.");
            }

            return Result<string>.Success(request.User.Id.ToString()!);
        }

        // تحقق شروط Identity، وباقيها عشوائي لا يعرفه أحد
        private static string GenerateUnknownPassword() =>
            "Aa1!" + Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}

