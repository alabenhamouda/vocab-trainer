using MediatR;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Reviews.Dtos;

namespace VocabTrainer.Application.Reviews.Queries;

public record GetDueReviewEntriesQuery(Guid DeckId, int Page = 1, int PageSize = 100)
    : IRequest<PaginatedList<ReviewVocabEntryDto>>,
        IPaginatedQuery;
