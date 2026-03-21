using MediatR;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Decks.Commands;

public class CreateDeckCommandHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<CreateDeckCommand, Guid>
{
    public async Task<Guid> Handle(CreateDeckCommand request, CancellationToken cancellationToken)
    {
        var deck = new Deck(request.Title, request.Description);

        dbContext.Decks.Add(deck);
        await dbContext.SaveChangesAsync(cancellationToken);

        return deck.Id;
    }
}
