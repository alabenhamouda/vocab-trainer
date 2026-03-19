using MediatR;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.VocabEntries.Dtos;

namespace VocabTrainer.Application.VocabEntries.Queries;

public record GetExpressionsQuery(int Page = 1, int PageSize = 20)
    : IRequest<PaginatedList<ExpressionDto>>,
        IPaginatedQuery;
