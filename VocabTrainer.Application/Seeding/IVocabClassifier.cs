using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Seeding;

public interface IVocabClassifier
{
    Task<List<VocabEntry>> ClassifyBatchAsync(
        IReadOnlyList<VocabEntry> entries,
        CancellationToken cancellationToken
    );
}
