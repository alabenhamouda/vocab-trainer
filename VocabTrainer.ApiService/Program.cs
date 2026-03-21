using Scalar.AspNetCore;
using VocabTrainer.ApiService.Endpoints;
using VocabTrainer.Application;
using VocabTrainer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddInfrastructureServices();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.MapGet("/", () => "VocabTrainer API is running.").ExcludeFromDescription();

app.MapVocabEndpoints();
app.MapCourseEndpoints();
app.MapLessonEndpoints();
app.MapDeckEndpoints();

app.MapDefaultEndpoints();

app.Run();
