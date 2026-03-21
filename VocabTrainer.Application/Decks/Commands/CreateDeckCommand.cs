using MediatR;

namespace VocabTrainer.Application.Decks.Commands;

public record CreateDeckCommand(string Title, string? Description) : IRequest<Guid>;
