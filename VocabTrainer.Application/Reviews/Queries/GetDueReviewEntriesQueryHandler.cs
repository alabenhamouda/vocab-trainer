using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Data;
using VocabTrainer.Application.Reviews.Dtos;
using VocabTrainer.Application.VocabEntries.Dtos;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Reviews.Queries;

public class GetDueReviewEntriesQueryHandler(
    IVocabTrainerDbContext dbContext,
    IValidator<IPaginatedQuery> validator
) : IRequestHandler<GetDueReviewEntriesQuery, PaginatedList<ReviewVocabEntryDto>>
{
    public async Task<PaginatedList<ReviewVocabEntryDto>> Handle(
        GetDueReviewEntriesQuery request,
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

        var deckEntries = dbContext
            .VocabEntries.AsNoTracking()
            .Where(v => directEntryIds.Contains(v.Id))
            .Union(fromLessons);

        var now = DateTime.UtcNow;

        var query =
            from v in deckEntries
            join r in dbContext.Reviews.Where(r => r.DeckId == request.DeckId)
                on v.Id equals r.VocabEntryId
                into reviews
            from r in reviews.DefaultIfEmpty()
            where r == null || r.NextReviewAt <= now
            select new { Entry = v, Review = r };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Review == null ? DateTime.MinValue : x.Review.NextReviewAt)
            .ThenBy(x => x.Entry.Term)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items
            .Select(x =>
            {
                VocabEntryDto entryDto = x.Entry switch
                {
                    Noun n => n.Adapt<NounDto>(),
                    Expression e => e.Adapt<ExpressionDto>(),
                    _ => x.Entry.Adapt<VocabEntryDto>(),
                };

                return new ReviewVocabEntryDto(
                    entryDto,
                    x.Review?.ConfidenceLevel,
                    x.Review?.LastReviewedAt,
                    x.Review?.NextReviewAt
                );
            })
            .ToList();

        return new PaginatedList<ReviewVocabEntryDto>(
            dtos,
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
