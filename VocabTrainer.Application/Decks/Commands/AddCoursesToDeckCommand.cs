using MediatR;

namespace VocabTrainer.Application.Decks.Commands;

public record AddCoursesToDeckCommand(Guid DeckId, List<Guid> CourseIds) : IRequest;
