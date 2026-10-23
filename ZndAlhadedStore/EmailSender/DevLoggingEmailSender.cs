using AuthKit.Services.Interfaces;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.EmailSender
{
    public class DevLoggingEmailSender : IAuthEmailSender<ApplicationUser>
    {
        private readonly ILogger<DevLoggingEmailSender> _logger;
        public DevLoggingEmailSender(ILogger<DevLoggingEmailSender> logger) => _logger = logger;

        public Task SendPasswordResetAsync(ApplicationUser user, string encodedToken, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[DEV] Password reset token for {Email}: {Token}", user.Email, encodedToken);
            return Task.CompletedTask;
        }

        public Task SendEmailConfirmationAsync(ApplicationUser user, string encodedToken, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[DEV] Email confirmation for {Email} | UserId: {UserId} | Token: {Token}",
                user.Email, user.Id, encodedToken);
            return Task.CompletedTask;
        }
    }
}
