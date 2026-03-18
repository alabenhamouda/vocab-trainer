using VocabTrainer.Application.Seeding;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Infrastructure.Seeding;

public class PassthroughVocabClassifier : IVocabClassifier
{
    public Task<List<VocabEntry>> ClassifyBatchAsync(
        IReadOnlyList<VocabEntry> entries,
        CancellationToken cancellationToken
    )
    {
        var result = entries
            .Select(e =>
                (VocabEntry)
                    new Expression(
                        e.Term,
                        e.Definition,
                        e.EnglishTranslation,
                        e.ImageUrl,
                        e.Example,
                        isClassified: true
                    )
            )
            .ToList();

        return Task.FromResult(result);
    }
}
