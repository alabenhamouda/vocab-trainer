using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Infrastructure.Data;

public class VocabTrainerDbContext(DbContextOptions<VocabTrainerDbContext> options)
    : DbContext(options),
        IVocabTrainerDbContext
{
    public DbSet<VocabEntry> VocabEntries => Set<VocabEntry>();
    public DbSet<Noun> Nouns => Set<Noun>();
    public DbSet<Expression> Expressions => Set<Expression>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Deck> Decks => Set<Deck>();
    public DbSet<DeckEntry> DeckEntries => Set<DeckEntry>();
    public DbSet<DeckLesson> DeckLessons => Set<DeckLesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- VocabEntry (TPH hierarchy) ---
        modelBuilder.Entity<VocabEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity
                .HasDiscriminator<string>("EntryType")
                .HasValue<Noun>(nameof(Noun))
                .HasValue<Expression>(nameof(Expression));
        });

        // --- Course ---
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity
                .HasMany(e => e.Lessons)
                .WithOne()
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Lesson ---
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.HasIndex(e => new { e.CourseId, e.Order }).IsUnique();
            entity
                .HasMany(e => e.VocabEntries)
                .WithOne()
                .HasForeignKey(v => v.LessonId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // --- Deck ---
        modelBuilder.Entity<Deck>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity
                .HasMany(e => e.DeckEntries)
                .WithOne()
                .HasForeignKey(de => de.DeckId)
                .OnDelete(DeleteBehavior.Cascade);
            entity
                .HasMany(e => e.DeckLessons)
                .WithOne()
                .HasForeignKey(dl => dl.DeckId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- DeckEntry ---
        modelBuilder.Entity<DeckEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DeckId, e.VocabEntryId }).IsUnique();
            entity
                .HasOne<VocabEntry>()
                .WithMany()
                .HasForeignKey(de => de.VocabEntryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- DeckLesson ---
        modelBuilder.Entity<DeckLesson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DeckId, e.LessonId }).IsUnique();
            entity
                .HasOne<Lesson>()
                .WithMany()
                .HasForeignKey(dl => dl.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
