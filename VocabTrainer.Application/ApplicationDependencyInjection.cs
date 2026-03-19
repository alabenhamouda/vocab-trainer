using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VocabTrainer.Application.Common;

namespace VocabTrainer.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<PaginatedQueryValidator>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationDependencyInjection).Assembly);
        });

        return services;
    }
}
