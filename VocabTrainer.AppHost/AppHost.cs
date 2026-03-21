var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithDataVolume();

var vocabDb = postgres.AddDatabase("vocabdb");

var migrations = builder
    .AddProject<Projects.VocabTrainer_MigrationService>("migrations")
    .WithReference(vocabDb)
    .WaitFor(vocabDb);

var groqApiKey = builder.AddParameter("groq-api-key", secret: true);

var seeding = builder
    .AddProject<Projects.VocabTrainer_SeedingService>("seeding")
    .WithReference(vocabDb)
    .WaitFor(vocabDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations)
    .WithEnvironment("Groq__ApiKey", groqApiKey)
    .WithExplicitStart();

var apiService = builder
    .AddProject<Projects.VocabTrainer_ApiService>("apiservice")
    .WithReference(vocabDb)
    .WaitFor(vocabDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations)
    .WithHttpHealthCheck("/health");

builder
    .AddViteApp("webfrontend", "../VocabTrainer.Web")
    .WithNpm()
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithExternalHttpEndpoints();

builder.Build().Run();
