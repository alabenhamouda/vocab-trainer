using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Seeding;

public interface IVocabClassifier
{
    Task<VocabEntry> ClassifyAsync(
        string rawName,
        string rawDefinition,
        string? imageUrl,
        CancellationToken cancellationToken
    );
}
