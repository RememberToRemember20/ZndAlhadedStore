using AuthKit.Common;
using AuthKit.Features.Auth.Login.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;


namespace AuthKit.Features.Auth.Login.Handler
{
    public class LoginCommandHandler<TUser, TKey> : IRequestHandler<LoginCommand, Result<AuthResult>>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;
        private readonly IUserAccessGuard<TUser>? _accessGuard;

        public LoginCommandHandler(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IAuthTokenIssuer<TUser, TKey> tokenIssuer,
            IUserAccessGuard<TUser>? accessGuard = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenIssuer = tokenIssuer;
            _accessGuard = accessGuard;
        }

        public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Result<AuthResult>.Failure("بيانات الدخول غير صحيحة.");

            if (_accessGuard is not null && !_accessGuard.CanSignIn(user))
                return Result<AuthResult>.Failure("بيانات الدخول غير صحيحة.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
                return Result<AuthResult>.Failure("تم قفل الحساب مؤقتاً بسبب محاولات دخول فاشلة متكررة.");

            if (!signInResult.Succeeded)
                return Result<AuthResult>.Failure("بيانات الدخول غير صحيحة.");

            var authResult = await _tokenIssuer.IssueTokensAsync(user);
            return Result<AuthResult>.Success(authResult);
        }
    }
}
