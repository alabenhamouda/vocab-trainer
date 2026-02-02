using Microsoft.Extensions.Hosting;
using VocabTrainer.Infrastructure.Data;

namespace VocabTrainer.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.AddDataServices();
    }
}
