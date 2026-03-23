using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Domain.Models;

public interface IReviewStrategy
{
    DateTime CalculateNextReviewDate(Review review, ConfidenceLevel level);
}
