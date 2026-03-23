using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Reviews.Commands;

public class RecordReviewCommandHandler(
    IVocabTrainerDbContext dbContext,
    IReviewStrategy reviewStrategy
) : IRequestHandler<RecordReviewCommand>
{
    public async Task Handle(RecordReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await dbContext.Reviews.FirstOrDefaultAsync(
            r => r.DeckId == request.DeckId && r.VocabEntryId == request.VocabEntryId,
            cancellationToken
        );

        if (review is null)
        {
            review = new Review(request.DeckId, request.VocabEntryId);
            dbContext.Reviews.Add(review);
        }

        review.RecordReview(request.Level, reviewStrategy);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
