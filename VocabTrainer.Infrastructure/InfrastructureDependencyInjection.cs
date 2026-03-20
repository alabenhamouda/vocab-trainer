using System.ClientModel;
using System.ClientModel.Primitives;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using OpenAI;
using Polly;
using VocabTrainer.Infrastructure.Data;
using VocabTrainer.Infrastructure.Seeding;

namespace VocabTrainer.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.AddDataServices();
    }

    public static void AddAiServices(this IHostApplicationBuilder builder)
    {
        var groqApiKey =
            builder.Configuration["Groq:ApiKey"]
            ?? throw new InvalidOperationException("Groq:ApiKey configuration is required.");

        builder
            .Services.AddHttpClient("ChatClient")
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.BackoffType = DelayBackoffType.Constant;
                options.Retry.Delay = TimeSpan.FromMinutes(1);
            });

        builder
            .Services.AddChatClient(services =>
            {
                var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("ChatClient");

                var client = new OpenAIClient(
                    new ApiKeyCredential(groqApiKey),
                    new OpenAIClientOptions
                    {
                        Endpoint = new Uri("https://api.groq.com/openai/v1"),
                        Transport = new HttpClientPipelineTransport(httpClient),
                    }
                );

                return client.GetChatClient("openai/gpt-oss-20b").AsIChatClient();
            })
            .UseOpenTelemetry()
            .UseLogging();
    }
}
