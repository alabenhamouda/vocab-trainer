using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI;
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

        var client = new OpenAIClient(
            new ApiKeyCredential(groqApiKey),
            new OpenAIClientOptions { Endpoint = new Uri("https://api.groq.com/openai/v1") }
        );

        builder
            .Services.AddChatClient(client.GetChatClient("openai/gpt-oss-20b").AsIChatClient())
            .UseOpenTelemetry()
            .UseLogging();
    }
}
