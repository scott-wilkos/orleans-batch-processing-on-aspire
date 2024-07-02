using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace BatchProcessing.Domain;

public static class DependencyInjection
{
    public static void AddDomainInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDBClient("mongoDb");

        builder.Services.AddScoped<ApplicationContext>(opt =>
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