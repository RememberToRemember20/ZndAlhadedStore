using AuthKit.Services.Interfaces;
using Microsoft.Extensions.Options;
using ZndAlhadedStore.Identity;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.Settings;

namespace ZndAlhadedStore.Email
{

    public class SmtpAuthEmailSender : IAuthEmailSender<ApplicationUser>
    {
        private readonly IEmailQueue _queue;
        private readonly FrontendSettings _frontend;
        private readonly ILogger<SmtpAuthEmailSender> _logger;

        public SmtpAuthEmailSender(IEmailQueue queue, IOptions<FrontendSettings> frontend, ILogger<SmtpAuthEmailSender> logger)
        {
            _queue = queue;
            _frontend = frontend.Value;
            _logger = logger;
        }

        public Task SendEmailConfirmationAsync(ApplicationUser user, string encodedToken, CancellationToken cancellationToken = default)
        {
            var link = BuildLink(_frontend.ConfirmEmailPath, ("userId", user.Id), ("token", encodedToken));
            var (subject, html, text) = AuthEmailTemplates.EmailConfirmation(user.FullName, link);
            Enqueue(user, subject, html, text);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetAsync(ApplicationUser user, string encodedToken, CancellationToken cancellationToken = default)
        {
            var link = BuildLink(_frontend.ResetPasswordPath, ("email", user.Email!), ("token", encodedToken));
            var (subject, html, text) = AuthEmailTemplates.PasswordReset(user.FullName, link);
            Enqueue(user, subject, html, text);
            return Task.CompletedTask;
        }

        private void Enqueue(ApplicationUser user, string subject, string html, string text)
        {
            if (!_queue.TryEnqueue(new EmailMessage(user.Email!, subject, html, text)))
                _logger.LogError("طابور الإرسال ممتلئ، تم إسقاط رسالة.");
        }

        private string BuildLink(string path, params (string Key, string Value)[] query)
        {
            var queryString = string.Join("&", query.Select(q => $"{q.Key}={Uri.EscapeDataString(q.Value)}"));
            return $"{_frontend.BaseUrl.TrimEnd('/')}{path}?{queryString}";
        }
    }
}
