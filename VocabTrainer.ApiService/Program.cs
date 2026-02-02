using MediatR;
using VocabTrainer.ApiService.Endpoints;
using VocabTrainer.Application.VocabEntries.Queries;
using VocabTrainer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddInfrastructureServices();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetVocabEntriesQuery).Assembly)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "VocabTrainer API is running.");

app.MapVocabEndpoints();

app.MapDefaultEndpoints();

app.Run();
