using AuthKit.Middleware;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Common.Behaviors
{
    internal class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    {
        private readonly ICurrentUser _currentUser;
        public AuthorizationBehavior(ICurrentUser currentUser) => _currentUser = currentUser;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is IPermissionProtectedRequest protectedRequest && !_currentUser.IsSystem)
            {
                if (!_currentUser.IsAuthenticated || !_currentUser.HasPermission(protectedRequest.RequiredPermission))
                    throw new ForbiddenException(_currentUser.IsAuthenticated);
            }

            return await next();
        }
    }
}

