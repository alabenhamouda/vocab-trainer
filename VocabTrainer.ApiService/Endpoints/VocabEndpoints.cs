using MediatR;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Application.VocabEntries.Commands;
using VocabTrainer.Application.VocabEntries.Queries;

namespace VocabTrainer.ApiService.Endpoints;

public static class VocabEndpoints
{
    public static void MapVocabEndpoints(this IEndpointRouteBuilder app)
    {
        var vocabGroup = app.MapGroup("/vocab").WithTags("Vocabulary");

        vocabGroup.MapGet(
            "/",
            async (ISender sender, int page = 1, int pageSize = 20) =>
            {
                var entries = await sender.Send(new GetVocabEntriesQuery(page, pageSize));
                return Results.Ok(entries);
            }
        );

        vocabGroup.MapGet(
            "/nouns",
            async (ISender sender, int page = 1, int pageSize = 20) =>
            {
                var nouns = await sender.Send(new GetNounsQuery(page, pageSize));
                return Results.Ok(nouns);
            }
        );

        vocabGroup.MapGet(
            "/expressions",
            async (ISender sender, int page = 1, int pageSize = 20) =>
            {
                var expressions = await sender.Send(new GetExpressionsQuery(page, pageSize));
                return Results.Ok(expressions);
            }
        );

        vocabGroup.MapPost(
            "/nouns",
            async (ISender sender, [FromBody] CreateNounCommand command) =>
            {
                var id = await sender.Send(command);
                return Results.Created($"/vocab/{id}", id);
            }
        );

        vocabGroup.MapPost(
            "/expressions",
            async (ISender sender, [FromBody] CreateExpressionCommand command) =>
            {
                var id = await sender.Send(command);
                return Results.Created($"/vocab/{id}", id);
            }
        );
    }
}
