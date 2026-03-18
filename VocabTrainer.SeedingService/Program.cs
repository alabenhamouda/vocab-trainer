using VocabTrainer.Infrastructure;
using VocabTrainer.Infrastructure.Data;
using VocabTrainer.Infrastructure.Seeding;
using VocabTrainer.SeedingService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder
    .Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSource(Worker.ActivitySourceName);
    });

builder.AddDataServices();
builder.AddAiServices();
builder.Services.AddSeedingServices();

var host = builder.Build();
host.Run();
