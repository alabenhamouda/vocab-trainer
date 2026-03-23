using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Domain.Models;

public class FixedIntervalReviewStrategy : IReviewStrategy
{
    public static readonly TimeSpan EasyInterval = TimeSpan.FromDays(7);
    public static readonly TimeSpan GoodInterval = TimeSpan.FromDays(2);
    public static readonly TimeSpan HardInterval = TimeSpan.FromHours(24);
    public static readonly TimeSpan AgainInterval = TimeSpan.Zero;

    public DateTime CalculateNextReviewDate(Review review, ConfidenceLevel level)
    {
        var interval = level switch
        {
            ConfidenceLevel.Easy => EasyInterval,
            ConfidenceLevel.Good => GoodInterval,
            ConfidenceLevel.Hard => HardInterval,
            _ => AgainInterval,
        };

        return DateTime.UtcNow + interval;
    }
}
