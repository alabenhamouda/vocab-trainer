using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Decks.Commands;

public class AddCoursesToDeckCommandHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<AddCoursesToDeckCommand>
{
    public async Task Handle(AddCoursesToDeckCommand request, CancellationToken cancellationToken)
    {
        var deckExists = await dbContext.Decks.AnyAsync(
            d => d.Id == request.DeckId,
            cancellationToken
        );

        if (!deckExists)
            throw new DeckNotFoundException(request.DeckId);

        var lessonIds = await dbContext
            .Lessons.Where(l => request.CourseIds.Contains(l.CourseId))
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        if (lessonIds.Count == 0)
            return;

        var existingLessonIds = await dbContext
            .DeckLessons.Where(dl => dl.DeckId == request.DeckId)
            .Select(dl => dl.LessonId)
            .ToListAsync(cancellationToken);

        var newLessonIds = lessonIds.Except(existingLessonIds);

        foreach (var lessonId in newLessonIds)
        {
            dbContext.DeckLessons.Add(new DeckLesson(request.DeckId, lessonId));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
