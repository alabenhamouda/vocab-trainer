using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;
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
            var courseProvider = scope.ServiceProvider.GetRequiredService<ICourseProvider>();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVocabTrainerDbContext>();

            var course = await courseProvider.FetchAsync(stoppingToken);

            var exists = await dbContext.Courses.AnyAsync(
                c => c.Title == course.Title,
                stoppingToken
            );

            if (!exists)
            {
                dbContext.Courses.Add(course);
                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        applicationLifetime.StopApplication();
    }
}
