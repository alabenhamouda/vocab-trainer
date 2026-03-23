using Microsoft.EntityFrameworkCore;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Data;

public interface IVocabTrainerDbContext
{
    DbSet<VocabEntry> VocabEntries { get; }
    DbSet<Noun> Nouns { get; }
    DbSet<Expression> Expressions { get; }
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<Deck> Decks { get; }
    DbSet<DeckEntry> DeckEntries { get; }
    DbSet<DeckLesson> DeckLessons { get; }
    DbSet<Review> Reviews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
