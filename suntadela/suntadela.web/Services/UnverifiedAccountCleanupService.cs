using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using suntadela.web.Models;

namespace suntadela.web.Services;

/// <summary>
/// Background service that periodically deletes unverified user accounts
/// that are older than 24 hours.
/// </summary>
public class UnverifiedAccountCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UnverifiedAccountCleanupService> _logger;

    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);
    private static readonly TimeSpan MaxUnverifiedAge = TimeSpan.FromHours(24);

    public UnverifiedAccountCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<UnverifiedAccountCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Unverified account cleanup service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(CheckInterval, stoppingToken);

            try
            {
                await CleanupUnverifiedAccountsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during unverified account cleanup.");
            }
        }
    }

    private async Task CleanupUnverifiedAccountsAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        DateTimeOffset cutoff = DateTimeOffset.UtcNow - MaxUnverifiedAge;

        List<ApplicationUser> expiredUsers = await userManager.Users
            .Where(u => !u.EmailConfirmed && u.CreatedAt < cutoff)
            .ToListAsync(cancellationToken);

        foreach (ApplicationUser user in expiredUsers)
        {
            IdentityResult result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
                _logger.LogInformation("Deleted unverified account: {Email} (created {CreatedAt})", user.Email, user.CreatedAt);
        }

        if (expiredUsers.Count > 0)
            _logger.LogInformation("Cleaned up {Count} unverified account(s).", expiredUsers.Count);
    }
}
