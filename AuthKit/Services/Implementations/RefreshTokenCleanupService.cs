using AuthKit.Entities;
using AuthKit.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Implementations
{
    public class RefreshTokenCleanupService<TKey, TContext> : BackgroundService
     where TKey : IEquatable<TKey>
     where TContext : DbContext
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AuthKitOptions _options;
        private readonly ILogger<RefreshTokenCleanupService<TKey, TContext>> _logger;

        public RefreshTokenCleanupService(
            IServiceScopeFactory scopeFactory,
            IOptions<AuthKitOptions> options,
            ILogger<RefreshTokenCleanupService<TKey, TContext>> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_options.TokenCleanupInterval <= TimeSpan.Zero)
            {
                _logger.LogInformation("تنظيف الـ Refresh Tokens المنتهية معطّل.");
                return;
            }

            using var timer = new PeriodicTimer(_options.TokenCleanupInterval);

            do
            {
                try
                {
                    var deleted = await CleanupOnceAsync(stoppingToken);
                    if (deleted > 0)
                        _logger.LogInformation("تم حذف {Count} من الـ Refresh Tokens المنتهية.", deleted);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "فشل تنظيف الـ Refresh Tokens المنتهية.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        public async Task<int> CleanupOnceAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TContext>();

            var cutoff = DateTime.UtcNow.AddDays(-_options.ExpiredTokenRetentionDays);

            return await db.Set<RefreshToken<TKey>>()
                .Where(rt => rt.ExpiresAt < cutoff)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
