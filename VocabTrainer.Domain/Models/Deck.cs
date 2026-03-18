using System;
using System.Collections.Generic;

namespace VocabTrainer.Domain.Models;

public class Deck
{
    private readonly List<DeckEntry> _deckEntries = [];
    private readonly List<DeckLesson> _deckLessons = [];

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<DeckEntry> DeckEntries => _deckEntries.AsReadOnly();
    public IReadOnlyCollection<DeckLesson> DeckLessons => _deckLessons.AsReadOnly();

    public Deck(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
    }
}
