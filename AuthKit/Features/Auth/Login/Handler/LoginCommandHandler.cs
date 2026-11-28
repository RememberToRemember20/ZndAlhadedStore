using AuthKit.Common;
using AuthKit.Features.Auth.Login.Command;
using AuthKit.Models;
using AuthKit.Services.Interfaces;
using AuthKit.Settings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;



namespace AuthKit.Features.Auth.Login.Handler
{
    internal class LoginCommandHandler<TUser, TKey> : IRequestHandler<LoginCommand, Result<AuthResult>>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;
        private readonly IUserAccessGuard<TUser>? _accessGuard;
        private readonly AuthKitOptions _options;
        private readonly IAuditContext _audit;
        private readonly IPasswordHasher<TUser> _passwordHasher;
        private static string? _dummyHash;

        public LoginCommandHandler(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IAuthTokenIssuer<TUser, TKey> tokenIssuer,
            IOptions<AuthKitOptions> options,
            IAuditContext audit,
            IPasswordHasher<TUser> passwordHasher,
            IUserAccessGuard<TUser>? accessGuard = null
                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenIssuer = tokenIssuer;
            _audit = audit;
            _accessGuard = accessGuard;
            _options = options.Value;
            _passwordHasher = passwordHasher;

        }
        private void BurnTime(string password)
        {
            _dummyHash ??= _passwordHasher.HashPassword(default!, "AuthKit-dummy-1!");
            _passwordHasher.VerifyHashedPassword(default!, _dummyHash, password);
        }

        public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) 
            { BurnTime(request.Password);
                return Result<AuthResult>.Failure(AuthErrorCodes.InvalidCredentials);
            }
            if (_accessGuard is not null && !_accessGuard.CanSignIn(user))
            { BurnTime(request.Password);
                return Result<AuthResult>.Failure(AuthErrorCodes.InvalidCredentials);
            }
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
                return Result<AuthResult>.Failure( AuthErrorCodes.LockedOut);

            if (!signInResult.Succeeded)
                return Result<AuthResult>.Failure(AuthErrorCodes.InvalidCredentials);   // الثلاثة أماكن (مستخدم غير موجود / Guard / كلمة سر غلط)
            if (_options.RequireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
                return Result<AuthResult>.Failure(AuthErrorCodes.EmailNotConfirmed);
            _audit.SetActor(user.Id.ToString()!);
            var authResult = await _tokenIssuer.IssueTokensAsync(user);
            return Result<AuthResult>.Success(authResult);
        }
    }
}

