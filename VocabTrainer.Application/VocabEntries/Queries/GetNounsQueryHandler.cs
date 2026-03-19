using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Data;
using VocabTrainer.Application.VocabEntries.Dtos;

namespace VocabTrainer.Application.VocabEntries.Queries;

public class GetNounsQueryHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<GetNounsQuery, PaginatedList<NounDto>>
{
    public async Task<PaginatedList<NounDto>> Handle(
        GetNounsQuery request,
        CancellationToken cancellationToken
    )
    {
        var totalCount = await dbContext.Nouns.CountAsync(cancellationToken);

        var items = await dbContext
            .Nouns.AsNoTracking()
            .OrderBy(n => n.Term)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<NounDto>(
            items.Adapt<List<NounDto>>(),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
