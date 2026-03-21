using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Decks.Commands;

public class AddEntryToDeckCommandHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<AddEntryToDeckCommand>
{
    public async Task Handle(AddEntryToDeckCommand request, CancellationToken cancellationToken)
    {
        var deckExists = await dbContext.Decks.AnyAsync(
            d => d.Id == request.DeckId,
            cancellationToken
        );

        if (!deckExists)
            throw new InvalidOperationException($"Deck '{request.DeckId}' not found.");

        var vocabEntryExists = await dbContext.VocabEntries.AnyAsync(
            v => v.Id == request.VocabEntryId,
            cancellationToken
        );

        if (!vocabEntryExists)
            throw new InvalidOperationException($"VocabEntry '{request.VocabEntryId}' not found.");

        var alreadyAdded = await dbContext.DeckEntries.AnyAsync(
            de => de.DeckId == request.DeckId && de.VocabEntryId == request.VocabEntryId,
            cancellationToken
        );

        if (alreadyAdded)
            return;

        var deckEntry = new DeckEntry(request.DeckId, request.VocabEntryId);
        dbContext.DeckEntries.Add(deckEntry);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
