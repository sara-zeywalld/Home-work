using backend.Models;

namespace backend.MetricsBL.Interfaces;

public interface IMetricService
{
    Task<IReadOnlyList<Metric>> GetMetricsAsync();
    Task<Metric> CreateAsync(Metric metric);
    Task<object?> GetRowsAsync(int metricId, int page, int pageSize, string? search);
    Task<IReadOnlyList<ImportBatch>> GetImportsAsync(int metricId);
    Task<ImportResult?> ImportAsync(int metricId, string fileName, int year, string period, Stream fileStream);
}
