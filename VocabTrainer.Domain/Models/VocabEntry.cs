using System;

namespace VocabTrainer.Domain.Models;

public abstract class VocabEntry
{
    public Guid Id { get; private set; }
    public string Term { get; private set; }
    public string? Definition { get; private set; } // German definition from DW
    public string? EnglishTranslation { get; private set; } // Translation for ease of understanding
    public string? ImageUrl { get; private set; } // Optional image aid

    protected VocabEntry(
        string term,
        string? definition,
        string? englishTranslation,
        string? imageUrl
    )
    {
        if (string.IsNullOrWhiteSpace(term))
            throw new ArgumentException("Term cannot be empty", nameof(term));

        Id = Guid.NewGuid();
        Term = term;
        Definition = definition;
        EnglishTranslation = englishTranslation;
        ImageUrl = imageUrl;
    }
}
