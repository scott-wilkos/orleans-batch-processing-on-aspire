using BatchProcessing.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BatchProcessing.Grains;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBatchProcessingEngineApplication(this IServiceCollection services,
        ConfigurationManager builderConfiguration)
    {
        services.Configure<EngineConfig>(builderConfiguration.GetSection("Engine"));

        return services;
    }
}