using System.Text.Json;
using backend.MetricsDAL.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.MetricsBL;

public class MetricService(IMetricRepository metric_repository, MetricsDAL.Import.IExcelImportReader excelReader) : Interfaces.IMetricService
{
    public async Task<IReadOnlyList<Metric>> GetMetricsAsync() => await metric_repository.GetAllAsync();

    public async Task<Metric> CreateAsync(Metric metric)
    {
        metric.Code = metric.Code.Trim();
        metric.Name = metric.Name.Trim();
        metric.SourceType = string.IsNullOrWhiteSpace(metric.SourceType) ? "Excel" : metric.SourceType.Trim();
        metric.IngestionFrequency = string.IsNullOrWhiteSpace(metric.IngestionFrequency) ? "Monthly" : metric.IngestionFrequency.Trim();
        await metric_repository.AddAsync(metric);
        return metric;
    }

    public async Task<object?> GetRowsAsync(int metricId, int page, int pageSize, string? search)
    {
        var metric = await metric_repository.GetWithFieldsAsync(metricId);
        if (metric is null) return null;
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var rows = metric_repository.QueryRows(metricId);
        if (!string.IsNullOrWhiteSpace(search)) rows = rows.Where(row => row.ValuesJson.Contains(search));
        var total = await rows.CountAsync();
        var data = await rows.OrderByDescending(row => row.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(row => new { row.Id, row.ImportBatchId, row.ValuesJson }).ToListAsync();
        return new { metric = new { metric.Id, metric.Name, metric.Code }, fields = metric.Fields.Where(field => field.IsRelevant).OrderBy(field => field.DisplayOrder), page, pageSize, total, data = data.Select(row => new { row.Id, row.ImportBatchId, values = JsonSerializer.Deserialize<Dictionary<string, object>>(row.ValuesJson) }) };
    }

    public async Task<IReadOnlyList<ImportBatch>> GetImportsAsync(int metricId) => await metric_repository.GetImportsAsync(metricId);

    public async Task<ImportResult?> ImportAsync(int metricId, string fileName, int year, string period, Stream fileStream)
    {
        var metric = await metric_repository.GetWithFieldsAsync(metricId);
        if (metric is null) return null;
        var importData = await excelReader.ReadAsync(fileStream, fileName);
        var fieldsByKey = metric.Fields.ToDictionary(field => field.Key, StringComparer.OrdinalIgnoreCase);
        var nextDisplayOrder = metric.Fields.Count == 0 ? 1 : metric.Fields.Max(field => field.DisplayOrder) + 1;
        foreach (var header in importData.Headers.Select(header => header.Trim()).Where(header => header.Length > 0))
        {
            if (fieldsByKey.TryGetValue(header, out var existingField))
            {
                existingField.IsRelevant = true;
                continue;
            }
            var field = new MetricField { Version = metric.Fields.Count == 0 ? 1 : metric.Fields.Max(item => item.Version) + 1, Key = header, Label = header, DataType = "text", IsRelevant = true, DisplayOrder = nextDisplayOrder++ };
            metric.Fields.Add(field);
            fieldsByKey[header] = field;
        }

        var relevantFields = metric.Fields.Where(field => field.IsRelevant).ToList();
        var schemaVersion = relevantFields.Count == 0 ? 1 : relevantFields.Max(field => field.Version);
        var batch = new ImportBatch { MetricId = metricId, SchemaVersion = schemaVersion, PeriodYear = year, PeriodLabel = period, FileName = fileName, ImportedAtUtc = DateTime.UtcNow, RowCount = importData.Rows.Count };
        foreach (var sourceRow in importData.Rows)
        {
            var values = sourceRow
                .Where(item => fieldsByKey.ContainsKey(item.Key))
                .ToDictionary(item => fieldsByKey[item.Key].Key, item => item.Value, StringComparer.OrdinalIgnoreCase);
            batch.Rows.Add(new MetricRow { ValuesJson = JsonSerializer.Serialize(values) });
        }
        await metric_repository.AddBatchAsync(batch);
        return new ImportResult(batch.Id, batch.FileName, batch.RowCount, batch.SchemaVersion);
    }
}
