using MediatR;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.VocabEntries.Dtos;

namespace VocabTrainer.Application.Decks.Queries;

public record GetDeckVocabEntriesQuery(Guid DeckId, int Page = 1, int PageSize = 20)
    : IRequest<PaginatedList<VocabEntryDto>>,
        IPaginatedQuery;
