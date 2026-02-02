using Microsoft.EntityFrameworkCore;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Data;

public interface IVocabTrainerDbContext
{
    DbSet<VocabEntry> VocabEntries { get; }
    DbSet<Noun> Nouns { get; }
    DbSet<Expression> Expressions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
