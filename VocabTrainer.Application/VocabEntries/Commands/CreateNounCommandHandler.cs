using MediatR;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.VocabEntries.Commands;

public class CreateNounCommandHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<CreateNounCommand, Guid>
{
    public async Task<Guid> Handle(CreateNounCommand request, CancellationToken cancellationToken)
    {
        var noun = new Noun(
            request.Term,
            request.Definition,
            request.EnglishTranslation,
            request.ImageUrl,
            request.Gender,
            request.PluralForm,
            request.IsSingularOnly,
            request.IsPluralOnly,
            request.Example
        );

        dbContext.Nouns.Add(noun);
        await dbContext.SaveChangesAsync(cancellationToken);

        return noun.Id;
    }
}
