using MediatR;
using VocabTrainer.Application.Reviews.Dtos;

namespace VocabTrainer.Application.Reviews.Queries;

public record GetDeckReviewStatsQuery(Guid DeckId) : IRequest<DeckReviewStatsDto>;
