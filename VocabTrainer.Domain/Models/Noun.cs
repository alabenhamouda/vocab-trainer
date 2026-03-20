using System;
using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Domain.Models;

public class Noun : VocabEntry
{
    public Gender Gender { get; set; }

    public string? PluralForm
    {
        get;
        set
        {
            if (IsSingularOnly && !string.IsNullOrEmpty(value))
                throw new ArgumentException("Singular-only nouns cannot have a plural form.");
            field = value;
        }
    }

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
        : base(term, definition, englishTranslation, imageUrl, example, isClassified: true)
    {
        if (isSingularOnly && isPluralOnly)
            throw new ArgumentException("A noun cannot be both singular-only and plural-only.");

        Gender = gender;
        IsSingularOnly = isSingularOnly;
        IsPluralOnly = isPluralOnly;
        PluralForm = pluralForm;
    }
}
