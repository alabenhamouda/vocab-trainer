using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VocabTrainer.Application.Data;

namespace VocabTrainer.Infrastructure.Data;

public static class DataDependencyInjection
{
    public static void AddDataServices(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<VocabTrainerDbContext>("vocabdb");
        builder.Services.AddScoped<IVocabTrainerDbContext>(sp =>
            sp.GetRequiredService<VocabTrainerDbContext>()
        );
    }
}
