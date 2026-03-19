using System.Text.Json.Serialization;

namespace VocabTrainer.Application.VocabEntries.Dtos;

[JsonPolymorphic]
[JsonDerivedType(typeof(NounDto))]
[JsonDerivedType(typeof(ExpressionDto))]
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
