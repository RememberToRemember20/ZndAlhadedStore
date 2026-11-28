using AuthKit.Api.Contracts;
using AuthKit.Api.Cookies;
using AuthKit.Api.Options;
using AuthKit.Api.Proplem;
using AuthKit.Common;
using AuthKit.Models;
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
        internal RefreshCookieService Cookies => HttpContext.RequestServices.GetRequiredService<RefreshCookieService>();

        /// رفض بدري لطلبات الكوكي غير الموثوقة. يرجع null لو كل شيء سليم
        protected IActionResult? Untrusted() =>
            Cookies.CookieRequested(HttpContext) && !Cookies.IsTrustedRequest(HttpContext)
                ? Failure(Result.Failure(AuthErrorCodes.CsrfRejected))
                : null;

        protected IActionResult FromTokens(Result<AuthResult> result)
        {
            if (!result.IsSuccess) return Failure(result);

            Response.Headers.CacheControl = "no-store";
            return Ok(Deliver(result.Value!));
        }

        protected IActionResult FromRegister(Result<RegisterResult> result)
        {
            if (!result.IsSuccess) return Failure(result);

            Response.Headers.CacheControl = "no-store";
            var value = result.Value!;
            return Ok(new RegisterResponse(value.Tokens is null ? null : Deliver(value.Tokens), value.RequiresEmailConfirmation));
        }

        private TokenResponse Deliver(AuthResult tokens)
        {
            if (!Cookies.CookieRequested(HttpContext))
                return new TokenResponse(tokens.AccessToken, tokens.RefreshToken, tokens.AccessTokenExpiresAt);

            Cookies.Write(HttpContext, tokens.RefreshToken);   // الـ Refresh Token ما بيطلعش في الجسم أبداً
            return new TokenResponse(tokens.AccessToken, null, tokens.AccessTokenExpiresAt);
        }
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
