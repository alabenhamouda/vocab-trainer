using VocabTrainer.Infrastructure.Data;
using VocabTrainer.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder
    .Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSource(Worker.ActivitySourceName);
    });

builder.AddNpgsqlDbContext<VocabTrainerDbContext>("vocabdb");

var host = builder.Build();
host.Run();
