using MediatR;

namespace VocabTrainer.Application.Decks.Commands;

public record AddLessonToDeckCommand(Guid DeckId, Guid LessonId) : IRequest;
