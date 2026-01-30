using Microsoft.EntityFrameworkCore;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Data;

public class VocabTrainerDbContext(DbContextOptions<VocabTrainerDbContext> options)
    : DbContext(options)
{
    public DbSet<VocabEntry> VocabEntries => Set<VocabEntry>();
    public DbSet<Noun> Nouns => Set<Noun>();
    public DbSet<Expression> Expressions => Set<Expression>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VocabEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity
                .HasDiscriminator<string>("EntryType")
                .HasValue<Noun>(nameof(Noun))
                .HasValue<Expression>(nameof(Expression));
        });

        base.OnModelCreating(modelBuilder);
    }
}
