using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace BatchProcessing.Domain;

public class ApplicationContext(DbContextOptions options, IMongoDatabase mongoDatabase) : DbContext(options)
{
    public static ApplicationContext Create(IMongoClient mongoClient, IMongoDatabase mongoDatabase) => new ApplicationContext(
        new DbContextOptionsBuilder<ApplicationContext>()
            .UseMongoDB(mongoClient, "mongoDb")
            .Options, mongoDatabase);

    public DbSet<BatchProcess> BatchProcesses => Set<BatchProcess>();

    public DbSet<BatchProcessItem> BatchProcessItems => Set<BatchProcessItem>();

    public async Task BulkInsert(IEnumerable<BatchProcessItem> items)
    {
        var entities = mongoDatabase.GetCollection<BatchProcessItem>(nameof(BatchProcessItem));
        await entities.InsertManyAsync(items);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatchProcess>().ToCollection(nameof(BatchProcess));
        modelBuilder.Entity<BatchProcessItem>().ToCollection(nameof(BatchProcessItem));
    }
}

public class ContextFactory(IServiceProvider serviceProvider)
{
    public ApplicationContext Create()
    {
        return serviceProvider.GetRequiredService<ApplicationContext>();
    }
}