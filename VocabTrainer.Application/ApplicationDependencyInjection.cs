using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VocabTrainer.Application.Common;
using VocabTrainer.Domain.Models;

namespace VocabTrainer.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        MappingConfig.RegisterMappings();

        services.AddValidatorsFromAssemblyContaining<PaginatedQueryValidator>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationDependencyInjection).Assembly);
        });

        services.AddSingleton<IReviewStrategy, FixedIntervalReviewStrategy>();

        return services;
    }
}
