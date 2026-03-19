using MediatR;
using VocabTrainer.Application.Common;
using VocabTrainer.Application.Courses.Dtos;

namespace VocabTrainer.Application.Courses.Queries;

public record GetCoursesQuery(int Page = 1, int PageSize = 20)
    : IRequest<PaginatedList<CourseDto>>,
        IPaginatedQuery;
