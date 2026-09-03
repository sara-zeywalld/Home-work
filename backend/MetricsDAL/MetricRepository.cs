using backend.MetricsDAL.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.MetricsDAL;

public class MetricRepository(AppDbContext db) : IMetricRepository
{
    public Task<List<Metric>> GetAllAsync() => db.Metrics.Include(metric => metric.Fields).ToListAsync();
    public async Task AddAsync(Metric metric) { db.Metrics.Add(metric); await db.SaveChangesAsync(); }
    public Task<Metric?> GetWithFieldsAsync(int id) => db.Metrics.Include(metric => metric.Fields).FirstOrDefaultAsync(metric => metric.Id == id);
    public Task<List<ImportBatch>> GetImportsAsync(int metricId) => db.ImportBatches.Where(batch => batch.MetricId == metricId).OrderByDescending(batch => batch.ImportedAtUtc).ToListAsync();
    public IQueryable<MetricRow> QueryRows(int metricId) => db.MetricRows.Where(row => db.ImportBatches.Any(batch => batch.Id == row.ImportBatchId && batch.MetricId == metricId));
    public async Task AddBatchAsync(ImportBatch batch) { db.ImportBatches.Add(batch); await db.SaveChangesAsync(); }
}
