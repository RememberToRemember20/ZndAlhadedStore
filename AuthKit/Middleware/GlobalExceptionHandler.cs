using AuthKit.Middleware;
using AuthKit.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Auth.Middlewares
{
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IAuthErrorDescriber _describer;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IAuthErrorDescriber describer)
        {
            _logger = logger;
            _describer = describer;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ForbiddenException forbidden)
            {
                var status = forbidden.IsAuthenticated ? StatusCodes.Status403Forbidden : StatusCodes.Status401Unauthorized;
                await WriteAsync(httpContext, new ProblemDetails { Status = status, Title = ReasonPhrases.GetReasonPhrase(status) }, cancellationToken);
                return true;
            }

            if (exception is ValidationException validation)
            {
                var codes = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorCode).Distinct().ToArray());

                var messages = codes.ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value.Select(c => _describer.Describe(c) ?? c).ToArray());

                var problem = new ValidationProblemDetails(messages)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)
                };
                problem.Extensions["codes"] = codes;

                await WriteAsync(httpContext, problem, cancellationToken);
                return true;
            }

            _logger.LogError(exception, "Unhandled exception while processing the request.");
            await WriteAsync(httpContext, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError)
            }, cancellationToken);
            return true;
        }

        private static async Task WriteAsync(HttpContext context, ProblemDetails problem, CancellationToken cancellationToken)
        {
            context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problem, problem.GetType(), cancellationToken);   // النوع الفعلي، عشان ما يضيعش Errors
        }
    }
}
