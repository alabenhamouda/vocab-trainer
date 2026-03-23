using VocabTrainer.Application.VocabEntries.Dtos;
using VocabTrainer.Domain.Enums;

namespace VocabTrainer.Application.Reviews.Dtos;

public record ReviewVocabEntryDto(
    VocabEntryDto Entry,
    ConfidenceLevel? ConfidenceLevel,
    DateTime? LastReviewedAt,
    DateTime? NextReviewAt
);
