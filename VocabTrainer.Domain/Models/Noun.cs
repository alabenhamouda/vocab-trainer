using System;
using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Domain.Models;

public class Noun : VocabEntry
{
    public Gender Gender { get; private set; }
    public string? PluralForm { get; private set; }
    public bool IsSingularOnly { get; private set; }
    public bool IsPluralOnly { get; private set; }

    public Noun(
        string term,
        string? definition,
        string? englishTranslation,
        string? imageUrl,
        Gender gender,
        string? pluralForm = null,
        bool isSingularOnly = false,
        bool isPluralOnly = false,
        string? example = null
    )
        : base(term, definition, englishTranslation, imageUrl, example)
    {
        if (isSingularOnly && isPluralOnly)
            throw new ArgumentException("A noun cannot be both singular-only and plural-only.");

        if (isSingularOnly && !string.IsNullOrEmpty(pluralForm))
            throw new ArgumentException("Singular-only nouns cannot have a plural form.");

        Gender = gender;
        PluralForm = pluralForm;
        IsSingularOnly = isSingularOnly;
        IsPluralOnly = isPluralOnly;
    }
}
