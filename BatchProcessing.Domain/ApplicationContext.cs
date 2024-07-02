using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace BatchProcessing.Domain;

public class ApplicationContext(DbContextOptions options) : DbContext(options)
{
    public static ApplicationContext Create(IMongoClient mongoClient) => new ApplicationContext(
        new DbContextOptionsBuilder<ApplicationContext>()
            .UseMongoDB(mongoClient, "mongoDb")
            .Options);

    public DbSet<BatchProcess> BatchProcesses => Set<BatchProcess>();

    public DbSet<BatchProcessItem> BatchProcessItems => Set<BatchProcessItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatchProcess>().ToCollection(nameof(BatchProcess));
        modelBuilder.Entity<BatchProcessItem>().ToCollection(nameof(BatchProcessItem));
    }
}