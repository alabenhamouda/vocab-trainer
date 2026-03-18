using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Seeding;

public interface ICourseProvider
{
    Task<Course> FetchAsync(CancellationToken cancellationToken);
}
