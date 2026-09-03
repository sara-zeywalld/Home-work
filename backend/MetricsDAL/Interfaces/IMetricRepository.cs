using backend.Models;

namespace backend.MetricsDAL.Interfaces;

public interface IMetricRepository
{
    Task<List<Metric>> GetAllAsync();
    Task AddAsync(Metric metric);
    Task<Metric?> GetWithFieldsAsync(int id);
    Task<List<ImportBatch>> GetImportsAsync(int metricId);
    IQueryable<MetricRow> QueryRows(int metricId);
    Task AddBatchAsync(ImportBatch batch);
}
