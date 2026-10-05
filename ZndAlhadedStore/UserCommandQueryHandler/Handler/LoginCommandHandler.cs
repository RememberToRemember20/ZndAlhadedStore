using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Identity;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.UserCommandQueryHandler.Command;

namespace ZndAlhadedStore.UserCommandQueryHandler.Handler
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthTokenIssuer _tokenIssuer;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAuthTokenIssuer tokenIssuer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenIssuer = tokenIssuer;
        }

        public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.IsActive)
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
