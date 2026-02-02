using MediatR;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Application.VocabEntries.Commands;
using VocabTrainer.Application.VocabEntries.Queries;

namespace VocabTrainer.ApiService.Endpoints;

public static class VocabEndpoints
{
    public static void MapVocabEndpoints(this IEndpointRouteBuilder app)
    {
        var vocabGroup = app.MapGroup("/vocab");

        vocabGroup.MapGet(
            "/",
            async (ISender sender) =>
            {
                var entries = await sender.Send(new GetVocabEntriesQuery());
                return Results.Ok(entries);
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
