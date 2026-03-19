using FluentValidation;
using MediatR;
using VocabTrainer.Application.Courses.Queries;

namespace VocabTrainer.ApiService.Endpoints;

public static class LessonEndpoints
{
    public static void MapLessonEndpoints(this IEndpointRouteBuilder app)
    {
        var lessonGroup = app.MapGroup("/lessons").WithTags("Lessons");

        lessonGroup.MapGet(
            "/{lessonId:guid}/vocab",
            async (ISender sender, Guid lessonId, int page = 1, int pageSize = 20) =>
            {
                try
                {
                    var entries = await sender.Send(
                        new GetLessonVocabEntriesQuery(lessonId, page, pageSize)
                    );
                    return Results.Ok(entries);
                }
                catch (ValidationException ex)
                {
                    return Results.ValidationProblem(ex.ToValidationErrors());
                }
            }
        );
    }
}
