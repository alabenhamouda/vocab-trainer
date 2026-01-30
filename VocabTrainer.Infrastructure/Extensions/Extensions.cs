using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VocabTrainer.Application.Data;

namespace VocabTrainer.Infrastructure;

public static class Extensions
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<VocabTrainerDbContext>("vocabdb");
    }
}
