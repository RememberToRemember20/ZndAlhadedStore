using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text.RegularExpressions;
using ZndAlhadedStore.Implment;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.Settings;

namespace ZndAlhadedStore.Services
{

    public class EmailBackgroundService : BackgroundService
    {
        private const int MaxAttempts = 3;

        private readonly EmailQueue _queue;
        private readonly SmtpSettings _settings;
        private readonly ILogger<EmailBackgroundService> _logger;
        private static readonly Regex PermanentStatusCode = new(@"\b5\.\d{1,3}\.\d{1,3}\b", RegexOptions.Compiled);


        public EmailBackgroundService(EmailQueue queue, IOptions<SmtpSettings> settings, ILogger<EmailBackgroundService> logger)
        {
            _queue = queue;
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in _queue.Reader.ReadAllAsync(stoppingToken))
                await SendWithRetryAsync(message, stoppingToken);
        }

        private async Task SendWithRetryAsync(EmailMessage message, CancellationToken ct)
        {
            for (var attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    await SendAsync(message, ct);
                    return;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex) when (IsPermanentFailure(ex))
                {
                    _logger.LogError(ex, "فشل دائم في الإرسال (مصادقة أو رفض من الخادم). لن تُعاد المحاولة.");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "فشل إرسال رسالة (محاولة {Attempt} من {Max}).", attempt, MaxAttempts);

                    if (attempt < MaxAttempts)
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), ct);
                }
            }

            _logger.LogError("تعذر إرسال الرسالة بعد {Max} محاولات.", MaxAttempts);
        }
        private static bool IsPermanentFailure(Exception ex) => ex switch
        {
            AuthenticationException => true,
            SmtpCommandException command => (int)command.StatusCode >= 500,
            SmtpProtocolException protocol => PermanentStatusCode.IsMatch(protocol.Message),
            _ => false
        };
        private async Task SendAsync(EmailMessage message, CancellationToken ct)
        {
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            mime.To.Add(MailboxAddress.Parse(message.To));
            mime.Subject = message.Subject;
            mime.Body = new BodyBuilder { HtmlBody = message.HtmlBody, TextBody = message.TextBody }.ToMessageBody();

            using var client = new SmtpClient { Timeout = 30_000 };
            var security = _settings.UseImplicitTls ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            await client.ConnectAsync(_settings.Host, _settings.Port, security, ct);
            await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
            await client.SendAsync(mime, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
