using MediatR;
using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Application.Reviews.Commands;

public record RecordReviewCommand(Guid DeckId, Guid VocabEntryId, ConfidenceLevel Level) : IRequest;
