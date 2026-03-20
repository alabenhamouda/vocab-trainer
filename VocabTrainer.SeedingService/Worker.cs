using System.Diagnostics;
using VocabTrainer.Application.Seeding;

namespace VocabTrainer.SeedingService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    : BackgroundService
{
    public const string ActivitySourceName = "Seeding";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity(
            "Seed Course Data",
            ActivityKind.Client
        );

        try
        {
            using var scope = serviceProvider.CreateScope();
            var fixup = scope.ServiceProvider.GetRequiredService<VocabDataFixup>();
            await fixup.FixupAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        applicationLifetime.StopApplication();
    }
}
