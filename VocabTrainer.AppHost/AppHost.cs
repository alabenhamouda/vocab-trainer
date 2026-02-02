var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithDataVolume();

var vocabDb = postgres.AddDatabase("vocabdb");

var migrations = builder
    .AddProject<Projects.VocabTrainer_MigrationService>("migrations")
    .WithReference(vocabDb)
    .WaitFor(vocabDb);

var cache = builder.AddRedis("cache");

var apiService = builder
    .AddProject<Projects.VocabTrainer_ApiService>("apiservice")
    .WithReference(vocabDb)
    .WaitFor(vocabDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations)
    .WithHttpHealthCheck("/health");

builder
    .AddProject<Projects.VocabTrainer_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
