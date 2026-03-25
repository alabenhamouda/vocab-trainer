using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Application.Decks.Commands;
using VocabTrainer.Application.Decks.Queries;
using VocabTrainer.Application.Reviews.Commands;
using VocabTrainer.Application.Reviews.Queries;
using VocabTrainer.Domain.Enums;

namespace VocabTrainer.ApiService.Endpoints;

public static class DeckEndpoints
{
    public static void MapDeckEndpoints(this IEndpointRouteBuilder app)
    {
        var deckGroup = app.MapGroup("/decks").WithTags("Decks");

        deckGroup.MapGet(
            "/",
            async (ISender sender, int page = 1, int pageSize = 20) =>
            {
                try
                {
                    var decks = await sender.Send(new GetDecksQuery(page, pageSize));
                    return Results.Ok(decks);
                }
                catch (ValidationException ex)
                {
                    return Results.ValidationProblem(ex.ToValidationErrors());
                }
            }
        );

        deckGroup.MapPost(
            "/",
            async (ISender sender, [FromBody] CreateDeckCommand command) =>
            {
                var id = await sender.Send(command);
                return Results.Created($"/decks/{id}", id);
            }
        );

        deckGroup.MapGet(
            "/{deckId:guid}/vocab",
            async (ISender sender, Guid deckId, int page = 1, int pageSize = 20) =>
            {
                try
                {
                    var entries = await sender.Send(
                        new GetDeckVocabEntriesQuery(deckId, page, pageSize)
                    );
                    return Results.Ok(entries);
                }
                catch (ValidationException ex)
                {
                    return Results.ValidationProblem(ex.ToValidationErrors());
                }
            }
        );

        deckGroup.MapPost(
            "/{deckId:guid}/lessons/{lessonId:guid}",
            async (ISender sender, Guid deckId, Guid lessonId) =>
            {
                await sender.Send(new AddLessonToDeckCommand(deckId, lessonId));
                return Results.NoContent();
            }
        );

        deckGroup.MapPost(
            "/{deckId:guid}/courses/{courseId:guid}",
            async (ISender sender, Guid deckId, Guid courseId) =>
            {
                await sender.Send(new AddCourseToDeckCommand(deckId, courseId));
                return Results.NoContent();
            }
        );

        deckGroup.MapPost(
            "/{deckId:guid}/entries/{vocabEntryId:guid}",
            async (ISender sender, Guid deckId, Guid vocabEntryId) =>
            {
                await sender.Send(new AddEntryToDeckCommand(deckId, vocabEntryId));
                return Results.NoContent();
            }
        );

        deckGroup.MapPost(
            "/{deckId:guid}/courses",
            async (ISender sender, Guid deckId, [FromBody] AddCoursesToDeckRequest request) =>
            {
                try
                {
                    await sender.Send(new AddCoursesToDeckCommand(deckId, request.CourseIds));
                    return Results.NoContent();
                }
                catch (DeckNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            }
        );

        deckGroup.MapGet(
            "/{deckId:guid}/review",
            async (ISender sender, Guid deckId, int page = 1, int pageSize = 100) =>
            {
                try
                {
                    var entries = await sender.Send(
                        new GetDueReviewEntriesQuery(deckId, page, pageSize)
                    );
                    return Results.Ok(entries);
                }
                catch (ValidationException ex)
                {
                    return Results.ValidationProblem(ex.ToValidationErrors());
                }
            }
        );

        deckGroup.MapGet(
            "/{deckId:guid}/review/stats",
            async (ISender sender, Guid deckId) =>
            {
                var stats = await sender.Send(new GetDeckReviewStatsQuery(deckId));
                return Results.Ok(stats);
            }
        );

        deckGroup.MapPost(
            "/{deckId:guid}/vocab/{vocabEntryId:guid}/review",
            async (
                ISender sender,
                Guid deckId,
                Guid vocabEntryId,
                [FromBody] RecordReviewRequest request
            ) =>
            {
                await sender.Send(
                    new RecordReviewCommand(deckId, vocabEntryId, request.ConfidenceLevel)
                );
                return Results.NoContent();
            }
        );
    }
}

public record AddCoursesToDeckRequest(List<Guid> CourseIds);

public record RecordReviewRequest(ConfidenceLevel ConfidenceLevel);
