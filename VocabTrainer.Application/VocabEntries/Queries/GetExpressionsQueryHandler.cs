using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Data;
using VocabTrainer.Application.VocabEntries.Dtos;

namespace VocabTrainer.Application.VocabEntries.Queries;

public class GetExpressionsQueryHandler(
    IVocabTrainerDbContext dbContext,
    IValidator<IPaginatedQuery> validator
) : IRequestHandler<GetExpressionsQuery, PaginatedList<ExpressionDto>>
{
    public async Task<PaginatedList<ExpressionDto>> Handle(
        GetExpressionsQuery request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var totalCount = await dbContext.Expressions.CountAsync(cancellationToken);

        var items = await dbContext
            .Expressions.AsNoTracking()
            .OrderBy(e => e.Term)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<ExpressionDto>(
            items.Adapt<List<ExpressionDto>>(),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
