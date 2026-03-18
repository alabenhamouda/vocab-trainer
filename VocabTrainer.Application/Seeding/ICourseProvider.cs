using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application.Seeding;

public interface ICourseProvider
{
    string CourseTitle { get; }
    Task<Course> FetchAsync(CancellationToken cancellationToken);
}
