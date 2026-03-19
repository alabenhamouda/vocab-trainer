using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Courses.Dtos;
using VocabTrainer.Application.Data;

namespace VocabTrainer.Application.Courses.Queries;

public class GetCoursesQueryHandler(IVocabTrainerDbContext dbContext)
    : IRequestHandler<GetCoursesQuery, PaginatedList<CourseDto>>
{
    public async Task<PaginatedList<CourseDto>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken
    )
    {
        var totalCount = await dbContext.Courses.CountAsync(cancellationToken);

        var items = await dbContext
            .Courses.AsNoTracking()
            .OrderBy(c => c.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<CourseDto>(
            items.Adapt<List<CourseDto>>(),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
