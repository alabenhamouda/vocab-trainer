namespace VocabTrainer.Domain.Models;

public class Expression(
    string term,
    string? definition,
    string? englishTranslation,
    string? imageUrl
) : VocabEntry(term, definition, englishTranslation, imageUrl) { }
