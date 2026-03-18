using System;

namespace VocabTrainer.Domain.Models;

public class DeckLesson
{
    public Guid Id { get; private set; }
    public Guid DeckId { get; private set; }
    public Guid LessonId { get; private set; }

    public DeckLesson(Guid deckId, Guid lessonId)
    {
        if (deckId == Guid.Empty)
            throw new ArgumentException("DeckId cannot be empty", nameof(deckId));
        if (lessonId == Guid.Empty)
            throw new ArgumentException("LessonId cannot be empty", nameof(lessonId));

        Id = Guid.NewGuid();
        DeckId = deckId;
        LessonId = lessonId;
    }
}
