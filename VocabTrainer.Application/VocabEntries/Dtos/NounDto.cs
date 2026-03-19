using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Application.VocabEntries.Dtos;

public record NounDto(
    Guid Id,
    string Term,
    string? Definition,
    string? EnglishTranslation,
    string? ImageUrl,
    string? Example,
    bool IsClassified,
    string Type,
    Gender Gender,
    string? PluralForm,
    bool IsSingularOnly,
    bool IsPluralOnly
) : VocabEntryDto(Id, Term, Definition, EnglishTranslation, ImageUrl, Example, IsClassified, Type);
