using AuthKit.Common;
using AuthKit.Features.Auth.Register.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;


namespace AuthKit.Features.Auth.Register.Handler
{
    public class RegisterCommandHandler<TUser, TKey> : IRequestHandler<RegisterCommand<TUser, TKey>, Result<AuthResult>>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;

        public RegisterCommandHandler(UserManager<TUser> userManager, IAuthTokenIssuer<TUser, TKey> tokenIssuer)
        {
            _userManager = userManager;
            _tokenIssuer = tokenIssuer;
        }

        public async Task<Result<AuthResult>> Handle(RegisterCommand<TUser, TKey> request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.User.Email!);
            if (existingUser is not null)
                return Result<AuthResult>.Failure("البريد الإلكتروني مستخدم بالفعل.");

            var createResult = await _userManager.CreateAsync(request.User, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                return Result<AuthResult>.Failure(errors);
            }

            var authResult = await _tokenIssuer.IssueTokensAsync(request.User);
            return Result<AuthResult>.Success(authResult);
        }
    }
}
