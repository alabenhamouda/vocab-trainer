namespace VocabTrainer.Application.VocabEntries.Dtos;

public record ExpressionDto(
    Guid Id,
    string Term,
    string? Definition,
    string? EnglishTranslation,
    string? ImageUrl,
    string? Example,
    bool IsClassified,
    string Type
) : VocabEntryDto(Id, Term, Definition, EnglishTranslation, ImageUrl, Example, IsClassified, Type);
