using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Data;
using VocabTrainer.Application.VocabEntries.Dtos;

namespace VocabTrainer.Application.VocabEntries.Queries;

public class GetVocabEntriesQueryHandler(
    IVocabTrainerDbContext dbContext,
    IValidator<IPaginatedQuery> validator
) : IRequestHandler<GetVocabEntriesQuery, PaginatedList<VocabEntryDto>>
{
    public async Task<PaginatedList<VocabEntryDto>> Handle(
        GetVocabEntriesQuery request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var totalCount = await dbContext.VocabEntries.CountAsync(cancellationToken);

        var items = await dbContext
            .VocabEntries.AsNoTracking()
            .OrderBy(v => v.Term)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<VocabEntryDto>(
            items.Adapt<List<VocabEntryDto>>(),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
