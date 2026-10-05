using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Identity;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.UserCommandQueryHandler.Command;

namespace ZndAlhadedStore.UserCommandQueryHandler.Handler
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthTokenIssuer _tokenIssuer;

        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, IAuthTokenIssuer tokenIssuer)
        {
            _userManager = userManager;
            _tokenIssuer = tokenIssuer;
        }

        public async Task<Result<AuthResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                return Result<AuthResult>.Failure("البريد الإلكتروني مستخدم بالفعل.");

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                return Result<AuthResult>.Failure(errors);
            }

            var authResult = await _tokenIssuer.IssueTokensAsync(user);
            return Result<AuthResult>.Success(authResult);
        }
    }
}
