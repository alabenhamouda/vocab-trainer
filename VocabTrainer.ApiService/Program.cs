using MediatR;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Application.VocabEntries.Commands;
using VocabTrainer.Application.VocabEntries.Queries;
using VocabTrainer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddInfrastructure();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(VocabTrainer.Application.VocabEntries.Queries.GetVocabEntriesQuery).Assembly
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "VocabTrainer API is running.");

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

app.MapDefaultEndpoints();

app.Run();
