using AuthKit.Api.Options;
using AuthKit.Api.Proplem;
using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Controllers
{
    [ApiController]
    public abstract class AuthKitControllerBase : ControllerBase
    {
        private ISender? _sender;
        protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        /// نجاح بدون جسم → 204
        protected IActionResult FromResult(Result result) =>
            result.IsSuccess ? NoContent() : Failure(result);

        /// نجاح بقيمة → 200
        protected IActionResult FromResult<T>(Result<T> result) =>
            result.IsSuccess ? Ok(result.Value) : Failure(result);

        /// نجاح بشكل مخصص (مثلاً 201)
        protected IActionResult FromResult<T>(Result<T> result, Func<T, IActionResult> onSuccess) =>
            result.IsSuccess ? onSuccess(result.Value!) : Failure(result);

        protected IActionResult Failure(Result result)
        {
            var services = HttpContext.RequestServices;
            var options = services.GetRequiredService<IOptions<AuthKitApiOptions>>().Value;
            var describer = services.GetRequiredService<IAuthErrorDescriber>();

            var code = result.ErrorCode ?? AuthErrorCodes.Exception;
            var status = AuthKitProblemMapper.StatusFor(code, options.StatusCodeOverrides);

            var problem = ProblemDetailsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: status,
                title: ReasonPhrases.GetReasonPhrase(status),
                detail: describer.Describe(code));

            problem.Extensions["code"] = code;
            if (result.Details.Count > 0)
                problem.Extensions["details"] = result.Details
                    .Select(d => new { code = d, message = describer.Describe(d) })
                    .ToArray();

            return new ObjectResult(problem) { StatusCode = status };
        }
    }
}
