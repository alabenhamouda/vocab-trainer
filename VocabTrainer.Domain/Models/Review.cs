using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Domain.Models;

public class Review
{
    public Guid Id { get; private set; }
    public Guid DeckId { get; private set; }
    public Guid VocabEntryId { get; private set; }
    public ConfidenceLevel ConfidenceLevel { get; private set; }
    public DateTime LastReviewedAt { get; private set; }
    public DateTime NextReviewAt { get; private set; }

    public Review(Guid deckId, Guid vocabEntryId)
    {
        if (deckId == Guid.Empty)
            throw new ArgumentException("DeckId cannot be empty", nameof(deckId));
        if (vocabEntryId == Guid.Empty)
            throw new ArgumentException("VocabEntryId cannot be empty", nameof(vocabEntryId));

        Id = Guid.NewGuid();
        DeckId = deckId;
        VocabEntryId = vocabEntryId;
        NextReviewAt = DateTime.UtcNow;
    }

    public void RecordReview(ConfidenceLevel level, IReviewStrategy strategy)
    {
        ConfidenceLevel = level;
        LastReviewedAt = DateTime.UtcNow;
        NextReviewAt = strategy.CalculateNextReviewDate(this, level);
    }
}
