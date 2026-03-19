using MediatR;
using VocabTrainer.Application.Courses.Queries;

namespace VocabTrainer.ApiService.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        var courseGroup = app.MapGroup("/courses").WithTags("Courses");

        courseGroup.MapGet(
            "/",
            async (ISender sender, int page = 1, int pageSize = 20) =>
            {
                var courses = await sender.Send(new GetCoursesQuery(page, pageSize));
                return Results.Ok(courses);
            }
        );

        courseGroup.MapGet(
            "/{courseId:guid}/lessons",
            async (ISender sender, Guid courseId, int page = 1, int pageSize = 20) =>
            {
                var lessons = await sender.Send(
                    new GetCourseLessonsQuery(courseId, page, pageSize)
                );
                return Results.Ok(lessons);
            }
        );
    }
}
