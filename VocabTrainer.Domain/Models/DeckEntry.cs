using System;

namespace VocabTrainer.Domain.Models;

public class DeckEntry
{
    public Guid Id { get; private set; }
    public Guid DeckId { get; private set; }
    public Guid VocabEntryId { get; private set; }
    public DateTime AddedAt { get; private set; }

    public DeckEntry(Guid deckId, Guid vocabEntryId)
    {
        if (deckId == Guid.Empty)
            throw new ArgumentException("DeckId cannot be empty", nameof(deckId));
        if (vocabEntryId == Guid.Empty)
            throw new ArgumentException("VocabEntryId cannot be empty", nameof(vocabEntryId));

        Id = Guid.NewGuid();
        DeckId = deckId;
        VocabEntryId = vocabEntryId;
        AddedAt = DateTime.UtcNow;
    }
}
