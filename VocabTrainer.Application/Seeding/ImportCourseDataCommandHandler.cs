using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Data;

namespace VocabTrainer.Application.Seeding;

public class ImportCourseDataCommandHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<ImportCourseDataCommand>
{
    public async Task Handle(ImportCourseDataCommand request, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Courses.AnyAsync(
            c => c.Title == request.Course.Title,
            cancellationToken
        );

        if (exists)
            return;

        dbContext.Courses.Add(request.Course);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
