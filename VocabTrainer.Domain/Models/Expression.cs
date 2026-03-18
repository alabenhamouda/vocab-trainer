namespace VocabTrainer.Domain.Models;

public class Expression(
    string term,
    string? definition,
    string? englishTranslation,
    string? imageUrl,
    string? example = null
) : VocabEntry(term, definition, englishTranslation, imageUrl, example) { }
