namespace VocabTrainer.Application.Reviews.Dtos;

public record DeckReviewStatsDto(
    int TotalEntries,
    int New,
    int Again,
    int Hard,
    int Good,
    int Easy
);
