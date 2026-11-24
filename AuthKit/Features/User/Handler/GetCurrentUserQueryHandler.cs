using AuthKit.Common;
using AuthKit.Features.User.Query;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.User.Handler
{
    internal sealed class GetCurrentUserQueryHandler<TUser, TKey> : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
     where TUser : IdentityUser<TKey>
     where TKey : IEquatable<TKey>
    {
        private readonly ICurrentUser _currentUser;
        private readonly UserManager<TUser> _userManager;
        private readonly IUserPermissionResolver<TKey> _permissions;
        private readonly IUserAccessGuard<TUser>? _accessGuard;

        public GetCurrentUserQueryHandler(
            ICurrentUser currentUser, UserManager<TUser> userManager,
            IUserPermissionResolver<TKey> permissions, IUserAccessGuard<TUser>? accessGuard = null)
        {
            _currentUser = currentUser;
            _userManager = userManager;
            _permissions = permissions;
            _accessGuard = accessGuard;
        }

        public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is null)
                return Result<CurrentUserDto>.Failure(AuthErrorCodes.AccountUnavailable);

            var user = await _userManager.FindByIdAsync(_currentUser.UserId);
            if (user is null || (_accessGuard is not null && !_accessGuard.CanSignIn(user)))
                return Result<CurrentUserDto>.Failure(AuthErrorCodes.AccountUnavailable);

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var permissions = (await _permissions.GetUserPermissionsAsync(user.Id, cancellationToken)).ToList();

            return Result<CurrentUserDto>.Success(
                new CurrentUserDto(user.Id.ToString()!, user.Email, user.EmailConfirmed, roles, permissions));
        }
    }
}
