using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;
using VocabTrainer.Application.Reviews.Dtos;
using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Application.Reviews.Queries;

public class GetDeckReviewStatsQueryHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<GetDeckReviewStatsQuery, DeckReviewStatsDto>
{
    public async Task<DeckReviewStatsDto> Handle(
        GetDeckReviewStatsQuery request,
        CancellationToken cancellationToken
    )
    {
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

        var query =
            from v in deckEntries
            join r in dbContext.Reviews.Where(r => r.DeckId == request.DeckId)
                on v.Id equals r.VocabEntryId
                into reviews
            from r in reviews.DefaultIfEmpty()
            select r;

        var totalEntries = await deckEntries.CountAsync(cancellationToken);

        var counts = await query
            .GroupBy(r => r == null ? (ConfidenceLevel?)null : r.ConfidenceLevel)
            .Select(g => new { Level = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int Count(ConfidenceLevel? level) =>
            counts.FirstOrDefault(c => c.Level == level)?.Count ?? 0;

        int newCount = Count(null);
        int againCount = Count(ConfidenceLevel.Again);
        int hardCount = Count(ConfidenceLevel.Hard);
        int goodCount = Count(ConfidenceLevel.Good);
        int easyCount = Count(ConfidenceLevel.Easy);

        return new DeckReviewStatsDto(
            totalEntries,
            newCount,
            againCount,
            hardCount,
            goodCount,
            easyCount
        );
    }
}
