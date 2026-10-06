using AuthKit.Common;
using AuthKit.Features.Auth.LogoutAll.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace AuthKit.Features.Auth.LogoutAll.Handler
{
    public class LogoutAllCommandHandler<TUser, TKey> : IRequestHandler<LogoutAllCommand, Result>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;

        public LogoutAllCommandHandler(IAuthTokenIssuer<TUser, TKey> tokenIssuer)
        {
            _tokenIssuer = tokenIssuer;
        }

        public async Task<Result> Handle(LogoutAllCommand request, CancellationToken cancellationToken)
        {
            var userId = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromString(request.UserId)!;
            await _tokenIssuer.RevokeAllActiveTokensAsync(userId, cancellationToken);
            return Result.Success();
        }
    }
}
