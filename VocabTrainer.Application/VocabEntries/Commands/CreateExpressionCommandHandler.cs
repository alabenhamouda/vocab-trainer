using MediatR;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.VocabEntries.Commands;

public class CreateExpressionCommandHandler(VocabTrainerDbContext dbContext)
    : IRequestHandler<CreateExpressionCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateExpressionCommand request,
        CancellationToken cancellationToken
    )
    {
        var expression = new Expression(
            request.Term,
            request.Definition,
            request.EnglishTranslation,
            request.ImageUrl
        );

        dbContext.Expressions.Add(expression);
        await dbContext.SaveChangesAsync(cancellationToken);

        return expression.Id;
    }
}
