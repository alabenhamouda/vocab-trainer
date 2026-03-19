namespace VocabTrainer.Application.VocabEntries.Dtos;

public record VocabEntryDto(
    Guid Id,
    string Term,
    string? Definition,
    string? EnglishTranslation,
    string? ImageUrl,
    string? Example,
    bool IsClassified,
    string Type
);
