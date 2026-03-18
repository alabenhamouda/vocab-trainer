using VocabTrainer.Application.Seeding;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Infrastructure.Seeding;

public class PassthroughVocabClassifier : IVocabClassifier
{
    public Task<VocabEntry> ClassifyAsync(
        string rawName,
        string rawDefinition,
        string? imageUrl,
        CancellationToken cancellationToken
    )
    {
        VocabEntry entry = new Expression(rawName, rawDefinition, null, imageUrl);
        return Task.FromResult(entry);
    }
}
