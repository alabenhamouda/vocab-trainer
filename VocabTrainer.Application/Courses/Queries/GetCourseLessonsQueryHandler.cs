using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Courses.Dtos;
using VocabTrainer.Application.Data;

namespace VocabTrainer.Application.Courses.Queries;

public class GetCourseLessonsQueryHandler(
    IVocabTrainerDbContext dbContext,
    IValidator<IPaginatedQuery> validator
) : IRequestHandler<GetCourseLessonsQuery, PaginatedList<LessonDto>>
{
    public async Task<PaginatedList<LessonDto>> Handle(
        GetCourseLessonsQuery request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var query = dbContext.Lessons.AsNoTracking().Where(l => l.CourseId == request.CourseId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(l => l.Order)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<LessonDto>(
            items.Adapt<List<LessonDto>>(),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
