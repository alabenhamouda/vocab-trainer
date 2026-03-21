using MediatR;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Decks.Dtos;

namespace VocabTrainer.Application.Decks.Queries;

public record GetDecksQuery(int Page = 1, int PageSize = 20)
    : IRequest<PaginatedList<DeckDto>>,
        IPaginatedQuery;
