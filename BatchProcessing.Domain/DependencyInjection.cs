using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace BatchProcessing.Domain;

/// <summary>
/// Provides extension methods for setting up domain infrastructure services in an <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the domain infrastructure services to the specified <see cref="IHostApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to add services to.</param>
    public static void AddDomainInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDBClient("mongoDb");

        builder.Services.AddTransient<ApplicationContext>(opt =>
        {
            var client = opt.GetRequiredService<IMongoClient>();
            var mongoDatabase = client.GetDatabase("mongoDb");

            return ApplicationContext.Create(client, mongoDatabase);
        });

        builder.Services.AddSingleton<IMongoDatabase>(opt =>
        {
            var client = opt.GetRequiredService<IMongoClient>();
            var mongoDatabase = client.GetDatabase("mongoDb");

            return mongoDatabase;
        });

        builder.Services.AddScoped<ContextFactory>();
    }
}
