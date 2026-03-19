using FluentValidation;
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
                try
                {
                    var courses = await sender.Send(new GetCoursesQuery(page, pageSize));
                    return Results.Ok(courses);
                }
                catch (ValidationException ex)
                {
                    return Results.ValidationProblem(ex.ToValidationErrors());
                }
            }
        );

        courseGroup.MapGet(
            "/{courseId:guid}/lessons",
            async (ISender sender, Guid courseId, int page = 1, int pageSize = 20) =>
            {
                try
                {
                    var lessons = await sender.Send(
                        new GetCourseLessonsQuery(courseId, page, pageSize)
                    );
                    return Results.Ok(lessons);
                }
                catch (ValidationException ex)
                {
                    return Results.ValidationProblem(ex.ToValidationErrors());
                }
            }
        );
    }
}
