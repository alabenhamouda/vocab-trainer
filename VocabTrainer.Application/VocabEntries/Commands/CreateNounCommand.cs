using MediatR;
using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Application.VocabEntries.Commands;

public record CreateNounCommand(
    string Term,
    string? Definition,
    string? EnglishTranslation,
    Gender Gender,
    string? PluralForm,
    bool IsSingularOnly,
    bool IsPluralOnly,
    string? ImageUrl = null
) : IRequest<Guid>;
