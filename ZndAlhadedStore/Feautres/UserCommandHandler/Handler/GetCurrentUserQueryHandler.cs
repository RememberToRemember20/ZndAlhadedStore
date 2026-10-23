using AuthKit.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;
using ZndAlhadedStore.Feautres.UserCommandHandler.Query;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Feautres.UserCommandHandler.Handler
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

        public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null || !user.IsActive)
                return Result<CurrentUserDto>.Failure("الحساب غير متاح.");

            return Result<CurrentUserDto>.Success(new CurrentUserDto(user.Id, user.Email!, user.FullName, user.EmailConfirmed));
        }
    }
}
