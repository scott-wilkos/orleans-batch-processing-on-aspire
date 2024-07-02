using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace BatchProcessing.Domain;

public static class DependencyInjection
{
    public static void AddDomainInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDBClient("mongodb");

        builder.Services.AddScoped<ApplicationContext>(opt =>
        {
            var client = opt.GetRequiredService<IMongoClient>();
            return ApplicationContext.Create(client);
        });
    }
}