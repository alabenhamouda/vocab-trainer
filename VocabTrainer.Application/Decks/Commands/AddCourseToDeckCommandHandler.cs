using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Decks.Commands;

public class AddCourseToDeckCommandHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<AddCourseToDeckCommand>
{
    public async Task Handle(AddCourseToDeckCommand request, CancellationToken cancellationToken)
    {
        var deckExists = await dbContext.Decks.AnyAsync(
            d => d.Id == request.DeckId,
            cancellationToken
        );

        if (!deckExists)
            throw new InvalidOperationException($"Deck '{request.DeckId}' not found.");

        var lessons = await dbContext
            .Lessons.Where(l => l.CourseId == request.CourseId)
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        if (lessons.Count == 0)
            throw new InvalidOperationException(
                $"Course '{request.CourseId}' not found or has no lessons."
            );

        var existingLessonIds = await dbContext
            .DeckLessons.Where(dl => dl.DeckId == request.DeckId)
            .Select(dl => dl.LessonId)
            .ToListAsync(cancellationToken);

        var newLessonIds = lessons.Except(existingLessonIds);

        foreach (var lessonId in newLessonIds)
        {
            dbContext.DeckLessons.Add(new DeckLesson(request.DeckId, lessonId));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
