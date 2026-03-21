using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Data;
using VocabTrainer.Application.Decks.Dtos;

namespace VocabTrainer.Application.Decks.Queries;

public class GetDecksQueryHandler(
    IVocabTrainerDbContext dbContext,
    IValidator<IPaginatedQuery> validator
) : IRequestHandler<GetDecksQuery, PaginatedList<DeckDto>>
{
    public async Task<PaginatedList<DeckDto>> Handle(
        GetDecksQuery request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var totalCount = await dbContext.Decks.CountAsync(cancellationToken);

        var items = await dbContext
            .Decks.AsNoTracking()
            .OrderBy(d => d.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<DeckDto>(
            items.Adapt<List<DeckDto>>(),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
