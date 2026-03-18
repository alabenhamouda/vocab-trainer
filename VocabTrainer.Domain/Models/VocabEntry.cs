using System;

namespace VocabTrainer.Domain.Models;

public abstract class VocabEntry
{
    public Guid Id { get; private set; }
    public string Term { get; private set; }
    public string? Definition { get; private set; } // German definition from DW
    public string? EnglishTranslation { get; private set; } // Translation for ease of understanding
    public string? ImageUrl { get; private set; } // Optional image aid
    public string? Example { get; private set; } // Example sentence using the term
    public bool IsClassified { get; private set; }
    public Guid? LessonId { get; private set; } // Nullable: vocab entries can be standalone
    public Lesson? Lesson { get; }

    protected VocabEntry(
        string term,
        string? definition,
        string? englishTranslation,
        string? imageUrl,
        string? example = null,
        bool isClassified = false
    )
    {
        if (string.IsNullOrWhiteSpace(term))
            throw new ArgumentException("Term cannot be empty", nameof(term));

        Id = Guid.NewGuid();
        Term = term;
        Definition = definition;
        EnglishTranslation = englishTranslation;
        ImageUrl = imageUrl;
        Example = example;
        IsClassified = isClassified;
    }

    public void AssignToLesson(Guid lessonId)
    {
        LessonId = lessonId;
    }
}
