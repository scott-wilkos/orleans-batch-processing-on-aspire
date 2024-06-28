using BatchProcessing.Grains.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BatchProcessing.Grains;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddGrainDataService(this IServiceCollection services)
    {
        services.AddScoped<IDataService, DataService>();
        return services;
    }
}