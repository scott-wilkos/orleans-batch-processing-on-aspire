using Microsoft.EntityFrameworkCore;

namespace BatchProcessing.Domain;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<BatchProcess> BatchProcesses { get; set; }
    
    public DbSet<BatchProcessItem> BatchProcessItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatchProcess>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.CreatedOn)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired();

            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(e => e.BatchProcessId);
        });

        modelBuilder.Entity<BatchProcessItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.BatchProcessId)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired();
        });
    }
}