using DigiMeeting.API.Data;
using Microsoft.EntityFrameworkCore;

namespace DigiMeeting.API.Services;

public class SchedulerBackgroundWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SchedulerBackgroundWorker> _logger;

    public SchedulerBackgroundWorker(IServiceProvider services, ILogger<SchedulerBackgroundWorker> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Background Engine Processing Loop Executing...");

            try
            {
                using (var scope = _services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SchedulerDbContext>();
                    var now = DateTime.UtcNow;

                    // Task A: Clean up Expired Active Waitlist Entries
                    var expiredWaitlists = await context.Waitlists
                        .Where(w => w.Status == "Active" && w.TargetStartTime < now)
                        .ToListAsync(stoppingToken);

                    foreach (var wait in expiredWaitlists)
                    {
                        wait.Status = "Expired";
                        _logger.LogInformation($"Waitlist Record {wait.Id} marked as Expired.");
                    }

                    // Task B: Process Notification Dispatch Queue
                    var pendingNotifications = await context.NotificationQueues
                        .Where(n => !n.IsProcessed)
                        .ToListAsync(stoppingToken);

                    foreach (var note in pendingNotifications)
                    {
                        // Mocking outbound SMTP or Slack webhook call
                        _logger.LogWarning($"[DISPATCH-ALERT] Sending message to {note.RecipientTeamName}: '{note.Message}'");
                        
                        note.IsProcessed = true;
                        note.ProcessedAt = now;
                    }

                    // Save all background modifications safely
                    if (expiredWaitlists.Any() || pendingNotifications.Any())
                    {
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing background tasks.");
            }

            // Wait 30 seconds before running the background loops again
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
