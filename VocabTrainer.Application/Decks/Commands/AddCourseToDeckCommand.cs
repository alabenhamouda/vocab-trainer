using MediatR;

namespace VocabTrainer.Application.Decks.Commands;

public record AddCourseToDeckCommand(Guid DeckId, Guid CourseId) : IRequest;
