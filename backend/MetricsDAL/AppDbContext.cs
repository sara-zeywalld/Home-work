using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.MetricsDAL;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<PremiumMethod> PremiumMethods => Set<PremiumMethod>();
    public DbSet<Metric> Metrics => Set<Metric>();
    public DbSet<MetricField> MetricFields => Set<MetricField>();
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();
    public DbSet<MetricRow> MetricRows => Set<MetricRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MetricField>().HasIndex(field => new { field.MetricId, field.Version, field.Key }).IsUnique();
        modelBuilder.Entity<ImportBatch>().HasIndex(batch => new { batch.MetricId, batch.PeriodYear, batch.PeriodLabel });
        modelBuilder.Entity<MetricRow>().HasIndex(row => row.ImportBatchId);
        modelBuilder.Entity<Metric>().HasIndex(metric => metric.Code).IsUnique();
    }
}
