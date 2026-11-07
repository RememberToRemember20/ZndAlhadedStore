using AuthKit.Entities;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Common.Behaviors
{
    public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
    {
        private readonly IAuditContext _context;
        private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;
        private readonly IAuditLogger? _auditLogger;

        public AuditBehavior(
            IAuditContext context,
            ILogger<AuditBehavior<TRequest, TResponse>> logger,
            IAuditLogger? auditLogger = null)
        {
            _context = context;
            _logger = logger;
            _auditLogger = auditLogger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not IAuditableRequest auditable || _auditLogger is null)
                return await next();

            TResponse response;
            try
            {
                response = await next();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                await WriteAsync(auditable, succeeded: false, AuthErrorCodes.Exception);
                throw;
            }

            if (response is Result result)
                await WriteAsync(auditable, result.IsSuccess, result.ErrorCode);
            else
                await WriteAsync(auditable, succeeded: true, failureCode: null);

            return response;
        }

        private async Task WriteAsync(IAuditableRequest request, bool succeeded, string? failureCode)
        {
            try
            {
                var descriptor = request.Describe();

                await _auditLogger!.LogAsync(new AuditLog
                {
                    OccurredAtUtc = DateTime.UtcNow,
                    EventType = descriptor.EventType,
                    Succeeded = succeeded,
                    FailureCode = failureCode,
                    ActorUserId = _context.ActorUserId,
                    Subject = descriptor.Subject,
                    TargetId = descriptor.TargetId,
                    IpAddress = _context.IpAddress,
                    UserAgent = _context.UserAgent,
                    Details = descriptor.Details
                }, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "تعذر تسجيل حدث التدقيق.");
            }
        }
    }
}
