using Microsoft.Extensions.DependencyInjection;
using VocabTrainer.Application.Seeding;

namespace VocabTrainer.Infrastructure.Seeding;

public static class SeedingDependencyInjection
{
    public static void AddSeedingServices(this IServiceCollection services)
    {
        services.AddScoped<IVocabClassifier, LlmVocabClassifier>();

        services.AddHttpClient<DwNicosWegB1Provider>(client =>
        {
            client.BaseAddress = new Uri("https://learngerman.dw.com");
        });
        services.AddScoped<ICourseProvider>(sp => sp.GetRequiredService<DwNicosWegB1Provider>());
    }
}
