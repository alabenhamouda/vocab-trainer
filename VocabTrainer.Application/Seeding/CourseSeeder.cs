using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VocabTrainer.Application.Data;

namespace VocabTrainer.Application.Seeding;

public class CourseSeeder(
    ICourseProvider courseProvider,
    IVocabClassifier vocabClassifier,
    IVocabTrainerDbContext dbContext,
    ILogger<CourseSeeder> logger
)
{
    private const int MaxBatchSize = 10;
    private static readonly TimeSpan DelayBetweenBatches = TimeSpan.FromSeconds(5);

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var courseExists = await dbContext.Courses.AnyAsync(
            c => c.Title == courseProvider.CourseTitle,
            cancellationToken
        );

        if (!courseExists)
        {
            var course = await courseProvider.FetchAsync(cancellationToken);
            dbContext.Courses.Add(course);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var unclassifiedEntries = await dbContext
            .VocabEntries.Include(v => v.Lesson)
            .Where(v => !v.IsClassified && v.Lesson != null)
            .ToListAsync(cancellationToken);

        if (unclassifiedEntries.Count == 0)
            return;

        var batches = unclassifiedEntries.Chunk(MaxBatchSize).ToList();

        for (var i = 0; i < batches.Count; i++)
        {
            var batch = batches[i];

            try
            {
                var classified = await vocabClassifier.ClassifyBatchAsync(batch, cancellationToken);

                for (var j = 0; j < batch.Length; j++)
                {
                    var original = batch[j];
                    var replacement = classified[j];

                    replacement.AssignToLesson(original.LessonId!.Value);
                    dbContext.VocabEntries.Remove(original);
                    dbContext.VocabEntries.Add(replacement);
                }

                if (classified.Count < batch.Length)
                {
                    logger.LogInformation(
                        "Batch {Batch}/{Total}: Classified {ClassifiedCount} out of {BatchSize} entries",
                        i + 1,
                        batches.Count,
                        classified.Count,
                        batch.Length
                    );
                }
                else
                {
                    logger.LogInformation(
                        "Classified batch {Batch}/{Total} ({Count} entries)",
                        i + 1,
                        batches.Count,
                        classified.Count
                    );
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(
                    ex,
                    "Classification failed for batch {Batch}/{Total}, skipping. Will retry on next run.",
                    i + 1,
                    batches.Count
                );
            }

            if (i < batches.Count - 1)
            {
                await Task.Delay(DelayBetweenBatches, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
