using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Application.VocabEntries.Dtos;

public record NounDto(
    Guid Id,
    string Term,
    string? Definition,
    string? EnglishTranslation,
    string? ImageUrl,
    string? Example,
    Gender Gender,
    string? PluralForm,
    bool IsSingularOnly,
    bool IsPluralOnly
);
