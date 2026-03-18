namespace VocabTrainer.Domain.Models;

public class Expression(
    string term,
    string? definition,
    string? englishTranslation,
    string? imageUrl,
    string? example = null,
    bool isClassified = false
) : VocabEntry(term, definition, englishTranslation, imageUrl, example, isClassified) { }
