using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Data;
using VocabTrainer.Application.VocabEntries.Dtos;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Decks.Queries;

public class GetDeckVocabEntriesQueryHandler(
    IVocabTrainerDbContext dbContext,
    IValidator<IPaginatedQuery> validator
) : IRequestHandler<GetDeckVocabEntriesQuery, PaginatedList<VocabEntryDto>>
{
    public async Task<PaginatedList<VocabEntryDto>> Handle(
        GetDeckVocabEntriesQuery request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var directEntryIds = dbContext
            .DeckEntries.Where(de => de.DeckId == request.DeckId)
            .Select(de => de.VocabEntryId);

        var lessonIds = dbContext
            .DeckLessons.Where(dl => dl.DeckId == request.DeckId)
            .Select(dl => dl.LessonId);

        var fromLessons = dbContext.VocabEntries.Where(v =>
            v.LessonId != null && lessonIds.Contains(v.LessonId.Value)
        );

        var query = dbContext
            .VocabEntries.AsNoTracking()
            .Where(v => directEntryIds.Contains(v.Id))
            .Union(fromLessons);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(v => v.Term)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items
            .Select<VocabEntry, VocabEntryDto>(v =>
                v switch
                {
                    Noun n => n.Adapt<NounDto>(),
                    Expression e => e.Adapt<ExpressionDto>(),
                    _ => v.Adapt<VocabEntryDto>(),
                }
            )
            .ToList();

        return new PaginatedList<VocabEntryDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
