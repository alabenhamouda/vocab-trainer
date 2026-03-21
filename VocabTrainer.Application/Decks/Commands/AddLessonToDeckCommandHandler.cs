using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Decks.Commands;

public class AddLessonToDeckCommandHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<AddLessonToDeckCommand>
{
    public async Task Handle(AddLessonToDeckCommand request, CancellationToken cancellationToken)
    {
        var deckExists = await dbContext.Decks.AnyAsync(
            d => d.Id == request.DeckId,
            cancellationToken
        );

        if (!deckExists)
            throw new InvalidOperationException($"Deck '{request.DeckId}' not found.");

        var lessonExists = await dbContext.Lessons.AnyAsync(
            l => l.Id == request.LessonId,
            cancellationToken
        );

        if (!lessonExists)
            throw new InvalidOperationException($"Lesson '{request.LessonId}' not found.");

        var alreadyAdded = await dbContext.DeckLessons.AnyAsync(
            dl => dl.DeckId == request.DeckId && dl.LessonId == request.LessonId,
            cancellationToken
        );

        if (alreadyAdded)
            return;

        var deckLesson = new DeckLesson(request.DeckId, request.LessonId);
        dbContext.DeckLessons.Add(deckLesson);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
