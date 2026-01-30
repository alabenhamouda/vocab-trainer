using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.VocabEntries.Queries;

public class GetVocabEntriesQueryHandler(VocabTrainerDbContext dbContext)
    : IRequestHandler<GetVocabEntriesQuery, List<VocabEntry>>
{
    public async Task<List<VocabEntry>> Handle(
        GetVocabEntriesQuery request,
        CancellationToken cancellationToken
    )
    {
        return await dbContext.VocabEntries.AsNoTracking().ToListAsync(cancellationToken);
    }
}
