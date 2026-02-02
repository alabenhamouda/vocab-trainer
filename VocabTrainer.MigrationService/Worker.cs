using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace VocabTrainer.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity("Run Migrations", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext =
                scope.ServiceProvider.GetRequiredService<Infrastructure.Data.VocabTrainerDbContext>();

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await dbContext.Database.MigrateAsync(stoppingToken);
            });
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        applicationLifetime.StopApplication();
    }
}
