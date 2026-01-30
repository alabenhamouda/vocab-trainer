using MediatR;

namespace VocabTrainer.Application.VocabEntries.Commands;

public record CreateExpressionCommand(
    string Term,
    string? Definition,
    string? EnglishTranslation,
    string? ImageUrl = null
) : IRequest<Guid>;
