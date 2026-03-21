using MediatR;

namespace VocabTrainer.Application.Decks.Commands;

public record AddEntryToDeckCommand(Guid DeckId, Guid VocabEntryId) : IRequest;
